using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecondPsdManager : MonoBehaviour
{
    public InputField psdInput;

    public Button okBtn;
    public Button cancelBtn;

    public MessagePanelManager messagePanelManager;

    public Action callBack;


    public void Awake()
    {
        var rectT = transform.GetComponent<RectTransform>();
        rectT.sizeDelta = new Vector2(1920, 1080);
    }

    // Start is called before the first frame update
    void Start()
    {
        okBtn.onClick.AddListener(CheckPsd);
        cancelBtn.onClick.AddListener(delegate
        {
            gameObject.SetActive(false);
        });
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void CheckPsd()
    {
        if(psdInput.text.Contains("1"))
        {
            gameObject.SetActive(false);
            messagePanelManager.Show("验证成功,正在保存文件");
            if (callBack != null)
            {
                callBack();
                callBack = null;
            }
        }
        else
        {
            messagePanelManager.Show("密码输入错误,请重新输入");
            psdInput.text = "";
        }
    }

}
