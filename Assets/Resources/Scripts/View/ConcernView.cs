using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConcernView : MonoBehaviour
{
    public Text warnText;
    public Button closeBtn;
    public Button applyBtn;//申请按钮
    public Button refreshBtn;//刷新收到列表
    public Button queryBtn;//查询选中商家数据
    public Button searchBtn;//搜索按钮
    public InputField searchInputField;//搜索框
    public Text currentIDText;
    public NetworkManager networkManager;
    public DelayPanelManager delayPanelManager;
    public GameObject itemPrefab;
    public GameObject applyPrefab;
    public GameObject content;//可申请的列表
    public GameObject content2;//已申请的列表
    public GameObject content3;//待处理的列表
    public TableView tbView;

    List<Button> applyBtns = new List<Button>();

    // Start is called before the first frame update
    void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();       
        closeBtn.onClick.AddListener(() =>
        {
            Show(false);
        });
        applyBtn.onClick.AddListener(DoApply);
        refreshBtn.onClick.AddListener(UpdateApplyView);
        queryBtn.onClick.AddListener(DoQuery);
        searchBtn.onClick.AddListener(DoSearch);
        tbView = FindObjectOfType<TableView>();
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
        UpdateApplyView();
    }

    public void UpdateApplyView()
    {
        delayPanelManager.Load();
        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
        //Debug.Log(networkManager.token);
        networkManager.DoGet(API.GetMerchants, (responseCode, data) =>
        {           
            Debug.Log("responseCode:" + responseCode + "|" + data);
            if (responseCode == 200)
            {
                JsonData jdata = JsonMapper.ToObject(data);
                //if (jdata["code"].ToString() == "0")
                //{
                try
                {
                    applyBtns.Clear();
                    foreach (JsonData a in jdata["rows"])
                    {
                        Debug.Log(a["id"] + "|" + a["merchant"] + "|" + a["is_link"].ToString());
                        GameObject t = Instantiate<GameObject>(itemPrefab);
                        t.GetComponentInChildren<Text>().text = a["id"] + "|" + a["merchant"] + "|" + a["email"] + "|" + a["is_link"];
                        t.transform.SetParent(content.transform);
                        t.GetComponent<Button>().onClick.AddListener(() => {
                            currentIDText.text = a["id"].ToString();
                        });
                        if (a["is_link"].ToString() == "False") t.SetActive(false);
                        applyBtns.Add(t.GetComponent<Button>());
                    }

                    //}
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
            delayPanelManager.Destory();
            UpdateGetLinkView();

        }, networkManager.token);
    }

    void DoApply()
    {
        string url = API.ApplyLink + "?merchant_id=" + currentIDText.text;
        networkManager.DoGet(url, (responseCode, data) =>
        {
            Debug.Log("responseCode:" + responseCode + "|" + data);
            if (responseCode == 200)
            {
                try
                {
                    JsonData jdata = JsonMapper.ToObject(data);
                    warnText.text = "code:" + jdata["code"] + " " + jdata["msg"].ToString();
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

    public void UpdateGetLinkView()
    {
        delayPanelManager.Load();
        for (int i = 0; i < content3.transform.childCount; i++)
        {
            Destroy(content3.transform.GetChild(i).gameObject);
        }
        //Debug.Log(networkManager.token);
        networkManager.DoGet(API.GetApplyLink, (responseCode, data) =>
        {
            Debug.Log("responseCode:" + responseCode + "|" + data);
            if (responseCode == 200)
            {
                JsonData jdata = JsonMapper.ToObject(data);
                try
                {
                    foreach (JsonData a in jdata["rows"])
                    {
                        Debug.Log(a["to_id"] + "<-" + a["from_id"] + "|" + a["status"]);
                        GameObject t = Instantiate<GameObject>(applyPrefab);
                        t.transform.Find("Text").GetComponent<Text>().text
                        = a["id"] + "|" + a["to_id"] + "<-" + a["from_id"] + "|" + a["status"];

                        t.transform.SetParent(content3.transform);

                        var yesBtn = t.transform.Find("YesButton").GetComponent<Button>();
                        var noBtn = t.transform.Find("NoButton").GetComponent<Button>();

                        if (a["status"].ToString() == "1")
                        {
                            yesBtn.gameObject.SetActive(false);
                            noBtn.gameObject.SetActive(false);
                        }
                        else
                        {
                            yesBtn.onClick.AddListener(() =>
                            {
                                HandleApply(a["id"].ToString(), 1);
                            });
                            noBtn.onClick.AddListener(() =>
                           {
                               HandleApply(a["id"].ToString(), 0);
                           });
                        }

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
            delayPanelManager.Destory();

        }, networkManager.token);
    }

    void HandleApply(string id, int status)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("status", status.ToString());

        networkManager.DoPost(API.HandApply, form, delegate (string responseCode, string data)
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

    void DoQuery()
    {
        tbView.DoWebQuery(currentIDText.text);
        Show(false);
    }

    void DoSearch()
    {
        for (int i = 0; i < applyBtns.Count; i++)
        {
            var a = applyBtns[i];          
            if (a.GetComponentInChildren<Text>().text.Contains(searchInputField.text)
                && !string.IsNullOrEmpty(searchInputField.text.Trim()))
                a.gameObject.SetActive(true);
            else
                a.gameObject.SetActive(false);
        }
    }
}
