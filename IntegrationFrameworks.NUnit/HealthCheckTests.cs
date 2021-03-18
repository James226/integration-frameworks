using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace IntegrationFrameworks.NUnit
{
    public class WhenHealthCheckEndpointIsCalled
    {
        private HttpResponseMessage _response;
        private string _content;

        [SetUp]
        public async Task Setup()
        {
            _response = await SetupFixture.HttpClient.GetAsync("/health");
            _content = await _response.Content.ReadAsStringAsync();
        }
        
        [Test]
        public void ThenTheStatusCodeIsCorrect()
        {
            Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void ThenTheResponseIsCorrect()
        {
            Assert.That(_content, Is.EqualTo("Healthy"));
        }
    }
}