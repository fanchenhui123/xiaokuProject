using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class carSourceMgr : MonoBehaviour
{
    public List<Text> texts=new List<Text>();
    private string _carCode;

    public void CreatCarCode()//生成车架号方法
    {
       //_carCode += LoginManager.Instance.userId;
        for (int i = 0; i < 2; i++)
        {
            if (!string.IsNullOrEmpty(texts[i].text))
            {
                _carCode += texts[i].text;
            }
            else
            {
                Debug.Log("缺少信息");
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
            Debug.Log("无车架号信息");
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
    public GameObject registorMgrItem;
    public Transform regisItemContainer;
    private PriceManagerItem item;
    public GameObject priceManagerItem, itemContainer;
    public void UpdateItems()
    {
        _item.carNumber = _carCode;
        _item.memo = texts[3].text;
        _item.carType = texts[1].text;
        _item.vehicleSystem = texts[0].text;
        PriceManager.Instance.priceInfos.Add(_item);
        /*//读取填写的数据并加入到priceInfos里；
        _gameObject = Instantiate(registorMgrItem,regisItemContainer);
        _registorItem = _gameObject.GetComponent<RegistorItem>();
        _registorItem.SetItemContent(PriceManager.Instance.count.ToString(),_carCode,texts[0].text,texts[3].text,texts[1].text);
        //车库管理页面添加此条信息
        go = Instantiate(priceManagerItem, itemContainer.transform);
        item = go.GetComponent<PriceManagerItem>();
        item.SetItemContent(PriceManager.Instance.count.ToString(), "",
            texts[0].text,texts[1].text, "未上架");//订单管理显示的*/
        //价格管理页面添加此条信息
    }

    List<string> _removeCarIndexList=new List<string>();
    public void countToggle(Transform transform)
    {
        if (transform.GetComponent<Toggle>().isOn)
        {
           _removeCarIndexList.Add(transform.parent.Find("Text_index").GetComponent<Text>().text);
        }
        else
        {
            if (_removeCarIndexList.Contains(transform.parent.Find("Text_index").GetComponent<Text>().text))
            {
                _removeCarIndexList.Remove(transform.parent.Find("Text_index").GetComponent<Text>().text);
            }
        }
    }

    List<PriceInfo> _removeCarPrice=new List<PriceInfo>();
    public void RemoveCar()
    {
        for (int i = 0; i < _removeCarIndexList.Count; i++)
        {
            _removeCarPrice.Add( PriceManager.Instance.priceInfos[Convert.ToInt32( _removeCarIndexList[i])]);
        }

        for (int i = 0; i < _removeCarPrice.Count; i++)
        {
            PriceManager.Instance.priceInfos.Remove(_removeCarPrice[i]);
        }
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


