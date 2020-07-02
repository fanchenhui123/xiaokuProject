using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
    public MsgCenterCtrl.MessageDataItem msg=new MsgCenterCtrl.MessageDataItem();
    //加数字的方法，小箭头的方法
    private InputField DK,SF,FQ,YG,LX,NM,ZJ;
    public GameObject  dialogItem;
    public Button btnConfirm;
    public List<Dropdown.OptionData> OptionDatas=new List<Dropdown.OptionData>();
    public string curOrderId="???";
    private MsgCenterCtrl.ReplyContent _replyContent;
    private void Awake()
    {
        Instance = this;
        SkipWindowPanel=MyLoginManager.instance.skipWindow;
        /*SF = GameObject.Find("SF").GetComponent<InputField>();
        FQ = GameObject.Find("FQ").GetComponent<InputField>();
        YG = GameObject.Find("YG").GetComponent<InputField>();
        LX = GameObject.Find("LX").GetComponent<InputField>();
        NM = GameObject.Find("name").GetComponent<InputField>();
        DK = GameObject.Find("DK").GetComponent<InputField>();
        ZJ = GameObject.Find("ZJ").GetComponent<InputField>();*/
        /*mehodTextList.Add(GameObject.Find("FAO").GetComponent<Text>());
        mehodTextList.Add(GameObject.Find("FAS").GetComponent<Text>());
        mehodTextList.Add(GameObject.Find("FAT").GetComponent<Text>());*/
        btnConfirm = SkipWindowPanel.transform.Find("Confirm").GetComponent<Button>();
        btnConfirm.onClick.AddListener(ShowWindow);
        //btnConfirm = GameObject.Find("keepLast").GetComponent<Button>();
       // btnConfirm.onClick.AddListener(ShowWindow);
       sendChatYJ.onClick.AddListener(SendChatYJ);//发送键添加监听
       // GameObject JPdrop = GameObject.FindGameObjectWithTag("JPDD");
       // JPdropDown = JPdrop.transform.GetComponent<Dropdown>();
      // JPdropDown.options = OptionDatas;
       
       

    }

    
    private void OnEnable()
    {
        StartCoroutine(GetYJinfo(curOrderId)); 
        InvokeRepeating("repeatRequest",60f,10000000000f);
      
    }


    /// <summary>
    /// 显示议价方案的基本信息，根据请求得到的数据获取
    /// </summary>
    /// <param name="message"></param>
    public void ShowData()
    {
       
        JsonData jsonData =JsonMapper.ToObject(resOrderInfo);
        for (int i = 0; i < jsonData.Count; i++)
        {
            if (jsonData[i]==null )
            {
                jsonData[i] = "NA";
            }
        }
       // Debug.Log(JsonMapper.ToJson(jsonData));
        Debug.Log(jsonData);
      //  JsonData jsonData = JsonMapper.ToObject(resOrderInfo);
        curOrderId=(jsonData["data"][0]["order_no"]).ToJson() ;
        for (int i = 0; i < jsonData["data"][0]["cart"].Count; i++)
        {
            if (jsonData["data"][0]["cart"][i]==null)
            {
                jsonData["data"][0]["cart"][i] = "NA";
            }
        }
        Texts[0].text =(jsonData["data"][0]["cart"]["carType"]).ToJson().Trim('"') ;
        Texts[1].text = (jsonData["data"][0]["cart"]["vehicleSystem"]).ToJson().Trim('"') ;
        Texts[2].text =(jsonData["data"][0]["cart"]["carNumber"]).ToJson().Trim('"') ;
        Texts[3].text =(jsonData["data"][0]["cart"]["appear_color"]).ToJson().Trim('"') ;
        Texts[4].text = (jsonData["data"][0]["cart"]["note"]).ToJson().Trim('"')+ "    "+ jsonData["data"][0]["cart"]["memo"].ToJson().Trim('"');//配置
        Texts[5].text ="" ;//显示接收的数据议价方案按揭资质等。
        
       
    }

    
    ////////////////////////////////////////////////////////////////以下是没用的方法了删除用户编辑方案的功能////////////////////////////////////////////
    
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
    public void FirstYJ()//第一次议价，选择方案，发送请求
    {
        string index = "";
        MsgCenterCtrl.YiJia firstYiJia=new MsgCenterCtrl.YiJia();
        firstYiJia.order_id = msg.order_no;
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
        StartCoroutine(postYJ(jsonData));
    }
    
   

    ////////////////////////////////////////////////////////////////以上是没用的方法了删除用户编辑方案的功能////////////////////////////////////////////
   

   

    public void ShowWindow()
    {
        Window.Skipwindow("确认方案并发送？",ConfirmYJMethod,SkipWindowPanel);
    }

    

    private void ConfirmYJMethod()
    {
        StartCoroutine(ConfirmMethod());
    }

    public IEnumerator ConfirmMethod()
    {
        MsgCenterCtrl.ConfirmYJ confirmYj=new MsgCenterCtrl.ConfirmYJ();
        confirmYj.order_id = curOrderId;
        string jsonstring = JsonMapper.ToJson(confirmYj);
        UnityWebRequest webRequest=new UnityWebRequest(API.PostConfirmYiJia);
        webRequest.uploadHandler=new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonstring));
        yield return  webRequest.SendWebRequest();
        if (webRequest.responseCode==200)
        {
            tip.instance.SetMessae("发送成功");
            SkipWindowPanel.SetActive(false);
        }
    }


    /////////////////////////////////////////////////////////////商家设置议价信息///////////////////////////////////////////
    /////////////////////////////////////////////////////////////显示议价信息////////////////////////////////////////
    

    
     public Button sendChatYJ,backToCard,ConfirmYJ;
   // private bool startUpdataMess;//是否开始频繁请求
    public Text chatPrice, chatMemo;
    private string ChatYJString;
    

  

  


    private void repeatRequest()
    {
        Debug.Log("再次请求");
        StartCoroutine(GetYJinfo(msg.order_no));//每隔一定时间请求获得数据

    }

    // Start is called before the first frame update
  
    
    public Transform dialogContainer;
   

    private void updateChatInfo()
    {
        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 0; i < dialogContainer.childCount; i++)
        {
            Destroy(dialogContainer.GetChild(i).gameObject);
        }

        JsonData jsonData = JsonMapper.ToObject(resOrderInfo);
        curOrderId = jsonData["data"][0]["order_no"].ToString();
        GameObject go;
        for (int i = 0; i < jsonData["data"][0]["repies"].Count; i++)
        {
            Debug.Log(" chat "+jsonData["data"][0]["repies"][i]["type"].ToJson());
            go = Instantiate(dialogItem, dialogContainer);
            if (jsonData["data"][0]["repies"][i]["type"].ToJson()=="1")
            {
             stringBuilder.Append(jsonData["data"][0]["name"]).Append(":  价格：").Append(jsonData["data"][0]["repies"][i]["price"]).Append("，备注：")
                    .Append(jsonData["data"][0]["repies"][i]["content"]); 
            }
            if (jsonData["data"][0]["repies"][i]["type"].ToJson()=="2")
            {
                stringBuilder.Append("我").Append(":  价格：").Append(jsonData["data"][0]["repies"][i]["price"]).Append("，备注：")
                    .Append(jsonData["data"][0]["repies"][i]["content"]); 
            }
            
            
            
            
          
           
            go.GetComponent<Text>().text = stringBuilder.ToString();
            stringBuilder.Clear();
        }

       
    }


    private string resOrderInfo = "??";
    public  IEnumerator GetYJinfo(string id)//获取订单详情
    {
        
        Debug.Log("start cor");
        UnityWebRequest request=new UnityWebRequest();
        request.downloadHandler=new DownloadHandlerBuffer();
        request.url = API._GetMsgList23+"?order_id="+id;
        
        yield return request.SendWebRequest();
        if (request.responseCode==200)//获取到数据后更新msg刷新聊天内容
        {
          
            resOrderInfo = request.downloadHandler.text;
            Debug.Log(resOrderInfo);
            /*jsonData = jsonData["data"]["repies"];
            _replyContent.id = Convert.ToInt32(jsonData["id"]);
            _replyContent.type = Convert.ToInt32(jsonData["type"]);
            _replyContent.order_id = Convert.ToInt32(jsonData["order_id"]);
            _replyContent.user_id = Convert.ToInt32(jsonData["user_id"]) ;
            _replyContent.merchant_id = jsonData["merchant_id"].ToString();
            _replyContent.id = Convert.ToInt32(jsonData["id"]) ;
            _replyContent.id = Convert.ToInt32(jsonData["id"]) ;
            _replyContent.id = Convert.ToInt32(jsonData["id"]) ;
            _replyContent.id = Convert.ToInt32(jsonData["id"]) ;*/
           



            // mess json = JsonMapper.ToJson(request.downloadHandler.text);
            //复制给新的msg
         //   msg = JsonMapper.ToObject<MsgCenterCtrl.MessageDataItem>(jsonData);
         updateChatInfo();
         ShowData();
           
        }
        else
        {
            Debug.Log("2222211112   " +request.responseCode);
        }
    }
    
    
    
    public void SendChatYJ()
    {
        //  btnChatSend.onClick.RemoveAllListeners();
        MsgCenterCtrl.YiJia ChatYiJia=new MsgCenterCtrl.YiJia();
       // msg = MsgCenterCtrl.Instance.chatMessage;
       ChatYiJia.order_id = msg.order_no;
       ChatYiJia.price = chatPrice.text;
        ChatYiJia.content = chatMemo.text;
        ChatYiJia.cart_loan_id = msg.cart_loan_id;
        ChatYiJia.cart_boutique_id = msg.cart_boutique_id.ToString();
        ChatYJString = JsonMapper.ToJson(ChatYiJia);
        StartCoroutine(postYJ(ChatYJString));
       
    }
    public IEnumerator postYJ(string jsonstring )//发送聊天议价信息
    {
        StringBuilder stringBuilder=new StringBuilder();
        UnityWebRequest webRequest=new UnityWebRequest(API.PostYiJia);
        webRequest.uploadHandler=new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonstring));
        yield return webRequest.SendWebRequest();
        if (webRequest.responseCode==200)
        {
            tip.instance.SetMessae("议价发送成功");
            GameObject go = Instantiate(dialogItem,dialogContainer);
            stringBuilder.Append("价格：") .Append(chatPrice.text).Append("备注：").Append(chatMemo.text);
            go.GetComponent<Text>().text = stringBuilder.ToString();
            chatPrice.transform.parent.GetComponent<InputField>().text = "";
            chatMemo.transform.parent.GetComponent<InputField>().text = "";
        }
        else
        {
            tip.instance.SetMessae("议价发送失败");
        }
    }

    public void backToMsg()
    {
        MsgCenterCtrl.Instance.ChangeToPage(1);
        MsgCenterCtrl.Instance.YJPage2.gameObject.SetActive(false);
    }

    public void ConfirmYJInfo()
    {
        
    }
    
    
}
