using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABLoadManager : ISingleton<ABLoadManager> {

	// Use this for initialization
	void Start () {
		string path = Application.streamingAssetsPath + "/AssetBundles/Cube.ab";
		AssetBundle ab= AssetBundle.LoadFromFile(path);
		GameObject obj = ab.LoadAsset<GameObject>("Cube");
		Instantiate(obj);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
