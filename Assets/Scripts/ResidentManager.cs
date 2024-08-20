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

    public bool IsSpaceOccupiedByResident(Vector2Int coordinates, Resident exclusion = null) 
    {
        ResidentController residentAtLocation = residentControllers.Find(x => x.resident != exclusion && x.resident.gridCoordinates == coordinates);
        return residentAtLocation != null;
    }

    internal void LockAllResidentsInPlace()
    {
        foreach (ResidentController residentController in residentControllers)
        {
            residentController.LockIn();
        }
    }

    internal void RegisterResident(ResidentController resident)
    {
        residentControllers.Add(resident);
    }

    internal bool AllResidentsAreInBuildings() 
    {
        return residentControllers.All(residentControler => residentControler.resident.homeApartment != null && residentControler.resident.homeApartment.IsInPlacementZone);
    }

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
}
