using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PriceManagerItem : MonoBehaviour
{
    public Text text_index;
    public Text text_price;
    public Text text_series;
    public Text text_type;
    public Text text_status;

    public Button btn_bjsz;

    public PriceInfo priceInfo;

    public PostDataForOfferPrice offerPriceData;

    public PriceManagerItem(string index, string price, string series, string type, string status)
    {
        text_index.text = index;
        text_price.text = price;
        text_series.text = series;
        text_type.text = type;
        text_status.text = status;

        btn_bjsz.onClick.RemoveAllListeners();
        btn_bjsz.onClick.AddListener(OnBtnBjszClick);
    }


    public void SetItemContent(string index, string price, string series, string type = "NA", string status = "未上架")
    {
        text_index.text = index;
        text_price.text = price;
        string[] result = series.Split(new string[] { "库存" }, System.StringSplitOptions.None);
        string series1 = "";
        if (result.Length > 1)
        {
            for (int i = 0; i < result.Length; i++)
            {
                series1 += result[i];
            }
        }
        else
            series1 = result[0];
        text_series.text = series1;
        text_type.text = type;
        text_status.text = status;

        btn_bjsz.onClick.RemoveAllListeners();
        btn_bjsz.onClick.AddListener(OnBtnBjszClick);
    }


    private void OnBtnBjszClick()
    {
        PriceManager priceManager = GameObject.FindObjectOfType<PriceManager>();
        if (priceManager == null)
        {
            priceManager = PriceManager.Instance;
        }
        priceManager.ChangeToPage(2);

        //if (offerPriceMsg == null)
        //{
        //    offerPriceMsg = new PostDataForOfferPrice();
        //}
        priceManager.SetItemForPage2(this);
    }


    public void UpdateItem(string price, string status = "未上架")
    {
        text_price.text = price;
        text_status.text = status;
    }
    
}

public class PostDataForOfferPrice
{
    public PostDataForOfferPrice()
    {
        carNumbers = new List<string>();
        jingpin = new List<string>();
    }

    public string car_id;           //序号，并非车架号
    public string carType;
    public string carSeries;
    public string status;          //是否上架
    public List<string> carNumbers;    //车架号

    public int registration_area_type;    //归属范围，0表示市内，1表示省内，2表示全国

    public string net_price;           //净车价
    public string registration_price;      //上牌费
    public string insurance_price;
    public string purchase_tax;
    public string financial_agents_price;  //金融服务费
    public string other_price;     //其他费用

    public int registerType;       //0:自付上牌资格  1:代办上牌资格   2:全选

    public int insuranceType;      //0:100万以上   1:其他险种

    public string bargainPrice;

    public string officialPrice;    //报价

    public List<string> jingpin;

    public string remarkOfDecoration;
}
