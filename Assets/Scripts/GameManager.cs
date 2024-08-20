using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Placing,
        ReadyToStartNextRound
    }
    public GameState gameState;

    public static GameManager instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            gameState = GameState.Placing;
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
            if (gameState == GameState.Placing)
            {
                if (ResidentManager.instance.AllResidentsAreInBuildings() && BuildingManager.instance.AreApartmentsAllPlacedInValidPositions())
                {
                    EndRound();
                }
                else
                {
                    Debug.Log($"Cannot lock in apartments and residents because Residents are {ResidentManager.instance.AllResidentsAreInBuildings()} and Apartments are {BuildingManager.instance.AreApartmentsAllPlacedInValidPositions()}");
                }
            }
            else if (gameState == GameState.ReadyToStartNextRound) 
            {
                BuildingManager.instance.SpawnApartments(8);
                ResidentManager.instance.SpawnResidents(8);
                gameState = GameState.Placing;
            }

        }
    }

    private void EndRound()
    {
        BuildingManager.instance.LockAllBuildingsInPlace();
        ResidentManager.instance.LockAllResidentsInPlace();
        gameState = GameState.ReadyToStartNextRound;
    }
}
