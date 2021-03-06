﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

public class MsgCenterCtrl : ISingleton<MsgCenterCtrl>
{

    [SerializeField]
    private Transform firstPage;
    [SerializeField]
    private Transform secondPage;
    [SerializeField]
    private Button confirmAndBack;
    [SerializeField]
    private Button continueAndBack;

    public GameObject mainManager;

    public NetworkManager networkManager;

    public List<MessageDataItem> msgItemList = new List<MessageDataItem>();

    public List<MessageDataItem> completedOrders = new List<MessageDataItem>();
    public List<MessageDataItem> needFinalConfirm = new List<MessageDataItem>();
    public List<MessageDataItem> repliesOrders = new List<MessageDataItem>();
    public CarTypeInfo carInfo=new CarTypeInfo();
    
    [Header("page1")]
    public GameObject messageCardPrefab;
    public Transform contentParentJYWC,contentParentDCL;
    public Transform contentParentYJ;

    public Transform YJPage2;
    public MessageDataItem curYJInfo=new MessageDataItem();
    private bool isGetAllOreders;

    private void OnEnable()
    {
        if (!isGetAllOreders)
        {
            GetAllOrders();
        }
       
        coroutine.instance.prompt.SetActive(false);
    }

    private void GetAllOrders()
    {
       // confirmAndBack.onClick.AddListener(OnClickConfirm);
      //  continueAndBack.onClick.AddListener(OnClickContinue);
        Debug.Log("获取所有订单消息");
        isGetAllOreders = true;
        completedOrders.Clear();
        needFinalConfirm.Clear();
        repliesOrders.Clear();
        for (int i = 0; i < contentParentYJ.childCount; i++)
        {
           
             contentParentYJ.GetChild(i).gameObject.SetActive(false);
        }
        
        for (int i = 0; i < contentParentJYWC.childCount; i++)
        {
           
            Destroy( contentParentJYWC.GetChild(i).gameObject);
        }
        for (int i = 0; i < contentParentDCL.childCount; i++)
        {
           
            Destroy( contentParentDCL.GetChild(i).gameObject);
        }
        
        
        networkManager = NetworkManager.Instance;
        if (networkManager != null)
        {
            //networkManager = mainManager.GetComponent<NetworkManager>();
            DoGetMessageList(() =>
            {
                for (int i = 0; i < completedOrders.Count; i++)//已经完成（可查看）
                {
                    GameObject msgCard = Instantiate(messageCardPrefab, contentParentJYWC);
                    msgCard.GetComponent<MsgCard>().text_appear_color.text = "颜色：" + completedOrders[i].cart.appear_color;
                    msgCard.GetComponent<MsgCard>().text_carNumber.text = "车架号：" + completedOrders[i].cart.carNumber;
                    msgCard.GetComponent<MsgCard>().text_cart_type.text = "车型：" + completedOrders[i].cart.carType;
                    msgCard.GetComponent<MsgCard>().text_order_no.text = "订单号：" + completedOrders[i].order_no;
                    if (completedOrders[i].repies.Length > 0)
                    {
                        msgCard.GetComponent<MsgCard>().text_price.text = "报价：" + completedOrders[i].repies[completedOrders[i].repies.Length - 1].price;
                        msgCard.GetComponent<MsgCard>().text_user_id.text = "用户ID：" + completedOrders[i].repies[completedOrders[i].repies.Length - 1].user_id.ToString();
                    }
                    msgCard.GetComponent<MsgCard>().text_vehicleSystem.text = "车系：" + completedOrders[i].cart.vehicleSystem;
                    msgCard.GetComponent<MsgCard>().text_yajin.text = "押金已支付";
                    msgCard.GetComponent<MsgCard>().text_yajin.color = Color.green;
                    msgCard.GetComponent<MsgCard>().text_status.text = "查看";
                    msgCard.GetComponent<MsgCard>().MessaeID = completedOrders[i].id.ToString();   ;//每个消息卡的数据传过去
                    Debug.Log("messid  "+ completedOrders[i].id.ToString());
                    msgCard.GetComponent<MsgCard>().MessaeIndex = i;
                }
              
                
                for (int i = 0; i < needFinalConfirm.Count; i++)//最终确认（待处理）
                {
                    GameObject msgCard = Instantiate(messageCardPrefab, contentParentDCL);
                    msgCard.GetComponent<MsgCard>().text_appear_color.text = "颜色：" + needFinalConfirm[i].cart.appear_color;
                    msgCard.GetComponent<MsgCard>().text_carNumber.text = "车架号：" + needFinalConfirm[i].cart.carNumber;
                    msgCard.GetComponent<MsgCard>().text_cart_type.text = "车型：" + needFinalConfirm[i].cart.carType;
                    msgCard.GetComponent<MsgCard>().text_order_no.text = "订单号：" + needFinalConfirm[i].order_no;
                    if (needFinalConfirm[i].repies.Length > 0)
                    {
                        msgCard.GetComponent<MsgCard>().text_price.text = "报价：" + needFinalConfirm[i].repies[needFinalConfirm[i].repies.Length - 1].price;
                        msgCard.GetComponent<MsgCard>().text_user_id.text = "用户ID：" + needFinalConfirm[i].repies[needFinalConfirm[i].repies.Length - 1].user_id.ToString();
                    }
                    msgCard.GetComponent<MsgCard>().text_vehicleSystem.text = "车系：" + needFinalConfirm[i].cart.vehicleSystem;
                    msgCard.GetComponent<MsgCard>().text_yajin.text = "";
                    msgCard.GetComponent<MsgCard>().text_yajin.color = Color.green;
                    msgCard.GetComponent<MsgCard>().text_status.text = "待处理";
                    msgCard.GetComponent<MsgCard>().MessaeID = needFinalConfirm[i].id.ToString();   ;//每个消息卡的数据传过去
                    Debug.Log("messid  "+ needFinalConfirm[i].id.ToString());
                    msgCard.GetComponent<MsgCard>().MessaeIndex = i;
                }
                
                
                Debug.Log("replies"+repliesOrders.Count);
                
                for (int i = 0; i < repliesOrders.Count; i++)//待回复
                {
                    GameObject msgCard = Instantiate(messageCardPrefab, contentParentYJ);
                    msgCard.GetComponent<MsgCard>().text_appear_color.text = "颜色：" + repliesOrders[i].cart.appear_color;
                    msgCard.GetComponent<MsgCard>().text_carNumber.text = "车架号：" + repliesOrders[i].cart.carNumber;
                    msgCard.GetComponent<MsgCard>().text_cart_type.text = "车型：" + repliesOrders[i].cart.carType;
                    msgCard.GetComponent<MsgCard>().text_order_no.text = "订单号：" + repliesOrders[i].order_no;
                   
                    if ( repliesOrders[i].repies.Length > 0)
                    {
                        msgCard.GetComponent<MsgCard>().text_price.text = "报价：" + repliesOrders[i].repies[repliesOrders[i].repies.Length - 1].price;
                        msgCard.GetComponent<MsgCard>().text_user_id.text = "用户ID：" + repliesOrders[i].repies[repliesOrders[i].repies.Length - 1].user_id.ToString();
                    }
                    msgCard.GetComponent<MsgCard>().text_vehicleSystem.text = "车系：" + repliesOrders[i].cart.vehicleSystem;
                    msgCard.GetComponent<MsgCard>().text_yajin.text = "押金已支付";
                    msgCard.GetComponent<MsgCard>().text_yajin.color = Color.green;

                    msgCard.GetComponent<MsgCard>().MessaeID = repliesOrders[i].id.ToString();   ;//每个消息卡的数据传过去
                    Debug.Log("messid  "+ repliesOrders[i].id.ToString());
                    msgCard.GetComponent<MsgCard>().MessaeIndex = i;
                    
                }
            });
            
           
        }

        isGetAllOreders = false;

    }

