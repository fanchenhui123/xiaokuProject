using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DelayPanelManager : MonoBehaviour
{
    public GameObject DelayPanel;

    private GameObject tmpObj;
    private GameObject parent;

    private bool isPlaying = false;

    public void Awake()
    {
        parent = GameObject.Find("Canvas");
        StartCoroutine(PrefabsInit());   
    }

    public void Load()
    {
        if(tmpObj == null)tmpObj = GameObject.Instantiate(DelayPanel,parent.transform);
        tmpObj.transform.SetParent(parent.transform);
        StartCoroutine(ImageAction());

    }

    public void SetText(string text)
    {
        if (tmpObj)
        {
            var t = tmpObj.transform.Find("Text").GetComponent<Text>();
            t.text = text;
        }
    }

    public void Destory()
    {
        SetText("");
        isPlaying = false;
    }


    private IEnumerator ImageAction()
    {
        tmpObj.SetActive(true);
        var img = tmpObj.transform.Find("Image");
        var rotate = img.transform.localEulerAngles;
        isPlaying = true;
        while (isPlaying)
        {
            yield return new WaitForFixedUpdate();
            rotate.z += 5f;
            img.localEulerAngles = rotate;
        }
        tmpObj.SetActive(false);
        //Destroy(tmpObj);
        //tmpObj = null;
    }


    private IEnumerator PrefabsInit()
    {
        DelayPanel = Resources.Load<GameObject>("Prefabs/DelayPanel") as GameObject;
        yield return new WaitForSeconds(0.1f);
        Debug.Log("初始化DelayPanel成功");
    }
}
