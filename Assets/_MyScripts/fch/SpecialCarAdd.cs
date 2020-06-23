using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialCarAdd : MonoBehaviour
{
   public List<Text> TextsList=new List<Text>();
   private SpecialCar _specialCar=new SpecialCar();
   public void SaveSpecialCar()
   {
      _specialCar.CarType = TextsList[0].text;
      _specialCar.CarSeris = TextsList[0].text;
      _specialCar.RealPrice = TextsList[0].text;
      _specialCar.CardPrice = TextsList[0].text;
      _specialCar.InsurePrice = TextsList[0].text;
      _specialCar.BuyTax = TextsList[0].text;
      _specialCar.CarType = TextsList[0].text;
      _specialCar.CarType = TextsList[0].text;
      _specialCar.CarType = TextsList[0].text;
      _specialCar.CarType = TextsList[0].text;
   }
}
