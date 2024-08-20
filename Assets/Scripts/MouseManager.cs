using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouseManager : MonoBehaviour
{

    public static MouseManager instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            Debug.LogWarning("Extra Mouse Manager Found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            List<ClickableObject> candidates = GetObjectsClicked();
            if (candidates.Any(candidate => candidate.clickableObjectType == ClickableObject.ClickableObjectType.Resident && candidate.IsObjectValidToClick()))
            {
                candidates.First(candidate => candidate.clickableObjectType == ClickableObject.ClickableObjectType.Resident && candidate.IsObjectValidToClick()).ClickObject();
            }
            else 
            {
                candidates.FirstOrDefault(candidate => candidate.IsObjectValidToClick())?.ClickObject();
            }
        }
    }

    public List<ClickableObject> GetObjectsClicked()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D[] hit = Physics2D.RaycastAll(mousePos2D, Vector2.zero);
        return hit.Where(h => h.transform.gameObject.GetComponent<ClickableObject>() != null).Select(co => co.transform.gameObject.GetComponent<ClickableObject>()).ToList();
    }
}
