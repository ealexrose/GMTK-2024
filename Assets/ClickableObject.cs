using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    public enum ClickableObjectType
    {
        Resident,
        Apartment,
    }

    public ClickableObjectType clickableObjectType;    
    public GameObject objectToClick;

    public bool IsObjectValidToClick() 
    {
        switch (clickableObjectType)
        {
            case ClickableObjectType.Apartment:
                return objectToClick.GetComponent<ApartmentController>().IsValidClickable();
            case ClickableObjectType.Resident:
                return objectToClick.GetComponent<ResidentController>().IsValidClickable();
            default:
                Debug.LogWarning($"Unhandled clickable object type {clickableObjectType}");
                return false;
        }
    }

    public bool ClickObject() 
    {
        switch (clickableObjectType) 
        {
            case ClickableObjectType.Apartment:
                return objectToClick.GetComponent<ApartmentController>().ClickedSelect();
            case ClickableObjectType.Resident:
                return objectToClick.GetComponent<ResidentController>().ClickedSelect();
            default:
                Debug.LogWarning($"Unhandled clickable object type {clickableObjectType}");
                return false;
        }
    }
}
