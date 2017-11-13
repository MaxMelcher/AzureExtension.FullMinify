using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using AzureExtension.FullMinify.Models;
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



            using (FileStream fs = new FileStream(Path.Combine(_logfolder, "AzureExtension.FullMinify.dll.csv"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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

            return View(results);
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
