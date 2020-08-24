using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;



public class UtiltyTool : EditorWindow
{

    [MenuItem("Plugins/小工具")]
    static void Init()
    {
        UtiltyTool abt = EditorWindow.GetWindow<UtiltyTool>();
        abt.Show();
    }

    static string effectPath;
    static string audioPath;

    private void OnGUI()
    {
        //UGUI 横向布局 纵向布局

        GUILayout.BeginVertical();//开始纵向布局


        if (GUILayout.Button("快速添加MeshColider"))//按钮
        {
            AddBoxColider();
        }
        if (GUILayout.Button("快速添加MeshColider OpenConvex"))//按钮
        {
            AddMeshColiderWithConvex();
        }
        if (GUILayout.Button("快速添加MeshColider OpenConvex OpenTrigger"))//按钮
        {
            AddMeshColiderWithTrigger();
        }
        if (GUILayout.Button("快速查找指定对象删除BoxColider组件"))//按钮
        {
            RemoveBoxColider();
        }
        if (GUILayout.Button("修改lua文件后缀为lua.txt"))
        {
            ReNameText();
        }

        effectPath = (GUILayout.TextField(effectPath));
       

        if(GUILayout.Button("转移特效至/RPGProject/ABAssets/Resources/Effects/" + effectPath)){
            MoveEffect();
        }

         audioPath = (GUILayout.TextField(audioPath));
        if(GUILayout.Button("音效特效至/RPGProject/ABAssets/Resources/Audios/"+audioPath)){
            MoveAudio();
        }

        GUILayout.EndVertical();//结束纵向布局
    }

    static void MoveEffect(){
      Object[] gameObjects=  Selection.objects;
      foreach(var data in gameObjects){
           Debug.Log(AssetDatabase.GetAssetPath(data));
           string path = AssetDatabase.GetAssetPath(data);
          AssetDatabase.MoveAsset(path,"Assets/RPGProject/ABAssets/Resources/Effects/" + effectPath +  "/" + path.Substring(path.LastIndexOf("/")));
      }
        AssetDatabase.Refresh();
    }
    static void MoveAudio(){
      Object[] gameObjects=  Selection.objects;
      
      foreach(var data in gameObjects){
           Debug.Log(AssetDatabase.GetAssetPath(data));
           string path = AssetDatabase.GetAssetPath(data);
          AssetDatabase.MoveAsset(path,"Assets/RPGProject/ABAssets/Resources/Audios/" + audioPath +  "/"+ path.Substring(path.LastIndexOf("/")));
      }

      AssetDatabase.Refresh();
    }


    static void AddBoxColider()
    {
        GameObject[] gos = Selection.GetFiltered<GameObject>(SelectionMode.Deep);
        MeshCollider mc;
        foreach (var data in gos)
        {
            mc = data.GetComponent<MeshCollider>();
            if (mc)
            {
                mc.convex = false;
                mc.isTrigger = false;
            }
            else
            {
                mc = data.AddComponent<MeshCollider>();
                mc.convex = false;
                mc.isTrigger = false;
            }
        }
    }

    static void AddMeshColiderWithConvex()
    {
        GameObject[] gos = Selection.GetFiltered<GameObject>(SelectionMode.Deep);
        MeshCollider mc;
        foreach (var data in gos)
        {
            mc = data.GetComponent<MeshCollider>();
            if (mc)
                mc.convex = true;
            else
            {
                mc = data.AddComponent<MeshCollider>();
                mc.convex = true;
            }
        }
    }

    static void AddMeshColiderWithTrigger()
    {
        GameObject[] gos = Selection.GetFiltered<GameObject>(SelectionMode.Deep);
        MeshCollider mc;
        foreach (var data in gos)
        {
            mc = data.GetComponent<MeshCollider>();
            if (mc)
            {
                mc.convex = true;
                mc.isTrigger = true;
            }
            else
            {
                mc = data.AddComponent<MeshCollider>();
                mc.convex = true;
                mc.isTrigger = true;
            }
        }
    }

    static void RemoveBoxColider()
    {
        GameObject[] gos = Selection.GetFiltered<GameObject>(SelectionMode.Deep);
        foreach (var data in gos)
        {
            if (data.GetComponent<MeshCollider>())
                DestroyImmediate(data.GetComponent<MeshCollider>());
        }
    }

    static void ReNameText()
    {
        Object[] gos = Selection.GetFiltered<Object>(SelectionMode.Deep);
        foreach (var data in gos)
        {
            string newName = data.name.Replace(".lua", "txt");
            string assetpath = AssetDatabase.GetAssetPath(data);
            AssetDatabase.RenameAsset(assetpath, newName);



        }

        AssetDatabase.Refresh();
    }
private static string _dirName = "";
    /// <summary>
    /// 批量清空所选文件夹下资源的AssetBundleName.
    /// </summary>
    [MenuItem("Tools/Asset Bundle/Reset Asset Bundle Name")]
    static void ResetSelectFolderFileBundleName()
    {
        UnityEngine.Object[] selObj = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Unfiltered);
        foreach (UnityEngine.Object item in selObj)
        {
            string objPath = AssetDatabase.GetAssetPath(item);
            DirectoryInfo dirInfo = new DirectoryInfo(objPath);
            if (dirInfo == null)
            {
                Debug.LogError("******请检查，是否选中了非文件夹对象******");
                return;
            }
             _dirName = null;
 
            string filePath = dirInfo.FullName.Replace('\\', '/');
            filePath = filePath.Replace(Application.dataPath, "Assets");
            AssetImporter ai = AssetImporter.GetAtPath(filePath);
            ai.assetBundleName = _dirName;
 
            SetAssetBundleName(dirInfo);
        }
        AssetDatabase.Refresh();
        Debug.Log("******批量清除AssetBundle名称成功******");
    }

    static void SetAssetBundleName(DirectoryInfo dirInfo)
    {
        FileSystemInfo[] files = dirInfo.GetFileSystemInfos();
        foreach (FileSystemInfo file in files)
        {
            if (file is FileInfo && file.Extension != ".meta" && file.Extension != ".txt")
            {
                string filePath = file.FullName.Replace('\\', '/');
                filePath = filePath.Replace(Application.dataPath, "Assets");
                AssetImporter ai = AssetImporter.GetAtPath(filePath);
                ai.assetBundleName = _dirName;
            }
            else if (file is DirectoryInfo)
            {
                string filePath = file.FullName.Replace('\\', '/');
                filePath = filePath.Replace(Application.dataPath, "Assets");
                AssetImporter ai = AssetImporter.GetAtPath(filePath);
                ai.assetBundleName = _dirName;
                SetAssetBundleName(file as DirectoryInfo);
            }
        }
    }

}
