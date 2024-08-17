using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ApartmentManager : MonoBehaviour
{
    public bool debugMessages;
    [SerializeField]
    public Apartment apartment;

    public enum ApartmentState
    {
        WaitingToMove,
        FollowingMouse,
        LockedIn,
    }

    public ApartmentState apartmentState;


    // Start is called before the first frame update
    void Start()
    {
        apartmentState = ApartmentState.WaitingToMove;
        InitializeApartment();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && apartmentState == ApartmentState.WaitingToMove)
        {
            if (CheckForClickedOnObject())
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition = new Vector2(mousePos.x, mousePos.y);
                Vector2 localPosition = (mousePosition - (Vector2)transform.position);
                localMouseGridPosition = new Vector2Int(Mathf.FloorToInt(localPosition.x), Mathf.FloorToInt(localPosition.y));
                if (debugMessages)
                {
                    Debug.Log("Name");
                    Debug.Log("Global Position: " + mousePosition);


                    Debug.Log("Local Position: " + localPosition);
                    Debug.Log("Local Coordinates: " + localMouseGridPosition);
                }
                StartFollowingMouse();
            }
        }

        if (apartmentState == ApartmentState.FollowingMouse)
        {
            MoveTowardsMouseGridPosition();
            CheckIfPositionIsValid();

            if (Input.GetMouseButtonUp(0))
            {
                StopFollowingMouse();
            }
        }


    }

    private bool CheckForClickedOnObject()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D[] hit = Physics2D.RaycastAll(mousePos2D, Vector2.zero);
        return hit.Any(h => h.collider.transform.IsChildOf(this.transform));
    }

    #region Dragging

    private Vector2 mousePosition;
    private Vector2Int localMouseGridPosition;
    private void MoveTowardsMouseGridPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector2(mousePos.x, mousePos.y);
        Vector2 mouseGridPosition = new Vector2(Mathf.Floor(mousePosition.x), Mathf.Floor(mousePosition.y));
        transform.position = mouseGridPosition - localMouseGridPosition;
    }

    public void StartFollowingMouse()
    {
        apartmentState = ApartmentState.FollowingMouse;
    }

    private void StopFollowingMouse()
    {
        apartmentState = ApartmentState.WaitingToMove;
        MoveApartment(LocalGridPosition);
    }

    #endregion

    private bool CheckIfPositionIsValid()
    {
        return true;
    }

    #region visuals
    public List<Sprite> sprites;

    public void MarkAsInvalid()
    {

    }

    public void MarkAsValid()
    {

    }

    #endregion

    private void InitializeApartment()
    {
        apartment.gridPosition = new Vector2Int(Mathf.FloorToInt(transform.position.x),Mathf.FloorToInt(transform.position.y));
        foreach (ApartmentBlock apartmentBlock in apartment.apartmentBlocks) 
        {
            apartmentBlock.parentApartment = apartment;
        }
        BuildingManager.instance.RegisterApartment(apartment);
    }

    private void MoveApartment(Vector2Int newPosition) 
    {
        BuildingManager.instance.MoveApartment(apartment, newPosition);
    }

    private Vector2Int LocalGridPosition { get { return new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y)); } }
}
