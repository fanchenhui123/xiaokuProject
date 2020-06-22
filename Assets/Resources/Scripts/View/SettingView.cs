using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Windows;
using System.IO;

public class SettingView : MonoBehaviour
{
    public Button CloseBtn;
    public Button OpenBtn;
    public Button BrandBtn;
    public Button LoggerBtn;
    public Button ClearBtn;
    public Toggle IsAutoToggle;
    public Text OpenDirText;
    public AutoLoadExcelManager autoManager;
    public string defaultPath;
    public Text BackwardStateText;
    public GameObject BrandViewObj;
    public GameObject loggerObj;

    public Text IntervalText;
    public Slider IntervalSlider;

    private DBManager dBManager;
    // Start is called before the first frame update
    void Start()
    {
        dBManager = DBManager._DBInstance();
        autoManager = FindObjectOfType<AutoLoadExcelManager>();
        CloseBtn.onClick.AddListener(Close);
        OpenBtn.onClick.AddListener(OpenDir);
        IsAutoToggle.onValueChanged.AddListener((isRun) =>
        {
            autoManager.SetRun(isRun);
        });
        IntervalSlider.onValueChanged.AddListener((v) =>
        {
            IntervalText.text = v.ToString();
            PlayerPrefs.SetInt("Interval", (int)v);
        });
        BrandBtn.onClick.AddListener(()=> {
            BrandViewObj.GetComponent<Brandview>().Show(true);
        });
        LoggerBtn.onClick.AddListener(ShowLog);
        ClearBtn.onClick.AddListener(ClearData);
    }

    // Update is called once per frame
    void Update()
    {
        IntervalSlider.interactable = !autoManager.isRun;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        OpenDirText.text = autoManager.AutoDir;
        IntervalText.text = autoManager.Interval.ToString();
        IntervalSlider.value = autoManager.Interval;
    }

    void Close()
    {
        gameObject.SetActive(false);
    }

    void OpenDir()
    {
        OpenDialogDir ofn2 = new OpenDialogDir();
        ofn2.pszDisplayName = new string(new char[2000]);     // 存放目录路径缓冲区    
        ofn2.lpszTitle = "Open Project";// 标题   
        //ofn2.ulFlags = BIF_NEWDIALOGSTYLE | BIF_EDITBOX; // 新的样式,带编辑框    
        IntPtr pidlPtr = DllOpenDir.SHBrowseForFolder(ofn2);

        char[] charArray = new char[2000];
        for (int i = 0; i < 2000; i++)
            charArray[i] = '\0';

        DllOpenDir.SHGetPathFromIDList(pidlPtr, charArray);
        string fullDirPath = new String(charArray);

        fullDirPath = fullDirPath.Substring(0, fullDirPath.IndexOf('\0'));

        Debug.Log(fullDirPath);//这个就是选择的目录路径

        if (Directory.Exists(fullDirPath))
        {
            OpenDirText.text = fullDirPath;
            PlayerPrefs.SetString("DefaultPath", fullDirPath);
            autoManager.AutoDir = fullDirPath;
        }

    }

    void ShowLog()
    {
        if (loggerObj.activeSelf)
        {
            loggerObj.SetActive(false);
        }
        else
        {
            loggerObj.SetActive(true);
        }
    }

    void ClearData()
    {
        dBManager.DeleteTable("excel_info");
        dBManager.DeleteTable("tmp_excel_info");
    }
}
