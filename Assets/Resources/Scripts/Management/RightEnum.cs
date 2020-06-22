using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Xiaoku;

public class RightEnum : MonoBehaviour
{

    private bool menuState = false;
    private Vector2 oldCursonPos;

    public Func<GameObject> callback;

    private string content;
    
    public GameObject rightEnumObj;
    public Transform enumNode;

    public Button copyBtn;
    public Button pasteBtn;
    public Button boldBtn;
    public Button normalBtn;
    public Button italicBtn;
    public Button colorSelectBtn;
    public Button searchBtn;

    public SearchManager searchManager;



    public ColorManager colorManager;

    private Canvas canvas;

    private bool isClick = false;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("鼠标点击右键");
            menuState = true;
            oldCursonPos = Input.mousePosition;
            RightMenu();
        }
        RightMenu();
    }

    void RightMenu()
    {
        if (menuState && callback!=null)
        {
            Debug.Log("执行RightMenu方法");
            rightEnumObj.transform.SetParent(canvas.transform);
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out pos);
            pos.x += 75;
            pos.y -= 25;
            rightEnumObj.GetComponent<RectTransform>().anchoredPosition = pos;
            isClick = false;
            copyBtn.onClick.AddListener(delegate
            {
                if (isClick) return;
                isClick = true;
                Debug.Log("点击copyBtn");
                rightEnumObj.transform.SetParent(enumNode);
                UnityEngine.GUIUtility.systemCopyBuffer = callback().GetComponent<InputField>().text;
                callback = null;
                copyBtn.onClick.RemoveAllListeners();
            });
            pasteBtn.onClick.AddListener(delegate
            {
                if (isClick) return;
                isClick = true;
                Debug.Log("点击pasteBtn");
                rightEnumObj.transform.SetParent(enumNode);
                var obj = callback();
                obj.GetComponent<InputField>().text = UnityEngine.GUIUtility.systemCopyBuffer;
                var id = obj.GetComponentInParent<RowCellView>().id;
                FindObjectOfType<Controller>().SetData(id, UnityEngine.GUIUtility.systemCopyBuffer);
                callback = null;
                pasteBtn.onClick.RemoveAllListeners();
            });
            boldBtn.onClick.AddListener(delegate {
                if (isClick) return;
                isClick = true;
                Debug.Log("点击字体加粗");
                rightEnumObj.transform.SetParent(enumNode);
                callback().transform.Find("Text").GetComponent<Text>().fontStyle = FontStyle.Bold;
                callback = null;
                boldBtn.onClick.RemoveAllListeners();
            });
            normalBtn.onClick.AddListener(delegate {
                if (isClick) return;
                isClick = true;
                Debug.Log("点击字体正常");
                rightEnumObj.transform.SetParent(enumNode);
                callback().transform.Find("Text").GetComponent<Text>().fontStyle = FontStyle.Normal;
                callback = null;
                normalBtn.onClick.RemoveAllListeners();
            });
            italicBtn.onClick.AddListener(delegate {
                if (isClick) return;
                isClick = true;
                Debug.Log("点击字体正常");
                rightEnumObj.transform.SetParent(enumNode);
                callback().transform.Find("Text").GetComponent<Text>().fontStyle = FontStyle.Italic;
                callback = null;
                italicBtn.onClick.RemoveAllListeners();
            });
            colorSelectBtn.onClick.AddListener(delegate
            {
                if (isClick) return;
                isClick = true;
                Debug.Log("调出颜色面板");
                rightEnumObj.transform.SetParent(enumNode);
                colorManager.transform.SetParent(canvas.transform);
                //colorManager.gameObject.SetActive(true);
                //colorManager.SetRelationInputFile(callback().GetComponent<InputField>());
                colorManager.relationInput = callback().GetComponent<InputField>();
                callback = null;
                colorSelectBtn.onClick.RemoveAllListeners();
            });
            searchBtn.onClick.AddListener(delegate
            {
                if (isClick) return;
                isClick = true;
                rightEnumObj.transform.SetParent(enumNode);
                searchManager.Show();
                //调出查找窗口,查找窗口按钮事件初始化的时候就加上
                callback = null;
                colorSelectBtn.onClick.RemoveAllListeners();
            });
            menuState = false;
            //if()


            //if (GUI.Button(new Rect(oldCursonPos.x+10, Screen.height - oldCursonPos.y, 100, 20), "复制"))
            //{
            //    content = callback().GetComponent<InputField>().text;
            //    Debug.Log("复制");
            //    menuState = false;
            //    callback = null;
            //    isWork = false;
            //}
            //if (GUI.Button(new Rect(oldCursonPos.x+10, Screen.height - oldCursonPos.y + 20, 100, 20), "粘贴"))
            //{
            //    Debug.Log("粘贴");
            //    callback().GetComponent<InputField>().text = content;
            //    menuState = false;
            //    callback = null;
            //    isWork = false;
            //}
        }
    }

}
