using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MsgCard : MonoBehaviour
{
   // [HideInInspector]
    public Button commit;
   // private Text[] msgs;
    public static MsgCard Instance; 
    public Text text_order_no;
    public Text text_cart_type;
    public Text text_vehicleSystem;
    public Text text_carNumber;
    public Text text_appear_color;
    public Text text_user_id;
    public Text text_price;
    public Text text_yajin;
    public MsgCenterCtrl.MessageDataItem msg;
    private Button resButton;
  
   
    // Start is called before the first frame update
   public void GetData(MsgCenterCtrl.MessageDataItem  messageDataItem)
    {
        msg=messageDataItem;
        resButton = transform.Find("resBtn").GetComponent<Button>();
        resButton.onClick.AddListener(HandleData);//消息卡回复按键
        // msgs = GetComponentsInChildren<Text>();
        // commit = GetComponentInChildren<Button>();
        //  dialogParent = transform.Find("DialogArea").transform;
        
    }

    public GameObject dialogItem;
    public void HandleData()
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
            
            //聊天模式下确认按键添加新的监听
            NegotiatePrice.Instance.msg=msg;
            //聊天界面
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
}
