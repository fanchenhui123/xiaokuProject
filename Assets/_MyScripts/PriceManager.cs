using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OfficeOpenXml;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using LitJson;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using UnityEngine.Networking;

public class PriceManager : MonoBehaviour
{
    public static PriceManager Instance;

    public Transform page1;
    public Transform page2;

    public GameObject priceManagerItem;
    public Transform itemContainer;

    public List<PriceInfo> priceInfos = new List<PriceInfo>();
    public List<PriceInfo> priceInfosLast = new List<PriceInfo>();
    public List<string> carNumberList = new List<string>();
    public List<string> carTypeList = new List<string>();
    public List<string> putSJ=new List<string>();//存入已上架的车型，普通报价后存入
    public Button specialCarbtn;

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
    

    public Dropdown dropRegistrationType;
    public Dropdown dropInsuranceType;  //根据要求删掉的部分2020.6.18 8:04AM

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
    public bool loadEnd = false;

    public GameObject mainManager;

    public NetworkManager networkManager;

    public List<PriceManagerItem> priceManagerItems = new List<PriceManagerItem>();     //全局（主页与价格管理）共享报价列表数据
  //  public List<PriceInfo> CanLoadedCars=new List<PriceInfo>();
    public static event Action LoadExcelEndEvent;

    private DBManager dBManager;

    public Dictionary<string, List<string>> vehicleSystemsDic = new Dictionary<string, List<string>>();    //保存车系与车型之间关系，用于Post上传到后台

    

    private int currRegisterAreaType = 0;

    private void Awake()
    {
        Instance = this;

    }

   

    private void OnEnable()
    {
        ChangeToPage(1);
        loadEnd = true;

    }

    private void Start()
    {
        Init();

        if (PlayerPrefs.HasKey("XiaoKuExcelPath"))
        {
            string temp = PlayerPrefs.GetString("XiaoKuExcelPath");
            //Debug.Log("____________XiaoKuExcelPath:" + temp);
            if (File.Exists(temp))
            {
                filePath = temp;
            }
        }

        objlock = new object();
     //  loadExcelsTest(PlayerPrefs.GetString("XiaoKuExcelPath"));
        Debug.Log("start");
        this.gameObject.SetActive(false);
    }

    private void Init()
    {
        networkManager = NetworkManager.Instance;
        dBManager = DBManager._DBInstance();

        dBManager.CreateTable(typeof(PriceInfo));
        priceInfos = dBManager.QueryTable<PriceInfo>();
        priceInfosLast = dBManager.QueryTable<PriceInfo>();

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
            UpdateUI();
        }

        btnSave.onClick.AddListener(() =>
        {
            DoPostOfferPrice();//

           // SaveInputInfo();
           
           
        });

