using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DouglasCrockford.JsMin;
using ZetaProducerHtmlCompressor;

namespace AzureExtension.FullMinify.Minify
{
    public class Minifier : IMinifier
    {
        private readonly List<string> _extensions;
        private readonly string _path;

        public Minifier(List<string> extensions, string path)
        {
            _extensions = extensions;
            _path = path;
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
                    Filter = filter,
                    IncludeSubdirectories = true,
                    NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.CreationTime |
                                   NotifyFilters.FileName
                };
                w.Changed += (s, e) => Minify(e.FullPath, DateTime.Now);
                w.Renamed += (s, e) => Minify(e.FullPath, DateTime.Now);
                w.EnableRaisingEvents = true;
            }
        }

        public void Minify(string path, DateTime date)
        {
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