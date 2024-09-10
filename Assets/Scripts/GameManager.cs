using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        PlacingApartments,
        PlacingResidents,
        ReadyToStartNextRound
    }
    public GameState gameState;

    public static GameManager instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            gameState = GameState.PlacingApartments;
            StartRound();
        }
        else
        {
            Destroy(this.gameObject);
            Debug.LogWarning("Extra Game Manager Found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            ProgressRound();
        }
    }

    public void EndApartmentPhase() 
    {
        BuildingManager.instance.LockAllBuildingsInPlace();
        ResidentManager.instance.UnlockResidents();
        gameState = GameState.PlacingResidents;
    }

    private void EndRound()
    {
        BuildingManager.instance.LockAllBuildingsInPlace();
        ResidentManager.instance.LockAllResidentsInPlace();
        gameState = GameState.ReadyToStartNextRound;
    }
    private void StartRound()
    {
        BuildingManager.instance.SpawnApartments(5);
        ResidentManager.instance.SpawnResidents(5);
        gameState = GameState.PlacingApartments;
    }

    //Rounds consist of three phases
    //Placing Apartments
    //Placing Residents
    //Upkeep
    internal void ProgressRound() 
    {

        if (gameState == GameState.PlacingApartments)
        {
            if (!BuildingManager.instance.AllApartmentsPlacedInZone())
            {
                WarningManager.instance.PlaceAllApartmentsWarning();
            }
            else if (!BuildingManager.instance.apartmentGrid.AllApartmentBlocksTouching())
            {
                WarningManager.instance.AllApartmentTouchingWarning();
            }
            else if (!BuildingManager.instance.ApartmentIsOnTheGround())
            {
                WarningManager.instance.ApartmentTouchingGroundWarning();
            }
            else
            {
                EndApartmentPhase();
            }
        }
        else if (gameState == GameState.PlacingResidents) 
        {
            if (!ResidentManager.instance.AllResidentsAreInBuildings())
            {
                WarningManager.instance.FlashGenericWarning("Must Place All Residents!!!");
            }
            else 
            {
                EndRound();
                StartRound();
            }
        }
        else if (gameState == GameState.ReadyToStartNextRound)
        {
            StartRound();
        }

    }
}
