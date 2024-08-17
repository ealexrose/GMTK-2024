using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ApartmentBlock 
{
    [SerializeReference]
    public Apartment parentApartment;
    public Vector2Int localGridPosition;
    [HideInInspector]
    public Vector2Int gridPosition;
}
