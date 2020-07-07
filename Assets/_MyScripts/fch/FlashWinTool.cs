using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class FlashWinTool{
    public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);
    [DllImport("user32.dll")] //闪烁窗体
    public static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetParent(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);

    [DllImport("kernel32.dll")]
    public static extern void SetLastError(uint dwErrCode);

    [StructLayout(LayoutKind.Sequential)]
    public struct FLASHWINFO
    {
        public UInt32 cbSize;
        public IntPtr hwnd;
        public UInt32 dwFlags;
        public UInt32 uCount;
        public UInt32 dwTimeout;
    }

    //闪烁窗体参数
    public const UInt32 FLASHW_STOP = 0;//停止闪动.系统将窗体恢复到初始状态.
    public const UInt32 FLASHW_CAPTION = 1;//闪动窗体的标题.
    public const UInt32 FLASHW_TRAY = 2;//闪动任务栏按钮
    public const UInt32 FLASHW_ALL = 3;//闪动窗体标题和任务栏按钮
    public const UInt32 FLASHW_TIMER = 4;//连续不停的闪动,直到此参数被设置为:FLASHW_STOP
    public const UInt32 FLASHW_TIMERNOFG = 12;//连续不停的闪动,直到窗体用户被激活.通常用法将参数设置为: FLASHW_ALL | FLASHW_TIMERNOFG

    /// <summary>
    /// 窗口闪烁
    /// </summary>
    /// <param name="handle">窗口句柄</param>
    public static void FlashWindow(IntPtr handle)
    {
        FLASHWINFO fInfo = new FLASHWINFO();

        fInfo.cbSize = Convert.ToUInt32(System.Runtime.InteropServices.Marshal.SizeOf(fInfo));
        fInfo.hwnd = handle;
        fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;//这里是闪动窗标题和任务栏按钮,直到用户激活窗体
        fInfo.uCount = UInt32.MaxValue;
        fInfo.dwTimeout = 0;

        FlashWindowEx(ref fInfo);
    }

    /// <summary>
    /// 获取窗口句柄
    /// </summary>
    /// <returns>窗口句柄</returns>
    public static IntPtr GetProcessWnd()
    {
        IntPtr ptrWnd = IntPtr.Zero;
        uint pid = (uint)Process.GetCurrentProcess().Id;  // 当前进程 ID  

        bool bResult = EnumWindows(new WNDENUMPROC(delegate (IntPtr hwnd, uint lParam)
        {
            uint id = 0;

            if (GetParent(hwnd) == IntPtr.Zero)
            {
                GetWindowThreadProcessId(hwnd, ref id);
                if (id == lParam)    // 找到进程对应的主窗口句柄  
                {
                    ptrWnd = hwnd;   // 把句柄缓存起来  
                    SetLastError(0);    // 设置无错误  
                    return false;   // 返回 false 以终止枚举窗口  
                }
            }

            return true;

        }), pid);

        return (!bResult && Marshal.GetLastWin32Error() == 0) ? ptrWnd : IntPtr.Zero;
    }
}
