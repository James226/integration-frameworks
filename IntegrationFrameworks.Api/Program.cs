using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Retail.Logging.AspNetCore;
using StatsdClient;

namespace IntegrationFrameworks.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DogStatsd.Configure(new StatsdConfig
            {
                StatsdServerName = "host.docker.internal"
            });
            
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .Build();
            
            using var host = CreateHostBuilder(args, configuration).Build();
            host.Start();
            
            DogStatsd.Increment("server.start");
            
            host.WaitForShutdown();
        }

        private static IHostBuilder CreateHostBuilder(string[] args, IConfigurationRoot configuration) =>
            Host.CreateDefaultBuilder(args)
                
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseConfiguration(configuration).UseStartup<Startup>(); })
                .UseLogging(configuration);
    }
}