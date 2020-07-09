using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class AddStoreMgr : MonoBehaviour
{
    //车库管理页面添加车辆
    public List<Text> texts=new List<Text>();//车型车系颜色指导价（车架号）备注
    private string _carCode;
    public int TJId=0;

    private void OnEnable()
    {
    }

    public void CreatCarCode()//生成车架号方法
    {
        _carCode += PlayerPrefs.GetString("username", "");
        for (int i = 0; i < 2; i++)
        {
            if (!string.IsNullOrEmpty(texts[i].text))
            {
                _carCode += PinYinHelper.ToPinYin(texts[i].text);
            }
            else
            {
               tip.instance.SetMessae("缺少信息"); 
                return;
            }
        }

        _carCode += "xxx";
       // texts[texts.Count-1].text = _carCode;
    }

    
    public GameObject storePanel;
    public void SendCarCode()//车架号发送给其他页面和服务器；添加保存按钮的方法,保存并退出
    {
        CreatCarCode();
        
        if (string.IsNullOrEmpty(texts[0].text)|| string.IsNullOrEmpty(texts[1].text))
        {
            tip.instance.SetMessae("缺少信息");
        }
        else
        {
            UpdateItems();
            _carCode = null;
            if (PriceManager.Instance.vehicleSystemsDic.ContainsKey(texts[0].text))
            {
                if (PriceManager.Instance.vehicleSystemsDic[texts[0].text].Contains(texts[1].text))
                {
                    return;
                }
                else
                {
                    PriceManager.Instance.vehicleSystemsDic[texts[0].text].Add(texts[1].text);
                }
            }
            else
            {
                List<string> carTypes=new  List<string>();
                carTypes.Add(texts[1].text);
                PriceManager.Instance.vehicleSystemsDic.Add(texts[0].text,carTypes);
                PriceManager.Instance.DoPostCarType();
            }
        }
    }

    
    
    private RegistorItem _registorItem;
    private GameObject _gameObject,go;

    public void UpdateItems()
    {
        PriceInfo _item = new PriceInfo();
        string defa="NA";
        _item.guidancePrice = texts[3].text;
        _item.carNumber = _carCode;
        _item.vehicleSystem = texts[0].text;
       _item.carType = texts[1].text.ToUpper();
       _item.brand =PlayerPrefs.GetString("brand_id") ;
        if (string.IsNullOrEmpty(texts[4].text))
        {
            _item.memo = "目前无现车，订车时间因颜色而定，需备注颜色";
        }
        else
        {
            _item.memo = texts[4].text;
        }
        _item.id = TJId++;
        _item.color = "全色";
        _item.discharge = defa;
        _item.releaseDate = defa;
        _item.arriveDate = defa;
        _item.garageAge = defa;
        _item.note = defa;
        _item.akkDate = defa;
        _item.qualityloss = defa;
        _item.certificate = defa;
        _item.userName = "";
        _item.adviser = "";
        _item.carGroup = defa;
        _item.signDate = defa;
        _item.useTime = defa;
        _item.payType = defa;
        
        for (int i = 0; i < texts.Count-1; i++)
        {
            texts[i].text = "";
        }
        PriceManager.Instance.priceInfos.Add(_item);
       PriceManager.Instance.StoreAddCar.Add(_item);
        tip.instance.SetMessae("添加成功");
        CloseAddCar();
    }

    public List<PriceInfo> StoreAddCar=new List<PriceInfo>();
   

   
    public void CloseAddCar()
    {
        storePanel.SetActive(true);
        gameObject.SetActive(false);
    }
}


