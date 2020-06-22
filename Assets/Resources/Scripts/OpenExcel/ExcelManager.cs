using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public class ExcelManager : MonoBehaviour
{
    public MessagePanelManager messagePanelManager;
    public DelayPanelManager delayPanelManager;
    public string currentFilePath = "";

    public List<string> Vals;
    public int Row = 0;
    public int Col = 0;

    public ExcelRange cells;

    public TableView tb;//绑定的显示表

    private DBManager dBManager;

    Dictionary<string, string> _carTypeDic = new Dictionary<string, string>();//车架匹配车型

    //产品表
    List<ProductInfo> _productInfo = new List<ProductInfo>();

    bool doUpateUI = false;//在线程中刷新UI
    public bool loadEnd = false;
    int _id = 0;
    int _wIndex = 0;
    int _worksheetsCount = 0;
    EventWaitHandle _waitHandle = new AutoResetEvent(false);//阻塞线程直到发出set消息
    public bool doUpdateTable = false;//刷新表格
    private static object objlock = new object();//读取对象锁
    public AutoLoadExcelManager autoExcelManager;

    void Awake()
    {
        autoExcelManager = FindObjectOfType<AutoLoadExcelManager>();
        dBManager = DBManager._DBInstance();
    }

    public void OpenExcel()
    {
        OpenFileDlg pth = new OpenFileDlg();
        pth.structSize = System.Runtime.InteropServices.Marshal.SizeOf(pth);
        pth.filter = "Excel文件(*.xls *.xlsx)\0*.xls;*.xlsx;*.*;";
        pth.file = new string(new char[256]);
        pth.maxFile = pth.file.Length;
        pth.fileTitle = new string(new char[64]);
        pth.maxFileTitle = pth.fileTitle.Length;
        pth.initialDir = Application.dataPath;  // default path  
        pth.title = "打开excel";
        pth.defExt = "xls";
        pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        //0x00080000   是否使用新版文件选择窗口
        //0x00000200   是否可以多选文件
        if (OpenFileDialog.GetOpenFileName(pth))
        {
            //复制打开新文件
            string path = Path.GetFullPath(pth.file);
            string openDir = Application.persistentDataPath + "/excelTmp/";
            if (!Directory.Exists(openDir)) Directory.CreateDirectory(openDir);
            string openFile = "tmp.xls";
            string openPath = openDir + openFile;
            if (File.Exists(openPath)) File.Delete(openPath);
            File.Copy(path, openPath);
            
            IntPtr result = Util.ShellExecute(IntPtr.Zero, "open", openPath, "/t", null, Util.ShowWindowCommands.SW_SHOWNORMAL);
            if (result.ToInt32() <= 32)
            {
                Debug.Log("打开失败");
                StartCoroutine(DelayMessage("打开失败"));
            }
        }
        //Debug.Log(Application.persistentDataPath);
        //IntPtr result = Util.ShellExecute(IntPtr.Zero, "open", "xkExcel.xlt", "/t", null, Util.ShowWindowCommands.SW_SHOWNORMAL);       
        //if (result.ToInt32() <= 32)
        //{
        //       Debug.Log("打开失败");
        //}
    }

    //type = 0；打开excel || type=1；打开车型表格
    public void LoadExcel(int type = 0)
    {
        OpenFileDlg pth = new OpenFileDlg();
        pth.structSize = System.Runtime.InteropServices.Marshal.SizeOf(pth);
        pth.filter = "Excel文件(*.xls)\0*.xls;*.xlsx;*.*;";//"xls (*.xls)";
        pth.file = new string(new char[256]);
        pth.maxFile = pth.file.Length;
        pth.fileTitle = new string(new char[64]);
        pth.maxFileTitle = pth.fileTitle.Length;
        pth.initialDir = Application.dataPath;  // default path  
        if (type == 0)
        {
            pth.title = "导入excel数据";
        }
        else
        {
            pth.title = "导入车库数据";
        }
        pth.defExt = "xls";
        pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        //0x00080000   是否使用新版文件选择窗口
        //0x00000200   是否可以多选文件
        if (OpenFileDialog.GetOpenFileName(pth))
        {
            currentFilePath = pth.file;//选择的文件路径;  
            Debug.Log(currentFilePath);
            if (type == 0)
            {
                ReadExcel();
            }
            else
            {
                LoadCarType();
            }
            
        }
    }

    public void ReadExcel()
    {
        FileInfo newFile = new FileInfo(currentFilePath);
        using (ExcelPackage package = new ExcelPackage(newFile))
        {
            var worksheets = package.Workbook.Worksheets;
            Debug.Log("worksheet:" + worksheets.Count);
            foreach (var w in worksheets)
            {
                Debug.Log(w + " " + w.Index);
            }

            var ws = worksheets[1];

            int minColumnNum = ws.Dimension.Start.Column;//工作区开始列
            int maxColumnNum = ws.Dimension.End.Column; //工作区结束列
            int minRowNum = ws.Dimension.Start.Row; //工作区开始行号
            int maxRowNum = ws.Dimension.End.Row; //工作区结束行号


            var cells = ws.Cells;
            this.cells = cells;
            this.Row = maxRowNum; this.Col = maxColumnNum;

            Debug.Log(minColumnNum + " " + maxColumnNum + " " + minRowNum + " " + maxRowNum);
            Debug.Log(cells.Rows + " " + cells.Columns);
            Vals.Clear();
            foreach (var a in cells)
            {
                //Debug.Log(a.Rows + "|" + a.Columns + "|" + a.Text);
                if (a.Text != string.Empty) Vals.Add(a.Text);
            }
            //for(int i = 0; i < maxRowNum; i++)
            //{
            //    string tmpTxt = "";
            //    for(int j = 0; j < maxColumnNum; j++)
            //    {
            //        tmpTxt = cells[i + 1, j + 1].Text;
            //        Debug.Log(i + "|" + j + "|" + tmpTxt);
            //        if (tmpTxt.Trim() == string.Empty || tmpTxt == "") break;
            //        Vals[i, j] = cells[i + 1, j + 1].Text;
            //    }
            //    if (tmpTxt.Trim() == string.Empty || tmpTxt == "") break;
            //}

            //tb.ExceltoTable(cells, Row, Col);
            tb.ExceltoTableV(this.Vals, Row, Col);

        }
    }

    private IEnumerator DelayMessage(string info)
    {
        messagePanelManager.Show(info);
        yield return new WaitForSeconds(1f);
        messagePanelManager.Hide();
    }

    //载入车型excel数据
    public void LoadCarType()
    {
        delayPanelManager.Load();
        //StartCoroutine(DoLoadExcel());
        doUpdateTable = true;
        autoExcelManager.SetRun(false);
        DoLoadThread();
    }

    //后台定时读取加载excel
    public void BackwardLoadExcel(string filePath)
    {
        doUpdateTable = false;
        currentFilePath = filePath;
        DoLoadThread();
    }

    void DoLoadThread()
    {
        Thread thread = new Thread(new ThreadStart(() =>
        {
            try
            {
                lock (objlock)
                {
                    DoLoadExcel();
                }
            }
            catch
            {
                Debug.LogError("打开线程失败");
                loadEnd = true;
            }
            
        }));
        thread.Start();
    }

    //读取线程
    private void DoLoadExcel()
    {
        _productInfo.Clear();
        //dBManager.DeleteAll<ExcelInfo>();删除数据表，改成替换数据
        FileInfo newFile = new FileInfo(currentFilePath);
        using (ExcelPackage package = new ExcelPackage(newFile))
        {
            var worksheets = package.Workbook.Worksheets;
            Debug.Log("worksheet:" + worksheets.Count);
            int wIndex = 1;
            int id = 1;

            foreach (var w in worksheets)
            {
                Debug.Log(w + " " + w.Index);
                if (w.Name.Contains("出库")) continue;//只读库存
                int minColumnNum = w.Dimension.Start.Column;//工作区开始列
                int maxColumnNum = w.Dimension.End.Column; //工作区结束列
                int minRowNum = w.Dimension.Start.Row; //工作区开始行号
                int maxRowNum = w.Dimension.End.Row; //工作区结束行号

                int[] tableTitle = new int[17];
                string tmpKey = "";

                Debug.Log(minColumnNum + "|" + maxColumnNum + "|" + minRowNum + "|" + maxRowNum);

                string tmpCarType = "";
                string prevCarType = "";//用于解决上次值为空的问题               
                for (int i = minRowNum; i < maxRowNum; i++)
                {
                    string note = "";
                    ExcelInfo item = new ExcelInfo();
                    for (int j = minColumnNum; j < maxColumnNum; j++)
                    {
                        var a = w.Cells[i, j];
                        //获取表头
                        /*
                         * 排放、车型、指导价、车架号、发车日期、到店日期、库龄、AAK日期、质sun、合格证、客户姓名、销售顾问、otc
                         */
                        if (a.RichText.Text.Contains("排放"))
                        {
                            tableTitle[0] = a.Start.Column;
                        }
                        else if (a.RichText.Text.Contains("车型") && a.RichText.Text.Length < 10)
                        {
                            //当再次遇到车型标签表示重新计算表头
                            for (int ii = 0; ii < tableTitle.Length; ii++)
                            {
                                tableTitle[ii] = -1;
                            }
                            tableTitle[1] = a.Start.Column;
                        }
                        else if (a.RichText.Text.Contains("指导价") && a.RichText.Text.Length < 10)
                        {
                            tableTitle[2] = a.Start.Column;
                        }
                        else if (a.RichText.Text.Contains("车架") && a.RichText.Text.Length < 10)
                        {
                            tableTitle[3] = a.Start.Column;
                        }
                        else if (a.RichText.Text.Contains("发车日期") && a.RichText.Text.Length < 10)
                        {
                            tableTitle[4] = a.Start.Column;
                        }
                        else if (a.RichText.Text.Contains("到店日期") && a.RichText.Text.Length < 10)
                        {
                            tableTitle[5] = a.Start.Column;
                        }
                        else if (a.RichText.Text.Contains("库龄") && a.RichText.Text.Length < 10)
                        {
                            tableTitle[6] = a.Start.Column;
                        }
                        else if (a.RichText.Text.Contains("AAK日期") && a.RichText.Text.Length < 10)
                        {
                            tableTitle[7] = a.Start.Column;
                        }
                        else if (a.RichText.Text.Contains("质损") && a.RichText.Text.Length < 10)
                        {
                            tableTitle[8] = a.Start.Column;
                        }
                        else if (a.RichText.Text.Contains("合格证") && a.RichText.Text.Length < 10)
                        {
                            tableTitle[9] = a.Start.Column;
                        }
                        else if (a.RichText.Text.Contains("客户姓名") && a.RichText.Text.Length < 10)
                        {
                            tableTitle[10] = a.Start.Column;
                        }
                        else if (a.RichText.Text.Contains("销售顾问") && a.RichText.Text.Length < 10)
                        {
                            tableTitle[11] = a.Start.Column;
                        }
                        else if (a.RichText.Text.Contains("组别") && a.RichText.Text.Length < 10)
                        {
                            tableTitle[12] = a.Start.Column;
                        }
                        else if (a.RichText.Text.Contains("签订日期") && a.RichText.Text.Length < 10)
                        {
                            tableTitle[13] = a.Start.Column;
                        }
                        else if (a.RichText.Text.Contains("配车天数") && a.RichText.Text.Length < 10)
                        {
                            tableTitle[14] = a.Start.Column;
                        }
                        else if (a.RichText.Text.Contains("付款方式") && a.RichText.Text.Length < 10)
                        {
                            tableTitle[15] = a.Start.Column;
                        }
                        else if (a.RichText.Text.Contains("备注") && a.RichText.Text.Length < 10)
                        {
                            tableTitle[16] = a.Start.Column;
                        }
                        else
                        {
                            for (int k = 0; k < tableTitle.Length; k++)
                            {
                                if (a.Start.Column == tableTitle[k])
                                {
                                    switch (k)
                                    {
                                        case 0:
                                            item.discharge = a.RichText.Text;//排放                                            
                                            break;
                                        case 1:
                                            //item.carType = a.RichText.Text;//车型
                                            tmpCarType = a.RichText.Text;//车型
                                            break;
                                        case 2:
                                            item.guidancePrice = a.RichText.Text;//指导价
                                            break;
                                        case 3:
                                            //item.carNumber = a.RichText.Text;//车架号
                                            tmpKey = a.RichText.Text;//车架号作为关键字
                                            //to do 改成id自增
                                            break;
                                        case 4:
                                            {
                                                string dateStr = a.RichText.Text;
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
                                                string dateStr = a.RichText.Text;
                                                Match mInfo = Regex.Match(dateStr, @"(?i)^[0-9]+$");
                                                if (dateStr.Trim() != string.Empty && dateStr.Length == 5 && mInfo.Success)
                                                {
                                                    dateStr = DateTime.FromOADate(double.Parse(dateStr)).ToString("yyyy/MM/dd");
                                                }
                                                item.arriveDate = dateStr;//到店日期
                                            }
                                            break;
                                        case 6:
                                            item.garageAge = a.RichText.Text;//库龄
                                            break;
                                        case 7:
                                            {
                                                string dateStr = a.RichText.Text;
                                                Match mInfo = Regex.Match(dateStr, @"(?i)^[0-9]+$");
                                                if (dateStr.Trim() != string.Empty && dateStr.Length == 5 && mInfo.Success)
                                                {
                                                    dateStr = DateTime.FromOADate(double.Parse(dateStr)).ToString("yyyy/MM/dd");
                                                }
                                                item.akkDate = dateStr;//AAK日期
                                            }
                                            break;
                                        case 8:
                                            item.qualityloss = a.RichText.Text;//质损
                                            break;
                                        case 9:
                                            item.certificate = a.RichText.Text;//合格证
                                            break;
                                        case 10:
                                            item.userName = a.RichText.Text;//客户姓名
                                            break;
                                        case 11:
                                            item.adviser = a.RichText.Text;//销售顾问
                                            break;
                                        case 12:
                                            item.carGroup = a.RichText.Text;//组别
                                            break;
                                        case 13:
                                            {
                                                string dateStr = a.RichText.Text;
                                                Match mInfo = Regex.Match(dateStr, @"(?i)^[0-9]+$");
                                                if (dateStr.Trim() != string.Empty && dateStr.Length == 5 && mInfo.Success)
                                                {
                                                    dateStr = DateTime.FromOADate(double.Parse(dateStr)).ToString("yyyy/MM/dd");
                                                }
                                                item.signDate = dateStr;//签订日期
                                            }
                                            break;
                                        case 14:
                                            item.useTime = a.RichText.Text;//配车天数
                                            break;
                                        case 15:
                                            item.payType = a.RichText.Text;//付款方式
                                            break;
                                        case 16:
                                            item.memo = a.RichText.Text;//备注
                                            break;
                                    }
                                }
                            }
                            if (a.Comment != null && a.Comment.RichText.Text.Trim() != string.Empty)
                            {
                                note += "\n" + a.Comment.RichText.Text;//获取备注
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
                            if (!_productInfo.Contains(item)){
                                _productInfo.Add(item);
                                //加入数据库，替换当前数据
                                dBManager.CheckReplace<ExcelInfo>("carNumber", item.carNumber, item);
                                //delayPanelManager.SetText("条数:" + id + " 页数：" + wIndex + "/" + worksheets.Count);
                                this._id = id;
                                this._wIndex = wIndex;
                                this._worksheetsCount = worksheets.Count;
                                id++;                              
                            }
                            prevCarType = tmpCarType;
                        }
                    }
                    //yield return new WaitForEndOfFrame();
                    Thread.CurrentThread.Join(1);
                    this.doUpateUI = true;
                    _waitHandle.WaitOne();
                }
                wIndex = wIndex + 1;
            }

            //test
            //foreach (var a in _productInfo)
            //{
            //    string tmpStr = "";
            //    foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(a))
            //    {
            //        string name = descriptor.Name;
            //        object value = descriptor.GetValue(a);
            //        tmpStr += ("|" + value);
            //    }
            //    Debug.Log(tmpStr);
            //}
            //tb.InsertDb(_productInfo);               
            // return new WaitForSeconds(1);
            Thread.CurrentThread.Join(1000);//阻止设定时间
            loadEnd = true;
        }
    }

    void Update()
    {
        //刷新读取UI
        if (doUpateUI)
        {
            if (doUpdateTable)
            {
                //Debug.Log("doUpateUI");
                delayPanelManager.SetText("条数:" + _id + " 页数：" + _wIndex + "/" + _worksheetsCount);

            }
            else
            {
                autoExcelManager.stateText.text = "状态:" + currentFilePath + "|条数:" + _id + " 页数：" + _wIndex + "/" + _worksheetsCount;
            }
            doUpateUI = false;
            _waitHandle.Set();
        }

        if (loadEnd)
        {
            delayPanelManager.Destory();
            tb.Init(4);//到时间之后界面都带界面刷新
            if (doUpdateTable)
            {
                //tb.Init(4);
                autoExcelManager.SetRun(true);
            }
            loadEnd = false;
        }
    }

}
