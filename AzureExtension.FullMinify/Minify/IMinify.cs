using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureExtension.FullMinify.Minify
{
    public interface IMinifier
    {
        void MinifyCSS(string filepath);
        void MinifyHtml(string filepath);
        void MinifyJs(string filepath);
        void Watch(Task task);
        void Minify(string path, DateTime date);
    }
}
