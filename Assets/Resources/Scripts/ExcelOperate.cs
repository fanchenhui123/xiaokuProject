using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using OfficeOpenXml;

public class ExcelOperate : MonoBehaviour
{
    void CreateExcel()
    {
        string outPutDir = Application.dataPath + "\\" + "MyExcel.xls";
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
            worksheet.Cells[1, 1].Value = "序号";
            worksheet.Cells[1, 2].Value = "姓名";
            worksheet.Cells[1, 3].Value = "电话";
            for (int i = 0; i < 100; i++)
            {
                worksheet.Cells[i + 2, 1].Value = i;
                worksheet.Cells[i + 2, 2].Value = "无名";
                worksheet.Cells[i + 2, 3].Value = 1356551124 + i;
            }
            package.Save();
            Debug.Log("导出Excel成功");
        }
    }




    // Start is called before the first frame update
    void Start()
    {
        CreateExcel();
        Debug.Log("///////////////////////////////////////////");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
