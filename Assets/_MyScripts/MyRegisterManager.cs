using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyRegisterManager : MonoBehaviour
{
    public Button OKBtn;
    public InputField[] inputFields;
    public Text warnText;
    NetworkManager NetworkManager;

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager = FindObjectOfType<NetworkManager>();
        //gameObject.SetActive(false);
        OKBtn.onClick.AddListener(Commit);
        inputFields = GetComponentsInChildren<InputField>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show(bool isOn)
    {
        gameObject.SetActive(isOn);
    }

    //提交
    public void Commit()
    {
        //公司邮箱	公司名称	登录密码	公司地址	联系人	联系电话	IP	营业执照	昵称	品牌
        //email	merchant	password	address	linkman	linkphone	ip	license_pic	nickname	brand_id
        string email = inputFields[0].text;
        //string merchant = inputFields[1].text;
        string password = inputFields[1].text;
        //string password2 = inputFields[3].text;
        string address = inputFields[2].text;
        //string linkman = inputFields[5].text;
        //string linkphone = inputFields[6].text;
        string ip = IPManager.GetIP(ADDRESSFAM.IPv4);
        string license_pic = "null";
        //string nickname = inputFields[7].text;
        string brand_id = "1";

        //if (password != password2)
        //{
        //    warnText.text = "两次密码不一致";
        //    return;
        //}

        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);
        form.AddField("address", address);
        Debug.Log("________________address:" + address);
        form.AddField("merchant", "顺丰");
        form.AddField("linkman", "王五");
        form.AddField("linkphone", "00000000000");
        form.AddField("ip", ip);
        form.AddField("license_pic", license_pic);
        form.AddField("nickname", "钻石王老五");
        form.AddField("brand_id", brand_id);

        NetworkManager.DoPost(API.RegisterUrl, form, PostCallBack);
    }

    void PostCallBack(string responseCode, string data)
    {
        Debug.Log("responseCode:" + responseCode + "|" + data);
        if (responseCode == "200")
        {
            JsonData jdata = JsonMapper.ToObject(data);
            if (jdata["code"].ToString() == "0")
            {
                warnText.text = jdata["msg"].ToString();
                OKBtn.interactable = false;
            }
            else
            {
                warnText.text = jdata["msg"].ToString();
            }
        }
        else
        {
            warnText.text = "网络故障";
        }
    }

}
