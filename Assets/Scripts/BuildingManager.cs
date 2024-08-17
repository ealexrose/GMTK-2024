using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{

    public List<ApartmentManager> apartmentManagers;
    public ApartmentGrid apartmentGrid;

    public static BuildingManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            apartmentGrid = new ApartmentGrid(32, 64);
        }
        else
        {
            Destroy(this.gameObject);
            Debug.LogWarning("Extra Building Manager Found");
        }
    }



    // Update is called once per frame
    void Update()
    {

    }

    public bool ApartmentOverlapsObstacle(Apartment apartment, Vector2Int gridPosition)
    {

        return false;
    }


    #region Adding and Moving Apartments
    internal void RegisterApartment(Apartment apartment)
    {
        apartmentGrid.AddApartment(apartment);
    }

    internal void MoveApartment(Apartment apartment, Vector2Int newPosition)
    {
        apartmentGrid.MoveApartment(apartment, newPosition);
    }

    #endregion

}
