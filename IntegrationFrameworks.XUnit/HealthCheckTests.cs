using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationFrameworks.XUnit
{
    [Collection("IntegrationTests")]
    public class WhenHealthCheckEndpointIsCalled : IAsyncLifetime
    {
        private readonly SetupFixture _setup;
        private readonly ITestOutputHelper _output;
        private HttpResponseMessage _response;
        private string _content;

        public WhenHealthCheckEndpointIsCalled(SetupFixture setup, ITestOutputHelper output)
        {
            _setup = setup;
            _output = output;
            setup.PrintOutput(output.WriteLine);
        }
        
        public async Task InitializeAsync()
        {
            using var client = new HttpClient();
            _response = await client.GetAsync("http://localhost/health");
            _content = await _response.Content.ReadAsStringAsync();
        }

        public Task DisposeAsync()
        {
            _setup.PrintOutput(_output.WriteLine);
            return Task.CompletedTask;
        }
        
        [Fact]
        public void ThenTheStatusCodeIsCorrect()
        {
            Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
        }

        [Fact]
        public void ThenTheResponseIsCorrect()
        {
            Assert.Equal("Healthy", _content);
        }
    }
}