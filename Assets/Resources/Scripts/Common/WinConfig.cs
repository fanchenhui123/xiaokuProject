/*
 windows窗口化的时候不显示边框方法，这个方法脚本放在启动的方法中即可自动配置



 */

using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class WinConfig : MonoBehaviour
{
    [DllImport("user32.dll")]
    static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);
    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hwnd, int hwndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    const uint SWP_SHOWWINDOW = 0x0040;
    const int GWL_STYLE = -16;
    const int WS_BORDER = 1;
    const int WS_POPUP = 0x800000;

    int _posX = 0;
    int _posY = 0;
    int _Txtwith = 1920;
    int _Txheight = 1080;



    private void Awake()
    {
        StartCoroutine("Setposition");
    }

    IEnumerator Setposition()
    {
        yield return new WaitForSeconds(0.1f);
        SetWindowLong(GetForegroundWindow(), GWL_STYLE, WS_POPUP);
        bool result = SetWindowPos(GetForegroundWindow(), 0, _posX, _posY, _Txtwith, _Txheight, SWP_SHOWWINDOW);
    }
}
