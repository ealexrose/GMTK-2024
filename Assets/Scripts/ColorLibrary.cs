using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorLibrary : MonoBehaviour
{
    public Color32 ApartmentBackgroundColor;
    public Color32 ApartmentBorderColor;
    public Color32 ApartmentBackgroundColorWarning;
    public Color32 ApartmentBorderColorWarning;
    public Color32 ApartmentBackgroundColorError;
    public Color32 ApartmentBorderColorError;

    public List<Color32> residentColors;
    public Color32 residentLockedColor;
    public Color32 residentUnlockedColor;
    public Color32 residentInApartmentColor;

    public Color32 haveMoneyColor;
    public Color32 noMoneyColor;
    public Color32 moneyGainColor;
    public Color32 moneyLossColor;
    public static ColorLibrary instance;
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
