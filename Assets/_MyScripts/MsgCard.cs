using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;
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
    public string MessaeID="??";
    public Button resBtn;
    public GameObject MessageCenter;
   

    private void Awake()
    {
        MessageCenter=GameObject.Find("MsgPage1");
        resBtn.onClick.AddListener(trasnDat);
      
    }

    private void trasnDat()
    {
        MessageCenter.SetActive(false);
        MsgCenterCtrl.Instance.YJPage2.gameObject.GetComponent<NegotiatePrice>().curOrderId = MessaeID;
        MsgCenterCtrl.Instance.YJPage2.gameObject.SetActive(true);
       
        Debug.Log(MessaeID);
    }
}
