using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecondPanelCtrl : View
{
    public static SecondPanelCtrl Instance;

    public GameObject PageHome;
    public GameObject PagePriceManager;
    public GameObject PageMsgCenter;
    public GameObject PageOrderManager;
    public GameObject PageSettings;
   // public GameObject pageCarSourceMgr;
    public GameObject pageRegistorMgr;

    public Text textNickName;
    public Text textUserID;

    private NetworkManager networkManager;

    protected override void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        networkManager = NetworkManager.Instance;
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void RegisterMsg()
    {

    }

    public override void InitView()
    {

    }

    public void OnClickHomePage()
    {
        PageHome.SetActive(true);
    }

    public override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        switch (go.name)
        {
            case "btn_home":
                CloseAllPages();
                PageHome.SetActive(true);
                break;
            case "btn_priceManager":
                CloseAllPages();
                PagePriceManager.SetActive(true);
                break;
            case "btn_msgCenter":
                CloseAllPages();
                PageMsgCenter.SetActive(true);
                break;
            case "btn_orderManager":
                CloseAllPages();
                PageOrderManager.SetActive(true);
                break;
            case "btn_stock":
                //CloseAllPages();
                //PageStock.SetActive(true);
                break;
            case "btn_settings":
                CloseAllPages();
                PageSettings.SetActive(true);
                break;
            /*case "btn_carSourceMgr":
                CloseAllPages();
                pageCarSourceMgr.SetActive(true);
                break;*/
            case "btn_registorMgr":
                CloseAllPages();
                pageRegistorMgr.SetActive(true);
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// 用指定名称打开对应的页面；为首页顶部的两个按钮设计
    /// </summary>
    /// <param name="pageName"></param>
    public void OpenTargetPage(string pageName)
    {
        switch (pageName)
        {
            case "btn_priceManager":
                CloseAllPages();
                PagePriceManager.SetActive(true);
                break;
            case "btn_msgCenter":
                CloseAllPages();
                PageMsgCenter.SetActive(true);
                break;
            case "btn_orderManager":
                CloseAllPages();
                PageOrderManager.SetActive(true);
                break;
            default:
                break;
        }
    }


    public void CloseAllPages()
    {
        PageHome.SetActive(false);
        PagePriceManager.SetActive(false);
        PageMsgCenter.SetActive(false);
        PageOrderManager.SetActive(false);
        PageSettings.SetActive(false);
        //pageCarSourceMgr.SetActive(false);
        pageRegistorMgr.SetActive(false);
    }

    


}