        btnAddJingPin.onClick.AddListener(OnClickAddJingPin);
    }

    private void Update()
    {
        if (loadEnd)
        {
            loadEnd = false;
            DoPostCarType();
            UpdateUI();

            LoadExcelEndEvent();
        }
    }

    /*try
    {
        for (int i = 0; i < SpecialCarr.instance.ChangeStatus.Count; i++)
        {
            for (int j = 0; j < registorItems.Count; j++)
            {
                if ( registorItems[j].GetComponent<RegistorItem>().carNumber==SpecialCarr.instance.ChangeStatus[i].carNumber)
                {
                    registorItems[j].GetComponent<RegistorItem>().text_statu.text = "已上架";
                }
            }
        }

        for (int i = 0; i < putSJ.Count; i++)
        {
            for (int j = 0; j < priceItems.Count; j++)
            {
                if (putSJ[i]==priceItems[j].GetComponent<PriceManagerItem>().carNumber)
                {
                        
                }
            }
        }
    }
    catch (Exception e)
    {
        Debug.Log(" 添加已上架");
        // throw;
    }*/
    
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

    
    
    public void loadExcelsTest(string ExcelPath)
    {
        string  SourceExcelPath = @ExcelPath.Replace(@"\",@"/");         // @"E:\C# Projects\ConsoleApplication1\";
        Debug.Log("   ??  "+SourceExcelPath);
        DirectoryInfo mydir = new DirectoryInfo(SourceExcelPath);
        if (SourceExcelPath.EndsWith(".xlsx"))
        {
            PriceManager.Instance.DoLoadThread(SourceExcelPath);//读文件
        }
        else
        {
            if (mydir.GetFiles().Length <= 0)
            {
                tip.instance.SetMessae("路径下没有文件");
            }
            else
            {
                Debug.Log("文件夹里面有文件");
            }
            ArrayList list = new ArrayList();
            foreach (FileSystemInfo fsi in mydir.GetFileSystemInfos())
            {
                if (fsi is FileInfo)//
                {
                    FileInfo fi=(FileInfo)fsi;
                    if(fi.Extension.ToUpper()==".XLSX")//
                    {
                        list.Add(fi.Name);
                    }
                }
            }
        
            foreach(string str in list)
            { 
                Instance.DoLoadThread(SourceExcelPath+@"/"+ str);//读文件夹里的文件
            }
           
        }
       
        loadEnd = true;
       
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
                    {
                        ReadCarPrice(path);
                        Debug.Log("startread "+path);
                    }
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
    
    public void ReadCarPrice(string path = "")
    {
        tip.instance.SetMessae("开始读表");
        Debug.Log("开始读表");
      //  priceInfosLast = priceInfos;
       // priceInfos.Clear();
        if (path != "") { 
            filePath = path;
            Debug.Log("path     "+path);
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
                if (!w.Name.Contains("库存")) continue;//只读工作表名中带  库存  俩字的
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
                        else if (cell.RichText.Text.Contains("客户姓名") && cell.RichText.Text.Length<10)
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
            Debug.Log("infos.count   "+priceInfos.Count  );
            Thread.CurrentThread.Join(1000);//阻止设定时间
            
        }
        
    }

    
    /// <summary>
    /// 依据数据库或Excel内数据，刷新报价列表
    /// </summary>
    
    public List<GameObject> priceItems;//存入所有的价格管理页面的Item和仓库管理的Item

    public List<string> vehicSys=new List<string>();
    List<PriceInfo> newCarPrice=new List<PriceInfo>();

    public void UpdateUI()
    {
        
        CleanBeforeUpdataUi();
        GameObject go;
        PriceManagerItem item;
        int count = 1;
        string curType="";
        if (priceInfos.Count>0)
        {
            curType = priceInfos[0].carType;
        }
        for (int i = 0; i < priceItems.Count; i++)
        {
            Destroy(priceItems[i]);
        }
        priceItems.Clear();
        
        
        for (int i = 0; i < priceInfos.Count; i++)
        {
           
            if (priceInfos[i].carType != "" && !carTypeList.Contains(priceInfos[i].carType))
            {
                go = Instantiate(priceManagerItem, itemContainer);
                priceItems.Add(go);
                item = go.GetComponent<PriceManagerItem>();
                if (putSJ.Contains(priceInfos[i].carType))
                {
                    item.SetItemContent(count.ToString(), priceInfos[i].carNumber, priceInfos[i].guidancePrice,
                        priceInfos[i].vehicleSystem, priceInfos[i].carType, "已上架"); //订单管理显示的
                }
                else
                {
                    item.SetItemContent(count.ToString(), priceInfos[i].carNumber, priceInfos[i].guidancePrice,
                        priceInfos[i].vehicleSystem, priceInfos[i].carType, "未上架"); //订单管理显示的
                }
                
                if (item.offerPriceData == null)
                    item.offerPriceData = new PostDataForOfferPrice();

                item.priceInfo = priceInfos[i];

                carTypeList.Add(priceInfos[i].carType);

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
            }//生成表格

           
            if (priceInfos[i].carType=="")
            {
                priceInfos[i].carType = curType;
            }
            else
            {
                curType=priceInfos[i].carType;
            }

            if (!putedSJ.Contains(priceInfos[i]) )
            {
                if (putSJ.Contains(priceInfos[i].carType))
                {
                    newCarPrice.Add(priceInfos[i]);
                }
            }
        }
        StartCoroutine(postNewCarPrice(newCarPrice));
    }

    IEnumerator postNewCarPrice(List<PriceInfo> postList)
    {
        WWWForm form = new WWWForm();
        for (int i = 0; i < postList.Count; i++)
        {
            string jsonstring = JsonMapper.ToJson(postList[i]);
            form.AddField("d[]",jsonstring);
        }

        networkManager.DoPost1(API.AddPostPrice, form, (responseCode, content) =>
        {
            Debug.Log("responseCode  " + responseCode);
            if (responseCode=="200")
            {
             tip.instance.SetMessae("数据已更新");
             newCarPrice.Clear();
            }
        },
          
            networkManager.token);
            yield break;
    }


    public void CleanBeforeUpdataUi()//初步筛选数据，去掉已销售
    {
        string cart="";
        if (priceInfos.Count!=0)
        {
           cart = priceInfos[0].carType; 
           for (int i = 0; i < priceInfos.Count; i++)
           {
               if (string.IsNullOrEmpty(priceInfos[i].carType) )
               {
                   priceInfos[i].carType = cart;
               }
               else
               {
                   cart = priceInfos[i].carType;
               }
           }
        
           for (int i = priceInfos.Count-1; i >= 0; i--)
           {
               if (!string.IsNullOrEmpty(priceInfos[i].adviser) )
               {
                   priceInfos.Remove(priceInfos[i]);
                   continue;
               }
            
               if (!string.IsNullOrEmpty(priceInfos[i].userName) )
               {
                   priceInfos.Remove(priceInfos[i]);
                   continue;
               }

               
            
               /*
               if (SpecialCarr.instance!=null)
               {
                   if (SpecialCarr.instance.TJSJ.Contains(priceInfos[i].carNumber))
                   {
                       priceInfos.Remove(priceInfos[i]);
                   }
               }*/
           }
        }
       
       
        carTypeList.Clear();

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

          //  dropInsuranceType.value = 0;
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
    /// 点击保存后，发起post传输currPriceInfo数据给后台,把所有同车型的数据信息都上传.特价车POST方法
    /// </summary>
    private List<Dictionary<string,cost>> dics=new List<Dictionary<string, cost>>();
    private List<PriceInfo> putedSJ=new List<PriceInfo>();//已经报价的
  //  private List<string> HavaPriceInfoCarType=new List<string>();
 
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
            jsonData["content_remark"] = inputDecorationContent.text;
            jsonData["appear_color"] = tempInfoList[i].color;
            jsonData["registration_area_type"] = currRegisterAreaType.ToString();

            Debug.Log("________准备上传的 jsonData:" + jsonData.ToJson());
            //jsonString = JsonMapper.ToJson(jsonData);
            jsonString = jsonData.ToJson();

            form.AddField("d[]", jsonString);
        }
       
      
        networkManager.DoPost1(API.PostCarsInfo, form, (responseCode, content) =>
        {
            Debug.Log("____responseCode:" + responseCode + ", content:" + content);
           
            if (responseCode=="200")
            {
                tip.instance.SetMessae("保存成功");
                for (int i = 0; i < tempInfoList.Count; i++)
                {
                    if (!putSJ.Contains(tempInfoList[i].carType))
                    {
                        putSJ.Add(tempInfoList[i].carType); 
                    }
                }
                
                ChangeToPage(1);
                UpdateUI();
                putedSJ = tempInfoList;
            }
            else
            {
                tip.instance.SetMessae("保存失败："+responseCode);
            }
        }, networkManager.token);
        
        
        
        /*
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

            cost postCost=new cost();
            postCost.carNumber = tempInfoList[0].carNumber;
            postCost.net_price= inputCarPrice.text;
            postCost.registration_price=inputRegistrationPrice.text;
            postCost.insurance_price=inputInsurance.text;
            postCost.purchase_tax=inputTax.text;
            postCost.financial_agents_price=inputFinancial.text;
            postCost.other_price= inputOtherPrice.text;
            postCost.offer_price = currPriceInfo.guidancePrice;
            postCost.cart_price_type = "1";
            postCost.vin = "";
            postCost.boutique= inputJingPin.text;
            postCost.content_remark=inputDecorationContent.text;
            postCost.registration_area_type=currRegisterAreaType.ToString();

         //   Debug.Log("________准备上传的 jsonData:" + jsonData.ToJson());
            //jsonString = JsonMapper.ToJson(jsonData);
            string jsonString = JsonMapper.ToJson(postCost);

            form.AddField("d[]", jsonString);
        }
        networkManager.DoPost1(API._PostOfferPrice1, form, (responseCode, content) =>
        {
            Debug.Log("____responseCode:" + responseCode + ", content:" + content);
        }, networkManager.token);
        */

        
    }

    private bool showAll=true;
    public List<PriceInfo> StoreAddCar=new List<PriceInfo>();
    List<GameObject> specialItems=new List<GameObject>();
    public void ShowSpecialCar()
    {
       
        if (showAll)
        {
            if (SpecialCarr.instance!=null)
            {
                for (int i = 0; i < priceItems.Count; i++)
                {
                    priceItems[i].SetActive(false);
                }

                
                for (int i = 0; i < SpecialCarr.instance.TJSJ.Count; i++)
                {
                    if ( SpecialCarr.instance.TJSJ.Count==0)
                    {
                         tip.instance.SetMessae("暂无特价车信息");
                    }
                    else
                    {
                        GameObject go;
                        go = Instantiate(priceManagerItem, itemContainer);
                        specialItems.Add(go);
                        PriceManagerItem  item = go.GetComponent<PriceManagerItem>();
                        item.SetItemContent(i.ToString(), priceInfos[i].carNumber, priceInfos[i].guidancePrice,
                            priceInfos[i].vehicleSystem, priceInfos[i].carType, "已上架"); //订单管理显示的
                            
                           
                        
                    }
                   
                }
            }
            else
            {
                tip.instance.SetMessae("暂无特价车信息");
            }
        }
        else
        {
            for (int i = 0; i < specialItems.Count; i++)
            {
                specialItems[i].SetActive(false);
            }

            specialItems.Clear();
            for (int i = 0; i < priceItems.Count; i++)
            {
                priceItems[i].SetActive(true);
            }
        }

        showAll = !showAll;

    }

    public void DoPostOfferPrice1()//普通报价
    {
       string carType = currPriceInfo.carType;//竟然不需要车型信息？？？
       List<cost> costList=new List<cost>();
       List<PriceInfo> needPost=new List<PriceInfo>();
       CleanBeforeUpdataUi();
       for (int i = 0; i < priceInfos.Count; i++)
       {
           if (priceInfos[i].carType==carType)
           {
               needPost.Add(priceInfos[i]);
           }
       }
       for (int i = 0; i < needPost.Count; i++)
       {
           cost postCost=new cost();
           postCost.cart_id = "NNN";
           postCost.carNumber = needPost[i].carNumber;
           postCost.net_price= inputCarPrice.text;
           postCost.registration_price=inputRegistrationPrice.text;
           postCost.insurance_price=inputInsurance.text;
           postCost.purchase_tax=inputTax.text;
           postCost.financial_agents_price=inputFinancial.text;
           postCost.other_price= inputOtherPrice.text;
           postCost.offer_price = needPost[i].guidancePrice;
           postCost.cart_price_type = "1";
           postCost.vin = "NA";
           postCost.boutique= inputJingPin.text;
           postCost.content_remark=inputDecorationContent.text;
           postCost.registration_area_type=currRegisterAreaType.ToString();
           costList.Add(postCost);
       }
       StartCoroutine(coroutine.instance.PostTypePrice(costList));
       

    }

  

    /// <summary>
/// 组装报价车型对应的数据
/// </summary>
/// <param name="curInfo"></param>当前车辆的数据
/// <returns></returns>
    /*private cost PackCartypeInfo(PriceInfo curInfo)
    {
        cost cost=new cost();

        cost.net_price = inputCarPrice.text;
        cost.financial_agents_price = inputFinancial.text;
        cost.insurance_price = inputInsurance.text;
        cost.registration_price = inputRegistrationPrice.text;
        cost.purchase_tax = inputTax.text;
        cost.other_price = inputOtherPrice.text;
        cost.cart_price_type = "";
        // cost.registration_type = dropRegistrationType.optionsdropRegistrationType.value.text;
        // cost.insurance_type = dropInsuranceType.optionsdropInsuranceType.value.text;
        cost.registration_type ="" ;
        cost.insurance_type = "";
        cost.content_remark = inputDecorationContent.text;
        cost.appear_color = curInfo.color;
        
        return cost;
    }*/

    private int index;

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
        index = int.Parse(currPriceManagerItem.text_index.text) - 1;

        priceManagerItems[index].UpdateItem(currPriceManagerItem.offerPriceData.officialPrice, "已上架");
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
        
        Debug.Log(  "dic count     "+vehicleSystemsDic.Count);
        
        string jsonData = JsonMapper.ToJson(bcti);
        form.AddField("d[]", jsonData);
        networkManager.DoPost1(url, form, (responseCode, data) =>
        {
            //Debug.Log("responseCode:" + responseCode + "|" + data);

        }, networkManager.token);
    }
    
    public void CloseSetPrice()
    {
        ChangeToPage(1);
       // GameObject.Find("AddSpecialCar").SetActive(false);
    }
    public void ClearAllData()
    {
        Debug.Log("清除所有数据");
        priceManagerItems.Clear();//清除所有的items
        carNumberList.Clear();
        carTypeList.Clear();
        vehicSys.Clear();
        vehicleSystemsDic.Clear();
        putedSJ.Clear();
        putSJ.Clear();
        
        priceInfos.Clear();
      //  coroutine.instance.dicItem.Clear();
        priceInfosLast.Clear();
        
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


public class cost//报价
{
    public string brand;
    public string vehicleSystrm;
    public string carType;
    public string discharge;
    public string guidancePrice;
    public string cart_id;//多余接口
    public string carNumber;
    public string appear_color;
    public string interoi_color;
    public string garageAge;
    public string note;
    public string qualityloss;
    public string memo;
    public string registration_area_type;
    public string province_id;
    public string city_id;
    public string net_price;
    public string registration_price;
    public string insurance_price;
    public string purchase_tax;
    public string financial_agents_price;
    public string other_price;
    public string vin;//特价车车架号
    public string cart_price_type;
    public string offer_price;
    public string boutique;
    public string content_remark;

    
}

public class carNumbs
{
    public string car_numbers;
}

public class CarTypeInfo
{
    public int id;
    public int merchant_id;
    public int brand_id;
    public string vehicleSystem;
    public string carType;
    public string discharge;
    public string guidancePrice;
    public string carNumber;    //车架号
    public string appear_color;
    public string interoi_color	;
    public string garageAge;
    public string note;
    public string qualityloss;
    public string memo;
    public string registration_area_type;
    public string province_id;
    public string city_id;
    public string status;
    public string create_time;           //报价
    public string update_time;          //是否上架
    public int deposit;
    
}

public class ChangeCarTypeVehic
{
    public string brand_name;
    public string line_name;
    public string model_name;
    public string cart_lines;
    public string cart_models;
}

