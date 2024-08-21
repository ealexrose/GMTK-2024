using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class WarningManager : MonoBehaviour
{

    public enum WarningState 
    {
        Ready,
        Flashing,
    }

    public WarningState warningState;

    public TextMeshProUGUI warningText;
    public Color32 warningColor;
    public Color32 normalColor;

    public static WarningManager instance;
    public void Awake()
    {
        instance = this;
    }


    public void FlashWarning(string warning, float warningDuration, float flashInterval) 
    {
        if(warningState == WarningState.Ready)
            StartCoroutine(FlashWarningCoroutine(warning, warningDuration, flashInterval));
    }

    private IEnumerator FlashWarningCoroutine(string warning, float warningDuration, float flashInterval)
    {
        warningText.text = warning;
        float intervalTime = 0;
        bool flashingWarning = false;
        warningState = WarningState.Flashing;
        for (float i = 0; i < warningDuration; i+= Time.deltaTime) 
        {
            if (intervalTime > flashInterval) 
            {
                intervalTime = 0;
                flashingWarning = !flashingWarning;
            }
            warningText.color = flashingWarning ? warningColor : normalColor;
            intervalTime += Time.deltaTime;
            yield return null;
        }
        warningText.text = "";
        warningState = WarningState.Ready;
    }

    internal void ApartmentTouchingGroundWarning()
    {
        FlashWarning("Apartment Must be on the ground!!!", 2f, 0.2f);
    }

    internal void AllApartmentTouchingWarning()
    {
        FlashWarning("All apartments must be touching!!!", 2f, 0.2f);
    }

    internal void PlaceAllApartmentsWarning()
    {
        FlashWarning("Must place all apartments first!!!", 2f, 0.2f);
    }
}
