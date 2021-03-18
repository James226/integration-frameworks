using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using Retail.DockerClient;
using Xunit;

namespace IntegrationFrameworks.XUnit
{
    public class SetupFixture : IDisposable
    {
        private Queue<string> _output;
        private readonly DockerContainer _container;
        private readonly StatsdListener _listener;

        public SetupFixture()
        {
            _output = new Queue<string>();
            
            var dockerContext = new DockerContext(Output);
            var dockerNetwork = DockerNetwork.EnsureNetworkExists(dockerContext, "integration-frameworks").Result;
            var image = GetImage(dockerContext);

            _container = DockerContainer.Start(dockerContext,
                dockerNetwork, 
                "test", 
                image, 
                new Dictionary<string, string>{
                    ["ASPNETCORE_ENVIRONMENT"] = "Development"
                },
                new[] {80});

            _listener = StatsdListener.Start();
            _listener.WaitForMetric(TimeSpan.FromSeconds(30), "service.started");

        }
        
        public void Dispose()
        {
            _listener.Dispose();
            _container.Dispose();
        }
        
        private void Output(string output)
        {
            lock (_output)
            {
                _output.Enqueue(output);
            }
        }

        public void PrintOutput(Action<string> writer)
        {
            lock (_output)
            {
                var output = _output;
                _output = new Queue<string>();
                foreach (var o in output)
                {
                    writer.Invoke(o);
                }
            }
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

    [CollectionDefinition("IntegrationTests")]
    public class SetupFixtureCollection : ICollectionFixture<SetupFixture>
    {
    }
}