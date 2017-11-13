using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AzureExtension.FullMinify.Log;
using AzureJobs.Common;
using DouglasCrockford.JsMin;
using ZetaProducerHtmlCompressor;
using System.Security.Cryptography;
using System.Threading;
using System.Linq;

namespace AzureExtension.FullMinify.Minify
{
    public class Minifier : IMinifier
    {
        private readonly List<string> _extensions;
        private readonly string _path;
        private readonly Logger _logger;
        private Dictionary<string, byte[]> _hash = new Dictionary<string, byte[]>();

        public Minifier(List<string> extensions, string path, Logger logger)
        {
            _extensions = extensions;
            _path = path;
            _logger = logger;
        }

        public void MinifyCSS(string filepath)
        {
            //read
            var css = File.ReadAllText(filepath);

            //compress
            css = Regex.Replace(css, @"[a-zA-Z]+#", "#");
            css = Regex.Replace(css, @"[\n\r]+\s*", string.Empty);
            css = Regex.Replace(css, @"\s+", " ");
            css = Regex.Replace(css, @"\s?([:,;{}])\s?", "$1");
            css = css.Replace(";}", "}");
            css = Regex.Replace(css, @"([\s:]0)(px|pt|%|em)", "$1");

            // Remove comments from CSS
            var compressed = Regex.Replace(css, @"/\*[\d\D]*?\*/", string.Empty);

            //write
            File.WriteAllText(filepath, compressed);
        }

        public void MinifyHtml(string filepath)
        {
            HtmlContentCompressor contentCompressor = new HtmlContentCompressor();

            //read
            var html = File.ReadAllText(filepath);

            //compress
            var compressed = contentCompressor.Compress(html);

            //write
            File.WriteAllText(filepath, compressed);
        }

        public void MinifyJs(string filepath)
        {
            var minifier = new JsMinifier();

            //read
            var js = File.ReadAllText(filepath);

            //compress
            string compressed = minifier.Minify(js);


            //write
            File.WriteAllText(filepath, compressed);
        }

        public void MinifyImage(string filepath)
        {
            throw new NotImplementedException();
        }

        public void Watch(Task prevTask)
        {
            foreach (string filter in _extensions)
            {
                FileSystemWatcher w = new FileSystemWatcher(_path)
                {
                    Filter = "*" + filter,
                    IncludeSubdirectories = true,
                    NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.CreationTime |
                                   NotifyFilters.FileName
                };
                w.Changed += (s, e) => Minify(e.FullPath, DateTime.Now);
                w.Renamed += (s, e) => Minify(e.FullPath, DateTime.Now);
                w.EnableRaisingEvents = true;
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public void Minify(string path, DateTime date)
        {
            if (_hash.ContainsKey(path))
            {
                var hash = _hash[path];

                byte[] content;
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    content = ReadFully(fs);
                }
                var newhash = MD5.Create().ComputeHash(content);
         
                if (hash.SequenceEqual(newhash)) return;
            }

            //todo handle images
            //todo figure out what to do with the date
            if (path.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase))
            {
                MinifyHtml(path);
            }
            else if (path.EndsWith(".css", StringComparison.InvariantCultureIgnoreCase))
            {
                MinifyCSS(path);
            }
            else if (path.EndsWith(".js", StringComparison.InvariantCultureIgnoreCase))
            {
                MinifyJs(path);
            }


            _logger.Write(new LogItem
            {
                FileName = path,
                OriginalSizeBytes = 0,
                NewSizeBytes = 0
            });

            var bytes = File.ReadAllBytes(path);
            var hashafter = MD5.Create().ComputeHash(bytes);

            _hash[path] = hashafter;
        }

        public void FullMinify()
        {
            foreach (string filter in _extensions)
            {
                foreach (string file in Directory.EnumerateFiles(_path, string.Concat("*", filter), SearchOption.AllDirectories))
                {
                    Minify(file, DateTime.MinValue);
                }
            }
        }

    }
}