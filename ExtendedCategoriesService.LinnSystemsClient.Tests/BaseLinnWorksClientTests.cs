using NUnit.Framework;

namespace ExtendedCategoriesService.LinnSystemsClient.Tests
{
    using System.Collections.Generic;
    using System.Net.Http;
    using NSubstitute;
    using Shouldly;

    public class BaseLinnWorksClientTests
    {
        private HttpClient client;
        
        [SetUp]
        public void Setup()
        {
            client = Substitute.For<HttpClient>();
        }

        [Test]
        public void Should_pass_query_parameters()
        {
            //given
            var parameters = new Dictionary<string, string>()
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };
            var url = "http://www.google.com/search";
            
            //when
            var serviceClient = CreateInstance();
            Should.NotThrow(serviceClient.SendRequestAsync(url, HttpMethod.Get, queryParameters: parameters));
            
            //then
            client.Received(1).SendAsync(
                Arg.Is<HttpRequestMessage>((requestMessage) => requestMessage.RequestUri.AbsoluteUri == "http://www.google.com/search?key1=value1&key2=value2"
                ), 
                HttpCompletionOption.ResponseHeadersRead, 
                default );
        }

        private BaseLinnWorksClient CreateInstance()
        {
            return new BaseLinnWorksClient(client);
        }
        
    }
}