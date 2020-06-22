using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OfficeOpenXml;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System;
using LitJson;

public class PriceManager : MonoBehaviour
{
    public static PriceManager Instance;

    public Transform page1;
    public Transform page2;

    public GameObject priceManagerItem,registorMgrItem;
    public Transform itemContainer,regisItemContainer;

    public List<PriceInfo> priceInfos = new List<PriceInfo>();
    private List<string> carNumberList = new List<string>();
    private List<string> carTypeList = new List<string>();


    #region page2 references
    [Header("page2")]
    public Text textCarType;
    public Text textCarSeries;
    public Toggle toggleCity;
    public Toggle toggleProvince;
    public Toggle toggleCountry;
    public ToggleGroup toggleGroupRegArea;

    public InputField inputCarPrice;
    public InputField inputRegistrationPrice;
    public InputField inputInsurance;
    public InputField inputTax;
    public InputField inputFinancial;
    public InputField inputOtherPrice;

    public InputField inputOfferPrice;
    public InputField inputBargainPrice;

    public InputField inputJingPin;
    public InputField inputDecorationContent;
    

  //  public Dropdown dropRegistrationType;
  //  public Dropdown dropInsuranceType;  根据要求删掉的部分2020.6.18 8:04AM

    public Button btnAddJingPin;

    public Button btnSave;

    public Dropdown dropJingPingList;

    private PriceInfo currPriceInfo;        //保存数据库字段(Excel字段) 

    private PriceManagerItem currPriceManagerItem;

    #endregion

    private object objlock;
    //private string filePath = Application.streamingAssetsPath + "/深圳锦奥库存CKD_200614.xlsx"; 
    public string filePath;
    private Thread loadThread;
    private bool loadEnd = false;

    public GameObject mainManager;

    public NetworkManager networkManager;

    public List<PriceManagerItem> priceManagerItems = new List<PriceManagerItem>();     //全局（主页与价格管理）共享报价列表数据

    public static event Action LoadExcelEndEvent;

    private DBManager dBManager;

    private Dictionary<string, List<string>> vehicleSystemsDic = new Dictionary<string, List<string>>();    //保存车系与车型之间关系，用于Post上传到后台

    

