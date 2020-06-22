using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Test : MonoBehaviour,IPointerDownHandler
{
    private RightEnum rightEnum;

    public void Start()
    {
       rightEnum = GameObject.Find("MainManager").GetComponent<RightEnum>();   
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("鼠标移动到了" + this.name);
        rightEnum.callback = delegate()
        {
            return this.gameObject;
        };
    }

    public void SetWidth(float w)
    {
        var h = gameObject.GetComponent<RectTransform>().sizeDelta.y;
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);
    }
}
