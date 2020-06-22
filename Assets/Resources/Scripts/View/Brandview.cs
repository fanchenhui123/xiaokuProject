using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Brandview : MonoBehaviour
{
    public DelayPanelManager delayPanelManager;
    public CarDataManager carDataManager;
    public NetworkManager networkManager;
    public Text warnText;
    public Button closeBtn;
    public Button searchBtn;
    public GameObject itemPrefab;
    public GameObject content1;//品牌
    public GameObject content2;//车系
    public GameObject content3;//车型

    // Start is called before the first frame update
    void Start()
    {
        delayPanelManager = FindObjectOfType<DelayPanelManager>();
        networkManager = FindObjectOfType<NetworkManager>();
        if (carDataManager == null) carDataManager = FindObjectOfType<CarDataManager>();
        closeBtn.onClick.AddListener(() =>
        {
            Show(false);
        });

        searchBtn.onClick.AddListener(UpdateView);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(bool isOn)
    {
        gameObject.SetActive(isOn);
        if (isOn == false) return;
        UpdateView();
    }

    public void UpdateView()
    {
        delayPanelManager.Load();
        for (int i = 0; i < content1.transform.childCount; i++)
        {
            Destroy(content1.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < content2.transform.childCount; i++)
        {
            Destroy(content2.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < content3.transform.childCount; i++)
        {
            Destroy(content3.transform.GetChild(i).gameObject);
        }
        carDataManager.DoSearchBrand(()=> {
            foreach (var kp in carDataManager.brandsList)
            {
                //Debug.Log("kp:" + kp.id + "|" + kp.title);
                var item = Instantiate<GameObject>(itemPrefab);
                item.transform.SetParent(content1.transform);
                item.GetComponentInChildren<Text>().text = kp.id + "|" + kp.title;
                item.GetComponent<Button>().onClick.AddListener(()=> {
                    UpdateCarLinesView(kp.id);
                });
            }
            delayPanelManager.Destory();
        });
        
    }

    public void UpdateCarLinesView(int brandID)
    {
        delayPanelManager.Load();
        for (int i = 0; i < content2.transform.childCount; i++)
        {
            Destroy(content2.transform.GetChild(i).gameObject);
        }
        carDataManager.DoCartLines(brandID,() => {
            foreach (var kp in carDataManager.cartLinesList)
            {
                //Debug.Log("kp:" + kp.id + "|" + kp.title);
                var item = Instantiate<GameObject>(itemPrefab);
                item.transform.SetParent(content2.transform);
                item.GetComponentInChildren<Text>().text = kp.id + "|" + kp.title;
                item.GetComponent<Button>().onClick.AddListener(() => {
                    UpdateCarModelsView(kp.id);
                });
            }
            delayPanelManager.Destory();
        });
    }

    public void UpdateCarModelsView(int carlineID)
    {
        delayPanelManager.Load();
        for(int i = 0; i < content3.transform.childCount; i++)
        {
            Destroy(content3.transform.GetChild(i).gameObject);
        }       
        carDataManager.DoCartModels(carlineID, () => {
            foreach (var kp in carDataManager.cartModelsList)
            {
                //Debug.Log("kp:" + kp.id + "|" + kp.title);
                var item = Instantiate<GameObject>(itemPrefab);
                item.transform.SetParent(content3.transform);
                item.GetComponentInChildren<Text>().text = kp.id + "|" + kp.title;
            }
            delayPanelManager.Destory();
        });
    }
}
