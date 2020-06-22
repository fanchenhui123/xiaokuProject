using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateAssetBundles : MonoBehaviour
{
    [MenuItem("AssetBundles/Build AssetBundles")] //特性
    static void BuildAssetBundle()
    {
        string dir = Application.persistentDataPath+"/AssetBundles"; //相对路径
        if (!Directory.Exists(dir))   //判断是否存在
        {
            Directory.CreateDirectory(dir);
        }
        print(dir);
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }

}