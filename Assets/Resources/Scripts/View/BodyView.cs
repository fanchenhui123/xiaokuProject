using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyView : MonoBehaviour
{
    private GameObject info;
    private Transform bussionNode;
    private Transform permissNode;
    private DBManager dbManager;

    private InputField name;
    private InputField nickname;
    private InputField relation;
    private InputField uersname;
    private InputField phone;
    private InputField password;

    private void Awake()
    {
        Debug.Log("Awake");
        dbManager = DBManager._DBInstance();
        info = Resources.Load<GameObject>("Prefabs/info") as GameObject;
        bussionNode = this.transform.Find("LeftPanel/Contant/Contant").transform;
        permissNode = this.transform.Find("LeftPanel/Contant2").transform;
        BussionInit();
        PermissInit();
        Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");

    }

    //初始化userInfo信息
    public void Init()
    {
        //Show();
        UserInfo userInfo = dbManager.QueryTable<UserInfo>()[0];
        name.text = userInfo.name;
        nickname.text = userInfo.nickname;
        relation.text = userInfo.relation;
        uersname.text = userInfo.username;
        phone.text = userInfo.phone;
        password.text = userInfo.psd;
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private void BussionInit()
    {
        bussionNode.DetachChildren();
        name = AddObjInParent(bussionNode, info, "商品名称", false);
        nickname = AddObjInParent(bussionNode, info, "商家昵称", true);
        relation = AddObjInParent(bussionNode, info, "关联品牌", false);
        uersname = AddObjInParent(bussionNode, info, "用户名", false);
        phone = AddObjInParent(bussionNode, info, "手机号码", false);
        password = AddObjInParent(bussionNode, info, "登录密码", true, true);

    }

    private void PermissInit()
    {
        var toggle = permissNode.Find("Contant/Panel/Toggle").GetComponent<Toggle>();
        toggle.isOn = false;
        toggle.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private InputField AddObjInParent(Transform parent,GameObject child,string name,bool canChange,bool isPassword=false)
    {
        var newChild = GameObject.Instantiate(child, parent.transform);
        newChild.transform.SetParent(parent.transform);
        newChild.transform.Find("param").GetComponent<Text>().text = name;
        var input = newChild.transform.Find("InputField").GetComponent<InputField>();
        input.interactable = canChange;
        if (isPassword)
            input.contentType = InputField.ContentType.Password;
        return input;
    }
}
