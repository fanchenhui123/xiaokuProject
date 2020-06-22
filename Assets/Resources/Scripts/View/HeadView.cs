using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadView : MonoBehaviour
{
    public Button RefreshBtn;
    public Button DownloadBtn;
    public Button ConcernBtn;
    public Button AccountBtn;
    public Button OptionBtn;
    public Button ZoomBtn;
    public Button CloseBtn;
    public Button LoadExcelBtn;
    public Button OpenExcelBtn;
    public Button LoadCarTypeBtn;
    public Button ShowMiniBtn;
    public Button SettingBtn;

    public Button MessageCenterBtn;

    public Dropdown dropdown;
    public Dropdown carSysDropdown;

    public Text currentPanelName;

    public Button merchantsBtn;
    public Button LogoBtn;

    public InputField searchInput;
    public Button searchBtn;

    public ClickEvent clickEvent;

    public GameObject BodyPanel;
    public GameObject AccountPanel;
    public GameObject TablePanel;
    public GameObject SettingPanel;
    public GameObject LoginPanel;

    //SecondPanel
    public GameObject FirstPanel;
    public GameObject SecondPanel;
    public GameObject MsgCenterPanel;

    public void Awake()
    {
        //var rectTransform = transform.GetComponent<RectTransform>();
        clickEvent = FindObjectOfType<ClickEvent>();//绑定按钮回调
        //rectTransform.sizeDelta = new Vector2(Screen.width, rectTransform.rect.height);
    }

    public void Start()
    {
        OptionBtnListening();
    }
    private void OptionBtnListening()
    {
        RefreshBtn.onClick.AddListener(delegate
        {
            TablePanel.GetComponent<TableView>().DoRefresh();
        });
        DownloadBtn.onClick.AddListener(clickEvent.Download);
        //AccountBtn.onClick.AddListener(clickEvent.Account);
        OptionBtn.onClick.AddListener(clickEvent.Option);
        //ZoomBtn.onClick.AddListener(delegate {clickEvent.Zoom(1);});
        CloseBtn.onClick.AddListener(clickEvent.Quit);
        ConcernBtn.onClick.AddListener(OpenBodypanel);
        AccountBtn.onClick.AddListener(OpenAccontpanel);
        merchantsBtn.onClick.AddListener(OpenBodypanel);
        LogoBtn.onClick.AddListener(OpenLoginPanel);
        searchBtn.onClick.AddListener(delegate
        {
            if (!searchInput.text.Equals(""))
                TablePanel.GetComponent<TableView>().ScreeningMethod(searchInput.text);
            //TablePanel.GetComponent<TableView>().SearchMethod(searchInput.text);
        });

        OpenExcelBtn.onClick.AddListener(clickEvent.OpenExcel);
        LoadExcelBtn.onClick.AddListener(clickEvent.LoadExcel);

        LoadCarTypeBtn.onClick.AddListener(clickEvent.LoadCarType);

        dropdown.onValueChanged.AddListener(dropChangeBack);

        carSysDropdown.onValueChanged.AddListener(carSysDropChangeBack);
        ShowMiniBtn.onClick.AddListener(clickEvent.ShowMini);
        SettingBtn.onClick.AddListener(() => {
            SettingPanel.GetComponent<SettingView>().Show();
        });

        MessageCenterBtn.onClick.AddListener(()=> {
            if (MsgCenterPanel != null)
            {
                FirstPanel.SetActive(false);
                SecondPanel.SetActive(true);
                //MsgCenterPanel.SetActive(true);
            }
        });
    }

    private void OpenBodypanel()
    {
        //BodyPanel.SetActive(true);
        BodyPanel.GetComponent<ConcernView>().Show(true);
    }

    private void OpenAccontpanel()
    {
        AccountPanel.GetComponent<AccountView>().Show(true);
    }

    public void OpenLoginPanel()
    {
        //BodyPanel.SetActive(false);
        LoginPanel.gameObject.SetActive(true);

    }

    private void dropChangeBack(int status)
    {
        Debug.Log(status);
        dropdown.GetComponent<Dropdown>().interactable = false;
        if (status == 0)
        {
            TablePanel.GetComponent<TableView>().Init(1, () =>
            {
                dropdown.GetComponent<Dropdown>().interactable = true;
            });
        }
        else if (status == 1)
        {
            TablePanel.GetComponent<TableView>().Init(2, () =>
            {
                dropdown.GetComponent<Dropdown>().interactable = true;
            });
        }
        else if (status == 2)
        {
            TablePanel.GetComponent<TableView>().Init(3, () =>
            {
                dropdown.GetComponent<Dropdown>().interactable = true;
            });
        }
        else if (status == 3)
        {
            TablePanel.GetComponent<TableView>().Init(4, () =>
            {
                dropdown.GetComponent<Dropdown>().interactable = true;
            });
        }
        else if (status == 4)
        {
            TablePanel.GetComponent<TableView>().Init(5, () =>
            {
                dropdown.GetComponent<Dropdown>().interactable = true;
            });
        }

    }

    private void carSysDropChangeBack(int status)
    {
        Debug.Log("carSysDropChangeBack:" + status);
        string sql = null;
        Debug.Log(carSysDropdown.options[status].text);
        if (carSysDropdown.options[status].text != "all")
        {
            sql = "where vehicleSystem='" + carSysDropdown.options[status].text + "'";
        }
        TablePanel.GetComponent<TableView>().Init(dropdown.value + 1, null, sql);
    }

    List<string> carSysData = new List<string>();

    public void SetCarDropDownData(int state ,List<string> data)
    {
        dropdown.value = state;
        carSysDropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        options.Add(new Dropdown.OptionData("all"));
        for (int i = 0; i < data.Count; i++) {
            options.Add(new Dropdown.OptionData(data[i]));
         }
        carSysDropdown.AddOptions(options);
    }
}
