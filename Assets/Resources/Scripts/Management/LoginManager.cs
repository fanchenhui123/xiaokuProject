using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{

    public Button closeBtn;
    public Button loginBtn;
    public Button exitBtn;
    public Button registerBtn;
    public Text warnText;

    public InputField phone;
    public InputField pwd;
    public Toggle remember;
    private string ip;

    public GameObject LoginPanel;
    public GameObject RegisterPanel;

	public MessagePanelManager messagePanelManager;

    public NetworkManager networkManager;

    public ClickEvent clickEvent;

    private DBManager dBManager;

    private UserInfo userInfo;

    public BodyView bodyView;
    public TableView tableView;

    bool isFirst = true;
    public string userId;
    public static LoginManager Instance;

    public void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
        }
        var rectT = transform.GetComponent<RectTransform>();
        rectT.sizeDelta = new Vector2(1920, 1080);
        loginBtn.onClick.AddListener(Login);
        closeBtn.onClick.AddListener(() =>
        {
            if (isFirst)
            {
                clickEvent.Quit();
            }
            else
            {
                gameObject.SetActive(false);
            }
        });
        exitBtn.onClick.AddListener(clickEvent.Quit);
        registerBtn.onClick.AddListener(OpenRegisterPanel);
        remember.onValueChanged.AddListener(RememberValChange);
        InitLoginPanel();
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
            phone.text = PlayerPrefs.GetString("username", "");
            pwd.text = PlayerPrefs.GetString("password", "");
        }
        else
        {
            Debug.Log("没有记住密码");
        }
    }


    public void Login()
	{
        PlayerPrefs.SetString("username", phone.text);
        PlayerPrefs.SetString("password", pwd.text);

        userId = phone.text;
        var form = new WWWForm();
        if(phone.text == string.Empty)
        {
            warnText.text = "邮箱不能为空";
            return;
        }
        form.AddField("ip", this.ip);
        form.AddField("email", phone.text);
        form.AddField("password", pwd.text);

        networkManager.DoPost(API.LoginUrl, form, delegate (string responseCode, string data)
        {
            Debug.Log("responseCode:" + responseCode + "|" + data);
            if (responseCode == "200")
            {
                JsonData jdata = JsonMapper.ToObject(data);
                if (jdata["code"].ToString() == "0")
                {
                    warnText.text = jdata["msg"].ToString();
                    networkManager.token = jdata["token"].ToString();
                    LoginSuccess();
                }
                else
                {
                    warnText.text = jdata["msg"].ToString();
                }
                Debug.Log(jdata.ToString());
            }
            else
            {
                warnText.text = "网络故障";
                LoginFail();
            }
        });       
    }

    private void LoginSuccess()
	{
        messagePanelManager.Show("登录成功");
        isFirst = false;//第二次不退出
        LoginPanel.SetActive(false);

        tableView.Init(4);
        FindObjectOfType<AutoLoadExcelManager>().StartAutoLoad();

        #region  2.0  登录成功后，加载价格管理数据、消息数目等



        #endregion
    }
    private void LoginFail()
    {
        messagePanelManager.Show("登录失败");
	}


    private IEnumerator DelayMessage(string info)
    {
        yield return new WaitForSeconds(1f);
        messagePanelManager.Show(info);
        yield return new WaitForSeconds(1f);
        messagePanelManager.Hide();
    }

    private void OpenRegisterPanel()
    {
        Debug.Log("click OpenRegisterPanel");
        RegisterPanel.GetComponent<RegisterManager>().Show(true);
    }
    
    private void RememberValChange(bool isOn)
    {
        int val = isOn ? 1 : 0;
        PlayerPrefs.SetInt("remember", val);
    }

}
