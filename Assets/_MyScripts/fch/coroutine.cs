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
    public int Interval = 20;
    private float time;
public Dictionary<string,string> DicBrand=new Dictionary<string, string>();
    private void Awake()
    {
        instance = this;
        networkManager=  NetworkManager.Instance;
        StartCoroutine(GetServerBrand());
       // StartCoroutine(GetServerVeh());
      time =Time.time;
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


private float mins=1200f;
private void Update()
{
    
    if (Time.time-time>1200f)
    {
        Debug.Log(Time.time.ToString()+"   "+(Time.time-time)+"   " );
        AutoLoadExcel();
    }
}

private void AutoLoadExcel()
{
        Debug.Log("zidong重新读取");
        tip.instance.SetMessae("自动重新读取表格");
        //考虑什么时候开始自动加载，决定了自动加载的路径是否是离线数据
        PriceManager.Instance. loadExcelsTest(PlayerPrefs.GetString("XiaoKuExcelPath"));
        PriceManager.Instance.isNeedCompare = true;
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
            Debug.Log("serverBrand "+request.downloadHandler.text);
            JsonData jsonData= JsonMapper.ToObject(request.downloadHandler.text)["data"];
            for (int i = 0; i < jsonData.Count; i++)
            {
                DicBrand.Add(jsonData[i]["id"].ToJson(),jsonData[i]["title"].ToString());
            }
            Debug.Log("server brand "+DicBrand.Count);
        }
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

