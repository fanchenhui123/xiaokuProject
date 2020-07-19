using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class storeMgr : MonoBehaviour
{
    public Dropdown dropDownCT, dropDownCB;
    private List<Dropdown.OptionData> ctList=new List<Dropdown.OptionData>();
    private List<Dropdown.OptionData> cbList=new List<Dropdown.OptionData>();
    private  List<string> brandList=new List<string>();
    private List<PriceInfo> priceInfos;
    public GameObject registorMgrItem;
    public Transform regisItemContainer;
    public GameObject addCarPanel;
    public static List<GameObject> StoreCarItems=new List<GameObject>();
  //  public GameObject registorContent;
    private void OnEnable()
    {
        if (PriceManager.Instance!=null)
        {
            priceInfos = PriceManager.Instance.priceInfos;//拿到读Excel表的数据
        }
       
        DataForTwoDrop();//顶部两个DropDown的数据

        if (PriceManager.Instance.priceInfos!=null)
        {
            RefreshUi();//刷新UI
        }
       
    }

    public void DataForTwoDrop()
    {
        cbList.Clear();
        Dropdown.OptionData opdaAll=new Dropdown.OptionData();
        opdaAll.text = "全部";
        cbList.Add(opdaAll);
        ctList.Add(opdaAll);

        foreach ( var s in PriceManager.Instance.vehicleSystemsDic)
        {
            Dropdown.OptionData opda=new Dropdown.OptionData();
            opda.text = s.Key;
            cbList.Add(opda);
        }

        dropDownCB.options = cbList;
        dropDownCB.onValueChanged.AddListener(CBTListenerFirst);

        
    }
    

    public void CBTListenerFirst(int index)//根据CB读CT
    {
        for (int i = 0; i < StoreCarItems.Count; i++)
        {
            if (StoreCarItems[i].GetComponent<RegistorItem>().text_series.text ==
                dropDownCB.captionText.text)
            {
                StoreCarItems[i].SetActive(true);
            }
            else
            {
                StoreCarItems[i].SetActive(false);
            }
        }
        dropDownCT.value = 0;
        ctList.Clear();
        Dropdown.OptionData opdaAll=new Dropdown.OptionData();
        opdaAll.text = "全部";
        ctList.Add(opdaAll);
        foreach (var s in PriceManager.Instance.vehicleSystemsDic)
        {
            if (s.Key==dropDownCB.captionText.text)
            {
                for (int i = 0; i < s.Value.Count; i++)
                {
                    Dropdown.OptionData opda=new Dropdown.OptionData();
                    opda.text = s.Value[i].Replace("库存","");
                    ctList.Add(opda);
                }
            }
        }
       
        dropDownCT.options = ctList;
        dropDownCT.onValueChanged.AddListener(CBTListener);
    }

    public void RefreshUi()
    {
       // PriceManager.Instance.CleanBeforeUpdataUi();
       
       
        GameObject gos;
        RegistorItem reItem;
        int countRe=1;
        string CurcarType="";
        if (priceInfos.Count>0)
        {
            CurcarType=priceInfos[0].carType;
        }
      
        for (int i = 0; i < StoreCarItems.Count; i++)
        {
            Destroy(StoreCarItems[i]);
        }
        StoreCarItems.Clear();
        
        
        for (int i = 0; i < priceInfos.Count; i++)
        {
            if (!string.IsNullOrEmpty(priceInfos[i].carType)) //当表格空白时与上一次的一样
            {
                CurcarType = priceInfos[i].carType;

            }
            
            gos = Instantiate(registorMgrItem, regisItemContainer);
            reItem = gos.GetComponent<RegistorItem>();
            StoreCarItems.Add(gos);
            if (PriceManager.Instance.putSJ.Contains(priceInfos[i].carType))
            {
                 reItem.SetItemContent(countRe.ToString(), priceInfos[i].carNumber, priceInfos[i].vehicleSystem,
                                priceInfos[i].memo, CurcarType,"已上架"); //库存管理显示的
            }
            else
            {
                reItem.SetItemContent(countRe.ToString(), priceInfos[i].carNumber, priceInfos[i].vehicleSystem,
                    priceInfos[i].memo, CurcarType); //库存管理显示的
            }
          
            countRe++;
        }
    }

    private void CBTListener(int index)//车系dropdown的监听
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
            for (int i = 0; i < StoreCarItems.Count; i++)
            {
                if ( StoreCarItems[i]!=null)
                {
                    StoreCarItems[i].SetActive(true);
                }
            }
            
        }

        if (dropDownCB.value==0 && dropDownCT.value!=0)
        {
           
            for (int i = 0; i < StoreCarItems.Count; i++)
            {
                if (StoreCarItems[i].GetComponent<RegistorItem>().text_type.text ==
                    ctList[dropDownCT.value].text)
                {
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
                    dropDownCB.captionText.text)
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
                    if (StoreCarItems[i].GetComponent<RegistorItem>().text_series.text==dropDownCB.captionText.text )
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
     private List<string> needRemoveCarNum=new List<string>();
     private List<PriceInfo> needRemoveCarInfo=new List<PriceInfo>();
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
                        
                         needRemoveCarNum.Add(priceInfos[k].carNumber);
                         needRemoveCarInfo.Add(priceInfos[k]);
                     }
                 }
             }
         }

         for (int i = 0; i < needRemoveCarInfo.Count; i++)
         {
             PriceManager.Instance.priceInfos.Remove(needRemoveCarInfo[i]);
         }//本地priceinfos清除删除的
         
         for (int i = 0; i < needRemoveCarInfo.Count; i++)//如果清楚的有已将报价的，上传服务器删除
         {
             if (!PriceManager.Instance.putSJ.Contains(needRemoveCarInfo[i].carType))
             {
                 needRemoveCarInfo.Remove(needRemoveCarInfo[i]);
             }
         }
         
         StartCoroutine(PostNeedRemoveCar(needRemoveCarNum));
       
     }

     public void AddCar()
     {
         addCarPanel.SetActive(true);
         gameObject.SetActive(false);
     }


     private void OnDestroy()
     {
         dropDownCB.onValueChanged.RemoveAllListeners();
         dropDownCT.onValueChanged.RemoveAllListeners();
     }
     
     public IEnumerator PostNeedRemoveCar(List<string> carNumbers)
     {
         StringBuilder stringBuilder=new StringBuilder();
         for (int i = 0; i < carNumbers.Count-1; i++)
         {
             stringBuilder =stringBuilder.Append(carNumbers[i]).Append(',');
         }

         stringBuilder.Append(carNumbers[carNumbers.Count - 1]);
         WWWForm form=new WWWForm();
         form.AddField("car_numbers",stringBuilder.ToString());
         NetworkManager.Instance.DoPost(API.PostDeleteCarinfo, form,(responseCode,content) =>
         {
             if (responseCode=="200")
             {
                 for (int i = 0; i < StoreCarItems.Count; i++)
                 {
                     Destroy(StoreCarItems[i]);
                 }

                 for (int i = 0; i < needRemoveCarInfo.Count; i++)
                 {
                     PriceManager.Instance.priceInfos.Remove(needRemoveCarInfo[i]);
                 }
                 StoreCarItems.Clear();
                 RefreshUi();
                 PriceManager.Instance.SavePlayerJson(PriceManager.Instance.priceInfos);
                 tip.instance.SetMessae("删除成功");
                 needRemoveCarInfo.Clear();
                 Debug.Log(stringBuilder);
             
             }
             else
             {
                 tip.instance.SetMessae("删除失败"+responseCode);
             }
         },NetworkManager.Instance.token);
         
         /*UnityWebRequest request=new UnityWebRequest(API.PostDeleteCarinfo,"POST");
         request.SetRequestHeader("Authorization",   NetworkManager.Instance.token);
         request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
         request.SetRequestHeader("Accept", "application/json");
         request.uploadHandler=new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
         yield return request.SendWebRequest();*/
        yield break;  
     }
}
