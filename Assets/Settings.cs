using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public static string ExcelPath;

    private string keyName = "XiaoKuExcelPath";

    public Button OpenPathBtn;

    public Text PathText;

 
    

    // Start is called before the first frame update
    void Start()
    {
        OpenPathBtn.onClick.AddListener(LoadExcel);
       // OpenPathBtn.onClick.AddListener(loadExcelsTest);
        if (PlayerPrefs.HasKey(keyName))
        {
            PathText.text = PlayerPrefs.GetString(keyName);
        }

      
    }

    public void LoadExcel()
    {
        OpenFileDlg pth = new OpenFileDlg();
        pth.structSize = System.Runtime.InteropServices.Marshal.SizeOf(pth);
        pth.filter = "Excel文件(*.xls)\0*.xls;*.xlsx;*.*;";//"xls (*.xls)";
        pth.file = new string(new char[256]);
        pth.maxFile = pth.file.Length;
        pth.fileTitle = new string(new char[64]);
        pth.maxFileTitle = pth.fileTitle.Length;
        pth.initialDir = Application.dataPath;  // default path  

        pth.title = "设置Excel文件路径";
       
        pth.defExt = "xls";
        pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        //0x00080000   是否使用新版文件选择窗口
        //0x00000200   是否可以多选文件
        if (OpenFileDialog.GetOpenFileName(pth))
        {
            ExcelPath = pth.file; //选择的文件路径;  
           Debug.Log("excelpath"+ ExcelPath);

            PathText.text = ExcelPath;

            if (ExcelPath!=PlayerPrefs.GetString(keyName))//路径相同文件名不同也是！=,如果文件不同就删除数据重新读ExcelPath != PlayerPrefs.GetString(keyName)
            { 
                // List<string> clearPostCar=new List<string>();
                // for (int i = 0; i < PriceManager.Instance.putSJ.Count; i++)
                // {
                //     clearPostCar.Add(PriceManager.Instance.putSJ[i]);
                // }

                StartCoroutine(coroutine.instance.GetDeleteAllCar());//clearPostCar));
                PriceManager.Instance.ClearAllData();
                MyLoginManager.instance.GetHadPrice();
                PriceManager.Instance.isNeedCompare = false;
                
                PriceManager.Instance. loadExcelsTest(ExcelPath);
                tip.instance.SetMessae("清除掉所有数据,读取表格文件");
            }
            else
            {
                PriceManager.Instance.isNeedCompare = true;
                PriceManager.Instance. loadExcelsTest(ExcelPath);
            }

            coroutine.instance.time = Time.time;
           
            PlayerPrefs.SetString(keyName, ExcelPath);
        }
    }

    private string SourceExcelPath;
   
  
   

}
