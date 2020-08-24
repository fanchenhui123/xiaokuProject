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

using UnityEngine.Networking;

public class PriceManager : MonoBehaviour
{
    public static PriceManager Instance;

    public Transform page1;
    public Transform page2;

    public GameObject priceManagerItem;
    public Transform itemContainer;

    public List<PriceInfo> priceInfos;//读表得到的所有数据
    public List<PriceInfo> priceInfosLast = new List<PriceInfo>();//为了方便新旧表对比，存入上一次读表数据
    public List<string> carNumberList = new List<string>();
    public List<string> carNumberListLast = new List<string>();
    public List<string> carTypeList = new List<string>();
    public List<string> carTypeListLast = new List<string>();
    public string curBrand;
    public List<string> putSJ=new List<string>();//存入已上架的车型，普通报价后存入
 //   public List<string> putSJCB=new List<string>();
    public Button specialCarbtn;
    public int curUserBrandId=1;
    public string curUserBrand = "";
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

    private object   objlock = new object();

    private int filsCount;
    //private string filePath = Application.streamingAssetsPath + "/深圳锦奥库存CKD_200614.xlsx"; 
    public string filePath;
    private Thread loadThread;
    public bool loadEnd = false;

    public GameObject mainManager;

    public NetworkManager networkManager;

    public List<PriceManagerItem> priceManagerItems = new List<PriceManagerItem>();     //全局（主页与价格管理）共享报价列表数据
  //  public List<PriceInfo> CanLoadedCars=new List<PriceInfo>();
   // public static event Action LoadExcelEndEvent;

    private DBManager dBManager;

    public Dictionary<string, List<string>> vehicleSystemsDic = new Dictionary<string, List<string>>();    //保存车系与车型之间关系，用于Post上传到后台
    public List<PriceInfo> priceInfosAdd=new List<PriceInfo>();//新增的车且已经报价
    
    public List<string> priceInfosRemove=new List<string>();//删除的车但已经报价
    public bool isNeedCompare;//是否需要进行数据对比

   // private string savepath;
    private int currRegisterAreaType = 0;

    private void Awake()
    {
        Instance = this;
        if (priceInfos==null)
        {
            priceInfos   = new List<PriceInfo>(); 
        }
        
        // savepath = Application.persistentDataPath + "save.xlsx";
    }

   

    private void OnEnable()
    {
       // ChangeToPage(1);
        UpdateUI();
       // DoPostCarType();

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

      

        this.gameObject.SetActive(false);
    }

    private void Init()
    {
       // curUserBrandId =int.Parse(PlayerPrefs.GetString("brand_id")) ;
       // curUserBrand = PlayerPrefs.GetString("Userbrand");
        networkManager = NetworkManager.Instance;
        dBManager = DBManager._DBInstance();
        dBManager.CreateTable(typeof(PriceInfo));
        priceInfos= LoadPlayerJson();
        LoadPlayerJsonHadPrice();
        //  SavePlayerJson(priceInfos);
      //  priceInfos = LoadPlayerJson();
       
      // Invoke(() => { tip.instance.SetMessae("读取数据库" + priceInfos.Count);},10f);
      
       // priceInfosLast = dBManager.QueryTable<PriceInfo>();

        toggleCity.onValueChanged.AddListener((value)=> {
            currRegisterAreaType = 3;
        });

        toggleProvince.onValueChanged.AddListener((value) => {
            currRegisterAreaType = 2;
        });

        toggleCountry.onValueChanged.AddListener((value) => {
            currRegisterAreaType = 1;
        });

        if (priceInfos!=null)
        {
            if (priceInfos.Count > 0)
            {
               // Debug.Log("????updateui");
                UpdateUI();
                loadEnd = true;
                isNeedCompare = false;
            }
            else
            {
                loadExcelsTest(PlayerPrefs.GetString("XiaoKuExcelPath"));
            }
        }
        else
        {
            loadExcelsTest(PlayerPrefs.GetString("XiaoKuExcelPath"));
        }
       

        btnSave.onClick.AddListener(() =>
        {
           tip.instance.SetMessae("DoPost");
            DoPostOfferPrice();//

           // SaveInputInfo();
        });

        btnAddJingPin.onClick.AddListener(OnClickAddJingPin);
    }

