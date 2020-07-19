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
    private bool continueChat;
    public Button sendChatYJ;//发送议价消息
    public Text chatPrice, chatMemo;//价格，备注输入
    private string carid;//当前议价车辆ID ，确认议价时需要
    public List<Toggle>  toggles;
    public GameObject SkipWindowPanel;//弹窗
    public Dropdown JPdropDown;//精品方案
    public List<Text> mehodTextList=new List<Text>();//金融方案的三个text
    public MsgCenterCtrl.MessageDataItem msg=new MsgCenterCtrl.MessageDataItem();
    //加数字的方法，小箭头的方法
    private InputField DK,SF,FQ,YG,LX,NM,ZJ;
    public GameObject  dialogItem;
    private Button btnConfirm;
    public Button btnConfOrder;
    [HideInInspector]
    public List<Dropdown.OptionData> OptionDatas=new List<Dropdown.OptionData>();
    [HideInInspector]
    public string curOrderId="???";
    private MsgCenterCtrl.ReplyContent _replyContent;
  //  public int dataIndex;
    private NetworkManager _networkManager;
    public string resOrderInfo = "??";
    public bool needShow;
    private void Awake()
    {
        Instance = this;
        SkipWindowPanel=MyLoginManager.instance.skipWindow;
        btnConfirm = SkipWindowPanel.transform.Find("Confirm").GetComponent<Button>();
        btnConfirm.onClick.AddListener(ShowWindow);
        sendChatYJ.onClick.AddListener(SendChatYJ);//发送键添加监听
        _networkManager=NetworkManager.Instance;
        Debug.Log("curID  "+   curOrderId);
        btnConfOrder.onClick.AddListener(ShowWindow);
       
    }

    
    private void OnEnable()
    {
        updateChatInfo();
        ShowData();
    }

    private void Update()
    {
        if (needShow)
        {
            updateChatInfo();
        }
    }

    private void updateChatInfo()//显示聊天信息
    {
        StringBuilder stringBuilder = new StringBuilder();
        JsonData jsonData = JsonMapper.ToObject(resOrderInfo);
        JsonData datas=new JsonData();
        for (int i = 0; i < jsonData["data"].Count; i++)
        {
            if (jsonData["data"][i]["id"].ToJson() == curOrderId)
            {
                datas = jsonData["data"][i];
             //   Debug.Log("chat  curorderID  " + curOrderId);
                for (int j = 0; j < datas.Count; j++)
                {
                    if (datas[j]==null)
                    {
                        datas[j] = "NA";
                    }
                }
            }
        }
      
        for (int i = 0; i < dialogContainer.childCount; i++)
        {
            Destroy(dialogContainer.GetChild(i).gameObject);
        }

       

        GameObject go;
        Debug.Log("repies  "+datas["repies"].ToJson());
        for (int i = 0; i <datas["repies"].Count; i++)
        {
            go = Instantiate(dialogItem, dialogContainer);
            if (datas["repies"][i]["type"].ToJson()=="1")
            {
                stringBuilder.Append("价格：").Append(datas["repies"][i]["price"]).Append("万元，备注：")
                    .Append(datas["repies"][i]["content"]);
                go.GetComponent<Image>().color = new Color(155/255f,108/255f,72/255f,255/255f);
            }
            if (datas["repies"][i]["type"].ToJson()=="2")
            {
                stringBuilder.Append("价格：").Append(datas["repies"][i]["price"]).Append("万元，备注：")
                    .Append(datas["repies"][i]["content"]); 
                go.GetComponent<Image>().color = new Color(72/255f,138/255f,155/255f,255/255f);
            }
            
            go.transform.Find("dialogtext").GetComponent<Text>().text = stringBuilder.ToString();
            stringBuilder.Clear();
        }

        needShow = false;
    }


    /// <summary>
    /// 显示议价方案的基本信息，根据请求得到的数据获取
    /// </summary>
    /// <param name="message"></param>
    public void ShowData()//显示方案信息用户信息记录
    {
        for (int i = 0; i < Texts.Count; i++)
        {
            Texts[i].text = "";
        }
        
        
        JsonData jsonData =JsonMapper.ToObject(resOrderInfo);
       
        /*for (int i = 0; i < jsonData.Count; i++)
        {
            if (jsonData[i]==null )
            {
                jsonData[i] = "NA";
            }
        }
        if (jsonData["data"] == null)
        {
            
            return;
        }*/
        
       // Debug.Log("jsondata[data] "+jsonData["data"].ToJson());
        for (int i = 0; i < jsonData["data"].Count; i++)
        {
            if (jsonData["data"][i]["id"].ToJson() == curOrderId)
            {
                for (int j = 0; j < jsonData["data"][i].Count; j++)
                {
                    if (jsonData["data"][i][j] == null)
                    {
                        jsonData["data"][i][j] = "NA";
                    }
                }
                
                if (jsonData["data"][i]["cart"] != null && jsonData["data"][i]["cart"].ToJson().Trim('"')!="NA")
                {
                    Debug.Log(jsonData["data"][i]["cart"].ToJson());
                    for (int j = 0; j < (jsonData["data"][i]["cart"].Count); j++)
                    {
                        if (jsonData["data"][i]["cart"][j] == null)
                        {
                            jsonData["data"][i]["cart"][j] = "NA";
                        }
                    }
                    UTF8String utf1 = new UTF8String((jsonData["data"][i]["cart"]["carType"]).ToJson().Trim('"'));
                    UTF8String utf2 = new UTF8String((jsonData["data"][i]["cart"]["vehicleSystem"]).ToJson().Trim('"'));
                    UTF8String utf3 = new UTF8String((jsonData["data"][i]["cart"]["carNumber"]).ToJson().Trim('"'));
                    UTF8String utf4 = new UTF8String((jsonData["data"][i]["cart"]["appear_color"]).ToJson().Trim('"'));
                    UTF8String utf5 = new UTF8String((jsonData["data"][i]["cart"]["note"]).ToJson().Trim('"'));
                    UTF8String utf6 = new UTF8String(jsonData["data"][i]["cart"]["memo"].ToJson().Trim('"'));
                    Texts[0].text = utf1.ToString();
                    Texts[1].text = utf2.ToString();
                    Texts[2].text = utf3.ToString();
                    Texts[3].text = utf4.ToString();
                    Texts[4].text = utf5.ToString() + "    " + utf6.ToString(); //配置
                    Texts[5].text = ""; //按揭资质
                    Texts[11].text = jsonData["data"][i]["cart"]["cost"]["registration_price"].ToString();
                    Texts[12].text = jsonData["data"][i]["cart"]["cost"]["insurance_price"].ToString();
                    Texts[13].text = jsonData["data"][i]["cart"]["cost"]["purchase_tax"].ToString();
                    Texts[14].text = jsonData["data"][i]["cart"]["cost"]["financial_agents_price"].ToString();
                    Texts[15].text = jsonData["data"][i]["cart"]["cost"]["other_price_description"].ToString();
                    
                }
                else
                {
                    tip.instance.SetMessae("订单数据不完整");
                }
                
                
                   
                    Texts[6].text = jsonData["data"][i]["name"].ToString();
                    Texts[7].text = jsonData["data"][i]["phone"].ToString();
                    Texts[8].text = jsonData["data"][i]["id_card"].ToString();
                    Texts[9].text = jsonData["data"][i]["final_total"].ToString();
                    Texts[10].text = jsonData["data"][i]["pay_time"].ToString();

                    if (jsonData["data"][i]["status"].ToString() == "6")
                    {
                        btnConfOrder.onClick.RemoveAllListeners();
                        sendChatYJ.onClick.RemoveAllListeners();
                        continueChat = false;
                    }
                    else if (jsonData["data"][i]["status"].ToString() == "5")
                    {
                        btnConfOrder.onClick.RemoveAllListeners();
                        sendChatYJ.onClick.RemoveAllListeners();
                        continueChat = false;
                        WWWForm form = new WWWForm();
                        form.AddField("order_id", curOrderId);
                        _networkManager.DoPost1(API.SendOrderStatus, form, (responscode, content) =>
                            {
                                if (responscode == "200")
                                {
                                    tip.instance.SetMessae("订单完成");
                                }
                                else
                                {
                                    Debug.Log("订单未完成" + content);
                                    tip.instance.SetMessae(
                                        "订单未完成" + JsonMapper.ToObject(content)["message"].ToJson().ToString());
                                }
                            }
                            , _networkManager.token); // 发送状态6
                    }
                    else
                    {
                        continueChat = true;
                        requestEndTime = Time.time;
                    }
                    
                // curOrderId=(jsonData["data"][i]["id"]).ToJson() ;
                  break;
            }

            
            
            
            
        }
      
       
    }

    private float requestEndTime;
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
   

   

    public void ShowWindow()//确认议价方案弹窗
    {
        Window.Skipwindow("确认方案并发送？",ConfirmYJMethod,SkipWindowPanel);
    }
    private void ConfirmYJMethod()
    {
        StartCoroutine(ConfirmMethod());
    }
    public IEnumerator ConfirmMethod()//发送确认订单消息
    {
        MsgCenterCtrl.ConfirmYJ confirmYj=new MsgCenterCtrl.ConfirmYJ();
        confirmYj.order_id = curOrderId;
        string jsonstring = JsonMapper.ToJson(confirmYj);
        UnityWebRequest webRequest=new UnityWebRequest(API.PostConfirmYiJia);
        webRequest.method = "POST";
        webRequest.SetRequestHeader("Authorization",   NetworkManager.Instance.token);
        webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        webRequest.SetRequestHeader("Accept", "application/json");
        webRequest.uploadHandler=new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonstring));
        webRequest.downloadHandler=new DownloadHandlerBuffer();
        webRequest.SetRequestHeader(AppConst.AccessToken, NetworkManager.Instance.token);
        yield return  webRequest.SendWebRequest();
      
        if (webRequest.responseCode==200)
        {
            tip.instance.SetMessae("发送成功");
            SkipWindowPanel.SetActive(false);
            backToMsg();
        }
        else
        {
            tip.instance.SetMessae("发送失败:"+JsonMapper.ToObject(webRequest.downloadHandler.text)["message"]);
            /*if (webRequest.responseCode==400)
            {
                  tip.instance.SetMessae("请等待用户回复");
                  SkipWindowPanel.SetActive(false);
            }
            else
            {
                tip.instance.SetMessae("发送失败:"+webRequest.responseCode);
            }*/

           
            Debug.Log("发送失败:"+webRequest.downloadHandler.text);
        }
    }


    /////////////////////////////////////////////////////////////商家设置议价信息///////////////////////////////////////////
    /////////////////////////////////////////////////////////////显示议价信息////////////////////////////////////////
    

    
     
    private string ChatYJString;
    

  

  


 
    // Start is called before the first frame update
  
    
    public Transform dialogContainer;
   

  

    
  
    
    
    public void SendChatYJ()
    {
        //  btnChatSend.onClick.RemoveAllListeners();
        MsgCenterCtrl.YiJia ChatYiJia=new MsgCenterCtrl.YiJia();
       // msg = MsgCenterCtrl.Instance.chatMessage;
       if (string.IsNullOrEmpty( chatPrice.text))
       {
           tip.instance.SetMessae("请输入价格");
       }
       else
       {
           if (Encoding.UTF8.GetBytes(chatPrice.text).Length>180)
           {
               tip.instance.SetMessae("备注内容过长");
           }
           else
           {
               ChatYiJia.order_id = curOrderId.Trim('"');
               ChatYiJia.price = chatPrice.text;
               ChatYiJia.content = chatMemo.text;
               ChatYiJia.cart_loan_id = msg.cart_loan_id;
               ChatYiJia.cart_boutique_id = msg.cart_boutique_id.ToString();
               ChatYJString = JsonMapper.ToJson(ChatYiJia);
               StartCoroutine(postYJ(ChatYJString));
           }
          
       }
        
       
    }
    public IEnumerator postYJ(string jsonstring )//发送聊天议价信息
    {
        StringBuilder stringBuilder=new StringBuilder();
        UnityWebRequest webRequest=new UnityWebRequest(API.PostYiJia);
        webRequest.method = "POST";
        webRequest.SetRequestHeader("Authorization", NetworkManager.Instance.token);
        webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        webRequest.SetRequestHeader("Accept", "application/json");
        webRequest.uploadHandler=new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonstring));
        webRequest.downloadHandler=new DownloadHandlerBuffer();
        yield return webRequest.SendWebRequest();
        if (webRequest.responseCode==200)
        {
            tip.instance.SetMessae("议价发送成功");
            GameObject go = Instantiate(dialogItem,dialogContainer);
            stringBuilder.Append("价格：") .Append(chatPrice.text).Append("万元, 备注：").Append(chatMemo.text);
            go.transform.GetChild(0).GetComponent<Text>().text = stringBuilder.ToString();
            go.GetComponent<Image>().color = new Color(72/255f,138/255f,155/255f,255/255f);
            chatPrice.transform.parent.GetComponent<InputField>().text = "";
            chatMemo.transform.parent.GetComponent<InputField>().text = "";
        }
        else
        {
            tip.instance.SetMessae("议价发送失败:"+JsonMapper.ToObject(webRequest.downloadHandler.text)["message"].ToJson());
            Debug.Log(JsonMapper.ToObject(webRequest.downloadHandler.text)["message"]);
        }
    }

    public void backToMsg()
    {
        MsgCenterCtrl.Instance.ChangeToPage(1);
        MsgCenterCtrl.Instance.YJPage2.gameObject.SetActive(false);
    }

    public void ConfirmYJInfo()
    {
        if (string.IsNullOrEmpty(curOrderId) )
        {
            tip.instance.SetMessae("没有订单号");
            return;
        }
       
        WWWForm form=new WWWForm();
        form.AddField("order_id",curOrderId);
        _networkManager.DoPost1(API.PostConfirmYiJia,form, (responsecode, data) =>
        {
            if (responsecode=="200")
            {
                tip.instance.SetMessae("议价成功");
            }
            else if (responsecode=="400")
            {
                tip.instance.SetMessae(JsonMapper.ToObject(data)["message"].ToString());
            }
            else
            {
                tip.instance.SetMessae("议价失败："+responsecode);
            }
        },_networkManager.token);
    }
    
    
    
}
