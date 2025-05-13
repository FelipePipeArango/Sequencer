using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AnimController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] bool  isMoving = false;

    void Update()
    {
        //animator.SetBool("isMoving", isMoving);
    }

    public void UpdateAnimations()
    {
        isMoving = true;
        animator.SetBool("isMoving", isMoving);
    }
}
