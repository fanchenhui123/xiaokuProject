using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;

public class SqliteDemo : MonoBehaviour
{

    private DBManager dbManager;

    public void Start()
    {
        dbManager = DBManager._DBInstance();
        dbManager.OpenConnect();

        string ipv4 = IPManager.GetIP(ADDRESSFAM.IPv4);
        string ipv6 = IPManager.GetIP(ADDRESSFAM.IPv6);

        Debug.Log("ipv4:" + ipv4 + "ipv6:" + ipv6);

        StartCoroutine(GetPublicIp());

        //SqliteDataReader reader = dbManager.SelectFullTableData("userInfo");
        //if (reader.Read())
        //{
        //    Debug.Log("有数据");
        //}
        //else
        //{
        //    Debug.Log("无数据");
        //}
        //判断是否第一次打开
            //第一次打开创建数据表
            //如果不是第一次打开，从用户账号信息表中获取是否第一次登陆

    }

    private IEnumerator GetPublicIp()
    {
        WWW w = new WWW(@"http://icanhazip.com/");
        yield return w;
        if (w.text.Equals(""))
            Debug.Log("无外网状态");
        else
            Debug.Log("打印公网ip" + w.text);
    }

}
