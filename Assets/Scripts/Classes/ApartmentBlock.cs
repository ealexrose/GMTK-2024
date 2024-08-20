using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ApartmentBlock 
{
    [HideInInspector]
    [SerializeReference]
    public Apartment parentApartment;
    public Vector2Int localGridPosition;
    [HideInInspector]
    public Vector2Int gridPosition;

}
