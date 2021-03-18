using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using NUnit.Framework;
using Retail.DockerClient;

namespace IntegrationFrameworks.NUnit
{
    [SetUpFixture]
    public class SetupFixture
    {
        public static HttpClient HttpClient;
        private DockerContainer _container;
        private StatsdListener _listener;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dockerContext = new DockerContext(TestContext.Out.WriteLine);
            var dockerNetwork = await DockerNetwork.EnsureNetworkExists(dockerContext, "integration-frameworks");
            var image = GetImage(dockerContext);

            _container = DockerContainer.Start(dockerContext,
                dockerNetwork, 
                "test", 
                image, 
                new Dictionary<string, string>
                {
                    ["ASPNETCORE_ENVIRONMENT"] = "Development"
                },
                new[] {80});

            _listener = StatsdListener.Start();
            _listener.WaitForMetric(TimeSpan.FromSeconds(30), "service.started");

            HttpClient = new HttpClient {BaseAddress = new Uri("http://localhost")};

        }

        [OneTimeTearDown]
        public void Teardown()
        {
            HttpClient?.Dispose();
            _listener.Dispose();
            _container.Dispose();
        }

        private DockerImage GetImage(DockerContext dockerContext)
        {
#if(DEBUG)

            using var nugetConfig =
                File.OpenRead(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "NuGet", "NuGet.config"));
            var config = XDocument.Load(nugetConfig);
            var source = config.XPathSelectElement("/configuration/packageSources/add[@key='Retail Nuget']");
            var currentDirectory = Directory.GetCurrentDirectory();
            return DockerImage.Build(dockerContext,
                "integrationframeworks.api",
                "latest",
                Path.GetFullPath(Path.Combine(currentDirectory, "../../../../")),
                new Dictionary<string, string>
                {
                    ["PrivateNugetSource"] = source?.Attribute("value")?.Value
                });
#else
            return DockerImage.Local("integrationframeworks.api", "latest");
#endif
        }
    }
}