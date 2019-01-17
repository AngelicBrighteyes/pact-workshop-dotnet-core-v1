using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TestsProvider.Middleware
{
    public class ProviderStateMiddleware
    {
        private const string dataPath = @"../../../../../data";
        private const string dataFileName = "somedata.txt";
        private const string ConsumerName = "Consumer";

        private readonly RequestDelegate requestDelegate;
        private readonly IDictionary<string, Action> providerStates;
        private readonly string dataFile;

        public ProviderStateMiddleware(RequestDelegate next)
        {
            this.requestDelegate = next;
            this.dataFile = Path.Combine(dataPath, dataFile);

            this.providerStates = new Dictionary<string, Action>
            {
                {
                    "There is no data",
                    AddData
                },
                {
                    "There is Data",
                    RemoveAllData
                }
            };
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value == "/provider-states")
            {
                this.HandleProviderStatesRequest(context);
                await context.Response.WriteAsync(String.Empty);
            }
            else
            {
                await this.requestDelegate(context);
            }
        }

        private void HandleProviderStatesRequest(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            if(context.Request.Method.ToUpper() == HttpMethods.Post.ToString().ToUpper()
                && context.Request.Body != null)
            {
                string jsonRequestBody = String.Empty;
                using(var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    jsonRequestBody = reader.ReadToEnd();
                }

                var providerState = JsonConvert.DeserializeObject<ProviderState>(jsonRequestBody);

                // A null or empty provider state key must be handled
                if(providerState != null && !String.IsNullOrEmpty(providerState.State)
                    && providerState.Consumer == ConsumerName)
                {
                    this.providerStates[providerState.State].Invoke();
                }
            }
        }

        private void RemoveAllData()
        {
            if (File.Exists(dataFile))
            {
                File.Delete(dataFile);
            }
        }

        private void AddData()
        {
            if (!File.Exists(dataFile))
            {
                File.Create(dataFile);
            }
        }
    }
}
