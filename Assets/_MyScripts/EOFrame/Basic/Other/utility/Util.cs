using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using UnityEngine.UI;

namespace EOFrame
{
    public static class Util
    {
        /// <summary>
        /// 字符串获取类
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type typen(string typeName)
        {
            Type type = null;
            Assembly[] assemblyArray = AppDomain.CurrentDomain.GetAssemblies();
            int assemblyArrayLength = assemblyArray.Length;
            for (int i = 0; i < assemblyArrayLength; ++i)
            {
                type = assemblyArray[i].GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            for (int i = 0; (i < assemblyArrayLength); ++i)
            {
                Type[] typeArray = assemblyArray[i].GetTypes();
                int typeArrayLength = typeArray.Length;
                for (int j = 0; j < typeArrayLength; ++j)
                {
                    if (typeArray[j].Name.Equals(typeName))
                    {
                        return typeArray[j];
                    }
                }
            }
            return type;
        }

        public static string DataPath;
        private static List<string> luaPaths = new List<string>();

        public static int Int(object o)
        {
            return Convert.ToInt32(o);
        }

        public static float Float(object o)
        {
            return (float)Math.Round(Convert.ToSingle(o), 2);
        }

        public static long Long(object o)
        {
            return Convert.ToInt64(o);
        }

        public static int Random(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static float Random(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static string Uid(string uid)
        {
            int position = uid.LastIndexOf('_');
            return uid.Remove(0, position + 1);
        }

        public static long GetTime()
        {
            TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
            return (long)ts.TotalMilliseconds;
        }

        /// <summary>
        /// 查找子物体
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static Transform FindChild(Transform parent, string childName)
        {
            Transform child = parent.Find(childName);

            if (child != null)
                return child;

            for (int i = 0; i < parent.childCount; i++)
            {
                child = FindChild(parent.GetChild(i), childName);

                if (child != null)
                    return child;

            }
            return null;

        }
        /// <summary>
        /// 查找子物体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static T FindChild<T>(Transform parent, string childName)
        {
            Transform child = FindChild(parent, childName);

            if (null != child)
                return child.GetComponent<T>();
            else
                return default(T);
        }

        /// <summary>
        /// 查找子节点对象
        /// 内部使用“递归算法”
        /// </summary>
        /// <param name="goParent">父对象</param>
        /// <param name="chiildName">查找的子对象名称</param>
        /// <returns></returns>
        public static Transform FindTheChildNode(GameObject goParent, string chiildName)
        {
            Transform searchTrans = null;                   //查找结果

            searchTrans = goParent.transform.Find(chiildName);
            if (searchTrans == null)
            {
                foreach (Transform trans in goParent.transform)
                {
                    searchTrans = FindTheChildNode(trans.gameObject, chiildName);
                    if (searchTrans != null)
                    {
                        return searchTrans;

                    }
                }
            }
            return searchTrans;
        }

        /// <summary>
        /// 获取子节点（对象）脚本
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="goParent">父对象</param>
        /// <param name="childName">子对象名称</param>
        /// <returns></returns>
        public static T GetTheChildNodeComponetScripts<T>(GameObject goParent, string childName) where T : Component
        {
            Transform searchTranformNode = null;            //查找特定子节点

            searchTranformNode = FindTheChildNode(goParent, childName);
            if (searchTranformNode != null)
            {
                return searchTranformNode.gameObject.GetComponent<T>();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 给子节点添加脚本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="goParent">父对象</param>
        /// <param name="childName">子对象名称</param>
        /// <returns></returns>
        public static T AddChildNodeCompnent<T>(GameObject goParent, string childName) where T : Component
        {
            Transform searchTranform = null;                //查找特定节点结果

            //查找特定子节点
            searchTranform = FindTheChildNode(goParent, childName);
            //如果查找成功，则考虑如果已经有相同的脚本了，则先删除，否则直接添加。
            if (searchTranform != null)
            {
                //如果已经有相同的脚本了，则先删除
                T[] componentScriptsArray = searchTranform.GetComponents<T>();
                for (int i = 0; i < componentScriptsArray.Length; i++)
                {
                    if (componentScriptsArray[i] != null)
                    {
                        GameObject.Destroy(componentScriptsArray[i]);
                    }
                }
                return searchTranform.gameObject.AddComponent<T>();
            }
            else
            {
                return null;
            }
            //如果查找不成功，返回Null.
        }

        /// <summary>
        /// 给子节点添加父对象
        /// </summary>
        /// <param name="parents">父对象的方位</param>
        /// <param name="child">子对象的方法</param>
        public static void AddChildNodeToParentNode(Transform parents, Transform child)
        {
            child.SetParent(parents, false);
            child.localPosition = Vector3.zero;
            child.localScale = Vector3.one;
            child.localEulerAngles = Vector3.zero;
        }

        /// <summary>
        /// 取平级对象
        /// </summary>
        public static GameObject Peer(GameObject go, string subnode)
        {
            return Peer(go.transform, subnode);
        }

        /// <summary>
        /// 取平级对象
        /// </summary>
        public static GameObject Peer(Transform go, string subnode)
        {
            Transform tran = go.parent.Find(subnode);
            if (tran == null) return null;
            return tran.gameObject;
        }

        /// <summary>
        /// 计算字符串的MD5值
        /// </summary>
        public static string md5(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
            byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
            md5.Clear();

            string destString = "";
            for (int i = 0; i < md5Data.Length; i++)
            {
                destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
            }
            destString = destString.PadLeft(32, '0');
            return destString;
        }

        /// <summary>
        /// 计算文件的MD5值
        /// </summary>
        public static string md5file(string file)
        {
            try
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(fs);
                fs.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("md5file() fail, error:" + ex.Message);
            }
        }

        /// <summary>
        /// 清除所有子节点
        /// </summary>
        public static void ClearChild(Transform go)
        {
            if (go == null) return;
            for (int i = go.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(go.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 取得行文本
        /// </summary>
        public static string GetFileText(string path)
        {
            return File.ReadAllText(path);
        }

        /// <summary>
        /// 网络可用
        /// </summary>
        public static bool NetAvailable
        {
            get
            {
                return Application.internetReachability != NetworkReachability.NotReachable;
            }
        }

        /// <summary>
        /// 是否是无线
        /// </summary>
        public static bool IsWifi
        {
            get
            {
                return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
            }
        }

        /// <summary>
        /// 应用程序内容路径
        /// </summary>
        public static string AppContentPath()
        {
            string path = string.Empty;
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    path = "jar:file://" + Application.dataPath + "!/assets/";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    path = Application.dataPath + "/Raw/";
                    break;
                default:
                    //path = Application.dataPath + "/" + AppConst.AssetDir + "/";
                    break;
            }
            return path;
        }

        public static void Log(string str)
        {
            Debug.Log(str);

        }
        public static void LogInWindow(string str)
        {
            Debug.Log(str);
            if (GameObject.Find("WinDebug"))
            {
                GameObject.Find("WinDebug").GetComponentInChildren<Text>().text += str + "\n";
            }

        }
        public static void LogWarning(string str)
        {
            Debug.LogWarning(str);
        }

        public static void LogError(string str)
        {
            Debug.LogError(str);
        }

        #region UI

        /// <summary>
        /// 创建ScrollView Item
        /// </summary>
        /// <typeparam name="T">item类型</typeparam>
        /// <param name="prefab">item预制体</param>
        /// <param name="datas">数据列表</param>
        /// <param name="items">item列表</param>
        /// <param name="content">item父对象</param>
        public static void CreateScrollViewItems<T>(GameObject prefab, List<T> items, List<object> datas, Transform content) where T : Item
        {
            for (int i = 0; i < datas.Count; i++)
            {
                T item = GameObject.Instantiate(prefab, content).GetComponent<T>();
                item.gameObject.SetActive(true);
                items.Add(item);
                //初始化
                item.InitItem(datas[i]);
            }
        }
        #endregion
    }
}
