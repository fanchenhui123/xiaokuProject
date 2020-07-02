using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SpecialCarr : MonoBehaviour
{
   public static SpecialCarr instance;
   public Text CarNumText;
   public List<Text> InfoTexts=new List<Text>();
   private string currRegisterAreaType="3";
   public ToggleGroup togglGroup;
   public Toggle toggleCity, toggleProvince, toggleCountry;
   public PriceInfo needPost=new PriceInfo();
   public Dropdown SearchResDropdown;
   public List<Dropdown.OptionData>  optionList;//精品方案
   public List<string> TJSJ=new List<string>();//特价车上架
   private NetworkManager networkManager = NetworkManager.Instance;
   private void Awake()
   {
      instance = this;
      
      optionList = new List<Dropdown.OptionData>();
      if (NegotiatePrice.Instance!=null)
      {
         NegotiatePrice.Instance.OptionDatas = optionList;
      }
      toggleCity.onValueChanged.AddListener((value)=> {
         currRegisterAreaType = "3";
      });

      toggleProvince.onValueChanged.AddListener((value) => {
         currRegisterAreaType = "2";
      });

      toggleCountry.onValueChanged.AddListener((value) => {
         currRegisterAreaType = "1";
      });
   }

   private void OnEnable()
   {
     
   }

   List<PriceInfo> SearchResults=new List<PriceInfo>();
   List<Dropdown.OptionData> carNumList=new List<Dropdown.OptionData>();
   public void  BtnSearch()
   {
      SearchResults.Clear();
      carNumList.Clear();
      /*for (int i = 0; i < PriceManager.Instance.priceInfos.Count; i++)
      {
         if (string.IsNullOrEmpty(PriceManager.Instance.priceInfos[i].adviser) && string.IsNullOrEmpty(PriceManager.Instance.priceInfos[i].userName))
         {
            if (PriceManager.Instance.priceInfos[i].carNumber.Contains(CarNumText.text))
            {
               if (!PriceManager.Instance.putSJ.Contains(PriceManager.Instance.priceInfos[i].carType))
               {
                  if (!TJSJ.Contains(PriceManager.Instance.priceInfos[i].carNumber))
                  {
                     SearchResults.Add(PriceManager.Instance.priceInfos[i]);
                     Debug.Log(PriceManager.Instance.priceInfos[i].carType);
                  }
               }
            }
         }
        
      }*/
      for (int i = 0; i < storeMgr.StoreCarItems.Count; i++)
      {
         if (storeMgr.StoreCarItems[i].GetComponent<RegistorItem>().carNumber.Contains(CarNumText.text))
         {
            for (int j = 0; j < PriceManager.Instance.priceInfos.Count; j++)
            {
               if (PriceManager.Instance.priceInfos[j].carNumber==storeMgr.StoreCarItems[i].GetComponent<RegistorItem>().carNumber)
               {
               
                  PriceManager.Instance.priceInfos[j].carType =
                     storeMgr.StoreCarItems[i].transform.Find("Text_type").GetComponent<Text>().text;
                  SearchResults.Add(PriceManager.Instance.priceInfos[j]);
                 
               }
            }
             
         }
         
      }

      if (SearchResults.Count==0)
      {
         tip.instance.SetMessae("未查找到符合条件的结果",1.5f);
      }
      else if (SearchResults.Count==1)
      {
         Dropdown.OptionData opdt=new Dropdown.OptionData();
         opdt.text = SearchResults[0].carNumber;
         carNumList.Add(opdt);
         InfoTexts[0].text = SearchResults[0].carType;
         Debug.Log("   11   "+SearchResults[0].carType);
         InfoTexts[1].text = SearchResults[0].vehicleSystem;
         needPost=SearchResults[0];
      }
      else
      {
         for (int i = 0; i < SearchResults.Count; i++)
         {
            Dropdown.OptionData opdt=new Dropdown.OptionData();
            opdt.text = SearchResults[i].carNumber;
            carNumList.Add(opdt);
         } 
         SearchResDropdown.onValueChanged.AddListener(showSearchData);
      }
      
     
     
      SearchResDropdown.options = carNumList;
      //SearchResDropdown.onValueChanged.
   }

   private void showSearchData(int index)
   {
      InfoTexts[0].text = SearchResults[index].carType;
      InfoTexts[1].text = SearchResults[index].vehicleSystem;
      needPost=SearchResults[index];
   }

 
   /*IEnumerator getCarinfo()
   {
      UnityWebRequest request=new UnityWebRequest();
      request.url = API.GetCarInfo + "?cart_id=" + CarNumText.text;
      request.downloadHandler=new DownloadHandlerBuffer();
      yield return request.SendWebRequest();
      if (request.responseCode==200)
      {
         JsonData jsonData = JsonMapper.ToObject(request.downloadHandler.text);
         InfoTexts[0].text = jsonData["data"]["carType"].ToString();
         InfoTexts[1].text = jsonData["data"]["vehicleSystem"].ToString();
         if (jsonData["data"]["status"].ToString()=="1")
         {
            InfoTexts[2].text = jsonData["data"][""].ToString();
            InfoTexts[3].text = jsonData["data"]["vehicleSystem"].ToString();
            InfoTexts[4].text = jsonData["data"]["vehicleSystem"].ToString();
            InfoTexts[5].text = jsonData["data"]["vehicleSystem"].ToString();
            InfoTexts[6].text = jsonData["data"]["vehicleSystem"].ToString();
            InfoTexts[7].text = jsonData["data"]["vehicleSystem"].ToString();
            InfoTexts[8].text = jsonData["data"]["vehicleSystem"].ToString();
         }
      }

   }*/

   public GameObject SpcialPriceCar;
   public void BtnBack()
   {
      SpcialPriceCar.SetActive(false);
      
   }

   public void BtnSave()
   {
      if (string.IsNullOrEmpty(InfoTexts[0].text) || string.IsNullOrEmpty(InfoTexts[1].text) )
      {
         tip.instance.SetMessae("未选择车辆");
      }
      else
      {
        
         cost postCost=new cost();
         postCost.cart_id =needPost.id.ToString();
         postCost.carNumber = needPost.carNumber;
         postCost.net_price=  InfoTexts[2].text;
         postCost.registration_price= InfoTexts[3].text;
         postCost.insurance_price=InfoTexts[4].text;
         postCost.purchase_tax=InfoTexts[5].text;
         postCost.financial_agents_price= InfoTexts[6].text;
         postCost.other_price= InfoTexts[7].text;
         postCost.boutique= InfoTexts[8].text;
         postCost.content_remark= InfoTexts[9].text;//装饰费
         postCost.registration_area_type=currRegisterAreaType.ToString();
         postCost.offer_price = needPost.guidancePrice;
         postCost.vin = needPost.carNumber;
         postCost.cart_price_type = "2";//特价车
         StartCoroutine(coroutine.instance.PostTypePrice(postCost));

      
      }
   }

   
  

  
   public Text EditorText;
   public Dropdown EditorDropdown;

   public void addInfo()//添加按钮功能
   {
      Dropdown.OptionData optionData=new Dropdown.OptionData();
      optionData.text = EditorText.text;
      optionList.Add(optionData);
      
   }

   public void AdddropDownValue()
   {
      addInfo();
      InfoTexts[InfoTexts.Count - 1].text = EditorDropdown.captionText.text;
      EditorDropdown.options = optionList;
   }
   
   public void RemovedropDownValue()//删除按钮
   {
      optionList.Remove(optionList[EditorDropdown.value]);
   }

 
  
}
