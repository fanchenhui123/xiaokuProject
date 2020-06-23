using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialOfferCar : MonoBehaviour
{
    private List<SpecialCar> _specialCars=new List<SpecialCar>();
    public List<Text> TextsList=new List<Text>();
    public static SpecialOfferCar Instance;
    public void SearchSpecial(Text text)
    {
        for (int i = 0; i < _specialCars.Count; i++)
        {
            if (_specialCars[i].CarNumber==text.text)
            {
                TextsList[0].text = _specialCars[i].CarType;
                TextsList[1].text = _specialCars[i].CarSeris;
                TextsList[2].text = _specialCars[i].RealPrice;
                TextsList[3].text = _specialCars[i].CardPrice;
                TextsList[4].text = _specialCars[i].InsurePrice;
                TextsList[5].text = _specialCars[i].BuyTax;
                TextsList[6].text = _specialCars[i].ServicePrice;
                TextsList[7].text = _specialCars[i].OthersPrice;
                TextsList[8].text = _specialCars[i].EditorInfo;
                TextsList[9].text = _specialCars[i].MemoInfo;
            }
        }
    }

    public void ClosePanel()
    {
        transform.gameObject.SetActive(false);
    }
}

public class SpecialCar
{
    public string CarType;
    public string CarSeris;
    public string CarNumber;
    public string RealPrice;
    public string CardPrice;
    public string InsurePrice;
    public string BuyTax;
    public string ServicePrice;
    public string OthersPrice;
    public string EditorInfo;
    public string MemoInfo;
}