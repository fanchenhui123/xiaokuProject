using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptTexture : MonoBehaviour
{
    public Color color1 = Color.green;
    public Color color2 = Color.blue;

    private float width;
    private float height;

    public RectTransform headPanel;


    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = new GameObject("spriet");
        //width = headPanel.tr
        width = headPanel.rect.width;
        height = headPanel.rect.height;
        Debug.LogFormat("打印高度:{0},打印宽度:{1}", height, width);
        GenerateSprite();
    }

    void GenerateSprite()
    {
        Texture2D t = new Texture2D((int)width, (int)height);
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                float wrate = (float)w / (float)width;
                float hrate = (float)h / (float)height;

                float bezierateVal = GetBezierat(0, 0.5f, 0.5f, 1, hrate);

                float dis = Vector2.Distance(new Vector2(0.5f, 0.5f), new Vector2(wrate, hrate));
                dis = 1 - GetBezierat(0, 0, 1, 2, dis) * 0.5f;

                Color c = new Color();
                c.r = Mathf.Lerp(color1.r, color2.r, bezierateVal) * dis;
                c.g = Mathf.Lerp(color1.g, color2.g, bezierateVal) * dis;
                c.b = Mathf.Lerp(color1.b, color2.b, bezierateVal) * dis;
                c.a = 225;

                t.SetPixel(w, h, c);
            }
        }
        t.Apply();

        Sprite pic = Sprite.Create(t, new Rect(0, 0, (int)width, (int)height), new Vector2(0.5f, 0.5f));

        transform.GetComponent<Image>().sprite = pic;

    }

    float GetBezierat(float a,float b,float c,float d,float t)
    {
        return (Mathf.Pow(1 - t, 3) * a + 3 * t * (Mathf.Pow(1 - t, 2)) * b + 3 * Mathf.Pow(t, 2) * c + Mathf.Pow(t, 3) * d);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
