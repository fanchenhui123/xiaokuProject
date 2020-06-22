using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanelManager : MonoBehaviour
{
    private GameObject messagePanel;

    private Text message;

    private GameObject tmpObj;
    private GameObject parent;

    private void Awake()
    {
        parent = GameObject.Find("Canvas");
        StartCoroutine(PrefabsInit());
    }

    public void Show(string info = "")
    {
        if (tmpObj == null)
        {
            tmpObj = GameObject.Instantiate(messagePanel, parent.transform);
            tmpObj.transform.SetParent(parent.transform);
            message = tmpObj.transform.Find("info").GetComponent<Text>();
        }
        tmpObj.SetActive(true);
        message.text = info;
        StartCoroutine(DelayHide());
    }

    public void Hide()
    {
        if (tmpObj != null)
            tmpObj.SetActive(false);
    }

    private IEnumerator PrefabsInit()
    {
        messagePanel = Resources.Load<GameObject>("Prefabs/MessagePanel") as GameObject;
        yield return new WaitForFixedUpdate();
        var rect = messagePanel.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(1920, 1080);
        Debug.Log("初始化MessagePanel成功");
    }

    private IEnumerator DelayHide()
    {
        yield return new WaitForSeconds(1f);
        Hide();
    }
}
