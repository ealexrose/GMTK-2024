using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickable 
{
    public bool IsValidClickable();
    public bool ClickedSelect();

    public bool ClickRelease();
}
