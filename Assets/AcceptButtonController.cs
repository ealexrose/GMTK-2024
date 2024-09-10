using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceptButtonController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ProgressRound() 
    {
        GameManager.instance.ProgressRound();
    }
}