    private bool comparaComplete;//判断是否对比完
    private void Update()
    {
        if (loadEnd)
        {
            loadEnd = false;
            if (isNeedCompare)
            {
                priceInfosLast = LoadPlayerJson();
                tip.instance.SetMessae("开始对比数据");
                for (int i = 0; i < priceInfos.Count; i++)
                {
                    
                    int same=0;
                    for (int j = 0; j < priceInfosLast.Count; j++)
                    {
                        if (priceInfos[i].carNumber==priceInfosLast[j].carNumber)
                        {
                            break;
                        }
                        else
                        {
                            same++;
                        }
                    }

                    if (same==priceInfosLast.Count)
                    {
                       // Debug.Log("有新增车辆"+priceInfos[i].brand+ priceInfos[i].vehicleSystem);
                        if (putSJ.Contains(priceInfos[i].carType))
                        {
                            priceInfos[i].vehicleSystem= priceInfos[i].vehicleSystem.Replace("库存", "");
                            priceInfosAdd.Add(priceInfos[i]);//新读表的第I个车架号跟旧表的所有的车架号都不一样，就说明是增加的车，再判断是否已经上架
                        }
                    }
                  
                }

                if (priceInfosAdd.Count>0)
                {
                    StartCoroutine(postNewCarPrice(priceInfosAdd));
                }
               
                for (int i = 0; i < priceInfosLast.Count; i++)
                {
                    if (!carNumberList.Contains(priceInfosLast[i].carNumber))
                    {
                      //  Debug.Log("需要删除的车辆11  "+priceInfosLast[i].carNumber);
                        if (putSJ.Contains(priceInfosLast[i].carType))
                        {

                            //Debug.Log("需要删除的车辆已经报价  "+priceInfosLast[i].carNumber);
                            priceInfosRemove.Add(priceInfosLast[i].carNumber);
                        }
                        
                    }
                }
              //  Debug.Log(priceInfosRemove.Count +"  "+priceInfosAdd.Count);
                if (priceInfosRemove.Count > 0)
                {
                    StartCoroutine( coroutine.instance.PostNeedRemoveCar(priceInfosRemove));
                }


                
                comparaComplete = true;
            }
            else
            {
               // Debug.Log("无需对比");
                comparaComplete = true;
            }

        }

        if (comparaComplete)
        {
          //  Debug.Log("      comparaComlete");
            comparaComplete = false;
            UpdateUI();
            DoPostCarType();
            SavePlayerJson(priceInfos);
        }
        
    }
    
   
    public void loadExcelsTest(string ExcelPath)
    {
        
        Debug.Log("开始分析路径");
        string  SourceExcelPath = @ExcelPath.Replace(@"\",@"/");         // @"E:\C# Projects\ConsoleApplication1\";
        DirectoryInfo mydir = new DirectoryInfo(SourceExcelPath);
        if (SourceExcelPath.EndsWith(".xlsx"))
        {
            if (File.Exists(Application.persistentDataPath + "/save.xlsx"))
            {
                File.SetAttributes(Application.persistentDataPath + "/save.xlsx", FileAttributes.Normal);
                File.Delete(Application.persistentDataPath + "/save.xlsx");
            }
      
            FileInfo newfile=new FileInfo(SourceExcelPath);
            newfile.CopyTo(Application.persistentDataPath + "/save.xlsx");
            SourceExcelPath = Application.persistentDataPath + "/save.xlsx";
           // Debug.Log("source   "+SourceExcelPath);
            DoLoadThread(SourceExcelPath);//读文件
            filsCount = 1;//多选的文件数
        }
        else
        {
            if (mydir.GetFiles().Length <= 0)
            {
                tip.instance.SetMessae("路径下没有文件");
                return;
            }
            else
            {
               // Debug.Log("文件夹里有文件");
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
                filsCount = list.Count;//多选的文件数
            }

           
          
            
            foreach(string str in list)
            { 
                if (File.Exists(Application.persistentDataPath +@"/"+ str))
                {
                    File.SetAttributes(Application.persistentDataPath  +@"/"+ str, FileAttributes.Normal);
                    File.Delete(Application.persistentDataPath +@"/" + str);
                }
      
                FileInfo newfile=new FileInfo(ExcelPath +@"/"+str);
               // Debug.Log(Application.persistentDataPath +@"/" + str);
                newfile.CopyTo(Application.persistentDataPath +@"/" + str);
                SourceExcelPath = Application.persistentDataPath +@"/" + str;
                Instance.DoLoadThread(SourceExcelPath);//读文件夹里的文件
            }
        }

        coroutine.instance.time = Time.time;



    }
    
    
    public void DoLoadThread(string path = "")
    {
       
          Debug.Log("开启线程");
        loadThread = new Thread(new ThreadStart(() =>
        {
            try
            {
                lock (objlock)
                {
                    
                    if (path != "")
                    {
                        ReadCarPrice(path);
                    }
                    else
                        ReadCarPrice();


                }
            }
            catch (Exception e)
            {
                  Debug.Log("打开线程失败 : " + e.ToString());
              //  tip.instance.SetMessae("打开线程失败");
              //  Debug.LogError("打开线程失败 : " + e.ToString());
            }
        }));
        loadThread.IsBackground = true;
        loadThread.Priority = System.Threading.ThreadPriority.Highest;
        loadThread.Start();
    }
   
    int Hadloadfiles;
    public void ReadCarPrice(string path = "")
    {
         Debug.Log("开始读表");
        if (priceInfos==null)
        {
            priceInfos   = new List<PriceInfo>(); 
        }
        

        if (Hadloadfiles == 0 && isNeedCompare)
        {
            if (priceInfosLast==null)
            {
             priceInfosLast=new List<PriceInfo>();   
            }
            priceInfosLast.Clear();
            priceInfos.Clear();
            carNumberList.Clear();
            carTypeList.Clear();
        }

        if (path != "")
        {
            filePath = path;

        }

        if (!File.Exists(filePath))
            return;
        Debug.Log("开始读表0.2");
       FileInfo newFile = new FileInfo(filePath);
       newFile.IsReadOnly = true;
       using (ExcelPackage package = new ExcelPackage(newFile))
       {
           var worksheets = package.Workbook.Worksheets;
          // Debug.Log("worksheet:" + worksheets.Count);
           int wIndex = 1;
           foreach (var w in worksheets)
           {
               if (!w.Name.Contains("库存")) continue; //只读工作表名中带  库存  俩字的
              // Debug.Log(w + " " + w.Index);
               int minColumnNum = w.Dimension.Start.Column; //工作区开始列
               int maxColumnNum = w.Dimension.End.Column; //工作区结束列
               int minRowNum = w.Dimension.Start.Row; //工作区开始行号
               int maxRowNum = w.Dimension.End.Row; //工作区结束行号

               int[] tableTitle = new int[18];
               string tmpKey = "";

               Debug.Log(minColumnNum + "|" + maxColumnNum + "|" + minRowNum + "|" + maxRowNum);

               string tmpCarType = "";
               string prevCarType = ""; //用于解决上次值为空的问题    
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
                                           item.discharge =
                                               cell.RichText.Text; //排放                                            
                                           break;
                                       case 1:
                                           //item.carType = a.RichText.Text;//车型
                                           tmpCarType = cell.RichText.Text; //车型
                                           break;
                                       case 2:
                                           item.guidancePrice = cell.RichText.Text; //指导价
                                           break;
                                       case 3:
                                           // item.carNumber = cell.RichText.Text;//车架号
                                           tmpKey = cell.RichText.Text; //车架号作为关键字
                                           //to do 改成id自增
                                           break;
                                       case 4:
                                       {
                                           string dateStr = cell.RichText.Text;
                                           Match mInfo = Regex.Match(dateStr, @"(?i)^[0-9]+$");
                                           if (dateStr.Trim() != string.Empty && dateStr.Length == 5 && mInfo.Success)
                                           {
                                               dateStr = DateTime.FromOADate(double.Parse(dateStr))
                                                   .ToString("yyyy/MM/dd");
                                           }

                                           item.releaseDate = dateStr; //发车日期
                                       }
                                           break;
                                       case 5:
                                       {
                                           string dateStr = cell.RichText.Text;
                                           Match mInfo = Regex.Match(dateStr, @"(?i)^[0-9]+$");
                                           if (dateStr.Trim() != string.Empty && dateStr.Length == 5 && mInfo.Success)
                                           {
                                               dateStr = DateTime.FromOADate(double.Parse(dateStr))
                                                   .ToString("yyyy/MM/dd");
                                           }

                                           item.arriveDate = dateStr; //到店日期
                                       }
                                           break;
                                       case 6:
                                           item.garageAge = cell.RichText.Text; //库龄
                                           break;
                                       case 7:
                                       {
                                           string dateStr = cell.RichText.Text;
                                           Match mInfo = Regex.Match(dateStr, @"(?i)^[0-9]+$");
                                           if (dateStr.Trim() != string.Empty && dateStr.Length == 5 && mInfo.Success)
                                           {
                                               dateStr = DateTime.FromOADate(double.Parse(dateStr))
                                                   .ToString("yyyy/MM/dd");
                                           }

                                           item.akkDate = dateStr; //AAK日期
                                       }
                                           break;
                                       case 8:
                                           item.qualityloss = cell.RichText.Text; //质损
                                           break;
                                       case 9:
                                           item.certificate = cell.RichText.Text; //合格证
                                           break;
                                       case 10:
                                           item.userName = cell.RichText.Text; //客户姓名
                                           break;
                                       case 11:
                                           item.adviser = cell.RichText.Text; //销售顾问
                                           break;
                                       case 12:
                                           item.carGroup = cell.RichText.Text; //组别
                                           break;
                                       case 13:
                                       {
                                           string dateStr = cell.RichText.Text;
                                           Match mInfo = Regex.Match(dateStr, @"(?i)^[0-9]+$");
                                           if (dateStr.Trim() != string.Empty && dateStr.Length == 5 && mInfo.Success)
                                           {
                                               dateStr = DateTime.FromOADate(double.Parse(dateStr))
                                                   .ToString("yyyy/MM/dd");
                                           }

                                           item.signDate = dateStr; //签订日期
                                       }
                                           break;
                                       case 14:
                                           item.useTime = cell.RichText.Text; //配车天数
                                           break;
                                       case 15:
                                           item.payType = cell.RichText.Text; //付款方式
                                           break;
                                       case 16:
                                           item.memo = cell.RichText.Text; //备注
                                           break;
                                       case 17:
                                           item.color = cell.RichText.Text; //外/内 颜色
                                           break;
                                   }
                               }
                           }

                           if (cell.Comment != null && cell.Comment.RichText.Text.Trim() != string.Empty)
                           {
                               note += "\n" + cell.Comment.RichText.Text; //获取备注
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

                           item.note = note; //批注
                           item.vehicleSystem = w.Name; //车系
                           item.brand = curUserBrand; // PlayerPrefs.GetString("brand_id");            
                         //  Debug.Log("读表后添加 brand="+curUserBrand);
                           if (!carNumberList.Contains(item.carNumber))
                           {
                               if (string.IsNullOrEmpty(item.adviser) && string.IsNullOrEmpty(item.userName))
                               {
                                   carNumberList.Add(item.carNumber);
                                   priceInfos.Add(item);
                               }

                           }

                           prevCarType = tmpCarType;
                       }
                   }

