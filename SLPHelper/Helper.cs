using System.Text;


namespace SLPHelper
{
    public static class Helper
    {
        public static string GetFileName(string regionName, string reportType, string reportFolderName, string source)
        {
            string sResult = "";

            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(DateTime.Now.ToString().Replace(".", "-").Replace(":", "-"));
                stringBuilder.Append("_" + regionName);
                stringBuilder.Append("_" + source);
                stringBuilder.Append("_" + reportType);
                stringBuilder.Append(".xlsx");

                sResult = stringBuilder.ToString();
            }
            catch 
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
            catch
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
            catch 
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
        public static string GetWaterTotalFormulaRC(int count)
        {
            string sResult = "";

            StringBuilder s_formula = new StringBuilder("=(");
            for (int k = 0; k < count; k++)
            {
                int index = 1 + (k * 2);
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

        public static string GetSumFormulaRC(int count)
        {
            StringBuilder s_formula = new StringBuilder("=SUM(R[");
            s_formula.Append(((-1) * count).ToString());
            s_formula.Append("]C");
            s_formula.Append(":");
            s_formula.Append("R[-1]C)");
            return s_formula.ToString();

        }


        public static bool CreateFolderReportByRegions(string RootReportFolder, string regionName)
        {
            bool bResult = false;
            string s_path = "";

            try
            {
                s_path = Path.Combine(RootReportFolder, regionName);

                if (!Directory.Exists(s_path))
                {
                    Directory.CreateDirectory(s_path);

                }

            }
            catch (Exception ex)
            { 
            }

            return bResult;
        }
        public static string CreateFullFileName(string path, string fileName)
        {
            string s_path = Path.Combine(path, fileName);   
            return s_path;
        }
        public static bool GetAtachedFileName(ref string[] files, string path)
        {
            bool bResult = false;

            try
            {
                files = Directory.GetFiles(path);

                bResult = true;
            }
            catch (Exception ex) { 

            }
            return bResult;

        }
        public static string GetLinkToTotalValue(int worksheet, int rowIndex)
        {
            StringBuilder builder = new StringBuilder(@"='");
            builder.Append(worksheet);
            builder.Append(@"'!B");
            builder.Append(rowIndex);

            return builder.ToString();
        }
    }
}
