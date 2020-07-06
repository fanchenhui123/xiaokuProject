using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;
    private SocketClient socket;
    static readonly object m_lockObject = new object();
    static Queue<KeyValuePair<int, ByteBuffer>> mEvents = new Queue<KeyValuePair<int, ByteBuffer>>();
    private string accessToken = "";
    private string uuid = "";
    private Action reachAbleCallback;
    public string token = "";

    Dictionary<string, Action> reconnectCallbacks = new Dictionary<string, Action>();

    private bool isDoReconnectCheck = false;

    private SocketClient SocketClient
    {
        get
        {
            if (socket == null)
                socket = new SocketClient();
            return socket;
        }
    }
    private void Awake()
    {
        Instance = this;
        Init();
    }
    void Init()
    {
        SocketClient.OnRegister();
    }


    public static void AddEvent(int _event, ByteBuffer data)
    {
        lock (m_lockObject)
        {
            mEvents.Enqueue(new KeyValuePair<int, ByteBuffer>(_event, data));
        }
    }

    public void SetInternetReachAbleCallback(Action callback = null)
    {
        reachAbleCallback = callback;
    }

    public void AddReconnectCallback(GameObject go, Action callback = null)
    {
        if (reconnectCallbacks.ContainsKey(go.name))
            reconnectCallbacks.Remove(go.name);
        reconnectCallbacks.Add(go.name, callback);
    }

    public void ClearReconnectCallback()
    {
        reconnectCallbacks.Clear();
    }

    public void SendConnect()
    {
        SocketClient.SendConnect();
    }

    public void SendMessage(ByteBuffer buffer)
    {
        SocketClient.SendMessage(buffer);
    }

    new void OnDestory()
    {
        SocketClient.OnRemove();
    }

    #region
    public void DoGet(string url, Action<long, string> callback = null, string accessToken = null,
        UnityAction<long, string> ufunc = null)
    {
        StartCoroutine(GetCo(url, callback, accessToken, ufunc));
    }


    IEnumerator GetCo(string url, Action<long, string> callback = null, string accessToken = null,
        UnityAction<long, string> ufunc = null)
    {
        var request = UnityWebRequest.Get(url);
        if (accessToken != null)
            request.SetRequestHeader(AppConst.AccessToken, accessToken);
        request.timeout = 5;
        yield return request.SendWebRequest();
        callback?.Invoke(request.responseCode, request.downloadHandler.text);
        ufunc?.Invoke(request.responseCode, request.downloadHandler.text);
        if (request.responseCode == 200)
            Debug.LogFormat("response:{0}", request.downloadHandler.text);
        request.Dispose();
    }


    public void DoPost(string url, WWWForm form, Action<string, string> callback = null, string accessToken = null)
    {
        StartCoroutine(PostCo(url, form, callback, accessToken));
    }


    IEnumerator PostCo(string url, WWWForm form, Action<string, string> callback = null, string accessToken = null)
    {
        var request = UnityWebRequest.Post(url, form);
        if (accessToken != null)
            request.SetRequestHeader("Authorization", accessToken);
        request.timeout = 5;
        yield return request.SendWebRequest();
        callback?.Invoke(request.responseCode.ToString(), request.downloadHandler.text);
        if (request.responseCode == 200)
            Debug.LogFormat("response:{0}", request.downloadHandler.text);
        request.Dispose();
    }


    public void DoPostJson(string url, string form, Action<string, string, string> callback = null,
        string accessToken = null, string memo = null)
    {
        StartCoroutine(PostCoBody(url, form, callback, accessToken, memo));
    }


    IEnumerator PostCoBody(string url, string form, Action<string, string, string> callback = null,
        string accessToken = null, string memo = null)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(form);
        var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        if (accessToken != null)
            request.SetRequestHeader(AppConst.AccessToken, accessToken);
        request.timeout = 5;
        yield return request.SendWebRequest();
        callback?.Invoke(request.responseCode.ToString(), request.downloadHandler.text, memo);
        if (request.responseCode == 200)
            Debug.LogFormat("DoPostJson response:{0}", request.downloadHandler.text);
        request.Dispose();
    }


    public void DownloadTexture(string url, string name, string dir, Action<Texture2D> callback = null)
    {
        StartCoroutine(DownloadTextureCo(url, name, dir, callback));
    }


    IEnumerator DownloadTextureCo(string url, string name, string dir, Action<Texture2D> callback)
    {
        using (var request = UnityWebRequest.Get(url))
        {
            request.timeout = 5;
            yield return request.SendWebRequest();

            int width = 800;
            int height = 600;
            byte[] results = request.downloadHandler.data;
            Texture2D texture = new Texture2D(width, height);
            if (results.Length > 0)
            {
                texture.LoadImage(results);
                //图片本地保存
                Util.StoreTextureToLocal(texture, dir, name, true);
            }
            Debug.LogFormat("DownloadTextur Status Code:{0}", request.responseCode);
            callback?.Invoke(texture);
            Resources.UnloadUnusedAssets();
        }
    }


    public void SendHeartbeat(float minute, string uuid = null, string accessTocken = null)
    {
        this.accessToken = accessTocken;
        this.uuid = uuid;
        InvokeRepeating("DoSendHeartbeat", 1f, minute * 60f);
    }

    //上传文件
    public void DoUploadFile(string url, string path, Action<string, string> callback = null, string accessToken = null)
    {
        StartCoroutine(uploadFile(url, path, callback, accessToken));
    }


    IEnumerator uploadFile(string url, string path, Action<string, string> callback = null, string accessToken = null)
    {
        UnityWebRequest file;
        WWWForm form = new WWWForm();

        file = UnityWebRequest.Get(path);
        yield return file.SendWebRequest();
        form.AddBinaryData("file", file.downloadHandler.data, Path.GetFileName(path));
        Debug.Log(path + " " + Path.GetFileName(path));
        //UnityWebRequest req = UnityWebRequest.Post("https://www.xiaokucc.cn/merchant/merchant/upload.html", form);
        UnityWebRequest req = UnityWebRequest.Post(url, form);
        if (accessToken != null)
            req.SetRequestHeader(AppConst.AccessToken, accessToken);
        yield return req.SendWebRequest();

        Debug.LogFormat("DoPostJson response:{0}", req.downloadHandler.text);

        if (req.isHttpError || req.isNetworkError)
            Debug.Log(req.error);
        else
            Debug.Log("Uploaded " + file + " files Successfully");
        if (callback != null) callback(req.responseCode.ToString(), req.downloadHandler.text);
    }
    #endregion

    public void DoGet1(string url, Action<long, string> callback = null, string accessToken = null,
        UnityAction<long, string> ufunc = null)
    {
        StartCoroutine(GetCo1(url, callback, accessToken, ufunc));
    }


    IEnumerator GetCo1(string url, Action<long, string> callback = null, string accessToken = null,
        UnityAction<long, string> ufunc = null)
    {
        var request = UnityWebRequest.Get(url);
       // request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
       // request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Authorization", accessToken);
       // if (accessToken != null)
           
        request.timeout = 5;
        yield return request.SendWebRequest();
        
        //Debug.Log("____url:" + url);
        //Debug.Log("____request Accept:" + request.GetRequestHeader("Accept"));
        Debug.Log("____request Authorization:" + request.GetRequestHeader("Authorization"));
        callback?.Invoke(request.responseCode, request.downloadHandler.text);
        ufunc?.Invoke(request.responseCode, request.downloadHandler.text);
        //if (request.responseCode == 200)
        //    Debug.LogFormat("response:{0}", request.downloadHandler.text);
        request.Dispose();
    }


    public void DoPost1(string url, WWWForm form, Action<string, string> callback = null, string accessToken = null)
    {
        StartCoroutine(PostCo1(url, form, callback, accessToken));
    }


    IEnumerator PostCo1(string url, WWWForm form, Action<string, string> callback = null, string accessToken = null)
    {
        var request = UnityWebRequest.Post(url, form);
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        request.SetRequestHeader("Accept", "application/json");
        if (accessToken != null)
            request.SetRequestHeader("Authorization",   accessToken);

        request.timeout = 5;
        yield return request.SendWebRequest();
        callback?.Invoke(request.responseCode.ToString(), request.downloadHandler.text);
        if(request.responseCode==200)
            Debug.LogFormat("response:{0}",request.downloadHandler.text);
        request.Dispose();
    }

    
}

