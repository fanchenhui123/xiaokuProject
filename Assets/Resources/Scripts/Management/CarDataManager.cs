using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

//品牌、车型、车系对比控制
public class CarDataManager : MonoBehaviour
{
    public NetworkManager networkManager;
    private DBManager dBManager;

    public List<BrandInfo> brandsList = new List<BrandInfo>();//品牌存放容器
    public List<CartLinesInfo> cartLinesList = new List<CartLinesInfo>();//车系存放容器
    public List<CartModelsInfo> cartModelsList = new List<CartModelsInfo>();//车型存放容器

    // Start is called before the first frame update
    void Start()
    {
        dBManager = DBManager._DBInstance();
        networkManager = FindObjectOfType<NetworkManager>();
        //GetAllCarData();//测试
    }

    // Update is called once per frame
    void Update()
    {

    }

    //搜索品牌
    public void DoSearchBrand(UnityAction callback = null)
    {
        brandsList.Clear();
        
        networkManager.DoGet(API.GetBrands, (responseCode, data) =>
        {
            Debug.Log("responseCode:" + responseCode + "|" + data);
            if (responseCode == 200)
            {
                JsonData jdata = JsonMapper.ToObject(data);
                if (jdata["code"].ToString() == "0")
                {
                    foreach (JsonData obj in jdata["brands"])
                    {
                        foreach (JsonData a in obj["label_brands"])
                        {
                            Debug.Log(a["id"] + "|" + a["title"]);
                            BrandInfo bitem = new BrandInfo();                      
                            bitem.id = int.Parse(a["id"].ToString());
                            bitem.title = a["title"].ToString();
                            bitem.logo = a["logo"].ToString();
                            brandsList.Add(bitem);
                        }
                    }
                }
            }
            StartCoroutine(SaveBrandToDB(callback));
        });
    }

    IEnumerator SaveBrandToDB(UnityAction callback = null)
    {
        foreach(var a in brandsList)
        {
            dBManager.Replace<BrandInfo>(a, true);
            yield return new WaitForEndOfFrame();
        }
        if (callback != null) callback();
    }

    //搜索车系
    public void DoCartLines(int brandID, UnityAction callback = null)
    {
        cartLinesList.Clear();   
        string url = API.GetCartLines + "?brand_id=" + brandID;
        Debug.Log(url);
        networkManager.DoGet(url, (responseCode, data) =>
        {
            Debug.Log("responseCode:" + responseCode + "|" + data);
            if (responseCode == 200)
            {
                JsonData jdata = JsonMapper.ToObject(data);
                if (jdata["code"].ToString() == "0")
                {
                    foreach (JsonData a in jdata["cartLines"])
                    {
                        Debug.Log(a["id"] + "|" + a["title"]);
                        CartLinesInfo citem = new CartLinesInfo();                   
                        citem.id = int.Parse(a["id"].ToString());
                        citem.title = a["title"].ToString();
                        citem.brand_id = int.Parse(a["brand_id"].ToString());
                        cartLinesList.Add(citem);
                    }

                }

            }
            StartCoroutine(SaveCartLinesToDB(callback));
        });
    }

    IEnumerator SaveCartLinesToDB(UnityAction callback = null)
    {
        foreach (var a in cartLinesList)
        {
            dBManager.Replace<CartLinesInfo>(a, true);
            yield return new WaitForEndOfFrame();
        }
        if (callback != null) callback();
    }

    //搜索车型
    public void DoCartModels(int cartLineID, UnityAction callback = null)
    {
        cartModelsList.Clear();
        string url = API.GetCartModels + "?cart_line_id=" + cartLineID;
        networkManager.DoGet(url, (responseCode, data) =>
        {
            Debug.Log("responseCode:" + responseCode + "|" + data);
            if (responseCode == 200)
            {
                JsonData jdata = JsonMapper.ToObject(data);
                if (jdata["code"].ToString() == "0")
                {
                    foreach (JsonData a in jdata["cartModels"])
                    {
                        Debug.Log(a["id"] + "|" + a["title"]);
                        CartModelsInfo ccitem = new CartModelsInfo();                   
                        ccitem.id = int.Parse(a["id"].ToString());
                        ccitem.title = a["title"].ToString();
                        ccitem.cart_line_id = int.Parse(a["cart_line_id"].ToString());
                        ccitem.price = a["price"].ToString();
                        ccitem.appear_color = a["appear_color"].ToString();
                        cartModelsList.Add(ccitem);
                    }

                }

            }
            StartCoroutine(SaveCartModelsToDB(callback));
        });
    }

    IEnumerator SaveCartModelsToDB(UnityAction callback = null)
    {
        foreach (var a in cartModelsList)
        {
            dBManager.Replace<CartModelsInfo>(a, true);
            yield return new WaitForEndOfFrame();
        }
        if (callback != null) callback();
    }


    //获取后台所有车系列和车型数据
    public void GetAllCarData()
    {
        DoSearchBrand(GetCarLinesData);
        Debug.LogError("GetAllCarData End");
    }

    void GetCarLinesData()
    {
        for (int i = 0; i < brandsList.Count; i++)
        {

            DoCartLines(brandsList[i].id, GetCarModelsData);
        }
    }

    void GetCarModelsData()
    {
        for (int i = 0; i < cartLinesList.Count; i++)
        {
            DoCartModels(cartLinesList[i].id);
        }
    }
}
