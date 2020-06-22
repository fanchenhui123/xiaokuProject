using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputChange : MonoBehaviour
{
    public InputField input;

    // Start is called before the first frame update
    void Start()
    {
        input.onValueChanged.AddListener(delegate
        {
            Debug.Log("参数发生改变,当前参数为:" + input.text);

        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeDemo(int i, int j)
    {

    }
}