  //  public Transform dialog;


  /// <summary>
    /// 消息中心一二级页面切换
    /// </summary>
    /// <param name="pageIndex">1 or 2</param>
    public void ChangeToPage(int pageIndex)
    {
        if (pageIndex == 1)
        {
            firstPage.gameObject.SetActive(true);
            secondPage.gameObject.SetActive(false);
        }
        else if (pageIndex == 2)
        {
            firstPage.gameObject.SetActive(false);
            secondPage.gameObject.SetActive(true);
        }
    }

    private void OnClickConfirm()
    {
        ChangeToPage(1);
    }

    private void OnClickContinue()
    {
        ChangeToPage(1);
    }
    
    public void DoGetMessageList(Action updateUI)
    {
        string url = API._GetMsgList1;
        networkManager.DoGet1(url, (responseCode, data) =>
        {
            if (responseCode == 200)
            {
                //Response_Msg responseMsg = JsonMapper.ToObject<Response_Msg>(data);

                coroutine.instance.GetAllMsg(data);
                Debug.Log("msgdata  "+data);
                if (JsonMapper.ToObject(data)["data"].Count==0)
                {
                     tip.instance.SetMessae("没有订单信息");
                }
                MsgCenterCtrl.Instance.completedOrders =coroutine.instance.curCompletedOrders;
                MsgCenterCtrl.Instance.repliesOrders =coroutine.instance.curRepliesOrders;
                MsgCenterCtrl.Instance.needFinalConfirm =coroutine.instance. curNeedFinalConfirm;
                updateUI();
                //Debug.Log("________" + msgItemList[msgItemList.Count - 1].cart_id);
            }
            else
            {
                tip.instance.SetMessae("获取订单列表错误"+responseCode);
              
              
            }
        }, networkManager.token);

    }


   



   

