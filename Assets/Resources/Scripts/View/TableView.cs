using LitJson;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Xiaoku;

public class TableView : MonoBehaviour
{
    public GameObject PanelParent;
    public GameObject Row;
    public GameObject Col;

    public HeadView headView;

    public Button saveBtn;//保存按钮

    public Button updateBtn;//同步数据按钮

    private GameObject TableLine;

    private float Width;

    private List<ProductInfo> productInfoList = new List<ProductInfo>();
    private List<ProductInfo1> productInfoList1 = new List<ProductInfo1>();
    private List<ProductInfo2> productInfoList2 = new List<ProductInfo2>();
    private List<ProductInfo3> productInfoList3 = new List<ProductInfo3>();
    private List<ExcelInfo> excelInfoList = new List<ExcelInfo>();
    private List<TmpExcelInfo> tmpExcelInfoList = new List<TmpExcelInfo>();

    private DBManager dBManager;

    public SecondPsdManager secondPsdManager;

    public DelayPanelManager delayPanelManager;

    public NetworkManager networkManager;

    public MessagePanelManager messagePanelManager;

    public Xiaoku.Controller loopListCtrl;

    private string[,] _data;

    //水平和垂直scroll
    public Scrollbar vScrollbar;
    public Scrollbar hScrollbar;

    public RectTransform scrollViewTectTransform;
    public RectTransform contentRectTransform;

    private int hMax = 0;//水平方向最大值  0>1
    private int vMax = 0;//垂直方向最大值  1>0

    public int currentDataType = 0;

    // Start is called before the first frame update
    void Awake()
    {
        //scrollViewTectTransform.sizeDelta = new Vector2(Screen.width*0.97f, Screen.height*0.777f);
        //var sizeD = scrollViewTectTransform.sizeDelta;
        //scrollViewTectTransform.anchoredPosition = new Vector2(sizeD.x/2, -sizeD.y/2);
        ///////////////////
        //var rectTransform = transform.GetComponent<RectTransform>();
        //rectTransform.sizeDelta = new Vector2(Screen.width, rectTransform.rect.height);
        dBManager = DBManager._DBInstance();
        saveBtn.onClick.AddListener(SaveMethod);
        updateBtn.onClick.AddListener(UpdateDataToWeb);
    }

    public void Init(int dataType = 0, UnityAction callback = null, string condition = "")
    {
        currentDataType = dataType;
        gameObject.SetActive(true);
        //从 ProductInfo 中获得需要新建多少列
        InitPrefabs(typeof(ProductInfo).GetProperties().Length);
        //从dbManager中获得需要新建多少列
        switch (dataType)
        {
            //case 1:
            //    productInfoList1 = dBManager.QueryTable<ProductInfo1>();
            //    productInfoList.Clear();
            //    productInfoList.AddRange(productInfoList1);
            //    break;

            case 2:
                productInfoList2 = dBManager.QueryTable<ProductInfo2>(condition);
                productInfoList.Clear();
                productInfoList.AddRange(productInfoList2);
                groupByCar<ProductInfo2>(1);
                break;

            case 3:
                productInfoList3 = dBManager.QueryTable<ProductInfo3>(condition);
                productInfoList.Clear();
                productInfoList.AddRange(productInfoList3);
                groupByCar<ProductInfo3>(2);
                break;

            case 4:
                excelInfoList = dBManager.QueryTable<ExcelInfo>(condition);
                productInfoList.Clear();
                productInfoList.AddRange(excelInfoList);
                //maxCount = productInfoList.Count;//设置最大数量
                groupByCar<ExcelInfo>(3);
                break;

            case 5:
                tmpExcelInfoList = dBManager.QueryTable<TmpExcelInfo>(condition);
                productInfoList.Clear();
                productInfoList.AddRange(tmpExcelInfoList);
                //maxCount = productInfoList.Count;//设置最大数量
                groupByCar<TmpExcelInfo>(4);
                break;

            default:
                productInfoList = dBManager.QueryTable<ProductInfo>(condition);
                groupByCar<ProductInfo>(0);
                break;
        }
        //productInfoList = dBManager.QueryTable<ProductInfo>();
        //左使用的老方法StartCoroutine(ChangeTable(callback));
        UpdateLoopList(callback);
    }

