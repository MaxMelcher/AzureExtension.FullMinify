using System.Collections.Generic;

namespace AzureExtension.FullMinify.CSV
{
    /// <summary>
    /// Taken from: https://github.com/ligershark/AzureJobs
    /// </summary>
    public class CsvHelper
    {
        public string Escape(string inputToClean)
        {
            const string DoubleQuote = "\"";
            const string EscapedItemFormat = "{0}{1}{0}";
            const string ItemsToCleanFor = ",\r\n\"";
            if (inputToClean.IndexOfAny(ItemsToCleanFor.ToCharArray()) > -1)
            {
                //wrap the input string in double quotes, while replacing any double quotes in the string with 2 double quotes
                return string.Format(EscapedItemFormat, DoubleQuote,
                    inputToClean.Replace(DoubleQuote, DoubleQuote + DoubleQuote));
            }
            return inputToClean;
        }

        public IEnumerable<string> EscapeAll(params object[] lineItems)
        {
            for (int i = 0; i < lineItems.Length; i++)
            {
                yield return this.Escape(lineItems[i].ToString());
            }
        }
    }
}