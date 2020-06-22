using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GlobalHelper {


    public static Process OpenEXE(string _exePathName, string _exeArgus)
    {
        try
        {
            Process myprocess = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo(_exePathName, _exeArgus);
            myprocess.StartInfo = startInfo;
            myprocess.StartInfo.UseShellExecute = false;
            myprocess.Start();
            return myprocess;
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log("出错原因：" + ex.Message);
            return null;
        }
    }
    public static void CloseEXE(string processName)
    {
        Process[] processes = Process.GetProcesses();
        foreach (Process process in processes)
        {
            try
            {
                if (!process.HasExited)
                {
                    if (process.ProcessName == processName)
                    {
                        process.Kill();
                        UnityEngine.Debug.Log("已关闭进程");
                    }
                }
            }
            catch (System.InvalidOperationException ex)
            {
                UnityEngine.Debug.Log(ex);
            }
        }

    }
}
