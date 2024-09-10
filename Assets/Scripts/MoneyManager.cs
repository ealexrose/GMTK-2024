using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MoneyManager : MonoBehaviour
{
    public TextMeshProUGUI moneyBanked;
    public TextMeshProUGUI moneyPerRoundAmount;
    public TextMeshProUGUI moneyPerRoundLabel;
    public int money;
    public int moneyPerRound;
    // Start is called before the first frame update
    void Start()
    {
        SetMoney(100);
        SetMoneyPerRound(250);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMoneyPerRound();
    }

    private void UpdateMoneyPerRound()
    {
        moneyPerRound = 0;
        moneyPerRound = -BuildingManager.instance.GetApartmentPrice();
        moneyPerRound += ResidentManager.instance.GetResidentRent();
        SetMoneyPerRound(moneyPerRound);
    }

    public void SetMoneyPerRound(int amount) 
    {
        string prefix = "";
        if (amount < 0) 
        {
            moneyPerRoundLabel.color = ColorLibrary.instance.moneyLossColor;
            moneyPerRoundAmount.color = ColorLibrary.instance.moneyLossColor;
        }
        else 
        {
            moneyPerRoundLabel.color = ColorLibrary.instance.moneyGainColor;
            moneyPerRoundAmount.color = ColorLibrary.instance.moneyGainColor;
            prefix = "+";
        }

        moneyPerRoundAmount.text = prefix + amount.ToString();
    }

    public void SetMoney(int amount) 
    {
        string prefix = "";
        if (amount < 0)
        {
            moneyBanked.color = ColorLibrary.instance.noMoneyColor;
        }
        else
        {
            moneyBanked.color = ColorLibrary.instance.haveMoneyColor;
        }

        moneyBanked.text = prefix + amount.ToString();
    }
}
