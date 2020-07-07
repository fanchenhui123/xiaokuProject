using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccountReset : MonoBehaviour
{
   public List<Text> infoTexts=new List<Text>();
    
   public void SendResetInfo()
   {
      AccountResInfo accountResInfo=new AccountResInfo();
      accountResInfo.email = "wfw2113@qq.com";// PlayerPrefs.GetString("username").Trim('"');
      accountResInfo.merchant = infoTexts[0].text;
      accountResInfo.password= infoTexts[1].text;
      accountResInfo.ConfirmPassword= infoTexts[2].text;
      accountResInfo.address= infoTexts[3].text;
      accountResInfo.ConnectPerson= infoTexts[4].text;
      accountResInfo.ConnectPersonPhone= infoTexts[5].text;
      accountResInfo.nicname= infoTexts[6].text;
      accountResInfo.BankAccountName= infoTexts[7].text;
      accountResInfo.BankAccount= infoTexts[8].text;
      accountResInfo.BankInfo= infoTexts[9].text;
      accountResInfo.brand_id = infoTexts[10].text;
      accountResInfo.ip=IPManager.GetIP(ADDRESSFAM.IPv4);
      StartCoroutine(CommitInfo(accountResInfo));
   }

   public void CloseResetInfoPanle()
   {
      transform.gameObject.SetActive(false);
   }
   
   public void OpenResetInfoPanle()
   {
      transform.gameObject.SetActive(true);
   }

   IEnumerator  CommitInfo(AccountResInfo info)//post修改的信息
   {
     
      string js = JsonMapper.ToJson(info);
      JsonData jsonData = JsonMapper.ToObject(js);
      for (int i = 0; i < jsonData.Count; i++)
      {
         if (jsonData[i]==null)
         {
            jsonData[i] = "NA";
         }
      }

      Debug.Log(jsonData[0].ToJson());
      UnityWebRequest request=new UnityWebRequest();
      request.method = "post";
      request.url = API.UpdataUserInfo;
      request.uploadHandler=new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData.ToJson()));
      request.downloadHandler=new DownloadHandlerBuffer();
      request.SetRequestHeader("Authorization", NetworkManager.Instance.token);
      request.SetRequestHeader("Content-Type", "application/json");
      request.SetRequestHeader("Accept", "application/json");
      yield return request.SendWebRequest();
      if (request.responseCode==200)
      {
         tip.instance.SetMessae("修改成功");
      }
      else
      {
         Debug.Log(request.responseCode+"  "+request.downloadHandler.text);
         tip.instance.SetMessae(JsonMapper.ToObject(request.downloadHandler.text)["message"].ToString());
      }
      /*
      form. = jsonData.ToJson();
      NetworkManager.Instance.DoPost1(API.UpdataUserInfo,form, (responscode, content) =>
      {
         if (responscode=="200")
         {
            tip.instance.SetMessae("更改成功");
            for (int i = 0; i < infoTexts.Count; i++)
            {
               infoTexts[i].text = "";
            }
         }
         else
         {
            Debug.Log(responscode+"  "+JsonMapper.ToObject(content)["message"].ToJson());
            tip.instance.SetMessae(JsonMapper.ToObject(content)["message"].ToJson());
         }
         
         
      },NetworkManager.Instance.token);*/
   }




 
}

public class AccountResInfo
{
   public string email;
   public string merchant;
   public string password;
   public string ConfirmPassword;
   public string address;
   public string ConnectPerson;
   public string ConnectPersonPhone;
   public string nicname;
   public string BankAccountName;
   public string BankAccount;
   public string BankInfo;
   public string brand_id;
   public string ip;
   public string province;
   public string city;
   public string license_pic;

}
