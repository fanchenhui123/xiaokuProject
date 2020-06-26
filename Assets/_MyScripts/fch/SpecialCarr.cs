using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SpecialCarr : MonoBehaviour
{
   public static SpecialCarr instance;
   public Text CarNumText;
   public List<Text> InfoTexts=new List<Text>();
   private string currRegisterAreaType;
   public ToggleGroup togglGroup;
   public Toggle toggleCity, toggleProvince, toggleCountry;
   PriceInfo needPost=new PriceInfo();
   private void Awake()
   {
      instance = this;
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
      for (int i = 0; i < PriceManager.Instance.priceInfos.Count; i++)
      {
         if (PriceManager.Instance.priceInfos[i].carNumber==CarNumText.text)
         {
            InfoTexts[0].text = PriceManager.Instance.priceInfos[i].carType;
            InfoTexts[1].text = PriceManager.Instance.priceInfos[i].vehicleSystem;
            needPost=(PriceManager.Instance.priceInfos[i]);
         }
      }
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
      StartCoroutine(PostNeedChangeData(needPost));
   }

   
   IEnumerator  PostNeedChangeData(PriceInfo ned)//post特价车
   {
      WWWForm form = new WWWForm();

      try
      {
         ned.vehicleSystem = ned.vehicleSystem.Replace("库存", "");
      }
      catch (Exception e)
      {
         Debug.Log(e);
        // throw;
      }
      string jsonString = JsonMapper.ToJson(ned);
         JsonData jsonData = JsonMapper.ToObject(jsonString);
         jsonData["net_price"] = InfoTexts[2].text;
         jsonData["registration_price"] = InfoTexts[3].text;
         jsonData["insurance_price"] = InfoTexts[4].text;
         jsonData["purchase_tax"] = InfoTexts[5].text;
         jsonData["financial_agents_price"] = InfoTexts[6].text;
         jsonData["other_price"] = InfoTexts[7].text;
         jsonData["content_remark"] = InfoTexts[8].text;//装饰费
                                                        //界面编辑的//的信息
         
         jsonData["cart_price_type"] ="2" ;//特价车
         jsonData["registration_area_type"] = currRegisterAreaType;//车的地区国 省 市
         Debug.Log("________准备上传的 jsonData:" + jsonData["net_price"]);
         jsonString = jsonData.ToJson();
         form.AddField("d[]", jsonString);
         PriceManager.Instance.networkManager.DoPost1(API._PostOfferPrice1, form, (responseCode, content) =>
      {
         if (responseCode=="200")
         {
            for (int i = 0; i < InfoTexts.Count; i++)
            {
               InfoTexts[i].text = "";
            }
         }
         Debug.Log("____responseCode:" + responseCode + ", content:" + content);
      },  PriceManager.Instance.networkManager.token);
      yield break;
   }

   public Text EditorText;
   public Dropdown EditorDropdown;
   public Button add, remove;
   public List<Dropdown.OptionData>  optionList=new List<Dropdown.OptionData>();//精品方案
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
