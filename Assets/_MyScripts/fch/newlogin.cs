using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using Newtonsoft.Json;
using Unity.Messenger.Json;
using Unity.UIWidgets.external.simplejson;
using UnityEngine;
using JsonArray = Unity.DocZh.Utility.Json.JsonArray;

public class newlogin : MonoBehaviour
{
    public string cityDataStr;
    public Dictionary<string,string> DicProvineToCity=new Dictionary<string, string>();
    private void Awake()
    {
        cityDataStr = File.ReadAllText(Application.streamingAssetsPath + "/CityData.json");
        JsonData cityDataJson = JsonMapper.ToObject(cityDataStr);//转成json格式
        string provinceStr = JsonMapper.ToJson(cityDataJson["provinces"]);//拿到省的string
        JsonArray jsonArray = JsonConvert.DeserializeObject(provinceStr) as JsonArray;
       // List<string> jsonList = JsonMapper.ToObject<List<string>>(provinceStr);//string 转List
        Debug.Log(jsonArray);
        /*for (int i = 0; i < jsonList.Count; i++)
        {
            JsonData jsonData = JsonMapper.ToJson(jsonList[i]);
            DicProvineToCity.Add(jsonData[]);
        }*/
    }
    
}
