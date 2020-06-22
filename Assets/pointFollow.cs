using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointFollow : MonoBehaviour
{
    public GameObject rightEnum;
    private Canvas canvas;


    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out pos);
        pos.x += 75;
        pos.y -= 25f;
        transform.GetComponent<RectTransform>().anchoredPosition = pos;
    }
}
