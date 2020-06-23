using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountReset : MonoBehaviour
{
   public List<Text> infoTexts=new List<Text>();
    
   public void SendResetInfo()
   {
      AccountResInfo accountResInfo=new AccountResInfo();
      accountResInfo.CompanyName = infoTexts[0].text;
      accountResInfo.NewPassword= infoTexts[1].text;
      accountResInfo.ConfirmPassword= infoTexts[2].text;
      accountResInfo.Address= infoTexts[3].text;
      accountResInfo.ConnectPerson= infoTexts[4].text;
      accountResInfo.ConnectPersonPhone= infoTexts[5].text;
      accountResInfo.NiName= infoTexts[6].text;
      accountResInfo.BankAccountName= infoTexts[7].text;
      accountResInfo.BankAccount= infoTexts[8].text;
      accountResInfo.BankInfo= infoTexts[9].text;
      
   }

   public void CloseResetInfoPanle()
   {
      transform.gameObject.SetActive(false);
   }
   
   public void OpenResetInfoPanle()
   {
      transform.gameObject.SetActive(true);
   }
}

public class AccountResInfo
{
   public string CompanyName;
   public string NewPassword;
   public string ConfirmPassword;
   public string Address;
   public string ConnectPerson;
   public string ConnectPersonPhone;
   public string NiName;
   public string BankAccountName;
   public string BankAccount;
   public string BankInfo;

}
