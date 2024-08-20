using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Resident 
{
    public string name;
    [HideInInspector]
    [SerializeReference]
    public Apartment homeApartment;

    [System.Flags]
    public enum ResidentType 
    {        
        Social = 0 << 1,
        Loud = 0 << 2,
        Territorial = 0 << 3,
        Irritable = 0 << 4,
    }

    public ResidentType residentType;
    public Vector2Int gridCoordinates;
}
