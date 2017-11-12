using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AzureExtension.FullMinify.Minify;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace AzureExtension.FullMinify
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var extensions = new List<string>();
            string path;
            
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddEnvironmentVariables().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var configuration = builder.Build();

            if (!string.IsNullOrEmpty(configuration["minify.extensions"]))
            {
                extensions.AddRange(configuration["minify.extensions"].Split(";", StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                extensions.AddRange(new [] {".css", ".html", ".js"});
            }

            if (!string.IsNullOrEmpty(configuration["minify.path"]))
            {
                path = configuration["minify.path"];
            }
            else
            {
                path = @"D:\home\site\wwwroot\";
            }

            Minifier minifier = new Minifier(extensions, path);
            Task.Run(() => minifier.FullMinify()).ContinueWith(minifier.Watch);

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
