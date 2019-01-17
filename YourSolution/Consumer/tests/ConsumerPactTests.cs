using System;
using Xunit;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Consumer;
using System.Collections.Generic;

namespace tests
{
    public class ConsumerPactTests : IClassFixture<ConsumerPactClassFixture>
    {
        private IMockProviderService mockProviderService;
        private string mockProviderServiceBaseUri;

        public ConsumerPactTests(ConsumerPactClassFixture fixture)
        {
            this.mockProviderService = fixture.MockProviderService;
            this.mockProviderService.ClearInteractions(); // NOTE: Clears any previously registered interactions before the test is rn
            this.mockProviderServiceBaseUri = fixture.MockProviderServiceBaseUri;
        }

        [Fact]
        public void ItHandlesInvalidDateParams()
        {
            // arange
            var invalidRequestMessage = "validDateTime is not a date or time";
            this.mockProviderService.Given("There is data")
                .UponReceiving("An invalid GET request for Date Validation with an invalid date parameter")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/api/provider",
                    Query = "validDateTime=lolz"
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 400,
                    Headers = new Dictionary<string, object>
                    {
                        {"content-type", "application/json; charset=utf-8" }
                    },
                    Body = new
                    {
                        message = invalidRequestMessage
                    }
                });

            // act
            var result = ConsumerApiClient
                .ValidateDateTimeUsingProviderApi("lolz", mockProviderServiceBaseUri)
                .GetAwaiter()
                .GetResult();
            var resultBodyText = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            // assert
            Assert.Contains(invalidRequestMessage, resultBodyText);
        }
    }
}