    public void DoRefresh()
    {
        Init(this.currentDataType);
    }

    //车系分类
    void groupByCar<T>(int state)
    {
        var data = dBManager.GroupBy<T>("vehicleSystem");
        Debug.Log("group by vehicleSystem");
        List<string> rdata = new List<string>();
        foreach (var dic in data)
        {
            Debug.Log(dic["vehicleSystem"]);
            rdata.Add(dic["vehicleSystem"]);
        }
        headView.SetCarDropDownData(state, rdata);
    }

    private void InitPrefabs(int num)
    {
        //这里的宽度通过产品信息表中得到
        Vector2 sizePanel = PanelParent.GetComponent<RectTransform>().sizeDelta;
        //Vector3 vc3 = Row.GetComponent<GridLayoutGroup>().cellSize;
        //Width = num * vc3.x;
        Width = 0;
        for (int i = 0; i < widths.Length; i++) Width += widths[i];
        sizePanel = new Vector2(Width, sizePanel.y);
        PanelParent.GetComponent<RectTransform>().sizeDelta = sizePanel;

        TableLine = GameObject.Instantiate(Row);

        for (int i = 0; i < num; i++)
        {
            GameObject obj = GameObject.Instantiate(Col);
            obj.name = "col" + i;
            obj.transform.SetParent(TableLine.transform);
        }
    }
    public void InitPanel()
    {
        //这个是初始化面板信息,
        //高度通过查询得到的数据决定
    }

    int maxCount = 200;//最大行数

    //单元格宽度
    int[] widths = new int[21]
        {80, 100, 50, 180, 100, 180, 100,
        100, 50, /**批注**/100, 100, /**质损**/100, 100,
        100, 100, 100, 100, 100,
        100, 100, 100};

