using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyLoginManager : MonoBehaviour
{
    public Button closeBtn;
    public Button loginBtn;
    public Button openLoginPanel;
    public Button openRegisterPanel;
    public Text warnText;
    public GameObject skipWindow;
    public static MyLoginManager instance;
    public InputField userName;
    public InputField pwd;
    public Toggle remember;
    private string ip;

    public GameObject LoginPanel;
    public GameObject RegisterPanel;
    [HideInInspector]
    public GameObject homePage;

    private MessagePanelManager messagePanelManager;

    private NetworkManager networkManager;

    private ClickEvent clickEvent;

    private UserInfo userInfo;


    bool isFirst = true;


    private void Awake()
    {
        instance = this;
        skipWindow.SetActive(false);
        loginBtn.onClick.AddListener(Login);
        closeBtn.onClick.AddListener(() =>
        {
            clickEvent.Quit();
        });
        float temp = 166 / 255f;
        openRegisterPanel.onClick.AddListener(() =>
        {
            //ResetRegisterPanel();
            LoginPanel.SetActive(false);
            RegisterPanel.SetActive(true);
            openRegisterPanel.GetComponentInChildren<Text>().color = Color.red;
            openLoginPanel.GetComponentInChildren<Text>().color = new Color(temp, temp, temp);
        });
        openLoginPanel.onClick.AddListener(() =>
        {
            //ResetLoginPanel();
            LoginPanel.SetActive(true);
            RegisterPanel.SetActive(false);
            openLoginPanel.GetComponentInChildren<Text>().color = Color.red;
            openRegisterPanel.GetComponentInChildren<Text>().color = new Color(temp, temp, temp);
        });

        remember.onValueChanged.AddListener(RememberValChange);
        InitLoginPanel();
       
    }

    // Start is called before the first frame update
    void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        clickEvent = FindObjectOfType<ClickEvent>();
        messagePanelManager = FindObjectOfType<MessagePanelManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void InitLoginPanel()
    {
        this.ip = IPManager.GetIP(ADDRESSFAM.IPv4);
        InitLogin();
    }

    //登陆初始化
    private void InitLogin()
    {
        //控制器初始化
        int isRemember = PlayerPrefs.GetInt("remember", 0);
        remember.isOn = isRemember > 0;

        if (remember.isOn)
        {
            Debug.Log("记住密码");
            userName.text = PlayerPrefs.GetString("username", "");
            pwd.text = PlayerPrefs.GetString("password", ""); ;
        }
        else
        {
            Debug.Log("没有记住密码");
        }
    }


    public void Login()
    {
        PlayerPrefs.SetString("username", userName.text);
        PlayerPrefs.SetString("password", pwd.text);

        var form = new WWWForm();
        if (userName.text == string.Empty)
        {
            warnText.text = "邮箱不能为空";
            return;
        }
        form.AddField("ip", this.ip);
        form.AddField("email", userName.text);
        form.AddField("password", pwd.text);

        networkManager.DoPost(API.LoginUrl1, form, delegate (string responseCode, string data)
        {
            if (responseCode == "200")
            {
                JsonData jdata = JsonMapper.ToObject(data);
                //if (jdata["code"].ToString() == "0")
                //{
                //    warnText.text = jdata["msg"].ToString();
                //    networkManager.token = jdata["token"].ToString();
                //    LoginSuccess();
                //}
                //else
                //{
                //    warnText.text = jdata["msg"].ToString();
                //}
                if (jdata["code"].ToString() == "200")
                {
                    warnText.text = "登陆成功";
                    networkManager.token ="Bearer"+ jdata["data"].ToString();
                    Debug.Log("token:  "+networkManager.token);
                    LoginSuccess();
                }
                else
                {
                    warnText.text = "登陆失败";
                }
            }
            else
            {
                Debug.Log(responseCode+JsonMapper.ToObject(data)["message"]);
                //warnText.text = jdata["msg"].ToString();
                LoginFail();
            }
        });
    }

    public bool isLoginSuccess;
    private void LoginSuccess()
    {
      //  messagePanelManager.Show("登录成功");
        isFirst = false;//第二次不退出

        if (homePage)
        {
            homePage.SetActive(true);
        }
        //LoginPanel.SetActive(false);
        //tableView.Init(4);
        //FindObjectOfType<AutoLoadExcelManager>().StartAutoLoad();

        gameObject.SetActive(false);

        #region  2.0  登录成功后，加载价格管理数据、消息数目等

        DoGetUserInfo();
        GetHadPrice();
        isLoginSuccess = true;
        //请求获取数据库里已经报价的车辆信息

      //  coroutine.instance.StartCompare();

        #endregion
    }

    private void LoginFail()
    {
        messagePanelManager.Show("登录失败");
    }

    List<string> hadPriceType=new List<string>(); 
    List<string> hadPriceNumber=new List<string>(); 
    public void GetHadPrice()//List<PriceInfo> newList,List<PriceInfo> oldList)获取已经报价车辆
    {
        
        NetworkManager.Instance.DoGet1(API.GetHadPriceCars, (responsecode, data) =>
        {
            if (responsecode==200)
            {
                JsonData jsonData= JsonMapper.ToObject(data)["data"]["data"];
                Debug.Log(jsonData.ToJson());
                for (int i = 0; i < jsonData.Count; i++)
                {
                     hadPriceType.Add(jsonData[i]["cart"]["car_Type"].ToJson());//todo 请求，获取已报价的信息，竟然没有车型，后端得加上给我返回
                     hadPriceNumber.Add(jsonData[i]["cart"]["carNumber"].ToJson());
                }
                Debug.Log("123  "+jsonData.ToJson());
            }
        },NetworkManager.Instance.token);
        // StartCoroutine(CompareData( newList, oldList, hadPrice));
        // return oldList;
    }
    public void DoGetUserInfo()
    {
        string url = API._GetUserInfo1;
        networkManager.DoGet1(url, (responseCode, data) =>
        {
            //Debug.Log("_________responseCode:" + responseCode + "|" + data);
            if (responseCode == 200)
            {
                JsonData jsonData = JsonMapper.ToObject(data);
                JsonData dataObj = jsonData["data"];
                //Debug.Log("nickname:" + dataObj["nickname"]);
                //Debug.Log("email:" + dataObj["email"]);
                //Debug.Log("id:" + dataObj["id"]);
                SecondPanelCtrl.Instance.textUserID.text = dataObj["id"].ToString();
                SecondPanelCtrl.Instance.textNickName.text = dataObj["email"].ToString();
                Debug.Log("barandid  "+dataObj["brand_id"].ToString());
                PlayerPrefs.SetString("brand_id",dataObj["brand_id"].ToString());
               // PriceManager.Instance.
            }
            else
            {
                Debug.Log(responseCode+"    ");
            }
        }, networkManager.token);
    }


    private void ResetRegisterPanel()
    {
        var registerManager = RegisterPanel.GetComponent<MyRegisterManager>();
        for (int i = 0; i < registerManager.inputFields.Length; i++)
        {
            registerManager.inputFields[i].text = "";
        }
    }

    public void ResetLoginPanel()
    {
        userName.text = "";
        pwd.text = "";
    }

    private void RememberValChange(bool isOn)
    {
        int val = isOn ? 1 : 0;
        PlayerPrefs.SetInt("remember", val);
    }

}
