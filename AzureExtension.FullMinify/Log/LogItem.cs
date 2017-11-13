using System;
using System.Linq;
using System.Threading.Tasks;
using AzureExtension.FullMinify.CSV;

namespace AzureExtension.FullMinify.Log
{

    /// <summary>
    /// Taken from: https://github.com/ligershark/AzureJobs
    /// </summary>
    public class LogItem
    {

        public DateTime OnDate { get; private set; }
        public string FileName { get; set; }
        public long OriginalSizeBytes { get; set; }
        public long NewSizeBytes { get; set; }

        public LogItem()
        {
            this.OnDate = DateTime.UtcNow;
        }
        public string CalcPercentageSavings()
        {
            if (OriginalSizeBytes == 0) return "N/A";
            decimal savings = this.NewSizeBytes / Convert.ToDecimal(this.OriginalSizeBytes);
            return (Math.Round(100 - (savings * 100), 1)).ToString();
        }

        public override string ToString()
        {
            CsvHelper helper = new CsvHelper();
            return string.Join(",", helper.EscapeAll(this.OnDate, this.FileName, this.OriginalSizeBytes, this.NewSizeBytes, this.CalcPercentageSavings()));
        }
    }

}
