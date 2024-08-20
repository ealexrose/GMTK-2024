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
}
