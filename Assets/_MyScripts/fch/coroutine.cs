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
    private float GetAllOrderListTime;
    public float time;
    public GameObject prompt;
    public Dictionary<string,string> messageDIc=new Dictionary<string, string>();
    public Dictionary<string,string> DicBrand=new Dictionary<string, string>();
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
    
    if (Time.time-time>120f)
    {
        AutoLoadExcel();
    }


    if ( NegotiatePrice.Instance!=null)
    {
        if ( Time.time - GetAllOrderListTime >= 60)
        {
            Debug.Log("再次请求");
            StartCoroutine(GetYJinfo()); //每隔一定时间请求获得数据
        }
    }
    
}

public  IEnumerator GetYJinfo()//获取订单详情
{
    Debug.Log("获取议价信息");
    string url = API._GetMsgList1;
    networkManager.DoGet1(url, (responseCode, data) =>
    {
        if (responseCode == 200) //获取到数据后更新msg刷新聊天内容
        {
            NegotiatePrice.Instance.resOrderInfo = data;
            Debug.Log("all response  " + data);
            GetAllOrderListTime = Time.time;
            NegotiatePrice.Instance.needShow = true;
            FlashWindow(data);
        }
        else
        {
            Debug.Log("responsecode  " + responseCode);
        }
    }, networkManager.token);
        
    /*UnityWebRequest request=new UnityWebRequest();
    request.downloadHandler=new DownloadHandlerBuffer();
    request.url = API._GetMsgList1;//+"?order_id="+id;*/
    yield  break;// return request.SendWebRequest();
       
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

private void FlashWindow(string data)
{
    JsonData jsonData = JsonMapper.ToObject(data);
    if (messageDIc.Count==0)
    {
        for (int i = 0; i < jsonData["data"].Count; i++)
        {
            messageDIc.Add(jsonData["data"][i]["id"].ToJson(),jsonData["data"][i]["repies"].Count.ToString());
        }
    }
    else
    {
        if ( jsonData["data"].Count-messageDIc.Count>0)
        {
            FlashWinTool.FlashWindow(FlashWinTool.GetProcessWnd());
            tip.instance.SetMessae("有新订单");
            prompt.SetActive(true);
            return;
        }
        if ( jsonData["data"].Count==messageDIc.Count)
        {
            Dictionary<string,string> messageDicTempo=new Dictionary<string, string>();
            for (int i = 0; i < jsonData["data"].Count; i++)
            {
                messageDicTempo.Add(jsonData["data"][i]["id"].ToJson(),jsonData["data"][i]["repies"].Count.ToString());
            }

            foreach (var dics in messageDicTempo)
            {
                if (messageDIc.ContainsKey(dics.Key))
                {
                    if (messageDIc[dics.Key]!=dics.Value)
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
        }
    } 
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

