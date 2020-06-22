using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System;

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
    public List<MessageDataItem> repliesOrders = new List<MessageDataItem>();

    [Header("page1")]
    public GameObject messageCardPrefab;
    public Transform contentParentJYWC;
    public Transform contentParentYJ;


    private void Start()
    {
        confirmAndBack.onClick.AddListener(OnClickConfirm);
        continueAndBack.onClick.AddListener(OnClickContinue);

        networkManager = NetworkManager.Instance;
        if (networkManager != null)
        {
            //networkManager = mainManager.GetComponent<NetworkManager>();
            DoGetMessageList(() =>
            {
                for (int i = 0; i < completedOrders.Count; i++)
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
                }
                for (int i = 0; i < repliesOrders.Count; i++)
                {
                    GameObject msgCard = Instantiate(messageCardPrefab, contentParentYJ);
                    msgCard.GetComponent<MsgCard>().text_appear_color.text = "颜色：" + repliesOrders[i].cart.appear_color;
                    msgCard.GetComponent<MsgCard>().text_carNumber.text = "车架号：" + repliesOrders[i].cart.carNumber;
                    msgCard.GetComponent<MsgCard>().text_cart_type.text = "车型：" + repliesOrders[i].cart.carType;
                    msgCard.GetComponent<MsgCard>().text_order_no.text = "订单号：" + repliesOrders[i].order_no;
                    if (completedOrders[i].repies.Length > 0)
                    {
                        msgCard.GetComponent<MsgCard>().text_price.text = "报价：" + repliesOrders[i].repies[repliesOrders[i].repies.Length - 1].price;
                        msgCard.GetComponent<MsgCard>().text_user_id.text = "用户ID：" + repliesOrders[i].repies[repliesOrders[i].repies.Length - 1].user_id.ToString();
                    }
                    msgCard.GetComponent<MsgCard>().text_vehicleSystem.text = "车系：" + repliesOrders[i].cart.vehicleSystem;
                    msgCard.GetComponent<MsgCard>().text_yajin.text = "押金已支付";
                    msgCard.GetComponent<MsgCard>().text_yajin.color = Color.green;
                }
            });
        }

    }


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
            Debug.Log("responseCode:" + responseCode + "|" + data);

            if (responseCode == 200)
            {
                //Response_Msg responseMsg = JsonMapper.ToObject<Response_Msg>(data);
                JsonData jsonData = JsonMapper.ToObject(data);
                //Debug.Log(jsonData["data"].Count);
                foreach (JsonData obj in jsonData["data"])
                {
                    try
                    {
                        MessageDataItem item = new MessageDataItem();
                        item.id = int.Parse(obj["id"].ToString());
                        item.user_id = int.Parse(obj["user_id"].ToString());
                        item.cart_id = int.Parse(obj["cart_id"].ToString());

                        item.order_no = obj["order_no"].ToString();
                        
                        item.cart = new CarInfo();
                        JsonData jsonData_cart = obj["cart"];
                        if (jsonData_cart != null)
                        {
                            item.cart.carType = jsonData_cart["carType"].ToString();
                            item.cart.vehicleSystem = obj["cart"]["vehicleSystem"].ToString();

                            item.cart.carNumber = obj["cart"]["carNumber"].ToString();

                            if (obj["cart"]["appear_color"] != null)
                                item.cart.appear_color = obj["cart"]["appear_color"].ToString();            //null
                            else
                                item.cart.appear_color = "NA";
                        }

                        item.repies = new ReplyContent[obj["repies"].Count];
                        for (int i = 0; i < item.repies.Length; i++)
                        {
                            ReplyContent rc = new ReplyContent();
                            rc.id = int.Parse(obj["repies"][i]["id"].ToString());
                            rc.price = obj["repies"][i]["price"].ToString();
                            rc.user_id = int.Parse(obj["repies"][i]["user_id"].ToString());
                            item.repies[i] = rc;
                        }

                        item.status = int.Parse(obj["status"].ToString());

                        item.last_reply_user_type = int.Parse(obj["last_reply_user_type"].ToString());

                        msgItemList.Add(item);

                        if (item.status == 5)
                        {
                            completedOrders.Add(item);          //交易完成
                        }
                        else if (item.status == 1)
                        {
                            if (item.repies.Length > 0)
                            {
                                repliesOrders.Add(item);        //议价列表
                            }
                        }
                    }
                    catch (System.Exception E)
                    {
                        Debug.LogError(E.ToString());
                    }
                }
                updateUI();
                //Debug.Log("________" + msgItemList[msgItemList.Count - 1].cart_id);
            }
        }, networkManager.token);

    }


    private void GetAllMsg()
    {

    }




    public class Response_Msg
    {
        public MessageDataItem[] data;
        public Links links;
        public Meta meta;
    }


    public class MessageDataItem
    {
        public int id;              //自增ID
        public int user_id;         //用户ID
        public int cart_id;         //车辆ID	
        public int cart_type;       //车辆类型 1-原价车 2-特价车
        public int merchant_id;     //商家ID
        public string order_no;     //订单号	
        public string trade_no;     //支付订单号
        public string body;         //支付标题
        public string total;        //定金
        public string final_total;  //最终的车辆价格（议价后）	
        public string trade_status; //支付商家返回的状态	
        public string pay_time;     //支付时间	
        public int status;          //状态 0 未付款 1 已付款 2-用户已确认 3-商家已确认 4-订单完成	
        public int if_mortgage;     //是否按揭 0-不按揭 1-按揭	
        public string cart_loan_id;    //按揭方案ID	(---null!!!!!!) JsonException: Can't assign null to an instance of type System.Int32

        public LoanData loan_data;

        public int cart_boutique_id;//精品方案ID	(INT)
        public string name;         //用户姓名
        public string phone;
        public string id_card;      //用户身份证信息
        public int closed;          //订单是否关闭 0-未关闭 1-已关闭
        public int last_reply_user_type;    //最近回复的用户类型 1-用户回复 2-商家回复
        public string created_at;   //订单创建时间
        public string updated_at;

        public UserInfo user;         //用户信息
        public CarInfo cart;         //车辆信息
        public CartLoanInfo cart_loan;    //贷款信息	
        public CartBoutiqueInfo cart_boutique;//精品方案信息	
        public ReplyContent[] repies;       //议价过程	


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

}



