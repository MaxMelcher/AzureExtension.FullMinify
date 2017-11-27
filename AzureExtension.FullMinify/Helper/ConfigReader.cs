using System.IO;
using AzureExtension.FullMinify.Models;
using Newtonsoft.Json;

namespace AzureExtension.FullMinify.Helper
{
    public class ConfigReader
    {
        public Config Read(string configPath)
        {
            if (File.Exists(configPath))
            {
                var text = File.ReadAllText(configPath);
                var config = JsonConvert.DeserializeObject<Config>(text);
                return config;
            }

            return new Config();
        }

        public void Write(Config config, string path)
        {
            var text = JsonConvert.SerializeObject(config);

            var directory = new FileInfo(path).Directory;

            if (!directory.Exists)
            {
                directory.Create();
            }

            var configPath = Path.Combine(directory.FullName, "config.json");
            File.WriteAllText(configPath, text);
        }

    }
}
