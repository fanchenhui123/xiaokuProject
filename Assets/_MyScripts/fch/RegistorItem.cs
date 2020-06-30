using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegistorItem : MonoBehaviour
{
    public int index;
    public string carSeris;
    public string carType;
    public string carNumber;
    public string carMemo;
    
    public Text text_index,text_price,text_series,text_type,text_memo,text_statu;

    /// <summary>
    /// 设置车库管理需要显示的数据
    /// </summary>
    /// <param 序号="index"></param>
    /// <param 车架号="carNum"></param>
    /// <param 车系="series"></param>
    /// <param 备注="memo"></param>
    /// <param 车型="type"></param>
   
    public void SetItemContent(string index, string carNum, string series,string memo, string type = "12",string status="未上架")
    {
        
        text_index.text = index.ToString();
        text_price.text = carNum;
        carNumber = carNum;
        
        if (series.Contains("库存"))
        {
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
        }
        else
        {
            text_series.text = series;
        }
        
        text_type.text = type;
        text_memo.text = memo;
        text_statu.text = status;
    }
}
