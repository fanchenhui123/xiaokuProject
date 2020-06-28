
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Window : MonoBehaviour
{
    private static bool goon;
    private static bool closePanel;
    public static Window instance;
    public static GameObject panel;
    private void Awake()
    {
        instance = this;

    }


    public static void Skipwindow(string message,UnityAction confiomMe,GameObject o)
    {
        panel = o;
        o.SetActive(true);
        o.transform.Find("message").GetComponent<Text>().text = message;
        o.transform.Find("Confirm").GetComponent<Button>().onClick.AddListener(confiomMe);
        o.transform.Find("Cancel").GetComponent<Button>().onClick.AddListener(close);
    }

    public static void close()
    {
       panel.SetActive(false); 
    }

}
