using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NegotiatePrice : MonoBehaviour
{
    public static NegotiatePrice Instance;
    public List<Text> Texts=new List<Text>();
    private List<string> methodList=new List<string>();//金融方案
    private float num = 1f;
    private string carid;//当前议价车辆ID ，确认议价时需要
    public List<Toggle>  toggles;
    public GameObject SkipWindowPanel;//弹窗
    public Dropdown JPdropDown;//精品方案
    public List<Text> mehodTextList=new List<Text>();//金融方案的三个text
    //加数字的方法，小箭头的方法
    public void AddCount(InputField tt)
    {
        if (float.TryParse(tt.GetComponent<InputField>().text,out num))
        {
            num+=1;
            if (num<0)
            {
                num = 0;
            }
           
        }
        else
        {
            tt.text = "0";
        }
        tt.GetComponent<InputField>().text = num.ToString();
    }
    public void CutCount(InputField tt)
    {
        if (  float.TryParse(tt.GetComponent<InputField>().text,out num))
        {
            num-=1;
            if (num<0)
            {
                num = 0;
            }
        }
        else
        {
            tt.text = "0";
        }

        tt.GetComponent<InputField>().text = num.ToString();
    }
    private InputField DK,SF,FQ,YG,LX,NM,ZJ;
    private void Awake()
    {
        Instance = this;
        SkipWindowPanel=MyLoginManager.instance.skipWindow;
        SF = GameObject.Find("SF").GetComponent<InputField>();
        FQ = GameObject.Find("FQ").GetComponent<InputField>();
        YG = GameObject.Find("YG").GetComponent<InputField>();
        LX = GameObject.Find("LX").GetComponent<InputField>();
        NM = GameObject.Find("name").GetComponent<InputField>();
        DK = GameObject.Find("DK").GetComponent<InputField>();
        ZJ = GameObject.Find("ZJ").GetComponent<InputField>();
        mehodTextList.Add(GameObject.Find("FAO").GetComponent<Text>());
        mehodTextList.Add(GameObject.Find("FAS").GetComponent<Text>());
        mehodTextList.Add(GameObject.Find("FAT").GetComponent<Text>());
        btnConfirm = GameObject.Find("confirm").GetComponent<Button>();
        btnConfirm.onClick.AddListener(ShowWindow);
        btnConfirm = GameObject.Find("keepLast").GetComponent<Button>();
        btnConfirm.onClick.AddListener(ShowWindow);
      
       
        RefreshPriceMeth();
        
    }

    
    private void Start()
    {
       // SkipWindowPanel.SetActive(false);
        GameObject JPdrop = GameObject.FindGameObjectWithTag("JPDD");
        JPdropDown = JPdrop.transform.GetComponent<Dropdown>();
        Debug.Log(SpecialCarr.instance);
        JPdropDown.options = SpecialCarr.instance.optionList;
    }


    /// <summary>
    /// 新建贷款方案并显示
    /// </summary>
    public void CreatMethod()//根据输入添加方案
    {
        string methName = NM.text;
        methName = methName + " : 首付" + SF.text + "万元 > " +
                   FQ.text + "期 > 月供" + 
                   YG.text + "元 > 总利息" +
                   LX.text + "万元";
        if (methodList.Count<3)
        {
            methodList.Add(methName);
        }
        else
        {
            methodList.Add(methName);
            methodList.Remove(methodList[0]);
        }
        float total=0;
        if ( float.TryParse(DK.text,out num))
        {
            total += num;
            if (float.TryParse(SF.text,out num))
            {
                total += num;
            }
        }
        
        ZJ.GetComponent<InputField>().text = total.ToString();
        RefreshPriceMeth();
    }

    private void RefreshPriceMeth()//刷新金融方案列表
    {
        for (int i = 0; i < mehodTextList.Count; i++)
        {
            if (string.IsNullOrEmpty(mehodTextList[i].text) )
            {
                mehodTextList[i].transform.Find("Button").gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < methodList.Count; i++)
        {
            mehodTextList[i].text = "金融方案"+(i+1).ToString()+" : "+methodList[i];
            mehodTextList[i].transform.Find("Button").gameObject.SetActive(true);
            mehodTextList[i].transform.Find("index").GetComponent<Text>().text = (i+1).ToString();
        }

        Debug.Log(   methodList.Count);
    }
    public void DeleteMeth(GameObject o)//删除方案
    {
         Debug.Log("??"+   methodList.Count);
        int index=int.Parse(o.transform.GetComponent<Text>().text) ;
        Debug.Log("index "+index+"   "+methodList.Count);
        methodList.Remove(methodList[index-1]);
        RefreshPriceMeth();
    }

    /// <summary>
    /// 显示议价方案的基本信息，根据请求得到的数据获取
    /// </summary>
    /// <param name="message"></param>
    public void ShowData(MsgCenterCtrl.MessageDataItem message)
    {
        carid = message.cart_id.ToString();
        Texts[0].text = message.cart.carType;
        Texts[1].text = message.cart.vehicleSystem;
        Texts[2].text = message.cart.carNumber;
        Texts[3].text = message.cart.appear_color;
        Texts[4].text = message.cart.carType;
        Texts[5].text = message.cart.note;//配置
        Texts[6].text = message.cart.costInfo.content_remark;
    }

    public void FirstYJ()//第一次议价，选择方案，发送请求
    {
        string index = "";
        MsgCenterCtrl.YiJia firstYiJia=new MsgCenterCtrl.YiJia();
        firstYiJia.cart_id = Texts[0].text;
        firstYiJia.price=Texts[7].text;
        firstYiJia.content=Texts[5].text;
        for (int i = 0; i < toggles.Count; i++)
        {
            if (toggles[i].isOn)
            {
                index = (i + 1).ToString();
            }
        }
        firstYiJia.cart_loan_id=index;//金融方案
        firstYiJia.cart_boutique_id = JPdropDown.value.ToString();

        string jsonData = JsonMapper.ToJson(firstYiJia);
        StartCoroutine(ChatYJ.instance.postYJ(jsonData));
    }
    
   

    public GameObject  dialogItem;
   

    public Button btnConfirm;

    private void ShowWindow()
    {
        Window.Skipwindow("确认方案并发送？",Confirm,SkipWindowPanel);
    }

    

    private void Confirm()
    {
        StartCoroutine(ConfirmMethod());
    }

    public IEnumerator ConfirmMethod()
    {
        MsgCenterCtrl.ConfirmYJ confirmYj=new MsgCenterCtrl.ConfirmYJ();
        confirmYj.cart_id = carid;
        string jsonstring = JsonMapper.ToJson(confirmYj);
        UnityWebRequest webRequest=new UnityWebRequest(API.PostConfirmYiJia);
        webRequest.uploadHandler=new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonstring));
        yield return  webRequest.SendWebRequest();
        if (webRequest.responseCode==200)
        {
            tip.instance.SetMessae("确认议价发送成功");
            SkipWindowPanel.SetActive(false);
        }
    }


    /////////////////////////////////////////////////////////////商家设置议价信息///////////////////////////////////////////
    /////////////////////////////////////////////////////////////显示议价信息////////////////////////////////////////
    

}
