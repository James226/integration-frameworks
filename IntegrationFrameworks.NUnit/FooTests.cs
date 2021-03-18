using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace IntegrationFrameworks.NUnit
{
    public class FooTests
    {
        [Test]
        public async Task FooShouldRespondWithCorrectPayload()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("http://localhost/foo");

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(content, Is.EqualTo("Bar"));
        }
    }
}