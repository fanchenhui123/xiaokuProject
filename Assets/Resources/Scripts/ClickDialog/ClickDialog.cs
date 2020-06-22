using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickDialog : MonoBehaviour
{
    public GameObject dialogObj;
    public Button closeBtn;
    public InputField inputField;
    public Text t;
    private void Awake()
    {
        dialogObj.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        closeBtn.onClick.AddListener(Close);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(string text)
    {
        dialogObj.gameObject.SetActive(true);
        inputField.text = text;
        t.text = text;
    }

    public void Close()
    {
        dialogObj.gameObject.SetActive(false);
    }
}
