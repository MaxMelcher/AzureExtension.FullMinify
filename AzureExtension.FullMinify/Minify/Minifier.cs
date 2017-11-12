using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzureExtension.FullMinify.Minify
{
    public class Minifier : IMinifier
    {
        private readonly List<string> _extensions = new List<string>();
        private readonly string _path;

        public Minifier(List<string> extensions, string path)
        {
            _extensions = extensions;
            _path = path;
        }

        public void MinifyCSS(string filepath)
        {
            throw new NotImplementedException();
        }

        public void MinifyHtml(string filepath)
        {
            throw new NotImplementedException();
        }

        public void MinifyJs(string filepath)
        {
            throw new NotImplementedException();
        }

        public void Watch(Task prevTask)
        {
            foreach (string filter in _extensions)
            {
                FileSystemWatcher w = new FileSystemWatcher(_path);
                w.Filter = filter;
                w.IncludeSubdirectories = true;
                w.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName;
                w.Changed += (s, e) => Minify(e.FullPath, DateTime.Now);
                w.Renamed += (s, e) => Minify(e.FullPath, DateTime.Now);
                w.EnableRaisingEvents = true;
            }
        }

        public void Minify(string path, DateTime date)
        {
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
                foreach (string file in Directory.EnumerateFiles(_path, filter, SearchOption.AllDirectories))
                {
                    Minify(file, DateTime.MinValue);
                }
            }
        }

    }
}