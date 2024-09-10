using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResidenAnimationController : MonoBehaviour
{
    public ResidentController residentController;
    public Animator animator;
    public enum ResidentAnimationState
    {
        Idle,
        Running,
        Mad,
        Jumping,
        Falling
    }

    public ResidentAnimationState residentAnimationState = ResidentAnimationState.Idle;
    public void Update()
    {
        ResidentAnimationState newAnimationState = DetermineAnimationState();

        if (newAnimationState != residentAnimationState)
        {
            switch (newAnimationState)
            {
                case ResidentAnimationState.Idle:
                    animator.SetTrigger("Idle");
                    break;
                case ResidentAnimationState.Running:
                    break;
                case ResidentAnimationState.Mad:
                    animator.SetTrigger("Mad");
                    break;
                case ResidentAnimationState.Jumping:
                    break;
                case ResidentAnimationState.Falling:
                    break;
            }

            residentAnimationState = newAnimationState;
        }
    }

    public ResidentAnimationState DetermineAnimationState()
    {
        if (ResidentIsUnhappy())
            return ResidentAnimationState.Mad;

        return ResidentAnimationState.Idle;
    }

    bool ResidentIsUnhappy()
    {
        if (residentController.resident.residentMood == Resident.ResidentMood.Mad)
            return true;

        return false;
    }
}
