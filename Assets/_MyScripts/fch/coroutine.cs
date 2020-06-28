﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;

public class coroutine : MonoBehaviour
{
    public static coroutine instance;

    private void Awake()
    {
        instance = this;
    }

    public List<PriceInfo> priceInfosLast=new List<PriceInfo>(); 
    public List<PriceInfo> priceInfos=new List<PriceInfo>();
    public List<PriceInfo> priceInfosAdd=new List<PriceInfo>();
    
    public List<string> priceInfosRemove=new List<string>();
    public  Dictionary<string,cost> dicItem=new Dictionary<string, cost>();//记录已经上传报价信息的车辆
    public void StartCompare()
    {
        StartCoroutine(CompareData());
    }

    private IEnumerator CompareData()//对比数据
    {
       
            for (int i = 0; i < priceInfos.Count; i++)
            {
               
                if (!priceInfosLast.Contains(priceInfos[i]))//老Excel表中包含了没有新Excel表数据，说明是新增数据
                {
                    if (dicItem.ContainsKey(priceInfos[i].carType))//且 上传的车型中有此新增的车型
                    {
                        priceInfosAdd.Add(priceInfos[i]);
                    }
                }
            }
            //新增的或有改动的数据
            
            
            for (int i = 0; i < priceInfosLast.Count; i++)
            {
                if (!priceInfos.Contains(priceInfosLast[i]))
                {
                    priceInfosRemove.Add(priceInfosLast[i].carNumber);
                }
            }
            //删除的数据
            

        StartCoroutine(PostNeedChangeData(priceInfosAdd));//新增数据发给服务器
        StartCoroutine(PostNeedRemoveCar(priceInfosRemove));//删除数据
        yield break;
    }

    public IEnumerator PostNeedChangeData(List<PriceInfo> needPost)
    {
        NetworkManager networkManager=new NetworkManager();
        WWWForm form = new WWWForm();
        for (int i = 0; i < needPost.Count; i++)
        {
            needPost[i].vehicleSystem = needPost[i].vehicleSystem.Replace("库存", "");
            cost cost = dicItem[needPost[i].carType];
            string jsonString = JsonMapper.ToJson(needPost[i]);
            JsonData jsonData = JsonMapper.ToObject(jsonString);
            jsonData["net_price"] = cost.net_price;
            jsonData["financial_agents_price"] = cost.financial_agents_price;
            jsonData["insurance_price"] = cost.insurance_price;
            jsonData["registration_price"] =cost.registration_price;
            jsonData["purchase_tax"] =cost.purchase_tax;
            jsonData["other_price"] = cost.other_price;
            
            jsonData["cart_price_type"] = cost.cart_price_type;
            jsonData["registration_type"] = "";
            jsonData["insurance_type"] = "";
            jsonData["content_remark"] = cost.content_remark;
            jsonData["appear_color"] =cost.appear_color;
            jsonData["registration_area_type"] = cost.registration_area_type;
            jsonString = jsonData.ToJson();
            form.AddField("d[]", jsonString);
        }
        networkManager.DoPost1(API._PostOfferPrice1, form, (responseCode, content) =>
        {
            Debug.Log("____responseCode:" + responseCode + ", content:" + content);
        }, networkManager.token);
        yield break;
    }

    public IEnumerator PostNeedRemoveCar(List<string> carNumbers)
    {
        carNumbs carNumbs=new carNumbs();
        StringBuilder stringBuilder=new StringBuilder();
        for (int i = 0; i < carNumbers.Count; i++)
        {
            stringBuilder = stringBuilder.Append(carNumbers[i]);
        }
        carNumbs.car_numbers = stringBuilder.ToString();
        String jsonData = JsonMapper.ToJson(carNumbers);
        UnityWebRequest request=new UnityWebRequest(API.PostDeleteCarinfo,"POST");
        request.uploadHandler=new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
        yield return request.SendWebRequest();
        if (request.responseCode==200)
        {
            tip.instance.SetMessae("对比数据后删除成功");
        }
        else
        {
            tip.instance.SetMessae("对比数据后删除失败");
        }
    }

}