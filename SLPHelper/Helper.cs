using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPHelper
{
    public static class Helper
    {
        public static string GetFileName(string regionName, string reportType)
        { 
            StringBuilder stringBuilder = new StringBuilder(DateTime.Now.ToString().Replace(".", "-").Replace(":", "-"));
            stringBuilder.Append("_"+ regionName);
            stringBuilder.Append("_" + reportType);
            stringBuilder.Append(".xlsx");

            return stringBuilder.ToString();

        }
    }
}
