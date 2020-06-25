using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.International.Converters.PinYinConverter;
using UnityEngine;

public static class PinYinHelper    {

    public static string ToPinYin(string txt)
    {
        txt = txt.Trim();
        byte[] arr = new byte[2]; //每个汉字为2字节
        StringBuilder result = new StringBuilder(); //使用StringBuilder优化字符串连接            
        char[] arrChar = txt.ToCharArray();
        foreach (char c in arrChar)
        {
            arr = System.Text.Encoding.Default.GetBytes(c.ToString()); //根据系统默认编码得到字节码
            if (arr.Length == 1) //如果只有1字节说明该字符不是汉字
            {
                result.Append(c.ToString());
                continue;
            }

            ChineseChar chineseChar = new ChineseChar(c);
            result.Append(chineseChar.Pinyins[0].Substring(0, chineseChar.Pinyins[0].Length - 1).ToLower());
            //result.Append(" ");
        }
        return result.ToString();
    }
    
}
 

