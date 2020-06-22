using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickEvent : MonoBehaviour
{
    public void Refresh()
    {
        Debug.Log("Refresh test");
    }
    public void Download()
    {
        Debug.Log("Download test");
    }
    public void Account()
    {
        Debug.Log("Account test");
    }

    public void Option()
    {
        Debug.Log("Option test");
    }
    public void Zoom(int state)
    {
        Debug.Log("Zoom test "+state);
    }
    public void OpenExcel()
    {
        Debug.Log("OpenExcel");
        FindObjectOfType<ExcelManager>().OpenExcel();
    }
    public void LoadExcel()
    {
        Debug.Log("LoadExcel");
        FindObjectOfType<ExcelManager>().LoadExcel(0);
    }

    public void LoadCarType()
    {
        Debug.Log("LoadCarType");
        FindObjectOfType<ExcelManager>().LoadExcel(1);
    }

    public void ShowMini()
    {
        Util.ShowWindow(Util.GetForegroundWindow(), 2);
    }

    public void Quit()
    {
        Debug.Log("退出游戏");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
