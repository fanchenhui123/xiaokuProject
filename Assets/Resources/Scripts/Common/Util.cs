using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System.Threading;
using System.Collections;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

public class Util
{
    public static void StoreTextureToLocal(Texture2D tx, string dir, string name, bool isAsyn)
    {
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        byte[] pngData = tx.EncodeToPNG();
        if (isAsyn)
        {
            Thread td = new Thread(() => { File.WriteAllBytes(dir + "/" + name, pngData); });
            td.Start();
        }
        else
        {
            File.WriteAllBytes(dir + "/" + name, pngData);
        }
    }

    public static void PrintProperty<T>(object t)
    {
        //获取属性列表
        var p_list = t.GetType().GetProperties();
        string str = "";
        for (int i = 0; i < p_list.Length; i++)
        {
            //获取属性
            var _name = p_list[i].Name;
            //反射获取对应数据需要传递本体object进去
            var _value = p_list[i].GetValue(t);
            str += _name + ":" + _value + ",";            
        }
        Debug.Log(str.Substring(0,str.Length-1));
    }

    public static bool CheckList(object obj)
    {
        return (obj is IList) ? true:false;
    }

    /// <summary>
    /// SHA1 加密, 返回大写字符串
    /// </summary>
    /// <param name="content">需要加密字符串</param>
    /// <returns>返回40位大写字符串</returns>
    public static string SHA1(string content)
    {
        return SHA1(content, Encoding.UTF8);
    }

    /// <summary>
    /// SHA1 加密,返回大写字符串
    /// </summary>
    /// <param name="content">需要加密字符串</param>
    /// <param name="encode">指定加密编码</param>
    /// <returns></returns>
    private static string SHA1(string content,Encoding encode)
    {
        try
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_in = encode.GetBytes(content);
            byte[] bytes_out = sha1.ComputeHash(bytes_in);
            sha1.Dispose();
            string result = BitConverter.ToString(bytes_out);
            result = result.Replace("-", "");
            return result;
        }catch(Exception ex)
        {
            throw new Exception("SHA1加密出错:" + ex.Message);
        }
    }

    public enum ShowWindowCommands : int
    {
        SW_HIDE = 0,
        SW_SHOWNORMAL = 1,    //用最近的大小和位置显示，激活
        SW_NORMAL = 1,
        SW_SHOWMINIMIZED = 2,
        SW_SHOWMAXIMIZED = 3,
        SW_MAXIMIZE = 3,
        SW_SHOWNOACTIVATE = 4,
        SW_SHOW = 5,
        SW_MINIMIZE = 6,
        SW_SHOWMINNOACTIVE = 7,
        SW_SHOWNA = 8,
        SW_RESTORE = 9,
        SW_SHOWDEFAULT = 10,
        SW_MAX = 10
    }
    [DllImport("shell32.dll")]
    public static extern IntPtr ShellExecute(
        IntPtr hwnd,
        string lpszOp,
        string lpszFile,
        string lpszParams,
        string lpszDir,
        ShowWindowCommands FsShowCmd
        );


    public static string get_uft8(string unicodeString)
    {
        UTF8Encoding utf8 = new UTF8Encoding();
        Byte[] encodedBytes = utf8.GetBytes(unicodeString);
        String decodedString = utf8.GetString(encodedBytes);
        return decodedString;
    }

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    public const int SW_SHOWMINIMIZED = 2; //{最小化, 激活}

    public const int SW_SHOWMAXIMIZED = 3;//最大化

    public const int SW_SHOWRESTORE = 1;//还原
  
}
