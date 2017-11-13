using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AzureExtension.FullMinify.Minify;
using Microsoft.AspNetCore.Mvc;
using AzureExtension.FullMinify.Models;
using AzureJobs.Common;
using Microsoft.Extensions.Configuration;

namespace AzureExtension.FullMinify.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _logfolder;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
            if (!string.IsNullOrEmpty(_configuration["minify.logpath"]))
            {
                _logfolder = _configuration["minify.logpath"];
            }
            else
            {
                _logfolder = @"D:\home\site\wwwroot\app_data\";
            }
        }

        public IActionResult Index()
        {
            List<Result> results = new List<Result>();
            var filepath = Path.Combine(_logfolder, "AzureExtension.FullMinify.dll.csv");

            try
            {
                using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fs))
                {

                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] args = line.Split(',');
                        var result = CreateResult(args);

                        if (result != null)
                        {
                            results.Add(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Exception: {ex}");
            }

            return View(results);
        }

        public IActionResult Log()
        {
            var logs = new List<string>();

            string logfolder;
            if (!string.IsNullOrEmpty(_configuration["minify.logpath"]))
            {
                logfolder = _configuration["minify.logpath"];
            }
            else
            {
                logfolder = @"D:\home\site\wwwroot\";
            }

            List<string> extensions = new List<string>();
            if (!string.IsNullOrEmpty(_configuration["minify.extensions"]))
            {
                extensions.AddRange(_configuration["minify.extensions"].Split(";", StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                extensions.AddRange(new[] { ".css", ".html", ".js" });
            }

            string path;
            if (!string.IsNullOrEmpty(_configuration["minify.path"]))
            {
                path = _configuration["minify.path"];
            }
            else
            {
                path = path = @"D:\home\site\wwwroot\";
            }

            try
            {
                Logger logger = new Logger(logfolder);
                Minifier minifier = new Minifier(extensions, path, logger);
                Task.Run(() => minifier.FullMinify());
            }
            catch (Exception ex)
            {
                logs.Add(ex.ToString());
            }

            return View(logs);
        }


        private Result CreateResult(string[] args)
        {
            if (args.Length < 4)
                return null;

            DateTime date;
            int original, optimized;

            Result result = new Result();

            if (!DateTime.TryParse(args[0], out date))
                return null;

            result.Date = date;
            result.FileName = HttpUtility.UrlDecode(args[1]);

            if (!int.TryParse(args[2], out original))
                return null;

            result.Original = original;

            if (!int.TryParse(args[3], out optimized))
                return null;

            result.Optimized = optimized;

            return result;
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
