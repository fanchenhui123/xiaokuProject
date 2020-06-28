using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class storeMgr : MonoBehaviour
{
    public Dropdown dropDownCT, dropDownCB;
    private List<Dropdown.OptionData> ctList=new List<Dropdown.OptionData>();
    private List<Dropdown.OptionData> cbList=new List<Dropdown.OptionData>();
    private  List<string> brandList=new List<string>();
    private List<PriceInfo> priceInfos =new List<PriceInfo>();
    public GameObject registorMgrItem;
    public Transform regisItemContainer;
    public GameObject addCarPanel;
    private List<GameObject> StoreCarItems=new List<GameObject>();
  //  public GameObject registorContent;
    private void OnEnable()
    { 
        priceInfos = PriceManager.Instance.priceInfos;//拿到读Excel表的数据
      
        DataForTwoDrop();//顶部两个DropDown的数据
      
        RefreshUi();//刷新UI
        Debug.Log("shuxin");
    }

    public void DataForTwoDrop()
    {
        Dropdown.OptionData opdaAll=new Dropdown.OptionData();
        opdaAll.text = "全部";
        cbList.Add(opdaAll);
        ctList.Add(opdaAll);
        for (int i = 0; i < PriceManager.Instance.carTypeList.Count; i++)
        {
            Dropdown.OptionData opda=new Dropdown.OptionData();
            opda.text = PriceManager.Instance.carTypeList[i];
            ctList.Add(opda);
        }
        dropDownCT.options = ctList;
      
        foreach (string brands in PriceManager.Instance.vehicleSystemsDic.Keys)
        {
            Dropdown.OptionData opda=new Dropdown.OptionData();
            opda.text = brands;
            brandList.Add(brands);
            cbList.Add(opda);
        }

        dropDownCB.options = cbList;
        dropDownCT.options = ctList;
        dropDownCB.onValueChanged.AddListener(CBTListener);
        dropDownCT.onValueChanged.AddListener(CBTListener);
    }

    public void RefreshUi()
    {
        Debug.Log("srefresh"+priceInfos.Count);
        GameObject gos;
        RegistorItem reItem;
        int countRe=1;
        string CurcarType=priceInfos[0].carType;
       
        for (int i = 0; i < StoreCarItems.Count; i++)
        {
            Destroy(StoreCarItems[i]);
        }
        StoreCarItems.Clear();
        
        
        for (int i = 0; i < priceInfos.Count; i++)
        {
            
            if (string.IsNullOrEmpty(priceInfos[i].adviser) && string.IsNullOrEmpty(priceInfos[i].userName) &&
                !string.IsNullOrEmpty(priceInfos[i].carNumber)) //客户用户都是空再显示
            {
               
                if (!string.IsNullOrEmpty(priceInfos[i].carType))
                {
                    CurcarType = priceInfos[i].carType;
                }


                if (SpecialCarr.instance != null && SpecialCarr.instance.TJSJ.Count>0) //当前车的车不是已经报价的特价车
                {
                    if (!SpecialCarr.instance.TJSJ.Contains(priceInfos[i].carNumber))
                    {
                        gos = Instantiate(registorMgrItem, regisItemContainer);
                        reItem = gos.GetComponent<RegistorItem>();
                        StoreCarItems.Add(gos);
                        reItem.SetItemContent(countRe.ToString(), priceInfos[i].carNumber, priceInfos[i].vehicleSystem,
                            priceInfos[i].memo, CurcarType); //库存管理显示的
                        countRe++;
                    }
                }
                else
                {
                    gos = Instantiate(registorMgrItem, regisItemContainer);
                    reItem = gos.GetComponent<RegistorItem>();
                    StoreCarItems.Add(gos);
                    reItem.SetItemContent(countRe.ToString(), priceInfos[i].carNumber, priceInfos[i].vehicleSystem,
                        priceInfos[i].memo, CurcarType); //库存管理显示的
                    countRe++;
                }
                
              
            }
            
        }
    }

    private void CBTListener(int index)
    {
        
        for (int i = 0; i < StoreCarItems.Count; i++)
        {
            if ( StoreCarItems[i]!=null)
            {
                StoreCarItems[i].SetActive(true);
            }
            
        }

        //Debug.Log(dropDownCT.value+"    "+dropDownCB.value);
      //  Debug.Log( ctList[dropDownCT.value-1].text+"   "+brandList[dropDownCB.value-1]);
        if (dropDownCB.value==0 && dropDownCT.value==0)
        {
            return;
        }

        if (dropDownCB.value==0 && dropDownCT.value!=0)
        {
           
            for (int i = 0; i < StoreCarItems.Count; i++)
            {
                if (StoreCarItems[i].GetComponent<RegistorItem>().text_type.text ==
                    ctList[dropDownCT.value].text)
                {
                    Debug.Log(StoreCarItems[i].GetComponent<RegistorItem>().text_type.text);
                    StoreCarItems[i].SetActive(true);
                }
                else
                {
                    StoreCarItems[i].SetActive(false);
                }

            }
        }
        
        if (dropDownCB.value!=0 && dropDownCT.value==0)
        {
            for (int i = 0; i < StoreCarItems.Count; i++)
            {
                if (StoreCarItems[i].GetComponent<RegistorItem>().text_series.text ==
                    brandList[dropDownCB.value-1])
                {
                    StoreCarItems[i].SetActive(true);
                }
                else
                {
                    StoreCarItems[i].SetActive(false);
                }

            }
        }

        if (dropDownCB.value!=0 && dropDownCT.value!=0)
        {
            for (int i = 0; i < StoreCarItems.Count; i++)
            {
                if (StoreCarItems[i].GetComponent<RegistorItem>().text_type.text==ctList[dropDownCT.value].text)
                {
                    if (StoreCarItems[i].GetComponent<RegistorItem>().text_series.text==brandList[dropDownCB.value-1] )
                    {
                        StoreCarItems[i].SetActive(true);
                    }
                    else
                    {
                        StoreCarItems[i].SetActive(false);
                    }
                }
                else
                {
                    StoreCarItems[i].SetActive(false);
                }
            }
        }
        
    }
     
     
     public void RemoveCar()
     {
        
         Toggle[] allRegistor = regisItemContainer.gameObject.GetComponentsInChildren<Toggle>().ToArray();
         string j=null;
         if (allRegistor.Length==0)
         {
             tip.instance.SetMessae("请选择要删除的车辆");
         }
        
         for (int i = 0; i < allRegistor.Length; i++)
         {
             if (allRegistor[i].isOn)
             {
                 j = (allRegistor[i].transform.parent.Find("Text_carNumber").GetComponent<Text>().text);
                 for (int k = 0; k < priceInfos.Count; k++)
                 {
                     if (priceInfos[k].carNumber==j && PriceManager.Instance!=null)
                     {
                         priceInfos.Remove(priceInfos[k]);
                     }
                 }
             }
         }

         
         for (int i = 0; i < StoreCarItems.Count; i++)
         {
             Destroy(StoreCarItems[i]);
         }
         StoreCarItems.Clear();
         RefreshUi();
        
         tip.instance.SetMessae("删除成功");
     }

     public void AddCar()
     {
         addCarPanel.SetActive(true);
         gameObject.SetActive(false);
     }

}
