using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentGrid
{
    public ApartmentBlock[,] grid;

    public ApartmentGrid(int xSize, int ySize) 
    {
        grid = new ApartmentBlock[xSize,ySize];
    }

    public bool IsGridCellBlocked(Vector2Int coordinate, Apartment exclusion = null) 
    {
        if (coordinate.x < 0 || coordinate.x >= grid.GetLength(0) || coordinate.y < 0 || coordinate.y >= grid.GetLength(1))
        {
            Debug.LogWarning("Out of bounds!");
            return false;
        }

        var apartmentBlockAtCoordinates = grid[coordinate.x,coordinate.y];
        if (apartmentBlockAtCoordinates == null)
            return false;

        if (exclusion != null && apartmentBlockAtCoordinates.parentApartment == exclusion) 
        {
            return false;
        }


        return true;
    }

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

            Debug.Log($"Moved an apartment block to ({x},{y})");
        }

        apartment.gridPosition = newPosition;
    }
}
