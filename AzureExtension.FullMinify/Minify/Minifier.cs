﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AzureExtension.FullMinify.Log;
using DouglasCrockford.JsMin;
using ZetaProducerHtmlCompressor;
using System.Threading;
using AzureExtension.FullMinify.Helper;

namespace AzureExtension.FullMinify.Minify
{
    public class Minifier : IMinifier
    {
        static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly List<string> _extensions;
        private readonly string _path;
        private readonly Logger _logger;
        public static BlockingCollection<string> Queue = new BlockingCollection<string>(new ConcurrentStack<string>());

        public static FileHashStore FileHashStore;

        public Minifier(List<string> extensions, string path, string logfolder)
        {
            _extensions = extensions;
            _path = path;
            _logger = new Logger(logfolder);
            FileHashStore = new FileHashStore(logfolder);

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var filepath = Queue.Take();
                    Minify(filepath);
                }
            });
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
            if (filepath.EndsWith(".png"))
            {
                Compressor compressor = new Compressor();
                compressor.CompressFile(filepath, true);
            }
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
                                   NotifyFilters.FileName,
                    InternalBufferSize = 64000
                };
                w.Changed += (s, e) => AddToQueue(e.FullPath);
                w.Renamed += (s, e) => AddToQueue(e.FullPath);
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

        public static void AddToQueue(string filepath)
        {
            if (FileHashStore.HasChangedOrIsNew(filepath))
            {
                Queue.Add(filepath);
            }
        }

        public void Minify(string path)
        {
            try
            {
                //if not changed - do nothing
                if (!FileHashStore.HasChangedOrIsNew(path)) return;

                long before;
                long after;

                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var content = ReadFully(fs);
                    before = content.LongLength;
                }

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
                else if (path.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase))
                {
                    MinifyImage(path);
                }
                else if (path.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase))
                {
                    MinifyImage(path);
                }
                else if (path.EndsWith(".jpeg", StringComparison.InvariantCultureIgnoreCase))
                {
                    MinifyImage(path);
                }
                else if (path.EndsWith(".gif", StringComparison.InvariantCultureIgnoreCase))
                {
                    MinifyImage(path);
                }

                var bytes = File.ReadAllBytes(path);
                after = bytes.LongLength;

                _logger.Write(new LogItem
                {
                    FileName = path,
                    OriginalSizeBytes = before,
                    NewSizeBytes = after
                });

                FileHashStore.Save(path);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Minify:: Exception! {ex}");
            }
        }

        public async void FullMinify()
        {
            await SemaphoreSlim.WaitAsync();
            try
            {
                foreach (string filter in _extensions)
                {
                    foreach (string file in Directory.EnumerateFiles(_path, string.Concat("*", filter), SearchOption.AllDirectories))
                    {
                        AddToQueue(file);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"FullMinify:: Exception! {ex}");
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

    }
}

