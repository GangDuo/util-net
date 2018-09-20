using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util
{
    public class Printer
    {
        public static string Default()
        {
            string name = "";
            var mos = new System.Management.ManagementObjectSearcher("Select * from Win32_Printer");
            System.Management.ManagementObjectCollection moc = mos.Get();

            //プリンタを列挙する
            foreach (System.Management.ManagementObject mo in moc)
            {
                //デフォルトのプリンタか調べる
                //mo["Default"]はWindows NT 4.0, 2000で使用できません
                if ((((uint)mo["Attributes"]) & 4) == 4)
                {
                    name = mo["Name"] as string;
                    mo.Dispose();
                    break;
                }
                mo.Dispose();
            }

            moc.Dispose();
            mos.Dispose();
            return name;
        }

        /// <summary>/// 「通常使うプリンタ」に設定する/// </summary>
        /// <param name="printerName">プリンタ名</param>
        public static void SetDefaultPrinter(string printerName)
        {
            System.Management.ManagementObjectSearcher mos =
                new System.Management.ManagementObjectSearcher(
                "Select * from Win32_Printer");
            System.Management.ManagementObjectCollection moc = mos.Get();
            //プリンタを列挙する
            foreach (System.Management.ManagementObject mo in moc)
            {
                if (((string)mo["Name"]) == printerName)
                {
                    //名前を見つけたとき、デフォルトプリンタに設定する
                    System.Management.ManagementBaseObject mbo =
                        mo.InvokeMethod("SetDefaultPrinter", null, null);
                    if (((uint)mbo["returnValue"]) != 0)
                        throw new Exception("失敗しました。");
                }
            }
        }
    }
}
