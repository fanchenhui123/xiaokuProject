using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class AutoLoadExcelManager : MonoBehaviour
{
    public string AutoDir;
    public bool isRun = true;
    EventWaitHandle _waitHandle = new AutoResetEvent(false);//阻塞线程直到发出set消息
    public ExcelManager excelManager;
    string[] files;
    Thread thread;
    public Text stateText;
    public int Interval = 1;

    private static object objlock = new object();//读取对象锁

    // Start is called before the first frame update
    void Start()
    {
        Interval = PlayerPrefs.GetInt("Interval", 10);
        string defaultPath = System.Environment.CurrentDirectory + "\\Excels";
        AutoDir = PlayerPrefs.GetString("DefaultPath", defaultPath);
        if (!Directory.Exists(AutoDir)) AutoDir = defaultPath;
        Debug.Log(AutoDir);
        excelManager = FindObjectOfType<ExcelManager>();
    }

    //开始自动读取
    public void StartAutoLoad()
    {
        InvokeRepeating("_startThread", 1, Interval * 60);
    }

    void _startThread()
    {
        Debug.Log("开始循环");
        if (thread == null) thread = new Thread(new ThreadStart(ThreadMethod));
        Debug.Log(thread.IsAlive + "|" + excelManager.loadEnd + "|" + thread.ThreadState);
        if (thread.IsAlive)
        {
            Debug.Log("什么也不做");
        }
        else
        {
            Debug.Log("启动线程");
            thread = new Thread(new ThreadStart(ThreadMethod));
            thread.Start();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!excelManager.loadEnd)
        {
            _waitHandle.Set();
        }
    }

    void ThreadMethod()
    {
        Debug.Log("开始线程");
        while (isRun)
        {
            Thread.CurrentThread.Join(Interval * 60 * 1000);//阻止设定时间
            try
            {
                if (Directory.Exists(AutoDir))
                {
                    files = Directory.GetFiles(AutoDir);
                    foreach (var file in files)
                    {
                        Debug.Log("BackwardLoadExcel:" + file);
                        excelManager.BackwardLoadExcel(file);//定时后台加载
                        _waitHandle.WaitOne();
                    }
                }
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    private void OnApplicationQuit()
    {
        isRun = false;
        if (thread != null) thread.Abort();
    }

    //设置线程状态
    public void SetRun(bool isRun)
    {
        this.isRun = isRun;
        if (isRun == false)
        {
            thread.Abort();
        }
        else
        {
            Start();
        }

    }
}
