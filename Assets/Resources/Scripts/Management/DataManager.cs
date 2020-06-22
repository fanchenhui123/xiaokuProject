using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    private static DataManager instance = null;
    private DBManager dbManager;

    public static DataManager getInstance()
    {
        if (instance == null)
            instance = new DataManager();
        return instance;
    }

    public void Init(DBManager dbManager=null)
    {
        Debug.Log("DataManager Init()");
        if (dbManager == null)
        {
            dbManager = DBManager._DBInstance();
            dbManager.OpenConnect();
        }
        this.dbManager = dbManager;
        //用户信息表
        UserInfoInit();
        //商户信息表
        BusinessInit();
        //产品信息表
        ProductInit();
        //品牌信息
        CarInfoInit();
    }

    private void UserInfoInit()
    {
        Debug.Log("生成UserInfo表");
        //id、密码、是否记住、商家名称、商家昵称、关联品牌、用户名、手机号码、ip、token
        dbManager.CreateTable(typeof(UserInfo));
        
    }
    private void BusinessInit()
    {
        Debug.Log("生成BussinessInfo表");
        //商户id、商户昵称、商户名称、商户手机、商户其他
        dbManager.CreateTable(typeof(BussinessInfo));
    }

    private void ProductInit()
    {
        Debug.Log("生成ProductInfo表");
        //序号、排放、车型、指导价、车架号、发车日期、到店日期、库龄、AAK日期、质sun、合格证、客户姓名、销售顾问、otc
        dbManager.CreateTable(typeof(ProductInfo));
        dbManager.CreateTable(typeof(TmpExcelInfo));
        dbManager.CreateTable(typeof(ExcelInfo));
    }

    private void CarInfoInit()
    {
        Debug.Log("生成CarInfo表");
        dbManager.CreateTable(typeof(BrandInfo));
        dbManager.CreateTable(typeof(CartLinesInfo));
        dbManager.CreateTable(typeof(CartModelsInfo));
    }

    //把数据存入数据库中
    public void InsertDb<T>(T item)
    {
        dbManager.Insert<T>(item);
    }
}
