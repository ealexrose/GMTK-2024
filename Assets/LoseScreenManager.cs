using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseScreenManager : MonoBehaviour
{
    public GameObject LoseScreenContainer;
    public static LoseScreenManager instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else 
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void ShowLoseScreen() 
    {
        LoseScreenContainer.SetActive(true);
    }

    internal void HideLoseScreen() 
    {
        LoseScreenContainer.SetActive(false);
    }
}
