using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class AddStoreMgr : MonoBehaviour
{
    //车库管理页面添加车辆
    public List<Text> texts=new List<Text>();//车型车系颜色指导价（车架号）备注
    private string _carCode;
    
    
    
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
        if (string.IsNullOrEmpty(_carCode))
        {
            tip.instance.SetMessae("无车架号");
        }
        else
        {
            UpdateItems();
            _carCode = null;
        }
    }

    
    private PriceInfo _item=new PriceInfo();
    private RegistorItem _registorItem;
    private GameObject _gameObject,go;

    public void UpdateItems()
    {
        _item.carNumber = _carCode;
        _item.guidancePrice = texts[3].text;
        if (string.IsNullOrEmpty(texts[4].text))
        {
            _item.memo = "目前无现车，订车时间因颜色而定，需备注颜色";
        }
        else
        {
            _item.memo = texts[4].text;
        }
        string defa="Undefined";
        _item.carType = texts[1].text;
        _item.vehicleSystem = texts[0].text;
        _item.color = "全色";
        _item.brand = defa;
        _item.discharge = defa;
        _item.guidancePrice = defa;
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
        
        Debug.Log("priceinfo count add before "+PriceManager.Instance.priceInfos.Count);
        PriceManager.Instance.priceInfos.Add(_item);
        Debug.Log("priceinfo count add after "+PriceManager.Instance.priceInfos.Count);
        tip.instance.SetMessae("添加成功");
        for (int i = 0; i < texts.Count-1; i++)
        {
            texts[i].text = "";
        }
        CloseAddCar();
    }

    
   

   
    public void CloseAddCar()
    {
        storePanel.SetActive(true);
        gameObject.SetActive(false);
    }
}


