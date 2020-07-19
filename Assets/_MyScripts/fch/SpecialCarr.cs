using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LitJson;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
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
  // private NetworkManager networkManager = NetworkManager.Instance;
  
   

   List<PriceInfo> SearchResults=new List<PriceInfo>();
   List<Dropdown.OptionData> carNumList=new List<Dropdown.OptionData>();
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
   public void  BtnSearch()
   {
      SearchResults.Clear();
      carNumList.Clear();
     
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
   

   public GameObject SpcialPriceCar;
   public void BtnBack()
   {
      SpcialPriceCar.SetActive(false);
   }

   public void BtnSave()
   {
      WWWForm form = new WWWForm();
      List<PriceInfo> tempInfoList=new List<PriceInfo>();
      tempInfoList.Add(needPost);
      if (string.IsNullOrEmpty(InfoTexts[0].text) || string.IsNullOrEmpty(InfoTexts[1].text) )
      {
         tip.instance.SetMessae("未选择车辆");
      }
      else
      {
         
         for (int i = 0; i < tempInfoList.Count; i++)
         {
            tempInfoList[i].vehicleSystem = tempInfoList[i].vehicleSystem.Replace("库存", "");

            string jsonString = JsonMapper.ToJson(tempInfoList[i]);
            JsonData jsonData = JsonMapper.ToObject(jsonString);
            jsonData["net_price"] = InfoTexts[2].text;
            jsonData["financial_agents_price"] = InfoTexts[6].text;
            jsonData["insurance_price"] =InfoTexts[4].text;
            jsonData["registration_price"] =  InfoTexts[3].text;
            jsonData["purchase_tax"] =InfoTexts[5].text;
            jsonData["other_price"] =InfoTexts[7].text;
            jsonData["boutique"] =InfoTexts[8].text;
            //  jsonData["registration_type"] = dropRegistrationType.options[dropRegistrationType.value].text;
            // jsonData["insurance_type"] = dropInsuranceType.options[dropInsuranceType.value].text;
            jsonData["content_remark"] =InfoTexts[9].text;//装饰费
            jsonData["appear_color"] = tempInfoList[i].color;
            jsonData["cart_price_type"] = "2";
            jsonData["vin"] =tempInfoList[i].carNumber ;
            jsonData["registration_area_type"] = currRegisterAreaType.ToString();

            for (int j = 0; j < jsonData.Count; j++)
            {
               if (jsonData[j]==null )
               {
                  jsonData[j] = "NA";
               }
            }
            //  Debug.Log("________准备上传的 jsonData:" + jsonData.ToJson());
            //jsonString = JsonMapper.ToJson(jsonData);
            string json = jsonData.ToJson();

            form.AddField("d[]", json);
         }
         NetworkManager.Instance.DoPost1(API.PostCarsInfo, form, (responseCode, content) =>
         {
            Debug.Log("____responseCode:" + responseCode + ", content:" +content );
            if (responseCode=="200")
            {
               tip.instance.SetMessae("保存成功");
               for (int i = 0; i < tempInfoList.Count; i++)
               {
                  if (!TJSJ.Contains(tempInfoList[i].carType))
                  {
                     TJSJ.Add(tempInfoList[i].carType); 
                  }

                  if (!PriceManager.Instance.putSJCB.Contains(tempInfoList[i].carNumber))
                  {
                     PriceManager.Instance.putSJCB.Add(tempInfoList[i].carNumber);
                  }

                  if (!PriceManager.Instance.putSJ.Contains(tempInfoList[i].carType))
                  {
                     PriceManager.Instance.putSJ.Add(tempInfoList[i].carType);
                  }
               }

 
               SearchResults.Clear();
               // CarNumText.text = "";
               for (int i = 0; i < InfoTexts.Count; i++)
               {
                  InfoTexts[i].text = "";
               }
            }
            else
            {
               tip.instance.SetMessae("保存失败："+JsonMapper.ToObject(content)["message"]);
            }
         }, NetworkManager.Instance.token);
         
      
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
