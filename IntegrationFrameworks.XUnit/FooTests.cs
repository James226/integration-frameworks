using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationFrameworks.XUnit
{
    [Collection("IntegrationTests")]
    public class FooTests : IDisposable
    {
        private readonly SetupFixture _setup;
        private readonly ITestOutputHelper _output;

        public FooTests(SetupFixture setup, ITestOutputHelper output)
        {
            _setup = setup;
            _output = output;
            setup.PrintOutput(output.WriteLine);
        }

        public void Dispose()
        {
            _setup.PrintOutput(_output.WriteLine);
        }
        
        [Fact]
        public async Task FooShouldRespondWithCorrectPayload()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("http://localhost/foo");

            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("Bar", content);
        }
    }
}