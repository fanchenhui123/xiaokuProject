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
    List<string> _removeCarIndexList=new List<string>();

    private void Start()
    {
       
    }

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
               // Debug.Log("before "+PriceManager.Instance.priceInfos[28].carNumber);
                //removeCar.Add(allRegistor[i].transform.parent.gameObject);
                j = (allRegistor[i].transform.parent.Find("Text_carNumber").GetComponent<Text>().text);
                for (int k = 0; k < PriceManager.Instance.priceInfos.Count; k++)
                {
                   // Debug.Log(PriceManager.Instance.carNumberList[k]);
                    if (PriceManager.Instance.priceInfos[k].carNumber==j)
                    {
                        PriceManager.Instance.priceInfos.Remove(PriceManager.Instance.priceInfos[k]);
                       // Debug.Log(PriceManager.Instance.priceInfos[k].carNumber+"   "+k);
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
       // PriceManager.Instance.loadEnd = true;
        PriceManager.Instance.UpdateUI();
        /*Debug.Log(_removeCarIndexList.Count+"   选择的数量");
        for (int i = 0; i < _removeCarIndexList.Count; i++)
        {
            _removeCarPrice.Add( PriceManager.Instance.priceInfos[Convert.ToInt32( _removeCarIndexList[i])]);
        }
        
        for (int i = 0; i < _removeCarPrice.Count; i++)
        {
            Debug.Log("remove car"+_removeRegisItem[i].name);
            PriceManager.Instance.priceInfos.Remove(_removeCarPrice[i]);
            _removeRegisItem[i].SetActive(false);
        }

        _removeCarIndexList = null;*/
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


