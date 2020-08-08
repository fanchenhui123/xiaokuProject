using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;

public class coroutine : MonoBehaviour
{
    //每次读表后请求数据库获取已经报价的车辆的信息，对比新旧两个表，更改、删除、新增车辆。
    public static coroutine instance;
    private NetworkManager networkManager;
    public float GetAllOrderListTime;
    public float time;
    public GameObject prompt;
    public Dictionary<string,string> messageDIc=new Dictionary<string, string>();
    public Dictionary<string,string> DicBrand=new Dictionary<string, string>();
    public List<MsgCenterCtrl.MessageDataItem> curCompletedOrders = new List<MsgCenterCtrl.MessageDataItem>();
    public List<MsgCenterCtrl.MessageDataItem> curNeedFinalConfirm = new List<MsgCenterCtrl.MessageDataItem>();
    public List<MsgCenterCtrl.MessageDataItem> curRepliesOrders = new List<MsgCenterCtrl.MessageDataItem>();
    private void Awake()
    {
        instance = this;
        networkManager=  NetworkManager.Instance;
        StartCoroutine(GetServerBrand());
       // StartCoroutine(GetServerVeh());
      time =Time.time;
      GetAllOrderListTime = Time.time;
      prompt.SetActive(false);
    
    }

    public List<PriceInfo> priceInfosLast=new List<PriceInfo>(); 
    public List<PriceInfo> priceInfos=new List<PriceInfo>();
    public List<PriceInfo> priceInfosAdd=new List<PriceInfo>();
    
