using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AnimController : MonoBehaviour
{
    [SerializeField] Animator animator;

    public void UpdateAnimations(bool isMoving)
    {
        animator.SetBool("isMoving", isMoving);

    }
}
