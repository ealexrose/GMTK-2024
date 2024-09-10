using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Apartment
{
    public Vector2Int footPrint;
    public List<ApartmentBlock> apartmentBlocks;
    [HideInInspector]
    public Vector2Int gridPosition;
    
    [HideInInspector]
    public bool Occupied { get { return resident.name != null && resident.name != ""; } }
    public bool IsInPlacementZone;
    public Resident resident;

    public int CountNeighboringSpaces() 
    {
        List<Vector2Int> apartmentSpaces = new List<Vector2Int>();
        List<Vector2Int> neighboringSpaces = new List<Vector2Int>();
        foreach (ApartmentBlock apartmentBlock in apartmentBlocks) 
        {
            apartmentSpaces.Add(apartmentBlock.localGridPosition);
        }

        foreach (Vector2Int apartmentSpace in apartmentSpaces) 
        {
            Vector2Int North = apartmentSpace + Vector2Int.up;
            Vector2Int South = apartmentSpace + Vector2Int.down;
            Vector2Int East = apartmentSpace + Vector2Int.right;
            Vector2Int West = apartmentSpace + Vector2Int.left;

            if (!apartmentSpaces.Contains(North) && !neighboringSpaces.Contains(North))
                neighboringSpaces.Add(North);
            if (!apartmentSpaces.Contains(South) && !neighboringSpaces.Contains(South))
                neighboringSpaces.Add(South);
            if (!apartmentSpaces.Contains(East) && !neighboringSpaces.Contains(East))
                neighboringSpaces.Add(East);
            if (!apartmentSpaces.Contains(West) && !neighboringSpaces.Contains(West))
                neighboringSpaces.Add(West);
        }
        return neighboringSpaces.Count;
    }

    public List<Vector2Int> GetNeighboringSpaces(bool gridSpace = false)
    {
        List<Vector2Int> apartmentSpaces = new List<Vector2Int>();
        List<Vector2Int> neighboringSpaces = new List<Vector2Int>();
        foreach (ApartmentBlock apartmentBlock in apartmentBlocks)
        {
            if (gridSpace)
            {
                apartmentSpaces.Add(apartmentBlock.localGridPosition);
            }
            else 
            {
                apartmentSpaces.Add(apartmentBlock.gridPosition);
            }

        }

        foreach (Vector2Int apartmentSpace in apartmentSpaces)
        {
            Vector2Int North = apartmentSpace + Vector2Int.up;
            Vector2Int South = apartmentSpace + Vector2Int.down;
            Vector2Int East = apartmentSpace + Vector2Int.right;
            Vector2Int West = apartmentSpace + Vector2Int.left;

            if (!apartmentSpaces.Contains(North) && !neighboringSpaces.Contains(North))
                neighboringSpaces.Add(North);
            if (!apartmentSpaces.Contains(South) && !neighboringSpaces.Contains(South))
                neighboringSpaces.Add(South);
            if (!apartmentSpaces.Contains(East) && !neighboringSpaces.Contains(East))
                neighboringSpaces.Add(East);
            if (!apartmentSpaces.Contains(West) && !neighboringSpaces.Contains(West))
                neighboringSpaces.Add(West);
        }
        return neighboringSpaces;
    }
}
