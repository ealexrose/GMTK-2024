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
        AtLeastOneNeighborSharesType = 1,
        NoNeighborsOfThisType = 2,
        AllNeighborsUnique = 4,
        RequiresOpenSpace = 8,
        AllNeighborsAreAlike = 16,
    }

    public ResidentType residentType;

    public enum ResidentMood 
    {
        Homeless,
        Happy,
        Mad,
        Annoyed
    }
    public ResidentMood residentMood = ResidentMood.Homeless;
    public Vector2Int gridCoordinates;
    public int colorNumber;
    public int moneyMultiplier;
}
