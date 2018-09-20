using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace Util
{
    public class ProcessorInfo
    {
        //コア数（物理CPUごとの情報）
        public int NumberOfCores { get; private set; }

        //論理CPU数（物理CPUごとの情報）
        public int NumberOfLogicalProcessors { get; private set; }

        //物理CPU数（マザーボード上にあるPCパーツとしてのCPUの個数）
        public static int NumberOfPhysicalProcessors { get; private set; }

        //コンストラクタ
        public ProcessorInfo(int core, int logProc, int phyProc)
        {
            NumberOfCores = core;
            NumberOfLogicalProcessors = logProc;
            NumberOfPhysicalProcessors = phyProc;
        }

        //PCが持つCPU情報を取得する
        public static ProcessorInfo[] GetProcessorInfos()
        {
            //インスタンスの数はシステム上で使用可能な物理プロセッサの数と同じ
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            int numofPhysicalProc = moc.Count;

            ProcessorInfo[] procInfos = new ProcessorInfo[numofPhysicalProc];
            int idx = 0;
            foreach (ManagementObject mo in moc)
            {
                int core = Convert.ToInt32(mo.GetPropertyValue("NumberOfCores"));
                int logProc = Convert.ToInt32(mo.GetPropertyValue("NumberOfLogicalProcessors"));
                procInfos[idx++] = new ProcessorInfo(core, logProc, numofPhysicalProc);
            }

            return procInfos;
        }
    }
}
