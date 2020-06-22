using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchManager : MonoBehaviour
{
    public GameObject searchPanel;

    public Button searchBtn;
    public Button closeBtn;
    public InputField searchContent;
    public TableView tableView;

    // Start is called before the first frame update
    void Start()
    {
        closeBtn.onClick.AddListener(Hide);
        searchBtn.onClick.AddListener(delegate
        {
            tableView.SearchMethod(searchContent.text);
        });
        searchPanel.SetActive(false);
    }

   
    public void Show()
    {
        searchPanel.SetActive(true);
    }
    public void Hide()
    {
        searchPanel.SetActive(false);
    }
}
