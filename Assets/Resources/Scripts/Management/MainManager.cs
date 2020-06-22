using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 程序整体控制入口
 */


public class MainManager : MonoBehaviour
{
    //初始化一些管理器
    public DelayPanelManager delayPanelManager;
    public MessagePanelManager messagePanelManager;
    public LoginManager loginManager;
    private DBManager dbManager;
    private DataManager dataManager;
    //页面默认打开状态

    //判断当前联网状态，如果无网络状态提示无网络，无网络提示全局

    //初始化数据库

    public void Awake()
    {
#if UNITY_EDITOR

#else
        
        //屏幕初始化
        //ScreenInit();
        //this.gameObject.AddComponent<WinConfig>();        
#endif
        //数据库初始化
        dbManager = DBManager._DBInstance();
        //登陆窗口Manager
        //其余窗口Manager
        //联网状态Manager
        //初始化消息提醒窗口信息Manager
        //登陆成功后数据库初始化
        //全局延时等待图片，可以起到屏蔽按钮功能
        //数据管理器初始化
        dataManager = DataManager.getInstance();
        ManagerInit();

        Debug.LogWarningFormat("获取当前分辨率:width:{0},height:{1}", Screen.width, Screen.height);

    }

    public void Start()
    {
        checkFirstOpen();
        StartCoroutine(CheckNetWork());        
    }

    //判断是否是第一次打开
    void checkFirstOpen()
    {
        int a = PlayerPrefs.GetInt("Init", -1);
        if (a != 1)
        {
            //第一次打开
            StartCoroutine(DelayAutoRun());
        }
        else
        {
            //非第一次打开
            StartCoroutine(DelayHide());
        }
    }

    IEnumerator DelayAutoRun()
    {
        yield return new WaitForSeconds(0.5f);
        AutoRun();
    }

    IEnumerator DelayHide()
    {
        yield return new WaitForSeconds(0.1f);
        Util.ShowWindow(Util.GetForegroundWindow(), 2);
    }

    public void ManagerInit()
    {
        dbManager.OpenConnect();
        //Debug.Log("ManagerInit 后续方法：dataManager.Init(dbManager)");÷
        dataManager.Init(dbManager);
        //Debug.Log("ManagerInit 后续方法：dataManager.Init(dbManager)");

    }

    protected void ScreenInit()
    {
        //Screen.SetResolution(1920, 1080, false);

    }


    private IEnumerator CheckNetWork()
    {
        yield return new WaitForSeconds(0.3f);
        delayPanelManager.Load();
        bool noNetWork = true;
        while (noNetWork)
        {
            WWW w = new WWW(@"http://icanhazip.com/");
            yield return w;
            if (w.text.Equals(""))
            {
                Debug.Log("无外网状态");
                messagePanelManager.Show("无外网状态");
                yield return new WaitForSeconds(3f);
            }
            else
            {
                Debug.Log("打印公网ip" + w.text);
                //loginManager.InitLoginPanel(w.text);
                noNetWork = false;
            }

        }
        delayPanelManager.Destory();
    }

    /// <summary>  
    /// 修改程序在注册表中的键值  
    /// </summary>  
    public void AutoRun()
    {
        try
        {
            string path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            RegistryKey rgkRun = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rgkRun == null)
            {
                rgkRun = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            }
            rgkRun.SetValue(Application.productName, path);
        }
        catch
        {
            Debug.Log(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

        }
        finally
        {
            regeditkey();
        }
    }

    /// <summary>
    /// 取消启动运行
    /// </summary>
    public void CancelAutoRun()
    {
        try
        {
            string path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            RegistryKey rgkRun = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                true);
            if (rgkRun == null)
            {
                rgkRun = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            }
            rgkRun.DeleteValue(Application.productName, false);
        }
        catch
        {
            Debug.Log("error");
        }
        finally
        {
            regeditkey();
        }

    }

    void regeditkey()
    {
        try
        {
            RegistryKey rgkRun = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rgkRun.GetValue(Application.productName) == null)
            {
                Debug.Log("自启动为关闭");
            }
            else
            {
                Debug.Log("自启动为打开");
                PlayerPrefs.SetInt("Init", 1);//设置成功
            }
        }
        catch
        {
            PlayerPrefs.SetInt("Init", -1);//设置失败
#if UNITY_EDITOR
            Debug.Log("无管理员权限");
#else
            MessageBox.Show(
                 "请右键以管理员身份运行!",
                 null,
                 (result) => { Exit(); }
             );
#endif
        }
    }

    void Exit()
    {
        Debug.Log("退出游戏");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
