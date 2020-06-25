using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NegotiatePrice : MonoBehaviour
{
    public List<Text> Texts=new List<Text>();
    public GameObject methods,content;
    
    private float num = 1f;
    //加数字的方法
    public void AddCount(Text tt)
    {
        float.TryParse(tt.text,out num);
        tt.text = (num-=1).ToString();
    }
    public void CutCount(Text tt)
    {
        float.TryParse(tt.text,out num);
        tt.text = (num-=1).ToString();
    }

    public void CreatMethod()
    {
        GameObject o = Instantiate(methods, content.transform);
        string methName = "金融方案：";
        if (Texts[5].text=="")
        {
            methName += (content.transform.childCount+1);
        }
        string message = methName + "首期还" + Texts[1].text + "万元>" + Texts[2].text + "期>月供" + Texts[3].text + "元>总利息" +
                         Texts[4] + "万元";
        o.GetComponent<Text>().text = message;

        float total=0;
        if ( float.TryParse(Texts[0].text,out num))
        {
            total += num;
            if (float.TryParse(Texts[0].text,out num))
            {
                total += num;
                if (float.TryParse(Texts[0].text,out num))
                {
                    total += num;
                }
            }
        }

        Texts[6].text = total.ToString();
    }

    public void DeleteMeth(GameObject o)
    {
        Destroy(o);
    }
    
    
}
