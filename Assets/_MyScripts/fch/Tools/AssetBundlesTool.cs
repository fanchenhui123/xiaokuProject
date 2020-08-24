﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetBundlesTool : EditorWindow {

    [MenuItem("Plugins/资源包打包工具")]
	static void Init()
    {
        AssetBundlesTool abt = EditorWindow.GetWindow<AssetBundlesTool>();
        abt.Show();
    }

    static string Abpath; //资源路径
    static string ext;//后缀名变量
    static string outPath;//输出路径
    static BuildTarget platform;//平台

    static PackageData data;

    private void OnGUI()
    {
        //UGUI 横向布局 纵向布局

        GUILayout.BeginVertical();//开始纵向布局

        
        GUILayout.Label("打包资源路径" + Abpath);//文本框
   
        if (GUILayout.Button("选择资源路径"))//按钮
        {
            Abpath = EditorUtility.OpenFolderPanel("选择要打包的资源路径", "", "");
        }
  
        GUILayout.Label("资源包放置路径" + outPath);//文本框
        if (GUILayout.Button("选择输出路径"))//按钮
        {
            outPath = EditorUtility.OpenFolderPanel("选择输出路径", "", "");
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("资源包后缀名");//文本框
        ext = GUILayout.TextField(ext);//输入框
        GUILayout.EndHorizontal();

        platform = (BuildTarget)EditorGUILayout.EnumPopup(platform); //针对枚举类型的下拉菜单

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("资源包命名"))//按钮
        {
            SetABNames(Abpath);
        }
        if (GUILayout.Button("资源包打包 "))//按钮
        {
            CreateAB();
        }
        GUILayout.EndHorizontal();

        data = (PackageData)EditorGUILayout.ObjectField(data, typeof(PackageData));


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("保存资源配置"))//按钮
        {
            Save();
        }
        if (GUILayout.Button("读取资源配置"))//按钮
        {
            Load();
        }
        GUILayout.EndHorizontal();
        if(GUILayout.Button("选择设置ab包名")){
           string[] strs = Selection.assetGUIDs;
           
           foreach(var pat in  strs){
               string pre = Application.dataPath.Replace("Assets",AssetDatabase.GUIDToAssetPath(pat));
              SelectSetAb(pre);
           }
        }
        

        GUILayout.EndVertical();
    }

    static void SetABNames(string path)
    {
        ///获取指定绝对路径下方的所有子文件夹路径
        string[] dirpaths = Directory.GetDirectories(path);
        
        foreach (var dirpath in dirpaths)
        {
            SetABNames(dirpath);//dirpath 当前文件夹路径下的所有文件夹路径 不停地递归知道没有文件夹可以查到
        }

        string[] files = Directory.GetFiles(path);//获取当前文件夹下的所有文件绝对路径

        foreach (var fileName in files)
        {
            if (fileName.Contains(".meta"))
                continue;

            ///对当前找到的文件进行资源包命名修改 修改规则基于自己错处的文件夹名 为ab包名 
            ///把资源路径传入AssetImporter对象，然后通过AssetImporter这个工具修改制定资源的资源名
            ///
            string temp = fileName.Replace(@"\", "/");//替换斜杠 规则一致
            temp = "Assets/" + temp.Replace(Application.dataPath + "/", "");
            AssetImporter asset = AssetImporter.GetAtPath(temp);//传入相对路径 以Assets文件夹为根节点
            string assetName = fileName.Replace(@"\", "/");
            assetName = assetName.Replace(Abpath + "/", "");//掐头

            assetName = assetName.Remove(assetName.LastIndexOf("/"));//去尾
            asset.assetBundleName = assetName + ext;//修改资源名 得到资源路径名 + 后缀名

        }
    }

    static void SelectSetAb(string path){
        
        ///获取指定绝对路径下方的所有子文件夹路径
        string[] dirpaths = Directory.GetDirectories(path);
        
        foreach (var dirpath in dirpaths)
        {
            SetABNames(dirpath);//dirpath 当前文件夹路径下的所有文件夹路径 不停地递归知道没有文件夹可以查到
        }

        string[] files = Directory.GetFiles(path);//获取当前文件夹下的所有文件绝对路径

        foreach (var fileName in files)
        {
            if (fileName.Contains(".meta"))
                continue;

            ///对当前找到的文件进行资源包命名修改 修改规则基于自己错处的文件夹名 为ab包名 
            ///把资源路径传入AssetImporter对象，然后通过AssetImporter这个工具修改制定资源的资源名
            ///
            string temp = fileName.Replace(@"\", "/");//替换斜杠 规则一致
            temp = "Assets/" + temp.Replace(Application.dataPath + "/", "");
            AssetImporter asset = AssetImporter.GetAtPath(temp);//传入相对路径 以Assets文件夹为根节点
            string assetName = fileName.Replace(@"\", "/");
            assetName = assetName.Replace(Abpath + "/", "");//掐头

            assetName = assetName.Remove(assetName.LastIndexOf("/"));//去尾
            asset.assetBundleName = assetName + ext;//修改资源名 得到资源路径名 + 后缀名

        }
    }
    static void CreateAB()
    {
        ///创建管线.打资源包
        ///outPath 打到哪
        ///
        BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.None, platform);
    }

    static void Save()
    {
        PackageData pd = CreateInstance<PackageData>();
        pd.Abpath = Abpath;
        pd.outPath = outPath;
        pd.ext = ext;
        pd.platform = platform;
        AssetDatabase.CreateAsset(pd,"Assets/save.asset");
    }

    static void Load() {

        Abpath = data.Abpath;
        outPath = data.outPath;
        ext = data.ext;
        platform = data.platform;
    }


}
