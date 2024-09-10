using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResidentManager : MonoBehaviour
{
    [HideInInspector]
    public List<ResidentController> residentControllers;
    public Transform spawnPoint;
    public List<ResidentSpawnable> residentPrefabs;
    public static ResidentManager instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            residentControllers = new List<ResidentController>();
        }
        else
        {
            Destroy(this.gameObject);
            Debug.LogWarning("Extra Resident Manager Found");
        }
    }




    #region Validation
    public bool IsSpaceOccupiedByResident(Vector2Int coordinates, Resident exclusion = null)
    {
        ResidentController residentAtLocation = residentControllers.Find(x => x.resident != exclusion && x.resident.gridCoordinates == coordinates);
        return residentAtLocation != null;
    }

    internal void RegisterResident(ResidentController resident)
    {
        residentControllers.Add(resident);
    }
    #endregion


    #region Round Helpers
    internal void LockAllResidentsInPlace()
    {
        foreach (ResidentController residentController in residentControllers)
        {
            residentController.LockIn();
        }
    }

    internal bool AllResidentsAreInBuildings()
    {
        return residentControllers.All(residentControler => residentControler.resident.homeApartment != null && residentControler.resident.homeApartment.IsInPlacementZone);
    }

    internal void UnlockResidents()
    {
        residentControllers.Where(res => res.residentState == ResidentController.ResidentState.WaitingForUnlock).ToList().ForEach(res => res.Unlock());
    }
    #endregion

    #region Resident Spawning
    private ResidentSpawnable GetRandomResident()
    {
        float maxWeight = residentPrefabs.Sum(x => x.spawnWeight);
        float spawnItem = UnityEngine.Random.Range(0f, maxWeight);
        float selectionLevel = 0;
        foreach (ResidentSpawnable option in residentPrefabs)
        {
            selectionLevel += option.spawnWeight;
            if (selectionLevel > spawnItem)
            {
                return option;
            }
        }

        Debug.LogWarning("Could not select random resident, returning default");
        return residentPrefabs[0];
    }

    private List<ResidentSpawnable> GetRandomResidents(int spawnCount)
    {
        List<ResidentSpawnable> spawnables = new List<ResidentSpawnable>();

        for (int i = 0; i < spawnCount; i++)
        {
            spawnables.Add(GetRandomResident());
        }
        return spawnables;
    }

    internal void SpawnResidents(int count)
    {

        List<ResidentSpawnable> residentsToSpawn = GetRandomResidents(count);
        //Some residents might have rules about partners they need, do that here
        int i = 0;
        foreach (ResidentSpawnable residentToSpawn in residentsToSpawn)
        {
            GameObject newResident = Instantiate(residentToSpawn.residentPrefab, null);
            newResident.transform.position = new Vector3(Mathf.Floor(spawnPoint.transform.position.x) + i, Mathf.Floor(spawnPoint.transform.position.y), 0);
            i += 1;
        }
    }
    #endregion

    #region Resident Happiness

    //Overload that lets you check if the resident of an apartment is happy
    public bool IsResidentHappy(Apartment apartment)
    {
        return IsResidentHappy(apartment.resident);
    }

    private bool IsResidentHappy(Resident resident, Apartment apartment = null)
    {
        if (apartment == null)
            apartment = resident.homeApartment;
        List<Resident> neighbors = BuildingManager.instance.apartmentGrid.GetAllNeighboringResidents(apartment);


        switch (resident.residentType)
        {
            case Resident.ResidentType.AtLeastOneNeighborSharesType:
                return neighbors.Any(nb => nb.residentType == Resident.ResidentType.AtLeastOneNeighborSharesType);
            case Resident.ResidentType.NoNeighborsOfThisType:
                return !neighbors.Any(nb => nb.residentType == Resident.ResidentType.NoNeighborsOfThisType);                
            case Resident.ResidentType.AllNeighborsUnique:
                return neighbors.Distinct().Count() == neighbors.Count();                
            case Resident.ResidentType.AllNeighborsAreAlike:
                return !(neighbors.Distinct().Count() > 1);
            case Resident.ResidentType.RequiresOpenSpace:
                return apartment.CountNeighboringSpaces() <= BuildingManager.instance.apartmentGrid.GetNeighboringApartmentBlocks(apartment).Count(block => block == null) * 2;
            default:
                break;
        }
        return true;
    }

    //Check each residents conditions and updates their happiness based on if they meet their happiness criteria
    //If a resident is not in an apartment, set them to a neutral 'homeless' state
    public void UpdateResidentHappiness() 
    {
        foreach (ResidentController residentController in residentControllers) 
        {
            if (residentController.resident.homeApartment == null)
                residentController.UpdateMood(Resident.ResidentMood.Homeless);
            else if (IsResidentHappy(residentController.resident))
                residentController.UpdateMood(Resident.ResidentMood.Happy);
            else
                residentController.UpdateMood(Resident.ResidentMood.Mad);
        }
    }

    public int ResidentValue(Resident resident) 
    {
        if (resident.residentMood == Resident.ResidentMood.Homeless)
            return 0;
        if (resident.residentMood == Resident.ResidentMood.Happy)
            return resident.moneyMultiplier * BuildingManager.instance.basePrice *  resident.homeApartment.apartmentBlocks.Count();
        if (resident.residentMood == Resident.ResidentMood.Mad)
            return -resident.moneyMultiplier * BuildingManager.instance.basePrice * resident.homeApartment.apartmentBlocks.Count();

        return 0;
    }

    public int ResidentValue(ResidentController residentController) 
    {
        return ResidentValue(residentController.resident);
    }

    internal int GetResidentRent()
    {
        int rent = 0;
        foreach (ResidentController residentController in residentControllers) 
        {
            rent += ResidentValue(residentController);
        }
        return rent;
    }
    #endregion
}
