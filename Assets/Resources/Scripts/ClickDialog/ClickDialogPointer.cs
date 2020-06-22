using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDialogPointer : MonoBehaviour, IPointerClickHandler
{
    public string text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("ClickDialog");
        FindObjectOfType<ClickDialog>().Show(text);
    }

}
