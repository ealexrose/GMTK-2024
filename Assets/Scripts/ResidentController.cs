using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResidentController : MonoBehaviour, IClickable
{
    public bool debugMessages;

    public Resident resident;
    public enum ResidentState
    {
        WaitingToMove,
        FollowingMouse,
        LockedIn,
    }

    public ResidentState residentState;
    // Start is called before the first frame update
    void Start()
    {
        residentState = ResidentState.WaitingToMove;
        resident.gridCoordinates = LocalGridPosition;
        ResidentManager.instance.RegisterResident(this);
    }

    // Update is called once per frame
    void Update()
    {

        if (residentState == ResidentState.FollowingMouse)
        {
            MoveTowardsMouseGridPosition();
            targetPositionIsValid = CheckIfPositionIsValid();

            if (targetPositionIsValid)
            {
                MarkAsValid();
            }
            else
            {
                MarkAsInvalid();
            }

            if (Input.GetMouseButtonUp(0))
            {
                AttemptToPlace();
            }
        }
    }

    internal void LockIn()
    {
        residentState = ResidentState.LockedIn;
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
    private Vector3 originalGridPosition;
    private bool targetPositionIsValid;
    private Vector2Int LocalGridPosition { get { return new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y)); } }
    private void MoveTowardsMouseGridPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector2(mousePos.x, mousePos.y);
        Vector2 mouseGridPosition = new Vector2(Mathf.Floor(mousePosition.x), Mathf.Floor(mousePosition.y));
        transform.position = mouseGridPosition - localMouseGridPosition;
    }

    public void Pickup()
    {
        residentState = ResidentState.FollowingMouse;
        originalGridPosition = transform.position;
        PlaceInFront();
    }

    private void AttemptToPlace()
    {
        residentState = ResidentState.WaitingToMove;
        if (!targetPositionIsValid)
        {
            transform.position = originalGridPosition;
        }
        else
        {
            Apartment apartmentAtPosition = BuildingManager.instance.GetApartmentAtPosition(LocalGridPosition);
            if (resident.homeApartment != null && apartmentAtPosition != resident.homeApartment)
                MoveResidentOut();
            if (apartmentAtPosition != null)
            {
                MoveResidentIn(apartmentAtPosition);
            }
            resident.gridCoordinates = LocalGridPosition;
        }
        MarkAsValid();
        PlaceBehind();
        Debug.Log("All Residents in Apartments? " + ResidentManager.instance.AllResidentsAreInBuildings());
    }

    private void MoveResidentIn(Apartment apartmentAtPosition)
    {
        apartmentAtPosition.resident = resident;
        resident.homeApartment = apartmentAtPosition;
    }

    private void MoveResidentOut()
    {
        resident.homeApartment.resident = new Resident();
        resident.homeApartment = null;
    }

    #endregion

    #region visuals
    public List<SpriteRenderer> sprites;

    public void MarkAsInvalid()
    {
        sprites.ForEach(x => x.color = Color.gray);
    }

    public void MarkAsValid()
    {
        sprites.ForEach(x => x.color = Color.white);
    }

    public void PlaceInFront()
    {
        sprites.ForEach(x => x.sortingOrder = 10);
    }

    public void PlaceBehind()
    {
        sprites.ForEach(x => x.sortingOrder = 6);
    }
    #endregion

    private bool CheckIfPositionIsValid()
    {
        bool notBlockedByApartment = BuildingManager.instance.CheckIfTargetResidentPositionIsValid(LocalGridPosition, resident);
        bool notBlockedByResident = !ResidentManager.instance.IsSpaceOccupiedByResident(LocalGridPosition, resident);
        return notBlockedByApartment && notBlockedByResident;
    }
    public bool IsValidClickable()
    {
        if (residentState != ResidentState.WaitingToMove)
            return false;

        return true;
    }

    public bool ClickedSelect()
    {
        if (residentState != ResidentState.WaitingToMove)
            return false;

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
        Pickup();
        return true;
    }

    public bool ClickRelease()
    {
        return true;
    }
}