    private IEnumerator ChangeTable(UnityAction callback = null)
    {
        delayPanelManager.Load();
        //清空原来所有的内容
        for (int i = 0; i < contentRectTransform.childCount; i++)
        {
            Destroy(contentRectTransform.GetChild(i).gameObject);
        }
        //设置第一行内容，输入功能关闭
        GameObject temp = GameObject.Instantiate(TableLine);
        temp.name = "Title";
        var InputArray = temp.GetComponentsInChildren<InputField>();
        var p_list = typeof(ProductInfo).GetProperties();

        _data = new string[maxCount, p_list.Length];

        Debug.Log("p_list.Length:" + p_list.Length);

        //记录_data的第一行内容
        string[] oneLine = new string[InputArray.Length];
        hMax = InputArray.Length;
        for (int i = 0; i < InputArray.Length; i++)
        {
            InputArray[i].interactable = false;
            InputArray[i].GetComponentInChildren<Text>().text = SQLiteTools.GetFieldTitle(p_list[i]);
            var inputId = InputArray[i].GetComponent<InputId>();
            inputId.rowId = 0;
            inputId.colId = i;
            _data[0, i] = SQLiteTools.GetFieldTitle(p_list[i]);
            InputArray[i].onValueChanged.AddListener(delegate
            {
                _data[inputId.rowId, inputId.colId] = InputArray[i].text;
            });
            InputArray[i].GetComponent<UI_Test>().SetWidth(widths[i]);
        }
        temp.transform.SetParent(PanelParent.transform);

        //后面填充数据
        int infoCount = Mathf.Min(productInfoList.Count, maxCount - 1);
        Debug.Log("infoCount:" + infoCount);
        for (int i = 0; i < infoCount; i++)
        {
            GameObject obj = GameObject.Instantiate(TableLine);
            obj.name = "row" + i;
            obj.transform.SetParent(PanelParent.transform);
            var inputArray = obj.GetComponentsInChildren<InputField>();
            //Debug.Log(inputArray.Length);

            string[] tmpStr = new string[p_list.Length];
            for (int j = 0; j < p_list.Length; j++)
            {
                var param = p_list[j].GetValue(productInfoList[i]).ToString();
                inputArray[j].text = param;
                var inputId = inputArray[j].GetComponent<InputId>();
                inputId.rowId = i + 1; ;
                inputId.colId = j;
                _data[i + 1, j] = param;
                inputArray[j].onValueChanged.AddListener(delegate
                {
                    _data[inputId.rowId, inputId.colId] = inputId.GetComponent<InputField>().text;
                });

                inputArray[j].GetComponent<UI_Test>().SetWidth(widths[j]);

                //等于9批注和最后备注可以弹出对话框
                if (j == 9 || j == p_list.Length - 1)
                {
                    var cg = inputArray[j].gameObject.AddComponent<ClickDialogPointer>();
                    cg.text = param;
                }
            }
            yield return new WaitForFixedUpdate();
        }

        //补充剩余行数,凑齐200行

        //todo 不一定要补齐
        /**
        for (int i = infoCount; i < 200 - infoCount; i++)
        {
            GameObject obj = GameObject.Instantiate(TableLine);
            obj.name = "row" + i;
            obj.transform.SetParent(PanelParent.transform);
            var inputArray = obj.GetComponentsInChildren<InputField>();
            //Debug.Log(inputArray.Length);

            string[] tmpStr = new string[p_list.Length];
            for (int j = 0; j < p_list.Length; j++)
            {
                inputArray[j].text = "";
                var inputId = inputArray[j].GetComponent<InputId>();
                inputId.rowId = i + 1; ;
                inputId.colId = j;
                _data[i + 1, j] = "";
                inputArray[j].onValueChanged.AddListener(delegate
                {
                    _data[inputId.rowId, inputId.colId] = inputId.GetComponent<InputField>().text;
                });
            }
            vMax = i;
            yield return new WaitForFixedUpdate();
            if (callback != null) callback();
            //yield return new WaitForSeconds(0.3f);
            //delayPanelManager.Destory();
        }
        **/

        if (callback != null) callback();
        delayPanelManager.Destory();



    }

