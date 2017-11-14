using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AzureExtension.FullMinify.Minify;
using AzureJobs.Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace AzureExtension.FullMinify
{
    public class Program
    {
        private static IConfigurationRoot configuration;

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

                    configuration = config.Build();
                })
                
                .UseStartup<Startup>()
                .Build();
    }
}
