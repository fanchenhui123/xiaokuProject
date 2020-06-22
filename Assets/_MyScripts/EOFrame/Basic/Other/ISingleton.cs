using UnityEngine;
using System.Collections;

public class ISingleton<T> :MonoBehaviour where T :ISingleton<T>
{
    
    protected static T _instance = null;

    protected static Transform _transform = null;
    protected ISingleton() { }
     public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject allSingleton= GameObject.Find("All_Singletons");

                if (allSingleton == null) {

                    allSingleton = new GameObject("All_Singletons");

                }
                GameObject go = new GameObject("_" + typeof(T).Name);

                go.AddComponent<T>();

                _instance = go.GetComponent<T>();

                _transform=go.transform;

                _transform.parent = allSingleton.transform;

            }
            return _instance;
        }
    }

    protected void SetParent(Transform trans)
    {
        _transform.parent = trans;
    }
    protected void Init(Transform transParent = null)
    {
        if(transParent != null)
        SetParent(transParent);
    }
    /// <summary>
    /// 该方法作用：若该单例类的子类已在GameObject下作为组件存在，并且子类没有重载Awake()方法，则自动获取自身赋予_instance
    /// 注意：若子类重载了Awake()方法则需手动在子类的Awake添加一行代码“_instance = this.GetComponent<T>();”
    /// </summary>
    void Awake()
    {
        _instance = this.GetComponent<T>();
    }
    void OnDestroy()
    {
        _instance = null;
    }
}
