using System;
using System.IO;

namespace AzureExtension.FullMinify.Controllers
{
    public class Result
    {
        public DateTime Date { get; set; }
        public string FileName { get; set; }
        public int Original { get; set; }
        public int Optimized { get; set; }

        public long Saving
        {
            get { return Original - Optimized; }
        }

        public double Percent
        {
            get { return (double)Saving / (double)Original * 100; }
        }

        public string ShortFileName
        {
            get { return Path.GetFileName(FileName); }
        }
    }
}