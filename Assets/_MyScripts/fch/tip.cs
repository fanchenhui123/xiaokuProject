using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tip : MonoBehaviour
{
    public static tip instance;
    private void Awake()
    {
        instance = this;
        Invoke("des",0.2f);
    }

    private void des()
    {
       // gameObject.transform.GetComponent<Image>().color.a-=
        gameObject.SetActive(false);
    }

    public void SetMessae(string mess)
    {
        try
        {
            gameObject.SetActive(true);
            transform.Find("info").GetComponent<Text>().text = mess;
            Invoke("des",1f);
        }
        catch (Exception e)
        {
            Debug.Log(e);
          //  throw;
        }
      
       
       
    }
    public void SetMessae(string mess,float delaytime)
    {
        gameObject.SetActive(true);
        transform.Find("info").GetComponent<Text>().text = mess;
        Invoke("des",delaytime);
    }
}
