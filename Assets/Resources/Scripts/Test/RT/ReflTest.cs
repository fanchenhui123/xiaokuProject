using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ReflTest : MonoBehaviour
{
    private DBManager dbManager;

    // Start is called before the first frame update
    void Start()
    {
        dbManager = DBManager._DBInstance();
        dbManager.OpenConnect();
        List<UserInfo> listUserInfo = dbManager.QueryTable<UserInfo>();
        for (int i = 0; i < listUserInfo.Count; i++)
        {
            Util.PrintProperty<UserInfo>(listUserInfo[i]);
            Util.CheckList(listUserInfo[i]);
            listUserInfo[i].id++;
            listUserInfo[i].name = "张三";
            listUserInfo[i].nickname = "三张";
        }
        Debug.Log("/////////////////");
        Util.CheckList(listUserInfo);
        //dbManager.Insert<UserInfo>(listUserInfo);
        //dbManager.Delete<UserInfo>(listUserInfo);
        dbManager.Insert<UserInfo>(listUserInfo);
        dbManager.Update<UserInfo>(listUserInfo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public T CreateTest<T>()where T : new()
    //{
    //    var tmp = new T();
    //    Type type = typeof(T);

    //    //类名
    //    string className = type.Name;
    //    Debug.Log("打印类名:" + className);
    //    //类型所有属性
    //    PropertyInfo[] p_list = type.GetProperties();
    //    for (int i = 0; i < p_list.Length; i++)
    //    {
    //        Debug.LogFormat("打印属性:{0}", p_list[i].Name);
    //        switch (p_list[i].Name)
    //        {
    //            case "name":
    //                object vName = Convert.ChangeType("张三", p_list[i].PropertyType);
    //                p_list[i].SetValue(tmp, vName);
    //                break;
    //            case "age":
    //                object vAge = Convert.ChangeType("10", p_list[i].PropertyType);
    //                p_list[i].SetValue(tmp, vAge);
    //                break;
    //        }
    //    }

    //    return tmp;
    //}


}

public class Person
{
    public string name { get; set; }
    public int age { get; set; }
}
