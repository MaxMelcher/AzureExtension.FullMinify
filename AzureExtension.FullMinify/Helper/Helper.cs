using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureExtension.FullMinify.Helper
{
    public static class Helper
    {
        public static string ReturnSize(double size, string sizeLabel)
        {
            if (size > 1024)
            {
                if (sizeLabel.Length == 0)
                    return ReturnSize(size / 1024, "KB");
                else if (sizeLabel == "KB")
                    return ReturnSize(size / 1024, "MB");
                else if (sizeLabel == "MB")
                    return ReturnSize(size / 1024, "GB");
                else if (sizeLabel == "GB")
                    return ReturnSize(size / 1024, "TB");
                else
                    return ReturnSize(size / 1024, "PB");
            }
            else
            {
                if (sizeLabel.Length > 0)
                    return string.Concat(size.ToString("0.00"), sizeLabel);
                else
                    return string.Concat(size.ToString("0.00"), "Bytes");
            }
        }
    }
}