                   Thread.CurrentThread.Join(1);
               }

               wIndex = wIndex + 1;
               //Debug.Log("__________current product infos count: " + priceInfos.Count);
           }
          // Debug.Log("infos.count    " + priceInfos.Count);
           Thread.CurrentThread.Join(1000); //阻止设定时间
       }
     
       File.SetAttributes(filePath,  FileAttributes.Normal);
      
       string curCarType = "";
       for (int i = 0; i < priceInfos.Count; i++)
       {
           if (!string.IsNullOrEmpty(priceInfos[i].carType) )
           {
               curCarType = priceInfos[i].carType;
           }
           else
           {
               priceInfos[i].carType = curCarType;
           }
       }



       Hadloadfiles++;
       if (Hadloadfiles==filsCount)
       {
         //  Debug.Log("load end");
            loadEnd = true;
            Hadloadfiles = 0;
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
      //  Debug.Log("刷新价格管理UI"+priceInfos.Count);
        carTypeList.Clear();
        GameObject go;
        PriceManagerItem item;
        int count = 1;
        if (priceItems.Count!=0)
        {
            for (int i = 0; i < priceItems.Count; i++)
            {
                Destroy(priceItems[i]);
            }
            priceItems.Clear();
        }
        
        
        for (int i = 0; i < priceInfos.Count; i++)//生成价格管理页面的的表格
        {
            if (priceInfos[i].carType != "" && !carTypeList.Contains(priceInfos[i].carType))
            {
                go = Instantiate(priceManagerItem, itemContainer);
                priceItems.Add(go);
                item = go.GetComponent<PriceManagerItem>();
                if (putSJ.Contains(priceInfos[i].carType))
                {
                    item.SetItemContent(count.ToString(), priceInfos[i].carNumber, priceInfos[i].guidancePrice,
                        priceInfos[i].vehicleSystem, priceInfos[i].carType, "已报架"); //订单管理显示的
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

        }
      
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
               
           }
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
            // Debug.Log(isNeedShowPriceInfo); 
      
             
            /*
            if (isNeedShowPriceInfo)
            {
                JsonData jsonData = JsonMapper.ToObject(hadPriceInfo)["data"].ToJson();
                Debug.Log("???");
                inputCarPrice.text=jsonData["net_price"].ToString();
                 inputFinancial.text=jsonData["financial_agents_price"].ToString();
                 inputInsurance.text=jsonData["insurance_price"].ToString();
                 inputRegistrationPrice.text=jsonData["registration_price"].ToString();
                 inputTax.text=jsonData["purchase_tax"].ToString();
                 inputOtherPrice.text=jsonData["other_price"].ToString();
                 inputDecorationContent.text=jsonData["content_remark"].ToString();
                
               //  currRegisterAreaType.ToString()=jsonData["registration_area_type"].ToString();
            }*/
        }

       
        //TODO: 页面切换后的(数据传递) 页面重置刷新
    }


    public void DoGetCarList(string page = "1", string pageSize = "10", string brand_id = "1", string vehicleSystem = "Q3", string carType = "车型")
    {
      //  Debug.Log("token: " + networkManager.token);
        //string url = API.GetMsgList + "&page=" + page + "&pageSize=" + pageSize + "&brand_id=" + brand_id
        //    + "&vehicleSystem=" + vehicleSystem + "&carType=" + carType;
        string url = API._GetCarList;
        networkManager.DoGet1(url, (responseCode, data) =>
        {
          //  Debug.Log("responseCode:" + responseCode + "|" + data);
        }, networkManager.token);

    }


    /// <summary>
    /// 点击保存后，发起post传输currPriceInfo数据给后台,把所有同车型的数据信息都上传.特价车POST方法
    /// </summary>
    private List<Dictionary<string,cost>> dics=new List<Dictionary<string, cost>>();
    private List<PriceInfo> putedSJ=new List<PriceInfo>();//已经报价的
  //  private List<string> HavaPriceInfoCarType=new List<string>();
  public string hadPriceInfo="";
  public bool isNeedShowPriceInfo;
  
    public void DoPostOfferPrice()//普通报价
    {
       
        WWWForm form = new WWWForm();
        List<PriceInfo> tempInfoList = new List<PriceInfo>();
        string carType = currPriceInfo.carType;
       // Debug.Log("curCarType  " + carType);
        for (int i = 0; i < priceInfos.Count; i++)
        {
            if (priceInfos[i].carType == carType)
            {
             //   Debug.Log("有");
                JsonData jsonData = JsonMapper.ToObject(JsonMapper.ToJson(priceInfos[i]));
                for (int j = 0; j < jsonData.Count; j++)
                {
                    if (jsonData[j]==null)
                    {
                        jsonData[j] = "NA";
                    }
                }
                tempInfoList.Add(priceInfos[i]);
              //  Debug.Log(i+"   "+priceInfos[i].carNumber);
            } 
           
        }
        
        tip.instance.SetMessae("开始整合表单数据");
       
        for (int i = 0; i < tempInfoList.Count; i++)
        {
            tempInfoList[i].vehicleSystem = tempInfoList[i].vehicleSystem.Replace("库存", "");
            tempInfoList[i].brand=curUserBrand;
          //  Debug.Log(tempInfoList[i].vehicleSystem+"   brand  "+tempInfoList[i].brand);
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

            for (int j = 0; j < jsonData.Count; j++)
            {
                if (jsonData[j]==null)
                {
                    jsonData[j] = "NA";
                }
            }
            string json = jsonData.ToJson();

            form.AddField("d[]", json);
            tip.instance.SetMessae(tempInfoList.Count+"   "+ i.ToString());
            Debug.Log(jsonData["carType"]+"  cartype");
        }
        
        networkManager.DoPost1(API.PostCarsInfo, form, (responseCode, content) =>
        {
            
            if (responseCode=="200")
            {
                tip.instance.SetMessae("保存成功");

                for (int i = 0; i < tempInfoList.Count; i++)
                {
                    Debug.Log(tempInfoList[i].carType);
                    if (!putSJ.Contains(tempInfoList[i].carType))
                    {
                        putSJ.Add(tempInfoList[i].carType);
                    }

                    /*if (!putSJCB.Contains(tempInfoList[i].carNumber))
                    {
                        putSJCB.Add(tempInfoList[i].carNumber);
                    }*/
                }
                SavePlayerJson(putSJ);
                ChangeToPage(1);
                UpdateUI();
            }
            else
            {
                Debug.Log(JsonMapper.ToObject(content)["message"].ToString());
                tip.instance.SetMessae(JsonMapper.ToObject(content)["message"].ToString());
            }
        }, networkManager.token);
        
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


                for (int i = 0; i < specialItems.Count; i++)
                {
                 Destroy(specialItems[i]);
                }

                specialItems.Clear();

                
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
                        item.SetItemContent(i+1.ToString(), priceInfos[i].carNumber, priceInfos[i].guidancePrice,
                            priceInfos[i].vehicleSystem, priceInfos[i].carType, "已报价"); //订单管理显示的
                        
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
        currPriceManagerItem.offerPriceData.id = currPriceInfo.id;       //车辆序号, 对(车系)价格管理 基本无用
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

    public  Dictionary<string,cost> dicItem=new Dictionary<string, cost>();//记录已经上传报价信息的车辆
    
   

  
    
    
    
    public IEnumerator postNewCarPrice(List<PriceInfo> postList)//
    {
      //  Debug.Log("删除数据");
        WWWForm form = new WWWForm();
        for (int i = 0; i < postList.Count; i++)
        {
            string jsonstring = JsonMapper.ToJson(postList[i]);
            form.AddField("d[]",jsonstring);
        }

        networkManager.DoPost1(API.AddPostPrice, form, (responseCode, content) =>
            {
                if (responseCode=="200")
                {
                    tip.instance.SetMessae("数据已更新");
                    for (int i = 0; i < postList.Count; i++)
                    {
                      //  Debug.Log("新增的车车架号：   "+postList[i].carNumber.ToString()); 
                    }
                   
                    newCarPrice.Clear();
                }
                else
                {
                  //  Debug.Log("新增content"+content);
                   // Debug.Log("新增   "+JsonMapper.ToObject(content)["message"].ToString());
                }
            },
          
            networkManager.token);
        yield break;
    }
   
    /// <summary>
    /// Post上传车系车型
    /// </summary>
    public void DoPostCarType()//上传读表读到的车系车型
    {
        if (string.IsNullOrEmpty(curUserBrand))
        {
            return;
        }
    
        string url = API._PostCarType;
        WWWForm form = new WWWForm();

      
        BrandCarTypeInfo bcti = new BrandCarTypeInfo();
        bcti.brand_name = curUserBrand; //todo  PlayerPrefs.GetString("brand_id");
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
        
    
      //  Debug.Log(curUserBrand+"  当前品牌  "+bcti.brand_name );
        string jsonData = JsonMapper.ToJson(bcti);
        form.AddField("d[]", jsonData);
        networkManager.DoPost1(url, form, (responseCode, data) =>
        {
            if (responseCode=="200")
            {
            
               // Debug.Log("responseCode:" + responseCode + "|" + data);
            }
            else
            {
              //  Debug.Log("没添加成功"+ JsonMapper.ToObject(data).ToString());
                tip.instance.SetMessae(JsonMapper.ToObject(data).ToJson());
            }

        }, networkManager.token);
    }
    
    public void CloseSetPrice()
    {
        ChangeToPage(1);
       // GameObject.Find("AddSpecialCar").SetActive(false);
    }
    public void ClearAllData()
    {
        
        
      //  Debug.Log("清除所有数据");
        priceManagerItems.Clear();//清除所有的items
        carNumberList.Clear();
        carTypeList.Clear();
        vehicSys.Clear();
        vehicleSystemsDic.Clear();
        putSJ.Clear();
        if (priceInfos!=null)
        {
            priceInfos.Clear(); 
        }
     
        priceInfosLast.Clear();

        if (SpecialCarr.instance!=null)
        {
            SpecialCarr.instance.TJSJ.Clear();
        }
        for (int i = 0; i < itemContainer.childCount; i++)
        {
            Destroy(itemContainer.GetChild(i).gameObject);
        }
    }

    private void OnDestroy()
    {
        if (loadThread!=null)
        {
            loadThread.Abort();
        }
      
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

    /// <summary>
    /// PriceItem回调，设置Page2的UI
    /// </summary>
    /// <param name="item"></param>
    public void SetItemForPage2(PriceManagerItem item)
    {
       //ug.Log("????没设置？？"+item.priceInfo.brand);
        currPriceInfo = item.priceInfo;

        currPriceManagerItem = item;

        var offerPriceMsg = item.offerPriceData;     //for post

        textCarType.text = item.priceInfo.carType;

        textCarSeries.text = item.priceInfo.vehicleSystem;

        /*switch (item.offerPriceData.registration_area_type)
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
        }*/

     
        if (item.text_status.text == "已报架")
        {
           // Debug.Log("已经报价");
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
          //  toggleGroupRegArea.NotifyToggleOn(toggle);
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
            
           // toggleGroupRegArea.NotifyToggleOn(toggleCity);
            inputDecorationContent.text = "";
            inputJingPin.text = "";

            dropJingPingList.ClearOptions();
        }

    }
    //保存数据
    public  void SavePlayerJson(List<PriceInfo> player)//保存车辆信息
    {
       // Debug.Log("保存数据，长度"+player.Count);
        string path = Application.persistentDataPath+"/priceinfos.json";
        if (!File.Exists(path))
        {
            File.Create(path).Dispose();
        }
        SaveFile saveFile=new SaveFile();
        saveFile.data = player;
        saveFile.brandid = curUserBrandId.ToString();// PlayerPrefs.GetString("brand_id");
        saveFile.userbrand = curUserBrand;//PlayerPrefs.GetString("Userbrand");
        var content = JsonMapper.ToJson(saveFile);
        File.WriteAllText(path,content);
    }

    public  void SavePlayerJson(List<string> player)//保存议价信息
    {
      //  Debug.Log("保存数据，长度"+player.Count);
        string path = Application.persistentDataPath+"/hadPrice.json";
        if (!File.Exists(path))
        {
            File.Create(path).Dispose();
        }
        SaveFileHadPrice saveFileHadPrice=new SaveFileHadPrice();
        saveFileHadPrice.data = player;
        var content = JsonMapper.ToJson(saveFileHadPrice);
        File.WriteAllText(path,content);
    }
    public  void LoadPlayerJsonHadPrice()
    {
        string path = Application.persistentDataPath+"/hadPrice.json";
        if(File.Exists(path)){
            var content = File.ReadAllText(path);
           // Debug.Log(" content "+content);
            if (content.Length==0)
            {
                return;
            }
            var playerData =JsonMapper.ToObject<SaveFileHadPrice>(content) ;//JsonUtility.FromJson<SaveFile>(content);
          //  Debug.Log( JsonMapper.ToJson(playerData));
            if (playerData.data!=null  )
            {
                if (playerData.data.Count!=0)
                {
                   // Debug.Log(playerData.data.Count .ToString());
                    putSJ= playerData.data;
                    
                }
                else
                {
                    // loadExcelsTest(PlayerPrefs.GetString("XiaoKuExcelPath"));
                    return ;
                }
                
            }
            else
            {
                //  loadExcelsTest(PlayerPrefs.GetString("XiaoKuExcelPath"));
                return ;
            }
           
        }else{
            //Debug.LogError("Save file not found in  "+path);
            return ;
        }
    }
    
    //读取数据
    public  List<PriceInfo> LoadPlayerJson()
    {
        string path = Application.persistentDataPath+"/priceinfos.json";
        if(File.Exists(path)){
            var content = File.ReadAllText(path);
           // Debug.Log(" content "+content);
            if (content.Length==0)
            {
                return null;
            }
            var playerData =JsonMapper.ToObject<SaveFile>(content) ;//JsonUtility.FromJson<SaveFile>(content);
          //  Debug.Log( JsonMapper.ToJson(playerData));
            if (playerData.data!=null  )
            {
                if (playerData.data.Count!=0)
                {
                   // Debug.Log(playerData.data.Count .ToString());
                    return playerData.data;
                }
                else
                {
                   // loadExcelsTest(PlayerPrefs.GetString("XiaoKuExcelPath"));
                    return null ;
                }
                
            }
            else
            {
              //  loadExcelsTest(PlayerPrefs.GetString("XiaoKuExcelPath"));
                return null ;
            }
           
        }else{
           // Debug.LogError("Save file not found in  "+path);
            return null ;
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

public class SaveFile
{
    public List<PriceInfo> data;
    public string brandid;
    public string userbrand;
}

public class SaveFileHadPrice
{
    public List<string> data;
}

public class sendStatus
{
    public string order_id;
}