using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ApartmentGrid
{

    public bool debugMessages;
    public ApartmentBlock[,] grid;

    public ApartmentGrid(int xSize, int ySize) 
    {
        grid = new ApartmentBlock[xSize,ySize];
    }



    #region Grid Management

    internal void AddApartment(Apartment apartment)
    {
        foreach (ApartmentBlock apartmentBlock in apartment.apartmentBlocks)
        {
            int x = apartment.gridPosition.x + apartmentBlock.localGridPosition.x;
            int y = apartment.gridPosition.y + apartmentBlock.localGridPosition.y;
            if (grid[x, y] != null)
                Debug.LogError($"Attempted to initialize an apartment block at ({x},{y}) but it was already occupied");
            grid[x, y] = apartmentBlock;
            apartmentBlock.gridPosition = new Vector2Int(x, y);
            //Debug.Log($"Initialize an apartment block at ({x},{y})");
        }
    }

    internal void MoveApartment(Apartment apartment, Vector2Int newPosition)
    {
        foreach (ApartmentBlock apartmentBlock in apartment.apartmentBlocks) 
        {
            int x = apartmentBlock.gridPosition.x;
            int y = apartmentBlock.gridPosition.y;
            if (grid[x, y] != apartmentBlock)
                Debug.LogWarning("A block has displaced another block while attempting to move.");

            grid[x, y] = null;
            if(debugMessages)
                Debug.Log($"Moved an apartment block from ({x},{y})");
        }

        foreach (ApartmentBlock apartmentBlock in apartment.apartmentBlocks)
        {
            int x = newPosition.x + apartmentBlock.localGridPosition.x;
            int y = newPosition.y + apartmentBlock.localGridPosition.y;
            if (grid[x, y] != null)
                Debug.LogWarning($"A block has replaced another block in the grid at ({x},{y}) while attempting to move.");

            grid[x, y] = apartmentBlock;
            apartmentBlock.gridPosition = new Vector2Int(x, y);
            if (debugMessages)
                Debug.Log($"Moved an apartment block to ({x},{y})");
        }

        apartment.gridPosition = newPosition;
    }
    #endregion

    #region Grid Occupation Helpers
    // Preview the placement of all apartmentblocks for an apartment at its current position, return true if all spaces are unoccupied and within placement bounds
    public bool IsApartmentPositionBlocked(Vector2Int gridPosition, Apartment apartment)
    {
        foreach (ApartmentBlock apartmentBlock in apartment.apartmentBlocks)
        {
            int x = gridPosition.x + apartmentBlock.localGridPosition.x;
            int y = gridPosition.y + apartmentBlock.localGridPosition.y;

            if (IsGridCellBlocked(new Vector2Int(x, y), apartment))
                return true;
        }
        return false;
    }

    //Check if a position is either outside placement bounds or occupied by an existing apartment block (ignore apartment blocks belonging to excluded apartments, typically the parent apartment)
    public bool IsGridCellBlocked(Vector2Int coordinate, Apartment exclusion = null)
    {
        if (coordinate.x < 0 || coordinate.x >= grid.GetLength(0) || coordinate.y < 0 || coordinate.y >= grid.GetLength(1))
        {
            Debug.LogWarning("Out of bounds!");
            return true;
        }

        var apartmentBlockAtCoordinates = grid[coordinate.x, coordinate.y];
        if (apartmentBlockAtCoordinates == null)
            return false;

        if (exclusion != null && apartmentBlockAtCoordinates.parentApartment == exclusion)
        {
            return false;
        }


        return true;
    }

    //Get the parent apartment of an apartment block at specified coordinates, returns null if none present
    internal Apartment GetApartmentAtPosition(Vector2Int gridPosition)
    {
        int x = gridPosition.x;
        int y = gridPosition.y;
        return grid[x, y]?.parentApartment;
    }
    
    //Checks if a grid coordinate is a legal position for a resident, which is considered valid if there is no apartment block, or if the apartment block at the coordinates belongs to an
    //unoccupied apartment or the apartment is already occupied by the resident trying to move (alows users to move residents within the same multiblock apartments
    internal bool IsGridPositionOpenForResident(Vector2Int gridPosition, Resident resident)
    {
        int x = gridPosition.x;
        int y = gridPosition.y;
        ApartmentBlock blockAtPosition = grid[x, y];
        if (blockAtPosition != null && !blockAtPosition.parentApartment.IsInPlacementZone)
            return false;

        if (blockAtPosition == null || !blockAtPosition.parentApartment.Occupied || blockAtPosition.parentApartment.resident == resident)
            return true;

        return false;
    }
    #endregion

    #region Grid Adjacency
    
    //Checks that all apartments touch each other cardinally using a depth first search
    internal bool AllApartmentBlocksTouching()
    {
        List<Apartment> visitedApartments = new List<Apartment>();
        List<Apartment> edgeApartments = new List<Apartment>();
        edgeApartments.Add(BuildingManager.instance.apartmentControllers[0].apartment);
        visitedApartments.Add(BuildingManager.instance.apartmentControllers[0].apartment);
        while (edgeApartments.Count > 0)
        {
            List<Apartment> neighbors = GetAllNeighboringApartments(edgeApartments[0]);
            List<Apartment> unvisitedNeighbors = neighbors.Where(apt => !visitedApartments.Contains(apt)).ToList();
            edgeApartments.AddRange(unvisitedNeighbors);
            visitedApartments.AddRange(unvisitedNeighbors);
            edgeApartments.RemoveAt(0);
        }
        return visitedApartments.Count == BuildingManager.instance.apartmentControllers.Count();
    }

    //Get a list of all apartments that neighbour this one (touching cardinally)
    internal List<Apartment> GetAllNeighboringApartments(Apartment apartment)
    {
        if (apartment == null)
            return new List<Apartment>();

        List<Apartment> neighbors = new List<Apartment>();
        foreach (ApartmentBlock apartmentBlock in apartment.apartmentBlocks)
        {
            int x = apartmentBlock.gridPosition.x;
            int y = apartmentBlock.gridPosition.y;
            Vector2Int northCoordinates = new Vector2Int(x, y + 1);
            Vector2Int eastCoordinates = new Vector2Int(x + 1, y);
            Vector2Int southCoordinates = new Vector2Int(x, y - 1);
            Vector2Int westCoordinates = new Vector2Int(x - 1, y);
            Apartment apartmentBeingExamined;

            //Check North
            if (PlacementAreaManager.instance.IsInPlacementBounds(northCoordinates))
            {
                apartmentBeingExamined = GetApartmentAtPosition(northCoordinates);
                if (apartmentBeingExamined != null && apartmentBeingExamined != apartment && !neighbors.Contains(apartmentBeingExamined))
                    neighbors.Add(apartmentBeingExamined);
            }
            //Check East
            if (PlacementAreaManager.instance.IsInPlacementBounds(eastCoordinates))
            {
                apartmentBeingExamined = GetApartmentAtPosition(eastCoordinates);
                if (apartmentBeingExamined != null && apartmentBeingExamined != apartment && !neighbors.Contains(apartmentBeingExamined))
                    neighbors.Add(apartmentBeingExamined);
            }
            //Check South
            if (PlacementAreaManager.instance.IsInPlacementBounds(southCoordinates))
            {
                apartmentBeingExamined = GetApartmentAtPosition(southCoordinates);
                if (apartmentBeingExamined != null && apartmentBeingExamined != apartment && !neighbors.Contains(apartmentBeingExamined))
                    neighbors.Add(apartmentBeingExamined);
            }
            //Check West
            if (PlacementAreaManager.instance.IsInPlacementBounds(westCoordinates))
            {
                apartmentBeingExamined = GetApartmentAtPosition(westCoordinates);
                if (apartmentBeingExamined != null && apartmentBeingExamined != apartment && !neighbors.Contains(apartmentBeingExamined))
                    neighbors.Add(apartmentBeingExamined);
            }
        }
        return neighbors;
    }

    //Get a list of all neighboring apartment blocks that touch a given apartment
    internal List<ApartmentBlock> GetNeighboringApartmentBlocks(Apartment apartment)
    {
        List<Vector2Int> coordinates = apartment.GetNeighboringSpaces();
        List<ApartmentBlock> apartmentBlocks = coordinates.Select(coord => grid[coord.x, coord.y]).ToList();
        return apartmentBlocks;
    }

    //Get a list of all residents occupying apartments that neighbor this one
    internal List<Resident> GetAllNeighboringResidents(Apartment apartment)
    {
        List<Apartment> neighobringApartments = GetAllNeighboringApartments(apartment);
        return neighobringApartments.Select(apt => apt.resident).ToList();
    }

    #endregion
}