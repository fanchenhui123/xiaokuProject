/*
页面UI管理

 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public HeadView headView;

    public ClickDialog cd;

    // Start is called before the first frame update
    void Start()
    {
        headView.currentPanelName.text = "关联商户";
    }

    public void Init()
    {
        headView.gameObject.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
