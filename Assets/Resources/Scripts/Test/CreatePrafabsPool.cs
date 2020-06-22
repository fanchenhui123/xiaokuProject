using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatePrafabsPool : MonoBehaviour
{
    public GameObject PanelParent;
    public GameObject Cow;      //容器  内部有多个输入内容
    public GameObject Row;      //输入的内容

    private GameObject TableLine;

    private float Width;

    // Start is called before the first frame update
    void Start()
    {
        //初始化Panel宽度
        Vector2 sizePanel = PanelParent.GetComponent<RectTransform>().sizeDelta;
        Vector3 vc3 = Cow.GetComponent<GridLayoutGroup>().cellSize;
        Width = 50 * vc3.x;
        sizePanel = new Vector2(Width, sizePanel.y);
        PanelParent.GetComponent<RectTransform>().sizeDelta = sizePanel;

        //先生成line
        TableLine = GameObject.Instantiate(Cow);

        //line下生成Row
        for (int i = 0; i < 50; i++)
        {
            GameObject obj = GameObject.Instantiate(Row);
            obj.name = "row" + i;
            obj.transform.SetParent(TableLine.transform);
        }
        //创建十行
        ChangeTable(100);
    }

    private void ChangeTable(int line)
    {
        //设置第一行内容，输入功能关闭
        GameObject temp = GameObject.Instantiate(TableLine);
        temp.name = "Title";
        var InputArray = temp.GetComponentsInChildren<InputField>();
        for (int i = 0; i < InputArray.Length; i++)
        {
            InputArray[i].interactable = false;
            InputArray[i].GetComponentInChildren<Text>().text = "标题内容" + i;
        }
        temp.transform.SetParent(PanelParent.transform);

        for (int i = 0; i < line; i++)
        {
            GameObject obj = GameObject.Instantiate(TableLine);
            obj.name = "cow" + i;
            obj.transform.SetParent(PanelParent.transform);
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
