using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{

    public List<ApartmentController> apartmentControllers;
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



    #region Adding and Moving Apartments
    internal void RegisterApartment(ApartmentController apartmentManager)
    {
        apartmentGrid.AddApartment(apartmentManager.apartment);
        apartmentControllers.Add(apartmentManager);
    }

    internal void LockAllBuildingsInPlace()
    {
        foreach (ApartmentController apartmentController in apartmentControllers)
        {
            apartmentController.LockIn();
        }
    }

    internal void MoveApartment(Apartment apartment, Vector2Int newPosition)
    {
        apartmentGrid.MoveApartment(apartment, newPosition);
        Debug.Log("Are apartments all placed? " + AreApartmentsAllPlacedInValidPositions());

    }

    #endregion

    #region Apartment Collision
    internal bool CheckIfTargetApartmentPositionIsValid(Vector2Int gridPosition, Apartment apartment)
    {
        return !apartmentGrid.IsApartmentPositionBlocked(gridPosition, apartment);
    }

    internal bool AreApartmentsAllPlacedInValidPositions()
    {
        foreach (ApartmentController apartmentManager in apartmentControllers)
        {
            //We can assume any previously placed apartments are still valid
            if (apartmentManager.apartmentState == ApartmentController.ApartmentState.LockedIn)
                continue;

            if (apartmentManager.apartmentState == ApartmentController.ApartmentState.FollowingMouse)
            {
                Debug.LogWarning("Attempted to check if apartments were all placed while one was moving, this should never happen");
                return false;
            }

            foreach (ApartmentBlock apartmentBlock in apartmentManager.apartment.apartmentBlocks)
            {
                if (!PlacementAreaManager.instance.IsInPlacementBounds(apartmentBlock.gridPosition))
                    return false;
            }
        }
        return true;
    }

    internal bool CheckIfTargetResidentPositionIsValid(Vector2Int gridPosition, Resident resident)
    {
        return apartmentGrid.IsGridPositionOpenForResident(gridPosition, resident);
    }

    internal Apartment GetApartmentAtPosition(Vector2Int gridPosition)
    {
        return apartmentGrid.GetApartmentAtPosition(gridPosition);
    }

    #endregion

    #region Spawning Apartments
    public List<ApartmentSpawnable> apartmentPrefabs;
    public Transform spawnPoint;
    public Vector2Int spawnArea;

    public void SpawnApartment() 
    {
        ApartmentSpawnable apartmentToSpawn = GetRandomApartment();
        GameObject newApartment = Instantiate(apartmentToSpawn.apartment, null);
        newApartment.transform.position = new Vector3(Mathf.FloorToInt(spawnPoint.transform.position.x), Mathf.FloorToInt(spawnPoint.transform.position.y), 0);
    }

    public ApartmentSpawnable GetRandomApartment() 
    {
        float maxWeight = apartmentPrefabs.Sum(x => x.spawnWeight);
        float spawnItem = UnityEngine.Random.Range(0f, maxWeight);
        float selectionLevel = 0;
        foreach (ApartmentSpawnable option in apartmentPrefabs)
        {
            selectionLevel += option.spawnWeight;
            if (selectionLevel > spawnItem)
            {
                return option;
            }
        }

        Debug.LogWarning("Could not select random apartment, returning default");
        return apartmentPrefabs[0];
    }

    public List<ApartmentSpawnable> GetRandomApartments(int spawnCount)
    {
        List<ApartmentSpawnable> spawnables = new List<ApartmentSpawnable>();

        for (int i = 0; i < spawnCount; i++) 
        {
            spawnables.Add(GetRandomApartment());
        }
        return spawnables;
    }

    public void SpawnApartments(int spawnCount) 
    {
        List<ApartmentSpawnable> spawnQueue = GetRandomApartments(spawnCount);


        int rowSize = 0;
        int rowOffset = 0;
        int rows = 0;
        int highest = 0;
        int xOffset = 0;
        List<ApartmentSpawnable> apartmentsOnDeck = new List<ApartmentSpawnable>();
        List<Vector2Int> apartmentDimensions = new List<Vector2Int>();
        for (int i = 0; i < spawnCount; i++)
        {
            Apartment apartment = spawnQueue[i].apartment.GetComponent<ApartmentController>().apartment;
            if (rowSize + apartment.footPrint.x > spawnArea.x)
            {
                highest = apartmentDimensions.Max(spawnDimension => spawnDimension.y);
                xOffset = 0;
                foreach (ApartmentSpawnable apartmentBeingSpawned in apartmentsOnDeck)
                {
                    float xSpawnPosition = Mathf.FloorToInt(spawnPoint.transform.position.x) + xOffset;
                    float ySpawnPosition = Mathf.FloorToInt(spawnPoint.transform.position.y) - highest - rowOffset;
                    GameObject newApartment = Instantiate(apartmentBeingSpawned.apartment, null);
                    newApartment.transform.position = new Vector3(xSpawnPosition, ySpawnPosition, 0);

                    xOffset += apartmentBeingSpawned.apartment.GetComponent<ApartmentController>().apartment.footPrint.x + 1;
                }

                apartmentsOnDeck = new List<ApartmentSpawnable>();
                apartmentDimensions = new List<Vector2Int>();
                rowOffset += highest + 1;
                rowSize = 0;
                rows++;
            }

            apartmentsOnDeck.Add(spawnQueue[i]);
            apartmentDimensions.Add(apartment.footPrint);

            //We add 1 to account for the gap
            rowSize += apartment.footPrint.x + 1;
            
        }

        if (apartmentsOnDeck.Count == 0)
            return;
        highest = apartmentDimensions.Max(spawnDimension => spawnDimension.y);
        xOffset = 0;
        foreach (ApartmentSpawnable apartmentBeingSpawned in apartmentsOnDeck)
        {
            float xSpawnPosition = Mathf.FloorToInt(spawnPoint.transform.position.x) + xOffset;
            float ySpawnPosition = Mathf.FloorToInt(spawnPoint.transform.position.y) - highest - rowOffset;
            GameObject newApartment = Instantiate(apartmentBeingSpawned.apartment, null);
            newApartment.transform.position = new Vector3(xSpawnPosition, ySpawnPosition, 0);

            xOffset += apartmentBeingSpawned.apartment.GetComponent<ApartmentController>().apartment.footPrint.x + 1;
        }
    }

    #endregion

}
