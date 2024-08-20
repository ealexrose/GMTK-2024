using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementAreaManager : MonoBehaviour
{
    public Vector2Int dimensions;
    private Vector2Int localOrigin { get {return  new Vector2Int(Mathf.FloorToInt(transform.position.x),Mathf.FloorToInt( transform.position.y)); } }

    public static PlacementAreaManager instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            Debug.LogWarning("Extra Placement Manager Found");
        }
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Vector2 corner1 = localOrigin;
        Vector2 corner2 = corner1 + Vector2.right * dimensions.x;
        Vector2 corner3 = corner2 + Vector2.up * dimensions.y;
        Vector2 corner4 = corner3 + Vector2.left * dimensions.x;
        Gizmos.DrawLine(corner1, corner2);
        Gizmos.DrawLine(corner2, corner3);
        Gizmos.DrawLine(corner3, corner4);
        Gizmos.DrawLine(corner4, corner1);
    }

    public bool IsInPlacementBounds(Vector2Int coordinates) 
    {
        if (coordinates.x < localOrigin.x || coordinates.x >= localOrigin.x + dimensions.x)
            return false;

        if (coordinates.y < localOrigin.y || coordinates.y >= localOrigin.y + dimensions.y)
            return false;

        return true;
    }

    public bool IsFootprintInBounds(Vector2Int coordinates, Vector2Int footprint) 
    {
        if (coordinates.x < localOrigin.x || coordinates.x + footprint.x > localOrigin.x + dimensions.x)
            return false;

        if (coordinates.y < localOrigin.y || coordinates.y + footprint.y > localOrigin.y + dimensions.y)
            return false;

        return true;
    }
}
