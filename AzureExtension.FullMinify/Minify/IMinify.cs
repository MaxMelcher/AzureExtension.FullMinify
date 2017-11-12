using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureExtension.FullMinify.Minify
{
    public interface IMinify
    {
        void MinifyCSS(string filepath);
        void MinifyHtml(string filepath);
        void MinifyJs(string filepath);
        void Watch();
        void Minify();
    }
}