    public List<string> priceInfosRemove=new List<string>();
   
    
  
/// <summary>
/// 对比两次读表数据
/// </summary>
/// <param name="newList">此次读表获取的数据  </param>
/// <param name="oldList">上次读表获取的数据  </param>
/// <param name="hadPrice">服务器获取到的已经报价的数据 </param>
/// <returns></returns>
    private IEnumerator CompareData(List<PriceInfo> newList,List<PriceInfo> oldList,List<PriceInfo> hadPrice)//对比数据
    {
        for (int i = 0; i < newList.Count; i++)
        {
            if (!oldList.Contains(newList[i]))
            {
                if (hadPrice.Contains(newList[i])|| PriceManager.Instance.putSJ.Contains(newList[i].carType))//新增的车辆且已经报价
                {
                    priceInfosAdd.Add(newList[i]);//需要报价的链表
                }
            }
        }

        for (int i = 0; i < oldList.Count; i++)
        {
            if (!newList.Contains(oldList[i]))
            {
                priceInfosRemove.Add(oldList[i].carNumber);
                newList.Remove(oldList[i]);
            }
        }
        
        
        /*StartCoroutine(PostNeedChangeData(priceInfosAdd));//新增数据发给服务器
        StartCoroutine(PostNeedRemoveCar(priceInfosRemove));//删除数据*/
        yield break;
    }


private void Update()
{
    //Debug.Log(Time.time-GetAllOrderListTime+"  ??");
    if (Time.time-time>1200f)
    {
        AutoLoadExcel();
    }


   // 一分钟重新获取订单信息，获取数据对比提示，Editor状态下Unity是没问题的，打包后没有提示。要做的人可以看一下BUG在哪，我实在不想搞了
   //
        // if ( Time.time - GetAllOrderListTime >= 60)
        // {
        //     Debug.Log("再次请求");
        //     StartCoroutine(GetYJinfo()); //每隔一定时间请求获得数据
        // }
    
    
}

public  IEnumerator GetYJinfo()//获取订单详情
{
    string url = API._GetMsgList1;
    NetworkManager.Instance.DoGet1(url, (responseCode, data) =>
    {
        if (responseCode == 200) //获取到数据后更新msg刷新聊天内容
        {
            if (NegotiatePrice.Instance != null)
            {
                NegotiatePrice.Instance.resOrderInfo = data;
                NegotiatePrice.Instance.needShow = true;
            }

           
            FlashWindow(data);

        }
        else
        {
           
            Debug.Log("responsecode  " + responseCode);
        }
      
    }, NetworkManager.Instance.token);
    GetAllOrderListTime = Time.time;   
    /*UnityWebRequest request=new UnityWebRequest();
    request.downloadHandler=new DownloadHandlerBuffer();
    request.url = API._GetMsgList1;//+"?order_id="+id;*/
    yield  break;// return request.SendWebRequest();
       
}

public void postPost(List<string>  strlist)
{
    StartCoroutine(PostNeedRemoveCar(strlist));
}

public IEnumerator PostNeedRemoveCar(List<string> carNumbers)
{
    if (carNumbers.Count>0)
    {
        Debug.Log("请求删除");
        StringBuilder stringBuilder=new StringBuilder();
        for (int i = 0; i < carNumbers.Count-1; i++)
        {
            stringBuilder =stringBuilder.Append(carNumbers[i]).Append(',');
        }
        stringBuilder.Append(carNumbers[carNumbers.Count - 1]);
        WWWForm form=new WWWForm();
        form.AddField("car_numbers",stringBuilder.ToString());
        NetworkManager.Instance.DoPost(API.PostDeleteCarinfo, form,(responseCode,content) =>
        {
            if (responseCode=="200")
            {
                Debug.Log("删除数据长度"+carNumbers.Count);
                tip.instance.SetMessae("删除成功");
                priceInfosRemove.Clear();
            }
            else
            {
                // Debug.Log("删除失败"+content.ToString());
                tip.instance.SetMessae("删除失败"+responseCode);
            }
        },NetworkManager.Instance.token);
            
    }
    else
    {
        tip.instance.SetMessae("没有需要删除的数据");
         
    }
    yield break;
}
 Dictionary<string, string> messageDicTempo = new Dictionary<string, string>();
public void FlashWindow(string data)
{
    
    GetAllMsg(data);

    if (curCompletedOrders.Count!=MsgCenterCtrl.Instance.completedOrders.Count || curRepliesOrders.Count!=MsgCenterCtrl.Instance.repliesOrders.Count || 
        curNeedFinalConfirm.Count!=MsgCenterCtrl.Instance.needFinalConfirm.Count)
    {
        Debug.Log("订单有变化");
        tip.instance.SetMessae("订单有变化");
        FlashWinTool.FlashWindow(FlashWinTool.GetProcessWnd());
        prompt.SetActive(true);
    }else   
    {
        if(curCompletedOrders.Count==MsgCenterCtrl.Instance.completedOrders.Count)
        {
             for(int i=0;i<curCompletedOrders.Count;i++)
             {
                 if(curCompletedOrders[i]!=MsgCenterCtrl.Instance.completedOrders[i])
                 {
                      tip.instance.SetMessae("订单有变化");
        FlashWinTool.FlashWindow(FlashWinTool.GetProcessWnd());
        prompt.SetActive(true);
        break; 
                 }
             }
        }
        
         if(curNeedFinalConfirm.Count==MsgCenterCtrl.Instance.needFinalConfirm.Count)
        {
             for(int i=0;i<curNeedFinalConfirm.Count;i++)
             {
                 if(curNeedFinalConfirm[i]!=MsgCenterCtrl.Instance.needFinalConfirm[i])
                 {
                      tip.instance.SetMessae("订单有变化");
        FlashWinTool.FlashWindow(FlashWinTool.GetProcessWnd());
        prompt.SetActive(true);
        break; 
                 }
             }
        }
         if(curRepliesOrders.Count==MsgCenterCtrl.Instance.repliesOrders.Count)
        {
             for(int i=0;i<curRepliesOrders.Count;i++)
             {
                 if(curRepliesOrders[i]!=MsgCenterCtrl.Instance.repliesOrders[i])
                 {
                      tip.instance.SetMessae("订单有变化");
        FlashWinTool.FlashWindow(FlashWinTool.GetProcessWnd());
        prompt.SetActive(true);
        break; 
                 }
             }
        }
    }
   
    

   
    JsonData jsonData = JsonMapper.ToObject(data);


   
    foreach (var dics in messageDicTempo)
    {
        if (messageDIc.ContainsKey(dics.Key))
        {
            if (messageDIc[dics.Key] != dics.Value)
            {
                FlashWinTool.FlashWindow(FlashWinTool.GetProcessWnd());
                tip.instance.SetMessae("有新消息");
                prompt.SetActive(true);
            }
        }
        else
        {
           
            FlashWinTool.FlashWindow(FlashWinTool.GetProcessWnd());
            tip.instance.SetMessae("有新订单");
            prompt.SetActive(true);
        }
    }

    messageDIc = messageDicTempo;
    MsgCenterCtrl.Instance.completedOrders = curCompletedOrders;
    MsgCenterCtrl.Instance.repliesOrders = curRepliesOrders;
    MsgCenterCtrl.Instance.needFinalConfirm = curNeedFinalConfirm;

}
private void AutoLoadExcel()
{
        Debug.Log("重新读取");
        tip.instance.SetMessae("自动重新读取表格");
        //考虑什么时候开始自动加载，决定了自动加载的路径是否是离线数据
        PriceManager.Instance.isNeedCompare = true;
        PriceManager.Instance. loadExcelsTest(PlayerPrefs.GetString("XiaoKuExcelPath"));
       
        time = Time.time;
}

    
    //一开始获取品牌列表--获取车系列表--获取车型列表。
    public IEnumerator GetServerBrand()
    {
        UnityWebRequest request=new UnityWebRequest();
        request.url = API.GetServerBrand;
        request.downloadHandler=new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.responseCode==200)
        {
            JsonData jsonData= JsonMapper.ToObject(request.downloadHandler.text)["data"];
            for (int i = 0; i < jsonData.Count; i++)
            {
               // Debug.Log("brand  "+jsonData[i]["title"].ToString());
                DicBrand.Add(jsonData[i]["id"].ToString(),jsonData[i]["title"].ToString());
            }
        }
        
