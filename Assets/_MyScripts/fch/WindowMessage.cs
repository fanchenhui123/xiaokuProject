
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Window : MonoBehaviour
{
    private static bool goon;
    private static bool closePanel;
    private void Awake()
    {
        
    }


    public static void Skipwindow(string message,UnityAction confiomMe,GameObject o)
    {
        o.SetActive(true);
        o.transform.Find("message").GetComponent<Text>().text = message;
        o.transform.Find("Confirm").GetComponent<Button>().onClick.AddListener(confiomMe);
        o.transform.Find("Cancel").GetComponent<Button>().onClick.AddListener(()=>o.SetActive(false));
    }

   
}