    private int currRegisterAreaType = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        ChangeToPage(1);
    }

    private void Start()
    {
        Init();

        if (PlayerPrefs.HasKey("XiaoKuExcelPath"))
        {
            string temp = PlayerPrefs.GetString("XiaoKuExcelPath");
            Debug.Log("____________XiaoKuExcelPath:" + temp);
            if (File.Exists(temp))
            {
                filePath = temp;
            }
        }

        objlock = new object();
        DoLoadThread();
        //DoGetCarList();
        this.gameObject.SetActive(false);
    }

    private void Init()
    {
        networkManager = NetworkManager.Instance;
        dBManager = DBManager._DBInstance();

        dBManager.CreateTable(typeof(PriceInfo));
        priceInfos = dBManager.QueryTable<PriceInfo>();

        toggleCity.onValueChanged.AddListener((value)=> {
            currRegisterAreaType = 3;
        });

        toggleProvince.onValueChanged.AddListener((value) => {
            currRegisterAreaType = 2;
        });

        toggleCountry.onValueChanged.AddListener((value) => {
            currRegisterAreaType = 1;
        });

        if (priceInfos.Count > 0)
        {
            Debug.Log("________priceInfos.Count:" + priceInfos.Count);
            UpdateUI();
        }

        btnSave.onClick.AddListener(() =>
        {
            DoPostOfferPrice();

            SaveInputInfo();

            ChangeToPage(1);
        });

        btnAddJingPin.onClick.AddListener(OnClickAddJingPin);
    }

    private void Update()
    {
        if (loadEnd)
        {
            DoPostCarType();
            UpdateUI();
            LoadExcelEndEvent();
            loadEnd = false;
        }
    }

    private void OnDestroy()
    {
        loadThread.Abort();
    }


    private void OnClickAddJingPin()
    {
        string temp = inputJingPin.text;
        if (temp != "")
        {
            currPriceManagerItem.offerPriceData.jingpin.Add(temp);
            dropJingPingList.ClearOptions();
            dropJingPingList.AddOptions(currPriceManagerItem.offerPriceData.jingpin);
        }
    }

    public void DoLoadThread(string path = "")
    {
        loadThread = new Thread(new ThreadStart(() =>
        {
            try
            {
                lock (objlock)
                {
                    if (path != "")
                        ReadCarPrice(path);
                    else
                        ReadCarPrice();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("打开线程失败 : " + e.ToString());
            }

        }));
        loadThread.IsBackground = true;
        loadThread.Priority = System.Threading.ThreadPriority.Highest;
        loadThread.Start();
    }
    
    private void ReadCarPrice(string path = "")
    {
        if (path != "") { 
            filePath = path;
        }
        if (!File.Exists(filePath))
            return;
        FileInfo newFile = new FileInfo(filePath);
        using (ExcelPackage package = new ExcelPackage(newFile))
        {
            var worksheets = package.Workbook.Worksheets;
            Debug.Log("worksheet:" + worksheets.Count);
            int wIndex = 1;

            foreach (var w in worksheets)
            {
                if (!w.Name.Contains("库存")) continue;//只读库存
                Debug.Log(w + " " + w.Index);
                int minColumnNum = w.Dimension.Start.Column;//工作区开始列
                int maxColumnNum = w.Dimension.End.Column; //工作区结束列
                int minRowNum = w.Dimension.Start.Row; //工作区开始行号
                int maxRowNum = w.Dimension.End.Row; //工作区结束行号

                int[] tableTitle = new int[18];
                string tmpKey = "";

                Debug.Log(minColumnNum + "|" + maxColumnNum + "|" + minRowNum + "|" + maxRowNum);

                string tmpCarType = "";
                string prevCarType = "";//用于解决上次值为空的问题    
                for (int i = minRowNum; i < maxRowNum; i++)
                {
                    string note = "";
                    PriceInfo item = new PriceInfo();
                    for (int j = minColumnNum; j < maxColumnNum; j++)
                    {
                        var cell = w.Cells[i, j];
                        //获取表头
                        /*
                         * 排放、车型、指导价、车架号、发车日期、到店日期、库龄、AAK日期、质sun、合格证、客户姓名、销售顾问、otc
                         */
                        if (cell.RichText.Text.Contains("排放"))
                        {
                            tableTitle[0] = cell.Start.Column;
                        }
                        else if (cell.RichText.Text.Contains("车型") && cell.RichText.Text.Length < 10)
                        {
                            //当再次遇到车型标签表示重新计算表头
                            for (int ii = 0; ii < tableTitle.Length; ii++)
                            {
                                tableTitle[ii] = -1;
                            }
                            tableTitle[1] = cell.Start.Column;
                        }
                        else if (cell.RichText.Text.Contains("指导价") && cell.RichText.Text.Length < 10)
                        {
                            tableTitle[2] = cell.Start.Column;
                        }
                        else if (cell.RichText.Text.Contains("车架") && cell.RichText.Text.Length < 10)
                        {
                            tableTitle[3] = cell.Start.Column;
                        }
                        else if (cell.RichText.Text.Contains("发车日期") && cell.RichText.Text.Length < 10)
                        {
                            tableTitle[4] = cell.Start.Column;
                        }
                        else if (cell.RichText.Text.Contains("到店日期") && cell.RichText.Text.Length < 10)
                        {
                            tableTitle[5] = cell.Start.Column;
                        }
                        else if (cell.RichText.Text.Contains("库龄") && cell.RichText.Text.Length < 10)
                        {
                            tableTitle[6] = cell.Start.Column;
                        }
                        else if (cell.RichText.Text.Contains("AAK日期") && cell.RichText.Text.Length < 10)
                        {
                            tableTitle[7] = cell.Start.Column;
                        }
                        else if (cell.RichText.Text.Contains("质损") && cell.RichText.Text.Length < 10)
                        {
                            tableTitle[8] = cell.Start.Column;
                        }
                        else if (cell.RichText.Text.Contains("合格证") && cell.RichText.Text.Length < 10)
                        {
                            tableTitle[9] = cell.Start.Column;
                        }
                        else if (cell.RichText.Text.Contains("客户姓名") && cell.RichText.Text.Length < 10)
                        {
                            tableTitle[10] = cell.Start.Column;
                        }
                        else if (cell.RichText.Text.Contains("销售顾问") && cell.RichText.Text.Length < 10)
                        {
                            tableTitle[11] = cell.Start.Column;
                        }
                        else if (cell.RichText.Text.Contains("组别") && cell.RichText.Text.Length < 10)
                        {
                            tableTitle[12] = cell.Start.Column;
                        }
                        else if (cell.RichText.Text.Contains("签订日期") && cell.RichText.Text.Length < 10)
                        {
                            tableTitle[13] = cell.Start.Column;
                        }
                        else if (cell.RichText.Text.Contains("配车天数") && cell.RichText.Text.Length < 10)
                        {
                            tableTitle[14] = cell.Start.Column;
                        }
                        else if (cell.RichText.Text.Contains("付款方式") && cell.RichText.Text.Length < 10)
                        {
                            tableTitle[15] = cell.Start.Column;
                        }
                        else if (cell.RichText.Text.Contains("备注") && cell.RichText.Text.Length < 10)
                        {
                            tableTitle[16] = cell.Start.Column;
                        }
                        else if (cell.RichText.Text.Contains("外/内") && cell.RichText.Text.Length < 10)
                        {
                            tableTitle[17] = cell.Start.Column;
                        }
                        else
                        {
                            for (int k = 0; k < tableTitle.Length; k++)
                            {
                                if (cell.Start.Column == tableTitle[k])
                                {
                                    switch (k)
                                    {
                                        case 0:
                                            item.discharge = cell.RichText.Text;//排放                                            
                                            break;
                                        case 1:
                                            //item.carType = a.RichText.Text;//车型
                                            tmpCarType = cell.RichText.Text;//车型
                                            break;
                                        case 2:
                                            item.guidancePrice = cell.RichText.Text;//指导价
                                            break;
                                        case 3:
                                           // item.carNumber = cell.RichText.Text;//车架号
                                            tmpKey = cell.RichText.Text;//车架号作为关键字
                                                                        //to do 改成id自增
                                            break;
                                        case 4:
                                            {
                                                string dateStr = cell.RichText.Text;
                                                Match mInfo = Regex.Match(dateStr, @"(?i)^[0-9]+$");
                                                if (dateStr.Trim() != string.Empty && dateStr.Length == 5 && mInfo.Success)
                                                {
                                                    dateStr = DateTime.FromOADate(double.Parse(dateStr)).ToString("yyyy/MM/dd");
                                                }
                                                item.releaseDate = dateStr;//发车日期
                                            }
                                            break;
                                        case 5:
                                            {
                                                string dateStr = cell.RichText.Text;
                                                Match mInfo = Regex.Match(dateStr, @"(?i)^[0-9]+$");
                                                if (dateStr.Trim() != string.Empty && dateStr.Length == 5 && mInfo.Success)
                                                {
                                                    dateStr = DateTime.FromOADate(double.Parse(dateStr)).ToString("yyyy/MM/dd");
                                                }
                                                item.arriveDate = dateStr;//到店日期
                                            }
                                            break;
                                        case 6:
                                            item.garageAge = cell.RichText.Text;//库龄
                                            break;
                                        case 7:
                                            {
                                                string dateStr = cell.RichText.Text;
                                                Match mInfo = Regex.Match(dateStr, @"(?i)^[0-9]+$");
                                                if (dateStr.Trim() != string.Empty && dateStr.Length == 5 && mInfo.Success)
                                                {
                                                    dateStr = DateTime.FromOADate(double.Parse(dateStr)).ToString("yyyy/MM/dd");
                                                }
                                                item.akkDate = dateStr;//AAK日期
                                            }
                                            break;
                                        case 8:
                                            item.qualityloss = cell.RichText.Text;//质损
                                            break;
                                        case 9:
                                            item.certificate = cell.RichText.Text;//合格证
                                            break;
                                        case 10:
                                            item.userName = cell.RichText.Text;//客户姓名
                                            break;
                                        case 11:
                                            item.adviser = cell.RichText.Text;//销售顾问
                                            break;
                                        case 12:
                                            item.carGroup = cell.RichText.Text;//组别
                                            break;
                                        case 13:
                                            {
                                                string dateStr = cell.RichText.Text;
                                                Match mInfo = Regex.Match(dateStr, @"(?i)^[0-9]+$");
                                                if (dateStr.Trim() != string.Empty && dateStr.Length == 5 && mInfo.Success)
                                                {
                                                    dateStr = DateTime.FromOADate(double.Parse(dateStr)).ToString("yyyy/MM/dd");
                                                }
                                                item.signDate = dateStr;//签订日期
                                            }
                                            break;
                                        case 14:
                                            item.useTime = cell.RichText.Text;//配车天数
                                            break;
                                        case 15:
                                            item.payType = cell.RichText.Text;//付款方式
                                            break;
                                        case 16:
                                            item.memo = cell.RichText.Text;//备注
                                            break;
                                        case 17:
                                            item.color = cell.RichText.Text;//外/内 颜色
                                            break;
                                    }
                                }
                            }
                            if (cell.Comment != null && cell.Comment.RichText.Text.Trim() != string.Empty)
                            {
                                note += "\n" + cell.Comment.RichText.Text;//获取备注
                            }
                        }
                    }
                    if (tmpKey.Trim() != string.Empty)
                    {
                        Match mInfo = Regex.Match(tmpKey, @"(?i)^[0-9a-z]+$");
                        if (mInfo.Success) //如果是英文和数字
                        {
                            //item.id = id;
                            item.carNumber = tmpKey;
                            if (tmpCarType.Trim() != string.Empty)
                            {
                                item.carType = tmpCarType;
                            }
                            else
                            {
                                item.carType = prevCarType;
                            }
                            item.note = note;//批注
                            item.vehicleSystem = w.Name;//车系
                            item.brand = "奥迪";                  //todo:改为实时输入
                            
                            if (!carNumberList.Contains(item.carNumber))
                            {
                                carNumberList.Add(item.carNumber);
                                Debug.Log("***********add carnumber:" + item.carNumber);
                                priceInfos.Add(item);
                                //加入数据库，替换当前数据
                                dBManager.CheckReplace<PriceInfo>("carNumber", item.carNumber, item);
                            }
                            prevCarType = tmpCarType;
                        }
                    }
                    Thread.CurrentThread.Join(1);
                }
                wIndex = wIndex + 1;
                //Debug.Log("__________current product infos count: " + priceInfos.Count);
            }


            Thread.CurrentThread.Join(1000);//阻止设定时间
            loadEnd = true;
        }
    }
   public  int count=1 ;
    /// <summary>
    /// 依据数据库或Excel内数据，刷新报价列表
    /// </summary>
    private void UpdateUI()
    {
        
        GameObject go,gos;
        RegistorItem reItem;
        PriceManagerItem item;
        for (int i = 0; i < priceInfos.Count; i++)
        {
            if (priceInfos[i].carNumber != null && !carNumberList.Contains(priceInfos[i].carNumber))
                carNumberList.Add(priceInfos[i].carNumber);             //避免读取Excel表格时候，重复添加 
            if (priceInfos[i].carType != "" && !carTypeList.Contains(priceInfos[i].carType))
            {
                go = Instantiate(priceManagerItem, itemContainer);

                item = go.GetComponent<PriceManagerItem>();

                item.SetItemContent(count.ToString(), priceInfos[i].guidancePrice,
                    priceInfos[i].vehicleSystem, priceInfos[i].carType, "未上架");//订单管理显示的

                gos = Instantiate(registorMgrItem,regisItemContainer);
                reItem = gos.GetComponent<RegistorItem>();
                reItem.SetItemContent(count.ToString(),priceInfos[i].carNumber,priceInfos[i].vehicleSystem,priceInfos[i].memo,priceInfos[i].carType);//库存管理显示的

                if (item.offerPriceData == null)
                    item.offerPriceData = new PostDataForOfferPrice();

                item.priceInfo = priceInfos[i];

                if (priceInfos[i].carType != "" && !carTypeList.Contains(priceInfos[i].carType))
                {
                    carTypeList.Add(priceInfos[i].carType);
                }

                priceManagerItems.Add(item);

                string vs = priceInfos[i].vehicleSystem.Replace("库存", "");
                if (!vehicleSystemsDic.ContainsKey(vs))
                {
                    vehicleSystemsDic.Add(vs, new List<string>());
                }

                if (priceInfos[i].carType != "" && !vehicleSystemsDic[vs].Contains(priceInfos[i].carType))
                {
                    vehicleSystemsDic[vs].Add(priceInfos[i].carType);
                }

                count++;
            }
            
        }
        Debug.Log("________priceManagerItems.count:" + priceManagerItems.Count);
    }

    /// <summary>
    /// PriceItem回调，设置Page2的UI
    /// </summary>
    /// <param name="item"></param>
    public void SetItemForPage2(PriceManagerItem item)
    {
        currPriceInfo = item.priceInfo;

        currPriceManagerItem = item;

        var offerPriceMsg = item.offerPriceData;     //for post

        textCarType.text = item.priceInfo.carType;

        textCarSeries.text = item.priceInfo.vehicleSystem;

        switch (item.offerPriceData.registration_area_type)
        {
            case 0:
                toggleGroupRegArea.NotifyToggleOn(toggleCity);
                break;
            case 1:
                toggleGroupRegArea.NotifyToggleOn(toggleProvince);
                break;
            case 2:
                toggleGroupRegArea.NotifyToggleOn(toggleCountry);
                break;
            default:
                toggleGroupRegArea.NotifyToggleOn(toggleCity);
                break;
        }

        if (item.text_status.text == "已上架")
        {
            inputCarPrice.text = offerPriceMsg.net_price;
            inputRegistrationPrice.text = offerPriceMsg.registration_price;
            inputFinancial.text = offerPriceMsg.financial_agents_price;
            inputInsurance.text = offerPriceMsg.insurance_price;
            inputOtherPrice.text = offerPriceMsg.other_price;
            inputTax.text = offerPriceMsg.purchase_tax;

           // dropInsuranceType.value = offerPriceMsg.insuranceType;
           // dropRegistrationType.value = offerPriceMsg.registerType;

            inputOfferPrice.text = offerPriceMsg.officialPrice;
            inputBargainPrice.text = offerPriceMsg.bargainPrice;

            Toggle toggle;
            switch (offerPriceMsg.registration_area_type)
            {
                case 1:
                    toggle = toggleCountry;
                    break;
                case 2:
                    toggle = toggleProvince;
                    break;
                case 3:
                    toggle = toggleCity;
                    break;
                default:
                    toggle = toggleCity;
                    break;
            }
            toggleGroupRegArea.NotifyToggleOn(toggle);
            inputDecorationContent.text = offerPriceMsg.remarkOfDecoration;

            dropJingPingList.ClearOptions();
            dropJingPingList.AddOptions(offerPriceMsg.jingpin);
            if (offerPriceMsg.jingpin.Count > 0)
            {
                inputJingPin.text = offerPriceMsg.jingpin[0];
            }
        }
        else
        {
            inputCarPrice.text = "";
            inputRegistrationPrice.text = "";
            inputFinancial.text = "";
            inputInsurance.text = "";
            inputOtherPrice.text = "";
            inputTax.text = "";

            inputOfferPrice.text = "";
            inputBargainPrice.text = "";

           // dropInsuranceType.value = 0;
          //  dropRegistrationType.value = 0;

            toggleGroupRegArea.NotifyToggleOn(toggleCity);
            inputDecorationContent.text = "";
            inputJingPin.text = "";

            dropJingPingList.ClearOptions();
        }

    }


    public void ChangeToPage(int page)
    {
        if (gameObject.activeSelf == false)
            SecondPanelCtrl.Instance.OpenTargetPage("btn_priceManager");
        if (page == 1)
        {
            page1.gameObject.SetActive(true);
            page2.gameObject.SetActive(false);
        }
        else if (page == 2)
        {
            page1.gameObject.SetActive(false);
            page2.gameObject.SetActive(true);
        }

        //TODO: 页面切换后的(数据传递) 页面重置刷新
    }


    public void DoGetCarList(string page = "1", string pageSize = "10", string brand_id = "1", string vehicleSystem = "Q3", string carType = "车型")
    {
        Debug.Log("token: " + networkManager.token);
        //string url = API.GetMsgList + "&page=" + page + "&pageSize=" + pageSize + "&brand_id=" + brand_id
        //    + "&vehicleSystem=" + vehicleSystem + "&carType=" + carType;
        string url = API._GetCarList;
        networkManager.DoGet1(url, (responseCode, data) =>
        {
            Debug.Log("responseCode:" + responseCode + "|" + data);
        }, networkManager.token);

    }


    /// <summary>
    /// 点击保存后，发起post传输currPriceInfo数据给后台
    /// </summary>
    public void DoPostOfferPrice()
    {
        Debug.Log("提交表单信息 ！！！");
        WWWForm form = new WWWForm();

        List<PriceInfo> tempInfoList = new List<PriceInfo>();
        string carType = currPriceInfo.carType;
        for (int i = 0; i < priceInfos.Count; i++)
        {
            if (priceInfos[i].carType == carType)
            {
                Debug.LogFormat("____{2}, carType: {0}, carNumber: {1}", carType, priceInfos[i].carNumber, i);
                tempInfoList.Add(priceInfos[i]);
            }
        }

        for (int i = 0; i < tempInfoList.Count; i++)
        {
            tempInfoList[i].vehicleSystem = tempInfoList[i].vehicleSystem.Replace("库存", "");

            string jsonString = JsonMapper.ToJson(tempInfoList[i]);
            JsonData jsonData = JsonMapper.ToObject(jsonString);

            jsonData["net_price"] = inputCarPrice.text;
            jsonData["financial_agents_price"] = inputFinancial.text;
            jsonData["insurance_price"] = inputInsurance.text;
            jsonData["registration_price"] = inputRegistrationPrice.text;
            jsonData["purchase_tax"] = inputTax.text;
            jsonData["other_price"] = inputOtherPrice.text;

          //  jsonData["registration_type"] = dropRegistrationType.options[dropRegistrationType.value].text;
           // jsonData["insurance_type"] = dropInsuranceType.options[dropInsuranceType.value].text;
            jsonData["content_remark"] = inputDecorationContent.text;
            jsonData["appear_color"] = tempInfoList[i].color;

            jsonData["registration_area_type"] = currRegisterAreaType.ToString();

            Debug.Log("________准备上传的 jsonData:" + jsonData.ToJson());
            //jsonString = JsonMapper.ToJson(jsonData);
            jsonString = jsonData.ToJson();

            form.AddField("d[]", jsonString);
        }
        networkManager.DoPost1(API._PostOfferPrice1, form, (responseCode, content) =>
        {
            Debug.Log("____responseCode:" + responseCode + ", content:" + content);
        }, networkManager.token);

    }

    private void SaveInputInfo()
    {
        if (currPriceManagerItem.offerPriceData == null)
        {
            currPriceManagerItem.offerPriceData = new PostDataForOfferPrice();
        }
        currPriceManagerItem.offerPriceData.carType = currPriceInfo.carType;
        currPriceManagerItem.offerPriceData.carSeries = currPriceInfo.vehicleSystem;
        currPriceManagerItem.offerPriceData.car_id = currPriceInfo.id.ToString();       //车辆序号, 对(车系)价格管理 基本无用
        currPriceManagerItem.offerPriceData.registration_area_type = currRegisterAreaType;       

        currPriceManagerItem.offerPriceData.net_price = inputCarPrice.text;
        currPriceManagerItem.offerPriceData.financial_agents_price = inputFinancial.text;
        currPriceManagerItem.offerPriceData.insurance_price = inputInsurance.text;
        currPriceManagerItem.offerPriceData.registration_price = inputRegistrationPrice.text;
        currPriceManagerItem.offerPriceData.purchase_tax = inputTax.text;
        currPriceManagerItem.offerPriceData.other_price = inputOtherPrice.text;

      //  currPriceManagerItem.offerPriceData.registerType = dropRegistrationType.value;
      //  currPriceManagerItem.offerPriceData.insuranceType = dropInsuranceType.value;

        currPriceManagerItem.offerPriceData.bargainPrice = inputBargainPrice.text;
        currPriceManagerItem.offerPriceData.officialPrice = inputOfferPrice.text;

        currPriceManagerItem.offerPriceData.remarkOfDecoration = inputDecorationContent.text;

        if (currPriceManagerItem.offerPriceData.carNumbers == null)
        {
            currPriceManagerItem.offerPriceData.carNumbers = new List<string>();
        }

        for (int i = 0; i < priceInfos.Count; i++)
        {
            if (priceInfos[i].carType == currPriceInfo.carType)
            {
                if (priceInfos[i].carNumber != "" && !currPriceManagerItem.offerPriceData.carNumbers.Contains(priceInfos[i].carNumber))
                {
                    currPriceManagerItem.offerPriceData.carNumbers.Add(priceInfos[i].carNumber);
                }
            }
        }

        int index = int.Parse(currPriceManagerItem.text_index.text) - 1;

        priceManagerItems[index].UpdateItem(currPriceManagerItem.offerPriceData.officialPrice, "已上架");

        MyHomePage.Instance.UpdateTargetItem(index);
    }

    /// <summary>
    /// Post上传车系车型
    /// </summary>
    public void DoPostCarType()
    {
        string url = API._PostCarType;
        WWWForm form = new WWWForm();

        BrandCarTypeInfo bcti = new BrandCarTypeInfo();
        bcti.brand_name = "奥迪";
        bcti.cart_lines = new List<CarLine>();
        foreach (var keyValuePair in vehicleSystemsDic)
        {
            CarLine cl = new CarLine();
            cl.line_name = keyValuePair.Key;
            cl.cart_models = new List<CartModel>();
            for (int i = 0; i < keyValuePair.Value.Count; i++)
            {
                CartModel cm = new CartModel();
                cm.model_name = keyValuePair.Value[i];
                cl.cart_models.Add(cm);
            }
            bcti.cart_lines.Add(cl);

        }

        string jsonData = JsonMapper.ToJson(bcti);
        form.AddField("d[]", jsonData);
        networkManager.DoPost1(url, form, (responseCode, data) =>
        {
            //Debug.Log("responseCode:" + responseCode + "|" + data);

        }, networkManager.token);
    }


    public void ClearAllData()
    {
        priceManagerItems.Clear();
        carNumberList.Clear();
        carTypeList.Clear();
        vehicleSystemsDic.Clear();
        priceInfos.Clear();

        for (int i = 0; i < itemContainer.childCount; i++)
        {
            Destroy(itemContainer.GetChild(i).gameObject);
        }
    }


    class BrandCarTypeInfo
    {
        public string brand_name;
        public List<CarLine> cart_lines;
    }

    class CarLine
    {
        public string line_name;
        public List<CartModel> cart_models;
    }

    class CartModel
    {
        public string model_name;
    }

}


public class CarTypeInfo
{
    public int index;
    public string vehicleSystem;
    public string carType;
    public string price;           //报价
    public string status;          //是否上架
    public string[] carNumbers;    //车架号
    public int area;

    public string netPrice;
    public string registerPrice;
    public string insurance;
    public string tax;
    public string financialServicePrice;
    public string otherPrice;

    public int registerType;       //0:自付上牌资格  1:代办上牌资格   2:全选
    public int insuranceType;      //0:100万以上   1:其他险种

    public string bargainPrice;

    public string[] jingpin;

    public string remarkOfDecoration;
}