        Debug.Log("bandid  ccount  "+   DicBrand.Count);
    }

    public IEnumerator GetServerVeh()
    {
        networkManager.DoGet1(API.GetServerVehcle+"?brand_id="+PlayerPrefs.GetString("brand_id"),
            (code, content) =>
            {
                if (code==200)
                {
                    JsonData jsonData= JsonMapper.ToObject(content)["data"];
                    Debug.Log("server vehic "+JsonMapper.ToJson(jsonData));
                    for (int i = 0; i < jsonData.Count; i++)
                    {
                        DicBrand.Add(jsonData[i]["id"].ToJson(),jsonData[i]["title"].ToJson());
                    }
                }
            },networkManager.token);
        
        /*UnityWebRequest request=new UnityWebRequest();
        request.url = API.GetServerVehcle+"?brand_id="+12;
        //request.SetRequestHeader("");
        request.downloadHandler=new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.responseCode==200)
        {
            JsonData jsonData= JsonMapper.ToObject(request.downloadHandler.text)["data"];
            Debug.Log("server vehic "+JsonMapper.ToJson(jsonData));
            for (int i = 0; i < jsonData.Count; i++)
            {
                DicBrand.Add(jsonData[i]["id"].ToJson(),jsonData[i]["title"].ToJson());
            }
        }*/
        yield break;
    }
    
       public void GetAllMsg(string data)
    {
                curCompletedOrders.Clear();
                curNeedFinalConfirm.Clear();
                curRepliesOrders.Clear();
                messageDicTempo.Clear();
                JsonData jsonData = JsonMapper.ToObject(data);
                messageDIc.Clear();
                for (int i = 0; i < jsonData["data"].Count; i++)
                {
                    messageDicTempo.Add(jsonData["data"][i]["id"].ToString(), jsonData["data"][i]["repies"].Count.ToString());
                }
                MsgCenterCtrl.Instance.msgItemList.Clear();
                foreach (JsonData obj in jsonData["data"])
                {
                    try
                    {
                        MsgCenterCtrl.MessageDataItem item = new MsgCenterCtrl.MessageDataItem();
                        item.id = int.Parse(obj["id"].ToString());
                      //  Debug.Log("item id   "+   item.id);
                        item.user_id = int.Parse(obj["user_id"].ToString());
                        item.cart_id = int.Parse(obj["cart_id"].ToString());

                        item.order_no = obj["order_no"].ToString();
                        
                        //Debug.Log("data "+obj["id"].ToString()+"  "+ item.status);
                        
                        item.cart = new MsgCenterCtrl.CarInfo();
                        JsonData jsonData_cart = obj["cart"];
                        if (jsonData_cart != null)
                        {
                            item.cart.carType = jsonData_cart["carType"].ToString();
                            item.cart.vehicleSystem = obj["cart"]["vehicleSystem"].ToString();

                            item.cart.carNumber = obj["cart"]["carNumber"].ToString();

                            if (obj["cart"]["appear_color"] != null)
                                item.cart.appear_color = obj["cart"]["appear_color"].ToString();           //null
                            else
                                item.cart.appear_color = "NA";
                        }

                        item.repies = new MsgCenterCtrl.ReplyContent[obj["repies"].Count];
                        for (int i = 0; i < item.repies.Length; i++)
                        {
                            MsgCenterCtrl.ReplyContent rc = new MsgCenterCtrl.ReplyContent();
                            rc.id = int.Parse(obj["repies"][i]["id"].ToString());
                            rc.price = obj["repies"][i]["price"].ToString();
                            rc.user_id = int.Parse(obj["repies"][i]["user_id"].ToString());
                            item.repies[i] = rc;
                        }

                        item.status = int.Parse(obj["status"].ToString());
                        item.last_reply_user_type = int.Parse(obj["last_reply_user_type"].ToString());

                        MsgCenterCtrl.Instance.msgItemList.Add(item);

                       // Debug.Log("item.status  "+ item.status);
                        if (item.status == 6)
                        {
                            curCompletedOrders.Add(item);
                           // MsgCenterCtrl.Instance. completedOrders.Add(item);          //交易完成
                        }
                        else if (item.status == 1)
                        {
                            curRepliesOrders.Add(item);
                          //  MsgCenterCtrl.Instance. repliesOrders.Add(item);        //议价列表
                            
                        }else if (item.status == 5)//需要商家查看确认
                        {
                            curNeedFinalConfirm.Add(item);
                           // MsgCenterCtrl.Instance. needFinalConfirm.Add(item);
                        }
                    }
                    catch (System.Exception E)
                    {
                        Debug.LogError(E.ToString());
                    }
                    
                }
    }

    /*public IEnumerator CreatCarTypeVehic(ChangeCarTypeVehic changeData )
    {
        
    }*/
    
   /*public IEnumerator PostTypePrice(List<cost> postCostList)//普通报价
   {
       int m = 0;
       int n = 0;
        for (int i = 0; i < postCostList.Count; i++)
        {
            JsonData js = JsonMapper.ToObject(JsonMapper.ToJson(postCostList[i]) );
            for (int j = 0; j < js.Count; j++)
            {
                if (js[i]==null)
                {
                    js[i] = "NA";
                }
            }
            string jstring = js.ToJson();
            UnityWebRequest request=new UnityWebRequest();
           // request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
          //  request.SetRequestHeader("Accept", "application/json");
          //  request.SetRequestHeader("Authorization", NetworkManager.Instance.token);
            request.url = API.PostCarsInfo;
            request.method = "POST";
            request.uploadHandler=new UploadHandlerRaw(Encoding.UTF8.GetBytes(jstring));
            request.downloadHandler=new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            if (request.responseCode==200)
            {
               // tip.instance.SetMessae("报价成功");
                m++;
                PriceManager.Instance.putSJ.Add(postCostList[i].carNumber);
            }
            else
            {
               // Debug.Log(JsonMapper.ToObject(request.downloadHandler.text)["message"] );
               // tip.instance.SetMessae(JsonMapper.ToObject(request.downloadHandler.text)["message"].ToString());
                n++;
            }

            jstring = "";
            js.Clear();
            request.Dispose();
        }
        
        tip.instance.SetMessae(m+"辆报价成功，"+n+"辆不存在",2f);
       
                
       PriceManager.Instance.ChangeToPage(1);
       PriceManager.Instance.UpdateUI();
       
       
    }*/
}