    private void SaveMethod()
    {
        Debug.Log("执行SaveMethod方法");

        secondPsdManager.Show();

        secondPsdManager.callBack = delegate
        {
            string outPutDir = SaveProject();
            if (outPutDir != null)
            {
                //遍历Content下的一级单元成List
                //string outPutDir = Application.dataPath + "\\" + "MyExcel.xls";
                Debug.Log("当前保存路径为:" + outPutDir);
                FileInfo newFile = new FileInfo(outPutDir);
                if (newFile.Exists)
                {
                    newFile.Delete();
                    Debug.Log("删除表");
                    newFile = new FileInfo(outPutDir);
                }

                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("我的Excel");
                    //原tabview使用的格式
                    //for (int i = 0; i < _data.GetLength(0); i++)
                    //{
                    //    for (int j = 0; j < _data.GetLength(1); j++)
                    //    {
                    //        worksheet.Cells[i + 1, j + 1].Value = _data[i, j];
                    //    }
                    //}

                    var titleLength = typeof(ProductInfo).GetProperties().Length;
                    var loopData = loopListCtrl.GetData();
                    for (int i = 0; i < loopData.Count; i++)
                    {
                        //Debug.Log("loopData:" + loopData[i].someText);
                        worksheet.Cells[i / titleLength + 1, i % titleLength + 1].Value = loopData[i].someText;
                    }
                    package.Save();
                    //to do 保存数据库
                    Debug.Log("导出Excel成功");
                }

                //上传文件
                //networkManager.DoUploadFile(outPutDir, (message) =>
                //{
                //    Debug.LogWarning(message);
                //    messagePanelManager.Show(message);
                //});
            }

        };

    }

    private string SaveProject()
    {
        SaveFileDlg pth = new SaveFileDlg();
        pth.structSize = Marshal.SizeOf(pth);
        pth.filter = "Excel文件(*.xls)\0*.xls";
        pth.file = new string(new char[256]);
        pth.maxFile = pth.file.Length;
        pth.fileTitle = new string(new char[64]);
        pth.maxFileTitle = pth.fileTitle.Length;
        pth.initialDir = Application.dataPath; //默认路径
        pth.title = "保存项目";
        pth.defExt = "dat";
        pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        if (SaveFileDialog.GetSaveFileName(pth))
        {
            return pth.file;
        }
        else
        {
            return null;
        }

    }


    public void SearchMethod(string info)
    {
        string result = "";
        int result_i = 0;
        int result_j = 0;
        Debug.LogWarningFormat("当前长度:{0},宽度:{1}", _data.GetLength(0), _data.GetLength(1));
        for (int i = 0; i < _data.GetLength(0); i++)
        {
            if (!result.Equals(""))
                break;
            for (int j = 0; j < _data.GetLength(1); j++)
            {
                if (_data[i, j] == null)
                    break;
                if (_data[i, j].Equals(info))
                {
                    result = _data[i, j];
                    result_i = i;
                    result_j = j;
                    break;
                }
            }
        }
        if (!result.Equals(""))
        {
            Debug.LogFormat("查询结果是i:{0},j{1},result:{2},vMax{3},hMax{4}", result_i, result_j, result, vMax, hMax);
            /*
             排放 i=0 j=1
             v 1
             h 0.133
             */
            //i对应是 对应H 0>1

            //j对应是 对应V 1>0

            //先测试横轴对应_data的j
            Vector3 vc3 = Row.GetComponent<GridLayoutGroup>().cellSize;
            int maxJ = hMax - (int)(scrollViewTectTransform.rect.width / vc3.x);
            int maxI = vMax - (int)(scrollViewTectTransform.rect.height / vc3.y);
            if (result_j > maxJ - 1)
                hScrollbar.value = 1;
            else
            {
                var h = (float)(200.0 * result_j) / (contentRectTransform.rect.width - scrollViewTectTransform.rect.width);
                //Debug.LogFormat("打印结果:{0}", h.ToString());
                hScrollbar.value = h;
            }
            if (result_i > maxI - 1)
                vScrollbar.value = 0;
            else
            {
                var v = 1 - (float)(50.0 * result_i) / (contentRectTransform.rect.height - scrollViewTectTransform.rect.height);
                vScrollbar.value = v;
            }

            //获取对应的InputField
            var tmpInputField = contentRectTransform.transform.GetChild(result_i).GetChild(result_j).GetComponent<InputField>();
            if (tmpInputField.interactable)
            {
                Debug.Log("打印查询到列名:" + tmpInputField.text);
                //tmpInputField.ActivateInputField();
                EventSystem.current.SetSelectedGameObject(tmpInputField.gameObject);
            }
        }
    }

    public void ScreeningMethod(string info)
    {
        List<int> intList = new List<int>();
        for (int i = 1; i < _data.GetLength(0); i++)
        {
            for (int j = 0; j < _data.GetLength(1); j++)
            {
                if (_data[i, j] == null)
                    break;
                if (_data[i, j].Contains(info))
                {
                    //Debug.LogFormat("查询到的结果i:{0},j:{1},v:{2}", i, j, _data[i, j]);
                    intList.Add(i);
                    break;
                }
            }
        }
        Queue<GameObject> queueObj = new Queue<GameObject>();
        //TODO:使用这个intList中对应的行 找到content中对应的行的gameobject放到一个临时的List中,存放完后再次遍历执行Destory
        //第一行必须保存
        queueObj.Enqueue(contentRectTransform.transform.GetChild(0).gameObject);
        for (int i = 0; i < intList.Count; i++)
        {
            queueObj.Enqueue(contentRectTransform.transform.GetChild(intList[i]).gameObject);
        }
        //删除根节点下的所有内容
        for (int i = 0; i < contentRectTransform.childCount; i++)
        {
            if (queueObj.Count == 0)
                break;
            if (contentRectTransform.GetChild(i).gameObject == queueObj.Peek())
            {
                queueObj.Dequeue();
                continue;
            }
            else
            {
                Destroy(contentRectTransform.GetChild(i).gameObject);
            }
        }
        ////执行完后再反向生成一次_data
        int newHeight = contentRectTransform.transform.childCount;
        int newWidth = contentRectTransform.transform.GetChild(0).childCount;
        _data = new string[newHeight, newWidth];

        //for (int i = 0; i < newHeight; i++)
        //{
        //    for(int j = 0; j< newWidth; j++)
        //    {
        //        InputField tmpIF = contentRectTransform.transform.GetChild(i).GetChild(j).GetComponent<InputField>();
        //        if (tmpIF.interactable)
        //        {
        //            _data[i, j] = tmpIF.text;
        //        }
        //        else
        //        {
        //            _data[i, j] = tmpIF.GetComponentInChildren<Text>().text;
        //        }
        //    }
        //}
    }

    //使用Vals注入单元格********废弃方法
    public void ExceltoTableV(List<string> vals, int row, int col)
    {
        StartCoroutine(ChangeExcelTable(vals, row, col));
    }

    //********废弃方法
    private IEnumerator ChangeExcelTable(List<string> vals, int row, int col)
    {
        //清空原来所有的内容
        for (int i = 0; i < contentRectTransform.childCount; i++)
        {
            Destroy(contentRectTransform.GetChild(i).gameObject);
        }
        //设置第一行内容，输入功能关闭
        GameObject temp = GameObject.Instantiate(TableLine);
        temp.name = "Title";
        var InputArray = temp.GetComponentsInChildren<InputField>();
        var p_list = typeof(ProductInfo).GetProperties();

        _data = new string[maxCount, col];

        //记录_data的第一行内容
        string[] oneLine = new string[InputArray.Length];
        hMax = InputArray.Length;
        for (int i = 0; i < InputArray.Length; i++)
        {
            InputArray[i].interactable = false;
            InputArray[i].GetComponentInChildren<Text>().text = SQLiteTools.GetFieldTitle(p_list[i]);
            var inputId = InputArray[i].GetComponent<InputId>();
            inputId.rowId = 0;
            inputId.colId = i;
            _data[0, i] = SQLiteTools.GetFieldTitle(p_list[i]);
            InputArray[i].onValueChanged.AddListener(delegate
            {
                _data[inputId.rowId, inputId.colId] = InputArray[i].text;
            });
        }
        temp.transform.SetParent(PanelParent.transform);

        for (int i = 1; i < row; i++)
        {
            GameObject obj = GameObject.Instantiate(TableLine);
            obj.name = "row" + i;
            obj.transform.SetParent(PanelParent.transform);
            var inputArray = obj.GetComponentsInChildren<InputField>();
            Debug.Log(inputArray.Length);

            string[] tmpStr = new string[p_list.Length];
            for (int j = 0; j < p_list.Length; j++)
            {
                //var param = p_list[j].GetValue(productInfoList[i]).ToString();
                int index = i * col + j;
                string param = "";
                if (index < vals.Count)
                {
                    Debug.Log(vals[index]);
                    param = vals[index];
                }
                inputArray[j].text = param;
                var inputId = inputArray[j].GetComponent<InputId>();
                inputId.rowId = i;
                inputId.colId = j;
                _data[i, j] = param;
                inputArray[j].onValueChanged.AddListener(delegate
                {
                    _data[inputId.rowId, inputId.colId] = inputId.GetComponent<InputField>().text;
                });
            }
            yield return new WaitForFixedUpdate();
        }
    }

    //插入数据库
    public void InsertDb(List<ProductInfo> plist)
    {
        dBManager.DeleteAll<ExcelInfo>();
        dBManager.Insert<ExcelInfo>(plist);
        Init(4);
    }

    //刷新循环列表
    public void UpdateLoopList(UnityAction callback = null)
    {
        if (productInfoList.Count > 0)
        {
            EnhancedUI.SmallList<Data> strList = new EnhancedUI.SmallList<Data>();

            //标题
            var titleList = typeof(ProductInfo).GetProperties();
            for (int i = 0; i < titleList.Length; i++)
            {
                strList.Add(new Data()
                {
                    someText = SQLiteTools.GetFieldTitle(titleList[i]),
                    isEnable = false
                });
            }
            int id = 0;
            //数据
            for (int i = 0; i < productInfoList.Count; i++)
            {
                for (int j = 0; j < titleList.Length; j++)
                {
                    var param = titleList[j].GetValue(productInfoList[i]).ToString();
                    strList.Add(new Data() { id = id, someText = param, isEnable = true });
                    id++;
                }
            }

            loopListCtrl.LoadData(strList, titleList.Length);
        }
        if (callback != null) callback();
    }

    //同步数据到后台
    public void UpdateDataToWeb()
    {
        delayPanelManager.Load();
        Debug.Log("do update");
        string filePath = Application.persistentDataPath + "/xiaoku.csv";

        FileInfo newFile = new FileInfo(filePath);
        if (newFile.Exists)
        {
            newFile.Delete();
            Debug.Log("删除表");
            newFile = new FileInfo(filePath);
        }
        //using (ExcelPackage package = new ExcelPackage(newFile))
        //{
        //    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("xiaoku");
        //    var titleLength = typeof(ProductInfo).GetProperties().Length;
        //    var loopData = loopListCtrl.GetData();
        //    for (int i = 0; i < loopData.Count; i++)
        //    {
        //        //Debug.Log("loopData:" + loopData[i].someText);
        //        //跳过第一行
        //        if (i < titleLength) continue;
        //        int index = i - titleLength;
        //        worksheet.Cells[index / titleLength + 1, index % titleLength + 1].Value = loopData[i].someText;
        //    }
        //    package.Save();
        //    //to do 保存数据库
        //    Debug.Log("导出Excel成功");
        //}

        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            string row_txt = "";

            var titleLength = typeof(ProductInfo).GetProperties().Length;
            var loopData = loopListCtrl.GetData();

            //foreach (DataColumn item in dt.Columns)// dt为DataTable
            //{
            //    col_txt += item.ToString() + ","; // 循环得到列名
            //}
            //col_txt = col_txt.Substring(0, col_txt.Length - 1);
            //sw.WriteLine(col_txt);//写入文件


            for (int i = titleLength; i < loopData.Count; i = i + titleLength)
            {
                row_txt = "";//此处容易遗漏，导致数据的重复添加
                //Debug.Log(i % titleLength);
                for (int j = 0; j < titleLength; j++)
                {
                    row_txt += "\"" + loopData[i + j].someText + "\","; //循环得到行数据
                }
                row_txt = row_txt.Substring(0, row_txt.Length - 1);
                sw.WriteLine(row_txt);//写入文件
            }
            sw.Flush();//提交所进行的操作            
        }

        networkManager.DoUploadFile(API.UpdateCartSource, filePath, delegate (string responseCode, string data)
            {
                Debug.Log("responseCode:" + responseCode + "|" + data);
                if (responseCode == "200")
                {
                    try
                    {
                        JsonData jdata = JsonMapper.ToObject(data);
                        if (jdata["code"].ToString() == "0")
                        {
                            //warnText.text = jdata["msg"].ToString();                      
                        }
                        else
                        {
                            //warnText.text = jdata["msg"].ToString();
                        }
                        Debug.Log(jdata["msg"].ToString());
                        messagePanelManager.Show(jdata["msg"].ToString());
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                        //warnText.text = e.Message;
                        messagePanelManager.Show(e.Message);
                    }
                }
                else
                {
                    //warnText.text = "responseCode:" + responseCode;
                    Debug.Log("responseCode:" + responseCode);
                }
                delayPanelManager.Destory();
            }, networkManager.token);
    }

    List<TmpExcelInfo> tmpLists = new List<TmpExcelInfo>();

    public void DoWebQuery(string id)
    {
        delayPanelManager.Load();
        delayPanelManager.SetText("查询中");
        tmpExcelInfoList.Clear();
        string url = API.GetLinkMerchantCartSource + "&merchant_id=" + id;
        networkManager.DoGet(url, (responseCode, data) =>
        {
            Debug.Log("responseCode:" + responseCode + "|" + data);
            if (responseCode == 200)
            {
                JsonData jdata = JsonMapper.ToObject(data);
                try
                {
                    if (jdata["code"].ToString() == "0")
                    {
                        var cartSource = jdata["cartSource"];
                        var rows = cartSource["rows"];
                        for (int i = 0; i < rows.Count; i++)
                        {
                            var a = rows[i];
                            TmpExcelInfo item = new TmpExcelInfo();
                            item.id = i + 1;
                            item.vehicleSystem = a["vehicleSystem"].ToString();
                            item.carType = a["carType"] == null ? "" : a["carType"].ToString();
                            item.discharge = a["discharge"] == null ? "" : a["discharge"].ToString();
                            item.guidancePrice = a["guidancePrice"] == null ? "" : a["guidancePrice"].ToString();
                            item.carNumber = a["carNumber"].ToString();
                            item.releaseDate = a["releaseDate"] == null ? "" : a["releaseDate"].ToString();
                            item.arriveDate = a["arriveDate"] == null ? "" : a["arriveDate"].ToString();
                            item.garageAge = a["garageAge"] == null ? "" : a["garageAge"].ToString();
                            item.note = a["note"] == null ? "" : a["note"].ToString();
                            item.akkDate = a["akkDate"] == null ? "" : a["akkDate"].ToString();
                            item.certificate = a["certificate"] == null ? "" : a["certificate"].ToString();
                            item.memo = a["memo"] == null ? "" : a["memo"].ToString();
                            //dataManager.InsertDb<TmpExcelInfo>(item);
                            //dBManager.Insert<TmpExcelInfo>(item);
                            tmpExcelInfoList.Add(item);
                        }
                    }
                    else
                    {
                        delayPanelManager.Destory();
                    }
                    //warnText.text = jdata["msg"].ToString();
                    messagePanelManager.Show(jdata["msg"].ToString());
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    //warnText.text = e.Message;
                    messagePanelManager.Show(e.Message);
                }
            }
            else
            {
                //warnText.text = "responseCode:" + responseCode;
                messagePanelManager.Show("responseCode:" + responseCode);
                delayPanelManager.Destory();
            }
            if (tmpExcelInfoList.Count > 0)
            {
                _doTmpInsertDb();//数据插入数据库
            }
            else
            {
                messagePanelManager.Show("该商家还没有上传数据");
                delayPanelManager.Destory();
            }

        }, networkManager.token);
    }

    void _doTmpInsertDb()
    {
        StartCoroutine(_iInsertDB());
    }

    IEnumerator _iInsertDB()
    {
        dBManager.DeleteAll<TmpExcelInfo>();
        for (int i = 0; i < tmpExcelInfoList.Count; i++)
        {
            dBManager.Insert<TmpExcelInfo>(tmpExcelInfoList[i]);
            delayPanelManager.SetText((i + 1) + "/" + tmpExcelInfoList.Count);
            yield return new WaitForEndOfFrame();
        }
        delayPanelManager.Destory();
        Init(5);
    }
}
