using System;
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


 ///
    /// UTF8字符串转换为汉字用的类
    /// 转换如"\\u8d35"之类的字符串为对应的汉字
    ///
    class UTF8String 
    {
        string m_strContent = "";

        ///
        /// 构造函数
        ///
        /// 要转换的字符串
        public UTF8String(string content)
        {
            m_strContent = content;
        }

        public string getContent()
        {
            return m_strContent;
        }

        ///
        /// 转换函数
        ///
        /// 返回转换好的字符串
        public string ToString( )
        {
            string reString = null;
            char[] content = m_strContent.ToCharArray(); //把字符串变为字符数组，以进行处理
            for (int i = 0; i < content.Length; i++) //遍历所有字符
            {
                if (content[i] == '\\') //判断是否转义字符 \
                {
                    switch (content[i + 1]) //判断转义字符的下一个字符是什么
                    {
                        case 'u': //转换的是汉字
                        case 'U':
                            reString += HexArrayToChar(content, i + 2); //获取对应的汉字
                            i = i + 5;
                            break;
                        case '/': //转换的是 /
                        case '\\': //转换的是 \
                        case '"':
                            break;
                        default: //其它
                            reString += EscapeCharacter(content[i + 1]); //转为其它类型字符
                            i = i + 1;
                            break;
                    }
                }
                else
                    reString += content[i]; //非转义字符则直接加入
            }
            return reString;
        } 


        ///
        /// 字符数组转对应汉字字符
        ///
        /// 要转换的数字
        /// 起始位置
        /// 对应的汉字
        public char HexArrayToChar(char[] content, int startIndex)
        {
            char[] ac = new char[4];
            for (int i = 0; i < 4; i++) //获取要转换的部分
                ac[i] = content[startIndex + i];
            string num = new string(ac); //字符数组转为字符串
            return HexStringToChar(num);
        }


        ///
        /// 转义字符转换函数
        /// 转换字符为对应的转义字符
        ///
        /// 要转的字符
        /// 对应的转义字符
        private char EscapeCharacter(char c) {
            char rc;
            switch (c)
            {
                case 't':
                    c = '\t';
                    break;
                case 'n':
                    c = '\n';
                    break;
                case 'r':
                    c = '\r';
                    break;
                case '\'':
                    c = '\'';
                    break;
                case '0':
                    c = '\0';
                    break;
            }
            return c;
        }


        ///
        /// 字符串转对应汉字字符
        /// 只能处理如"8d34"之类的数字字符为对应的汉字
        /// 例子："9648" 转为 '陈'
        ///
        /// 转换的字符串
        /// 对应的汉字
        public static char HexStringToChar(string content)
        {
            int num = Convert.ToInt32(content, 16);
            return (char)num;
        }


        ///
        /// 把string转为UTF8String类型
        ///
        ///
        ///
        public static UTF8String ValueOf(string content)
        {
            string reString = null;
            char[] ac = content.ToCharArray();
            int num;
            foreach (char c in ac)
            {
                num = (int)c;
                string n = num.ToString("X2");
                if (n.Length == 4)
                    reString += "\\u" + n;
                else
                    reString += c;
            }
            return new UTF8String(reString);
        }


    }
 

