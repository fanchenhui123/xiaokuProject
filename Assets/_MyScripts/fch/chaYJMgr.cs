using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class chaYJMgr : MonoBehaviour
{
    public static chaYJMgr instance;
    public Button btnChatMessage;
    public Text chatPrice, chatMemo;
    private string ChatYJString;
    public MsgCenterCtrl.MessageDataItem msg=new MsgCenterCtrl.MessageDataItem();

    private void Awake()
    {
        instance = this;
    }

    public void ChatYJ()
    {
        //  btnChatSend.onClick.RemoveAllListeners();
        MsgCenterCtrl.YiJia ChatYiJia=new MsgCenterCtrl.YiJia();
        msg = MsgCenterCtrl.Instance.chatMessage;
        ChatYiJia.cart_id = msg.cart_id.ToString();
        ChatYiJia.price = chatPrice.text;
        ChatYiJia.content = chatMemo.text;
        ChatYiJia.cart_loan_id = msg.cart_loan_id;
        ChatYiJia.cart_boutique_id = msg.cart_boutique_id.ToString();
        ChatYJString = JsonMapper.ToJson(ChatYiJia);
        StartCoroutine(postYJ(ChatYJString));
       
    }
   public IEnumerator postYJ(string jsonstring )
    {
        UnityWebRequest webRequest=new UnityWebRequest(API.PostYiJia);
        webRequest.uploadHandler=new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonstring));
        yield return webRequest.SendWebRequest();
        if (webRequest.responseCode==200)
        {
            tip.instance.SetMessae("议价发送成功");
            chatPrice.transform.parent.GetComponent<InputField>().text = "";
            chatMemo.transform.parent.GetComponent<InputField>().text = "";
        }
        else
        {
            tip.instance.SetMessae("议价发送失败");
        }
    }
}
