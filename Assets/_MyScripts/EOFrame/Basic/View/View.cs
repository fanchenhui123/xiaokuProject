
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class View : MonoBehaviour {
	[HideInInspector]
	public string viewName;
	public UIShowType uiShowType=UIShowType.always;
	/// <summary>
	/// 注册消息事件
	/// </summary>
	public abstract void RegisterMsg();
	/// <summary>
	/// 注册UI事件
	/// </summary>
	public virtual void RegisterEvent() {
		Button[] btnArr= GetComponentsInChildren<Button>(true);
		for(int i = 0; i < btnArr.Length; i++)
		{
			EventTriggerListener.Get(btnArr[i].gameObject).onClick += OnBtnClick;
		}
	}
	public virtual void OnBtnClick(GameObject go)
	{

	}

	/// <summary>
	/// 初始化页面
	/// </summary>
	public abstract void InitView();

	protected virtual void Awake()
	{
		if (gameObject.name.Contains("(Clone)"))
		{
			viewName = gameObject.name.Substring(0,gameObject.name.IndexOf("(Clone)") );
		}
		else
		{
			viewName = gameObject.name;
		}
		RegisterMsg();

	}
	// Use this for initialization
	protected virtual void Start () {
		InitView();
		RegisterEvent();
	}

	// Update is called once per frame
	protected virtual void Update () {
		
	}
	
	public virtual void Show(bool isShow)
	{
		if (isShow) { 
			gameObject.SetActive(true);
		}
		else
		{
			gameObject.SetActive(false);
		}
	}
	
}
