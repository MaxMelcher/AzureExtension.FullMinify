using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace AzureExtension.FullMinify
{
    public class Program
    {
        private static IConfigurationRoot _configuration;

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
            Console.WriteLine("bye");
        }


        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
               .CaptureStartupErrors(true)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config.AddEnvironmentVariables()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json");

                    _configuration = config.Build();
                })
                
                .UseStartup<Startup>()
                .Build();
    }
}
