using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class carSourceMgr : MonoBehaviour
{
    public List<Text> texts=new List<Text>();
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
                texts[texts.Count - 1].text = "缺少信息";
                return;
            }
        }

        _carCode += "xxx";
        texts[texts.Count-1].text = _carCode;
    }


    public void SendCarCode()//车架号发送给其他页面和服务器；添加保存按钮的方法
    {
        if (string.IsNullOrEmpty(_carCode))
        {
            texts[texts.Count - 1].text = "无车架号";
        }
        else
        {

            UpdateItems();
            _carCode = null;
            texts[texts.Count - 1].text = null;
        }
        
        
    }

    private PriceInfo _item=new PriceInfo();
    private RegistorItem _registorItem;
    private GameObject _gameObject,go;
    private PriceManagerItem item;
    
    public void UpdateItems()
    {
        _item.carNumber = _carCode;
        if (string.IsNullOrEmpty(texts[3].text))
        {
            _item.memo = "目前无现车，订车时间因颜色而定，需备注颜色";
        }
        else
        {
            _item.memo = texts[3].text;
        }
        
        _item.carType = texts[1].text;
        _item.vehicleSystem = texts[0].text;
        PriceManager.Instance.priceInfos.Add(_item);
      
    }

   
    
    
    public void CountToggle(Transform transf)
    {
       /*// Debug.Log(_removeCarIndexList.Count);
        for (int i = 0; i < _removeCarIndexList.Count; i++)
        {
            Debug.Log(_removeCarIndexList[i]);
            if (Convert.ToInt32(_removeCarIndexList[i])>PriceManager.Instance.priceInfos.Count )
            {
                _removeCarIndexList.Remove(_removeCarIndexList[i]);
            }
        }
        
        //Debug.Log("选的的序号是： "+transf.parent.Find("Text_index").GetComponent<Text>().text);
        if (transf.GetComponent<Toggle>().isOn)
        {
           _removeCarIndexList.Add(transf.parent.Find("Text_index").GetComponent<Text>().text);
           _removeRegisItem.Add(transf.parent.gameObject);
           //Debug.Log("add  "+transf.parent.Find("Text_index").GetComponent<Text>().text);
        }
        else
        {
            if (_removeCarIndexList.Contains(transf.parent.Find("Text_index").GetComponent<Text>().text))
            {
                _removeCarIndexList.Remove(transf.parent.Find("Text_index").GetComponent<Text>().text);
                _removeRegisItem.Remove(transf.parent.gameObject);
            }
        }
        Debug.Log("  已经共选择了几个 "+_removeCarIndexList.Count);*/
    }
    public Transform registorContent;
   // List<GameObject> removeCar=new List<GameObject>();
    public void RemoveCar()
    {
        
        Toggle[] allRegistor = registorContent.GetComponentsInChildren<Toggle>().ToArray();
        string j=null;
        for (int i = 0; i < allRegistor.Length; i++)
        {
            if (allRegistor[i].isOn)
            {
               
                j = (allRegistor[i].transform.parent.Find("Text_carNumber").GetComponent<Text>().text);
                for (int k = 0; k < PriceManager.Instance.priceInfos.Count; k++)
                {
                    if (PriceManager.Instance.priceInfos[k].carNumber==j)
                    {
                        PriceManager.Instance.priceInfos.Remove(PriceManager.Instance.priceInfos[k]);
                    }
                }
            }
        }

       
        for (int i = 0; i < PriceManager.Instance.priceItems.Count; i++)
        {
            Destroy(PriceManager.Instance.priceItems[i]);
        }

        for (int i = 0; i < PriceManager.Instance.registorItems.Count; i++)
        {
            Destroy(PriceManager.Instance.registorItems[i]);
        }
        PriceManager.Instance.priceItems.Clear();
        PriceManager.Instance.carTypeList.Clear();
        PriceManager.Instance.count = 1;
        PriceManager.Instance.UpdateUI();
    }

    public void OpenAddCar(Transform trans)
    {
        trans.gameObject.SetActive(true);
    }
    public void CloseAddCar(Transform trans)
    {
        trans.gameObject.SetActive(false);
    }
}


