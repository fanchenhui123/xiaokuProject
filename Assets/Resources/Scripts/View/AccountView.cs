using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountView : MonoBehaviour
{
    public Text warnText;
    public Button closeBtn;
    public Button addBtn;//增加按钮
    public Button refreshBtn;//刷新收到列表
    public Button deleteBtn;//删除按钮
    public InputField nameInputField;//姓名
    public InputField numberInputField;//号码
    public Text currentIDText;
    public NetworkManager networkManager;
    public DelayPanelManager delayPanelManager;
    public GameObject itemPrefab;
    public GameObject content;//员工列表

    // Start is called before the first frame update
    void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        closeBtn.onClick.AddListener(() =>
        {
            Show(false);
        });
        refreshBtn.onClick.AddListener(UpdateAccountView);
        addBtn.onClick.AddListener(AddAccount);
        deleteBtn.onClick.AddListener(DeleteAccount);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show(bool isOn)
    {
        gameObject.SetActive(isOn);
        if (isOn == false) return;
        warnText.text = "";
        UpdateAccountView();
    }

    public void UpdateAccountView()
    {
        delayPanelManager.Load();
        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
        //Debug.Log(networkManager.token);
        networkManager.DoGet(API.GetSubAccounts, (responseCode, data) =>
        {
            Debug.Log("responseCode:" + responseCode + "|" + data);
            if (responseCode == 200)
            {
                JsonData jdata = JsonMapper.ToObject(data);
                //if (jdata["code"].ToString() == "0")
                //{
                try
                {
                    foreach (JsonData a in jdata["rows"])
                    {
                        Debug.Log(a["id"] + "|" + a["name"]);
                        GameObject t = Instantiate<GameObject>(itemPrefab);
                        t.GetComponentInChildren<Text>().text = a["id"] + "|" + a["name"]
                        + "|" + a["phone"] + "|所属商家id:" + a["merchant_id"] + "|状态:" + a["status"];
                        t.transform.SetParent(content.transform);
                        t.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            currentIDText.text = a["id"].ToString();
                        });
                        if (a["status"].ToString() == "0")
                        {
                            t.SetActive(false);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    warnText.text = e.Message;
                }
                //}
            }
            else
            {
                warnText.text = "responseCode:" + responseCode;
            }
            delayPanelManager.Destory();
        }, networkManager.token);
    }

    void AddAccount()
    {
        WWWForm form = new WWWForm();
        form.AddField("phone", numberInputField.text);
        form.AddField("name", nameInputField.text);

        networkManager.DoPost(API.AddSubAccount, form, delegate (string responseCode, string data)
        {
            Debug.Log("responseCode:" + responseCode + "|" + data);
            if (responseCode == "200")
            {
                try
                {
                    JsonData jdata = JsonMapper.ToObject(data);
                    if (jdata["code"].ToString() == "0")
                    {
                        warnText.text = jdata["msg"].ToString();
                        numberInputField.text = "";
                        nameInputField.text = "";
                        UpdateAccountView();
                    }
                    else
                    {
                        warnText.text = jdata["msg"].ToString();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    warnText.text = e.Message;
                }
            }
            else
            {
                warnText.text = "responseCode:" + responseCode;
            }
        }, networkManager.token);
    }

    void DeleteAccount()
    {
        string url = API.UpdateSubAccountStatus + "?status=0&id=" + currentIDText.text;
        networkManager.DoGet(url, (responseCode, data) =>
        {
            Debug.Log("responseCode:" + responseCode + "|" + data);
            if (responseCode == 200)
            {
                try
                {
                    JsonData jdata = JsonMapper.ToObject(data);
                    warnText.text = "code:" + jdata["code"] + " " + jdata["msg"].ToString();
                    numberInputField.text = "";
                    nameInputField.text = "";
                    UpdateAccountView();
                }
                catch (Exception e)
                {
                    warnText.text = e.Message;
                }

            }
            else
            {
                warnText.text = "responseCode:" + responseCode;
            }
        }, networkManager.token);
    }

}
