using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MsgCard : MonoBehaviour
{
    [HideInInspector]
    public Button commit;
    private Text[] msgs;

    public Text text_order_no;
    public Text text_cart_type;
    public Text text_vehicleSystem;
    public Text text_carNumber;
    public Text text_appear_color;
    public Text text_user_id;
    public Text text_price;
    public Text text_yajin;


    // Start is called before the first frame update
    void Start()
    {
        msgs = GetComponentsInChildren<Text>();
        commit = GetComponentInChildren<Button>();
        commit.onClick.AddListener(OnCommitClick);
    }

    public void OnCommitClick()
    {
        MsgCenterCtrl.Instance.ChangeToPage(2);
    }
}
