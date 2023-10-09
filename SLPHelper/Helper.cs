using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SLPHelper
{
    public static class Helper
    {
        public static string GetFileName(string regionName, string reportType, string reportFolderName)
        {
            string sResult = "";

            try
            {
                StringBuilder stringBuilder = new StringBuilder(DateTime.Now.ToString().Replace(".", "-").Replace(":", "-"));
                stringBuilder.Append("_" + regionName);
                stringBuilder.Append("_" + reportType);
                stringBuilder.Append(".xlsx");

                sResult =String.Concat(reportFolderName, stringBuilder.ToString());
            }
            catch (Exception e) 
            {
                sResult = String.Empty;
            }
                   

            return sResult;

        }
        public static string GetReportFolderByRegionName(string reportFolder, string regionName) {
            string sResult = "";

            try
            {
                string folderName = String.Concat(reportFolder, @"\", regionName);

                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }
                sResult = folderName;
            }
            catch (Exception e) 
            { 
            }
            return sResult;
        }

        public static bool ClearReportFolder(string reportFolder) {
            bool bResult = false;
            string[] files;

            try
            {
                if (Directory.Exists(reportFolder))
                {
                    files = Directory.GetFiles(reportFolder);

                    for (int i = 0; i < files.Length; i++)
                    {
                        File.Delete(files[i]);
                    }
                }
                bResult = true;
            }
            catch(Exception e) {

            }

            return bResult;
        }

    }
}
