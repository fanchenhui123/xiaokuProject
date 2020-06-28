using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MsgCenterCtrl;

public class MyHomePage : MonoBehaviour
{
    public static MyHomePage Instance;

    public Button btnShare;

    public Button btnReply;
    public Button btnDeal;

    public GameObject priceManagerPageObj;
    public GameObject priceManagerItem;
    public Transform itemContainer;

    public Text textWaitForReplyCount;
    public Text textCompleteCount;

    private PriceManager priceManager;

    private NetworkManager networkManager;
    private DBManager dBManager;
    private List<PriceInfo> priceInfos = new List<PriceInfo>();

    private List<MessageDataItem> completedOrders = new List<MessageDataItem>();
    private List<MessageDataItem> ordersWaitForReply = new List<MessageDataItem>();

    private List<string> carTypeList = new List<string>();      //存储的汽车列表

    public List<PriceManagerItem> priceManagerItems = new List<PriceManagerItem>();     //全局（主页与价格管理）共享报价列表数据


    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        btnShare.onClick.AddListener(OnBtnShareClick);
        btnDeal.onClick.AddListener(() => SecondPanelCtrl.Instance.OpenTargetPage("btn_orderManager"));
        btnReply.onClick.AddListener(() => SecondPanelCtrl.Instance.OpenTargetPage("btn_msgCenter"));

        if (priceManagerPageObj == null)
        {
            priceManagerPageObj = GameObject.Find("PriceManager");
        }
        priceManager = priceManagerPageObj.GetComponent<PriceManager>();

        networkManager = NetworkManager.Instance;
        dBManager = DBManager._DBInstance();

        dBManager.CreateTable(typeof(PriceInfo));
        priceInfos = dBManager.QueryTable<PriceInfo>();
        if (priceInfos.Count > 0)
        {
            UpdateUI();
        }
        PriceManager.LoadExcelEndEvent += PriceManager_LoadExcelEndEvent;
    }
    

    private void OnEnable()
    {
        //Debug.Log("MyHomePage OnEnable !!!");
        if (networkManager != null)
        {
            DoGetMessageList(() => {
                textWaitForReplyCount.text = string.Format(@"<color=red><size=45>{0}</size></color>" + "\n待回复", ordersWaitForReply.Count);
                textCompleteCount.text = string.Format(@"<color=green><size=45>{0}</size></color>" + "\n已成交", completedOrders.Count);
            });
        }
    }


    private void PriceManager_LoadExcelEndEvent()
    {
        Debug.Log(" event load excel");
            // priceInfos = dBManager.QueryTable<PriceInfo>();
       priceInfos = PriceManager.Instance.priceInfos;
        if (priceInfos.Count > 0)
        {
            UpdateUI();
        }
    }


    private void OnBtnShareClick()
    {
        gameObject.SetActive(false);
        priceManagerPageObj.SetActive(true);
    }



    public void DoGetMessageList(Action updateUI)
    {
        string url = API._GetMsgList1;
        networkManager.DoGet1(url, (responseCode, data) =>
        {
            //Debug.Log("responseCode:" + responseCode + "|" + data);

            if (responseCode == 200)
            {
                //Response_Msg responseMsg = JsonMapper.ToObject<Response_Msg>(data);
                //Debug.Log(responseMsg.data.Length);
                JsonData jsonData = JsonMapper.ToObject(data);
                //Debug.Log(jsonData["data"].Count);
                completedOrders.Clear();
                ordersWaitForReply.Clear();
                foreach (JsonData obj in jsonData["data"])
                {
                    try
                    {
                        MessageDataItem item = new MessageDataItem();
                        item.id = int.Parse(obj["id"].ToString());
                        item.user_id = int.Parse(obj["user_id"].ToString());
                        item.cart_id = int.Parse(obj["cart_id"].ToString());
                        item.status = int.Parse(obj["status"].ToString());

                        item.last_reply_user_type = int.Parse(obj["last_reply_user_type"].ToString());
                        
                        if (item.status == 5)
                        {
                            completedOrders.Add(item);
                        }
                        else if (item.status == 1)
                        {
                            if (item.last_reply_user_type == 1)
                            {
                                ordersWaitForReply.Add(item);
                            }
                        }
                    }
                    catch (System.Exception E)
                    {
                        Debug.LogError(E.ToString());
                    }
                }
                updateUI();
            }
        }, networkManager.token);

    }


    //private void UpdateUI()
    //{
    //    for (int i = 0; i < priceInfos.Count; i++)
    //    {
    //        if (priceInfos[i].carNumber != null)
    //        {
    //            GameObject go = Instantiate(priceManagerItem, itemContainer);
    //            PriceManagerItem item = go.GetComponent<PriceManagerItem>();
    //            item.SetItemContent((i + 1).ToString(), priceInfos[i].guidancePrice,
    //                priceInfos[i].vehicleSystem, priceInfos[i].carType, "未上架");

    //            if (item.offerPriceData == null)
    //            {
    //                item.offerPriceData = new PostDataForOfferPrice();
    //            }
    //            item.offerPriceData.carType = priceInfos[i].carType;
    //            item.offerPriceData.carSeries = priceInfos[i].vehicleSystem;
    //            item.offerPriceData.car_id = priceInfos[i].id.ToString();       //序号

    //            item.offerPriceData.registration_area_type = 0;         //默认：市内

    //            item.priceInfo = priceInfos[i];
    //        }
    //    }

    //}


    public void RefreshItem()
    {
        for (int i = 0; i < priceManagerItems.Count; i++)
        {
            Destroy(priceManagerItems[i]);
        }
        UpdateUI();
    }

    /// <summary>
    /// 依据数据库或Excel内数据，刷新报价列表
    /// </summary>
    public void UpdateUI()
    {
        int count = 1;
        Debug.Log("count   "+priceInfos.Count);
        for (int i = 0; i < priceInfos.Count; i++)
        {
            if (priceInfos[i].carType != "" && !carTypeList.Contains(priceInfos[i].carType))
            {
                GameObject go = Instantiate(priceManagerItem, itemContainer);

                PriceManagerItem item = go.GetComponent<PriceManagerItem>();

                item.SetItemContent((count).ToString(), priceInfos[i].guidancePrice,
                    priceInfos[i].vehicleSystem, priceInfos[i].carType, "未上架");

                if (priceManager.priceManagerItems.Count > 0)
                {
                    item.offerPriceData = priceManager.priceManagerItems[count - 1].offerPriceData;
                }
                
                item.priceInfo = priceInfos[i];

                priceManagerItems.Add(item);

                if (priceInfos[i].carType != "" && !carTypeList.Contains(priceInfos[i].carType))
                {
                    carTypeList.Add(priceInfos[i].carType);
                }
                count++;
            }
        }
        Debug.Log("count   "+priceManagerItems.Count);
    }


    public void UpdateTargetItem(int index)
    {
        Debug.Log(index);
        Debug.Log(this.priceManagerItems.Count);
        var priceManagerItem = priceManagerItems[index];

        priceManagerItem.UpdateItem(priceManagerItem.offerPriceData.officialPrice, "已上架");
    }

    public void ClearAllData()
    {
        priceManagerItems.Clear();
        carTypeList.Clear();
        priceInfos.Clear();

        for (int i = 0; i < itemContainer.childCount; i++)
        {
            Destroy(itemContainer.GetChild(i).gameObject);
        }
    }


}
