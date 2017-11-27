using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureExtension.FullMinify.Models
{
    public class Config
    {
        private const string DefaultPath = @"D:\home\site\wwwroot\";
        private const string DefaultExtensions = ".css;.html;.js;.png;.jpeg;.jpg;.gif";
        private const string DefaultLogPath = @"D:\home\site\wwwroot\appdata\Azure.FullMinify";


        public string Path { get; set; } = DefaultPath;
        public string Extensions { get; set; } = DefaultExtensions;
        public string LogPath { get; set; } = DefaultPath;
    }
}
