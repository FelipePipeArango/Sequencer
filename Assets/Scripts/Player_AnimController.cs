using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerStates;

public class Player_AnimController : MonoBehaviour
{
    [SerializeField] Animator animator;

    [HideInInspector] public bool animationChange;

    private void OnEnable()
    {
        PlayerManager.playerDidSomething += AnimateAction;
    }

    private void OnDisable()
    {
        PlayerManager.playerDidSomething -= AnimateAction;
    }

    private void FixedUpdate()
    {
        if (animationChange)
        {
            foreach (var param in animator.parameters)
            {
                if (param.type == AnimatorControllerParameterType.Bool)
                {
                    animator.SetBool(param.name, false);
                    animationChange = false;
                }
            }
        }
    }

    void AnimateAction(playerStates currentPlayerState)
    {
        switch (currentPlayerState)
        {
            case playerStates.Idle:
                break;

            case playerStates.Celebrating:
                AnimatePlayerVictory();
                break;

            case playerStates.Moving:
                AnimatePlayerMovement();
                break;

            case playerStates.PickingUp:
                AnimatePlayerPickingUp();
                break;  
        }
    }
    void AnimatePlayerVictory()
    {
        animator.SetBool("Completed", true);
    }

    void AnimatePlayerMovement()
    {
        animator.SetBool("isMoving", true);
    }

    void AnimatePlayerPickingUp()
    {
        animator.SetBool("pickedUp", true);
    }
}
