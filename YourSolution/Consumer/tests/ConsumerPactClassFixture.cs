using PactNet;
using PactNet.Mocks.MockHttpService;
using System;
using Xunit;

namespace tests
{
    /* This class is responsible for setting up a shared
     * mock server for Pact used by all the tests.
     * Xunit can use a Class Fixture for this.
     * See: https://goo.gl/hSq4nv
     */
    public class ConsumerPactClassFixture : IDisposable
    {
        public IPactBuilder PactBuilder { get; private set; }
        public IMockProviderService MockProviderService { get; private set; }

        public int MockServerPort { get { return 9222; } }

        public string MockProviderServiceBaseUri
        {
            get
            {
                return String.Format("http://localhost:{0}", this.MockServerPort);
            }
        }

        public ConsumerPactClassFixture()
        {
            // Using spec version 2.0.0 more details at https://goo.gl/UrBSRc
            var pactConfig = new PactConfig
            {
                SpecificationVersion =  "2.0.0",
                PactDir = @"..\..\..\..\..\pacts",
                LogDir = @".\pact_logs"
            };

            this.PactBuilder = new PactBuilder(pactConfig);

            this.PactBuilder.ServiceConsumer("consumer")
                .HasPactWith("Provider");

            this.MockProviderService = PactBuilder.MockService(this.MockServerPort);
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // this will save the pact file once finished
                    PactBuilder.Build();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // do not change this code. put cleanup code in Dispose(bool disposing)
            Dispose(true);
        }
        #endregion IDisposable Support
    }
}
