using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ApartmentController : MonoBehaviour, IClickable
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
        apartment.IsInPlacementZone = ApartmentInZone;

        targetPositionIsValid = CheckIfPositionIsValid();
        if (targetPositionIsValid)
        {
            if (ApartmentInZone)
            {
                MarkAsValid();
            }
            else
            {
                MarkAsPartiallyValid();
            }
        }
        else
        {
            MarkAsInvalid();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (apartmentState == ApartmentState.FollowingMouse)
        {
            MoveTowardsMouseGridPosition();
            targetPositionIsValid = CheckIfPositionIsValid();

            if (targetPositionIsValid)
            {
                if (ApartmentInZone)
                {
                    MarkAsValid();
                }
                else
                {
                    MarkAsPartiallyValid();
                }

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
        apartmentState = ApartmentState.LockedIn;
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
    private void MoveTowardsMouseGridPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector2(mousePos.x, mousePos.y);
        Vector2 mouseGridPosition = new Vector2(Mathf.Floor(mousePosition.x), Mathf.Floor(mousePosition.y));
        transform.position = mouseGridPosition - localMouseGridPosition;
    }

    public void Pickup()
    {
        apartmentState = ApartmentState.FollowingMouse;
        originalGridPosition = transform.position;
        PlaceInFront();
    }

    private void AttemptToPlace()
    {
        apartmentState = ApartmentState.WaitingToMove;
        if (!targetPositionIsValid)
        {
            transform.position = originalGridPosition;
            MarkAsValid();
        }
        else
        {
            MoveApartment(LocalGridPosition);
        }
        PlaceBehind();
    }

    #endregion

    private bool CheckIfPositionIsValid()
    {
        return BuildingManager.instance.CheckIfTargetApartmentPositionIsValid(LocalGridPosition, apartment);
    }

    #region visuals
    public List<SpriteRenderer> sprites;

    public void MarkAsInvalid()
    {
        sprites.ForEach(x => x.color = Color.red);
    }

    public void MarkAsValid()
    {
        sprites.ForEach(x => x.color = Color.white);
    }

    public void MarkAsPartiallyValid()
    {
        sprites.ForEach(x => x.color = Color.gray);
    }

    public void PlaceInFront()
    {
        sprites.ForEach(x => x.sortingOrder = 5);
    }

    public void PlaceBehind()
    {
        sprites.ForEach(x => x.sortingOrder = 1);
    }

    #endregion

    private void InitializeApartment()
    {
        apartment.gridPosition = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        foreach (ApartmentBlock apartmentBlock in apartment.apartmentBlocks)
        {
            apartmentBlock.parentApartment = apartment;
        }
        BuildingManager.instance.RegisterApartment(this);
    }

    private void MoveApartment(Vector2Int newPosition)
    {
        BuildingManager.instance.MoveApartment(apartment, newPosition);
        apartment.IsInPlacementZone = ApartmentInZone;
    }

    public bool IsValidClickable()
    {
        if (apartmentState != ApartmentState.WaitingToMove)
            return false;

        return true;
    }

    public bool ClickedSelect()
    {
        if (apartmentState != ApartmentState.WaitingToMove)
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
        throw new NotImplementedException();
    }

    private Vector2Int LocalGridPosition { get { return new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y)); } }
    public bool ApartmentInZone { get { return PlacementAreaManager.instance.IsFootprintInBounds(LocalGridPosition, apartment.footPrint); } }
}