    public class Response_Msg
    {
        public MessageDataItem[] data;
        public Links links;
        public Meta meta;
    }


    public class MessageDataItem
    {
        public int id { get; set; } //自增ID
        public int user_id{ get; set; }        //用户ID
        public int cart_id{ get; set; }       //车辆ID	
        public int cart_type{ get; set; }       //车辆类型 1-原价车 2-特价车
        public int merchant_id{ get; set; }    //商家ID
        public string order_no{ get; set; }    //订单号	
        public string trade_no{ get; set; }    //支付订单号
        public string body{ get; set; }        //支付标题
        public string total{ get; set; }     //定金
        public string final_total{ get; set; } //最终的车辆价格（议价后）	
        public string trade_status{ get; set; }//支付商家返回的状态	
        public string pay_time{ get; set; }    //支付时间	
        public int status{ get; set; }         //状态 0 未付款 1 已付款 2-用户已确认 3-商家已确认 4-订单完成	
        public int if_mortgage{ get; set; }     //是否按揭 0-不按揭 1-按揭	
        public string cart_loan_id{ get; set; }    //按揭方案ID	(---null!!!!!!) JsonException: Can't assign null to an instance of type System.Int32

        public LoanData loan_data{ get; set; }

        public int cart_boutique_id{ get; set; }//精品方案ID	(INT)
        public string name{ get; set; }       //用户姓名
        public string phone{ get; set; }
        public string id_card{ get; set; }     //用户身份证信息
        public int closed{ get; set; }          //订单是否关闭 0-未关闭 1-已关闭
        public int last_reply_user_type{ get; set; }    //最近回复的用户类型 1-用户回复 2-商家回复
        public string created_at{ get; set; }   //订单创建时间
        public string updated_at{ get; set; }

        public UserInfo user{ get; set; }     //用户信息
        public CarInfo cart{ get; set; }       //车辆信息
        public CartLoanInfo cart_loan{ get; set; }   //贷款信息	
        public CartBoutiqueInfo cart_boutique{ get; set; }//精品方案信息	
        public ReplyContent[] repies{ get; set; }      //议价过程	


    }



    public class UserInfo
    {
        public int id;
        public string mobile;
        public string nickname;
        public string avatar;
        public string openid;
        public string created_at;
        public string updated_at;
    }

    public class CarInfo
    {
        public int id;
        public int merchant_id;
        public int brand_id;
        public string vehicleSystem;
        public string carType;
        public string discharge;
        public string guidancePrice;
        public string carNumber;
        public string appear_color;
        public string interoi_color;
        public string garageAge;
        public string note;
        public string qualityloss;
        public string memo;
        public int registration_area_type;
        public int province_id;
        public string city_id;
        public int status;
        public string create_time;
        public string update_time;
        public int deposit;
        public cost costInfo;
    }

   
    public class CartLoanInfo
    {
        public int id;
        public int merchant_id;
        public int cart_id;
        public string title;
        public string total_amount;
        public string down_payment_amount;
        public int nper;
        public string monthly_payment_amount;
        public string interest_amount;
        public string created_at;
        public string updated_at;
    }

    public class CartBoutiqueInfo
    {
        public int id;
        public int merchant_id;
        public int cart_id;
        public string content;
        public string created_at;
        public string updated_at;
    }

    public class ReplyContent
    {
        public int id;
        public int type;
        public int order_id;
        public int user_id;
        public string merchant_id;
        public string price;
        public string content;
        public string created_at;
        public string updated_at;
    }

    public class LoanData
    {
        public int total_amount;
        public int down_payment_amount;
        public string nper;
        public int monthly_payment_amount;
        public string credit;
    }



    public class Links
    {
        public string first;
        public string last;
        public string prev;
        public string next;
    }

    public class Meta
    {
        public int current_page;
        public int from;
        public int last_page;
        public string path;
        public string per_page;
        public int to;
        public int total;
    }
    
    public class  YiJia
    {
        public string order_id;
        public string price;
        public string content;
        public string cart_loan_id;
        public string cart_boutique_id;
    }//议价接口
    
    public class  ConfirmYJ
    {
        public string order_id;
    }

}



