using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public static string ExcelPath;

    private string keyName = "XiaoKuExcelPath";

    public Button OpenPathBtn;

    public Text PathText;

    public int Interval = 30;
    

    // Start is called before the first frame update
    void Start()
    {
        OpenPathBtn.onClick.AddListener(LoadExcel);
        if (PlayerPrefs.HasKey(keyName))
        {
            PathText.text = PlayerPrefs.GetString(keyName);
        }

        StartCoroutine(AutoLoadExcel());
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
            Debug.Log("path "+ExcelPath);

            PathText.text = ExcelPath;

            if (ExcelPath != PlayerPrefs.GetString(keyName))//路径相同文件名不同也是！=,如果文件不同就删除数据重新读
            {
                Debug.Log("!=");
                PriceManager.Instance.ClearAllData();
                PriceManager.Instance.DoLoadThread(ExcelPath);
               // PriceManager.Instance.ReadCarPrice(ExcelPath);
                tip.instance.SetMessae("删除数据，重新读取表格");
            }
            else//否则不删除数据重新读
            {
               // PriceManager.Instance.DoLoadThread(ExcelPath);
            }
            PlayerPrefs.SetString(keyName, ExcelPath);
        }
    }


   
    public IEnumerator AutoLoadExcel()
    {
        while (true)
        {
            yield return new WaitForSeconds(Interval*60 );
            tip.instance.SetMessae("自动重新读取表格");
            //考虑什么时候开始自动加载，决定了自动加载的路径是否是离线数据
            PriceManager.Instance.ReadCarPrice(ExcelPath);
        }
    }

}
