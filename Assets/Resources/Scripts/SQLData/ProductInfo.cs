using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SQLTable(Name ="product_info")]
public class ProductInfo
{
    /*
     * 序号、排放、车型、指导价、车架号、发车日期、到店日期、库龄、AAK日期、质sun、合格证、客户姓名、销售顾问、otc
     */

    [SQLField(Name = "id", Type = "integer", AutoIncrement = true, IsNotNull = true, IsPrimaryKey = true, Title = "序号")]
	public int id { get; set; }

    //品牌
    [SQLField(Name = "brand", Type = "text", Title = "品牌")]
    public string brand { get; set; }
    //车系
    [SQLField(Name = "vehicleSystem", Type = "text", Title = "车系")]
    public string vehicleSystem { get; set; }

    //排放
    [SQLField(Name = "discharge", Type = "text",Title ="排放")]
    public string discharge { get; set; }
    //车型
    [SQLField(Name = "carType", Type = "text", Title = "车型")]
    public string carType { get; set; }
    //指导价
    [SQLField(Name = "guidancePrice", Type = "text", Title = "指导价")]
    public string guidancePrice { get; set; }

    //颜色（外/内）
    [SQLField(Name = "color", Type = "text", Title = "颜色")]
    public string color { get; set; }

    //车架号
    [SQLField(Name = "carNumber", Type = "text", Title = "车架号", IsUnique = true)]
    public string carNumber { get; set; }
    //发车日期
    [SQLField(Name = "releaseDate", Type = "text", Title = "发车日期")]
    public string releaseDate { get; set; }
    //到店日期
    [SQLField(Name = "arriveDate", Type = "text", Title = "到店日期")]
    public string arriveDate { get; set; }
    //库龄
    [SQLField(Name = "garageAge", Type = "text", Title = "库龄")]
    public string garageAge { get; set; }

    //批注
    [SQLField(Name = "note", Type = "text", Title = "批注")]
    public string note { get; set; }

    //AAK日期
    [SQLField(Name = "akkDate", Type = "text", Title = "AAK日期")]
    public string akkDate { get; set; }
    //质损
    [SQLField(Name = "qualityloss", Type = "text", Title = "质损")]
    public string qualityloss { get; set; }
    //合格证
    [SQLField(Name = "certificate", Type = "text", Title = "合格证")]
    public string certificate { get; set; }
    //客户姓名
    [SQLField(Name = "userName", Type = "text", Title = "客户姓名")]
    public string userName { get; set; }
    //销售顾问
    [SQLField(Name = "adviser", Type = "text", Title = "销售顾问")]
    public string adviser { get; set; }
    //组别
    [SQLField(Name = "carGroup", Type = "text", Title = "组别")]
    public string carGroup { get; set; }
    //签订日期
    [SQLField(Name = "signDate", Type = "text", Title = "签订日期")]
    public string signDate { get; set; }
    //配车天数
    [SQLField(Name = "useTime", Type = "text", Title = "配车天数")]
    public string useTime { get; set; }
    //付款方式
    [SQLField(Name = "payType", Type = "text", Title = "付款方式")]
    public string payType { get; set; }
    
    //备注
    [SQLField(Name = "memo", Type = "text", Title = "备注")]
    public string memo { get; set; }


}
[SQLTable(Name = "product_info1")]
public class ProductInfo1 : ProductInfo
{

}

[SQLTable(Name = "product_info2")]
public class ProductInfo2 : ProductInfo
{

}

[SQLTable(Name = "product_info3")]
public class ProductInfo3 : ProductInfo
{

}

[SQLTable(Name = "excel_info")]
public class ExcelInfo : ProductInfo
{

}

[SQLTable(Name = "tmp_excel_info")]
public class TmpExcelInfo : ProductInfo
{

}

[SQLTable(Name = "price_info")]
public class PriceInfo : ProductInfo
{

}