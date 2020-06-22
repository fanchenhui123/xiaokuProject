using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour {

	public virtual void Awake()
	{

	}
	public virtual void Start()
	{
		RegisterEvent();
	}

	/// <summary>
	/// 初始化Item
	/// </summary>
	/// <param name="data"></param>
	public virtual void InitItem(object data) { 

	}
	public virtual void RegisterEvent()
	{
		Button[] btnArr = GetComponentsInChildren<Button>(true);
		for (int i = 0; i < btnArr.Length; i++)
		{
			EventTriggerListener.Get(btnArr[i].gameObject).onClick += OnBtnClick;
		}
	}
	public virtual void OnBtnClick(GameObject go)
	{

	}
	
}
