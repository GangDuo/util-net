using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

namespace Util
{
    public class MsExcel
    {
        public static string SaveAs(string filename, Excel.XlFileFormat xlType = Excel.XlFileFormat.xlOpenXMLWorkbook)
        {
            var path = string.Empty;
            Transaction(filename, (wkbk) =>
            {
                wkbk.SaveAs(GetPathWithoutExtension(filename), (int)xlType);
                path = wkbk.FullName;
            });
            return path;
        }

        public static string SaveAsCsv(string filename, bool isFirstLineEliminated = true)
        {
            var path = string.Empty;
            Transaction(filename, (wkbk) =>
            {
                if (isFirstLineEliminated)
                {
                    Excel.Worksheet sheet = wkbk.Sheets[1];
                    sheet.get_Range("1:1").Delete();
                }
                wkbk.SaveAs(GetPathWithoutExtension(filename), Excel.XlFileFormat.xlCSV);
                path = wkbk.FullName;
            });
            return path;
        }

        // シート名、行、列、値
        public static void SetValue(string filename, List<Tuple<object, int, int, object>> xs)
        {
            Transaction(filename, (wkbk) =>
            {
                foreach (var x in xs)
                {
                    Excel.Worksheet sheet = wkbk.Sheets[x.Item1];
                    sheet.Cells[x.Item2, x.Item3] = x.Item4;                    
                }
                wkbk.Save();
            });
        }

        private static string GetPathWithoutExtension(string path)
        {
            return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
        }

        private static void Transaction(string filename, Action<Excel.Workbook> callback)
        {
            Excel.Application excelApp = null;
            Excel.Workbook wkbk = null;

            try
            {
                excelApp = new Excel.Application();
#if DEBUG
                excelApp.Visible = true;
#else
                excelApp.Visible = false;
#endif
                excelApp.DisplayAlerts = false;
                wkbk = excelApp.Workbooks.Open(filename);
                callback(wkbk);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (null != wkbk)
                {
                    wkbk.Close();
                    wkbk = null;
                }

                // Close Excel.
                if (null != excelApp)
                {
                    excelApp.Quit();
                    excelApp = null;
                }
            }
        }
    }
}
