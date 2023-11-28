﻿using System.Text;

namespace SLPHelper
{
    public static class Helper
    {
        public static string GetFileName(string regionName, string reportType, string reportFolderName, string source)
        {
            string sResult = "";

            try
            {
                StringBuilder stringBuilder = new StringBuilder(reportFolderName + @"\");
                stringBuilder.Append(regionName + @"\");
                stringBuilder.Append(DateTime.Now.ToString().Replace(".", "-").Replace(":", "-"));
                stringBuilder.Append("_" + source);
                stringBuilder.Append("_" + regionName);
                stringBuilder.Append("_" + reportType);
                stringBuilder.Append(".xlsx");

                sResult = stringBuilder.ToString();
            }
            catch (Exception e)
            {
                sResult = String.Empty;
            }


            return sResult;

        }
        public static string GetReportFolderByRegionName(string reportFolder, string regionName)
        {
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
        public static bool ClearReportFolder(string reportFolder)
        {
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
            catch (Exception e)
            {

            }

            return bResult;
        }
        public static string GetTotalFormulaRC(int count)
        {
            string sResult = "";

            StringBuilder s_formula = new StringBuilder("=(");
            for (int k = 0; k < count; k++)
            {
                int index = 4 + (k * 4);
                s_formula.Append("RC[");
                s_formula.Append(index.ToString());
                s_formula.Append("]");

                if (k < (count - 1))
                {
                    s_formula.Append(" + ");
                }

            }
            s_formula.Append(')');

            sResult = s_formula.ToString(); 
            return sResult;

        }

    }
}
