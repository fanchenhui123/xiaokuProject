using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChatYJ : MonoBehaviour
{
    public static ChatYJ instance;
    public MsgCenterCtrl.MessageDataItem msg;
    public Button sendChatYJ;
   // private bool startUpdataMess;//是否开始频繁请求
    public Text chatPrice, chatMemo;
    private string ChatYJString;

    private void Awake()
    {
        instance = this;
        
    }

    private void Start()
    {
        InvokeRepeating("repeatRequest",60f,10000000000f);
    }


    private void repeatRequest()
    {
        Debug.Log("再次请求");
        StartCoroutine(GetYJinfo(msg.order_no));//每隔一定时间请求获得数据
       
    }

    // Start is called before the first frame update
  

    public GameObject dialogItem;
    public void HandleData()  //待回复按键点击后刷新聊天框内容
    {
        
        updateChatInfo();
        sendChatYJ.onClick.AddListener(SendChatYJ);
    }
   
    private void updateChatInfo()
    {
        StringBuilder stringBuilder=new StringBuilder();
        if (msg.repies.Length>0)//多次议价，开始聊天,清楚之前的，新建后来的
        {
            for (int i = 0; i < MsgCenterCtrl.Instance.dialog.childCount; i++)
            {
                Destroy(MsgCenterCtrl.Instance.dialog.GetChild(i).gameObject);  
            }
            GameObject go;
            for (int i = 0; i < msg.repies.Length; i++)
            {
                go = Instantiate(dialogItem, MsgCenterCtrl.Instance.dialog);
                stringBuilder.Append(msg.name).Append(":").Append("价格：").Append(msg.repies[i].price).Append("，备注：").Append(msg.repies[i].content);
                go.GetComponent<Text>().text = stringBuilder.ToString();
            }
            
        }
        else if (msg.repies.Length==0)
        {
            Debug.Log("第一次议价");
            FirstYJ();
        }
    }

    private void FirstYJ()
    {
         MsgCenterCtrl.Instance.ChangeToPage(2);
         NegotiatePrice.Instance.ShowData(msg);
    }
    
    public  IEnumerator GetYJinfo(string id)//获取订单详情
    {
        yield return new WaitForSeconds(10);
        Debug.Log("start cor");
        UnityWebRequest request=new UnityWebRequest();
        request.downloadHandler=new DownloadHandlerBuffer();
        request.url = API._GetMsgList23+"?order_id="+id;
        
        yield return request.SendWebRequest();
        if (request.responseCode==200)//获取到数据后更新msg刷新聊天内容
        {
          
            JsonData jsonData =JsonMapper.ToObject(request.downloadHandler.text);
            Debug.Log(jsonData);
            jsonData = jsonData["data"];
            string js = jsonData.ToJson();
            MsgCenterCtrl.MessageDataItem dataItem =
                JsonMapper.ToObject<MsgCenterCtrl.MessageDataItem>( js);
            Debug.Log(dataItem.cart_id);



            // mess json = JsonMapper.ToJson(request.downloadHandler.text);
            //复制给新的msg
         //   msg = JsonMapper.ToObject<MsgCenterCtrl.MessageDataItem>(jsonData);
            Debug.Log(msg.cart_boutique_id);
            updateChatInfo();
        }
        else
        {
            Debug.Log("2222211112   " +request.responseCode);
        }
    }
    
    
    
    public void SendChatYJ()
    {
        //  btnChatSend.onClick.RemoveAllListeners();
        MsgCenterCtrl.YiJia ChatYiJia=new MsgCenterCtrl.YiJia();
       // msg = MsgCenterCtrl.Instance.chatMessage;
        ChatYiJia.cart_id = msg.cart_id.ToString();
        ChatYiJia.price = chatPrice.text;
        ChatYiJia.content = chatMemo.text;
        ChatYiJia.cart_loan_id = msg.cart_loan_id;
        ChatYiJia.cart_boutique_id = msg.cart_boutique_id.ToString();
        ChatYJString = JsonMapper.ToJson(ChatYiJia);
        StartCoroutine(postYJ(ChatYJString));
       
    }
    public IEnumerator postYJ(string jsonstring )//发送聊天议价信息
    {
        StringBuilder stringBuilder=new StringBuilder();
        UnityWebRequest webRequest=new UnityWebRequest(API.PostYiJia);
        webRequest.uploadHandler=new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonstring));
        yield return webRequest.SendWebRequest();
        if (webRequest.responseCode==200)
        {
            tip.instance.SetMessae("议价发送成功");
            GameObject go = Instantiate(dialogItem, MsgCenterCtrl.Instance.dialog);
            stringBuilder.Append("价格：") .Append(chatPrice.text).Append("备注：").Append(chatMemo.text);
            go.GetComponent<Text>().text = stringBuilder.ToString();
            chatPrice.transform.parent.GetComponent<InputField>().text = "";
            chatMemo.transform.parent.GetComponent<InputField>().text = "";
        }
        else
        {
            tip.instance.SetMessae("议价发送失败");
        }
    }
}
