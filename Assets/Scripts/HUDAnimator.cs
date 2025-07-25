using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDAnimator : MonoBehaviour
{
    [SerializeField] GameObject cardsInLevel;
    public Animator[] cardAnimator;

    private void OnEnable()
    {
        NumberItem.DragingNumber += AnimateNumber;
        CardTrigger.DroppedNumber += AnimateNumber;
    }

    private void OnDisable()
    {
        NumberItem.DragingNumber -= AnimateNumber;
        CardTrigger.DroppedNumber -= AnimateNumber;
    }

    void Start()
    {
        cardAnimator = new Animator[cardsInLevel.transform.childCount];
        for (int i = 0; i < cardAnimator.Length; i++)
        {
            cardAnimator[i] = cardsInLevel.transform.GetChild(i).GetComponent<Animator>();
        }
    }

    void AnimateNumber(int value, bool isDragging)
    {
        if (isDragging == true)
        {
            for (int i = 0; i < cardAnimator.Length; i++)
            {
                cardAnimator[value - 1].SetBool("isNext", true);
                cardAnimator[i].SetBool("isGrabbingNumber", true);
            } 
        }
        else
        {
            for (int i = 0; i < cardAnimator.Length; i++)
            {
                cardAnimator[i].SetBool("isNext", false);
                cardAnimator[i].SetBool("isGrabbingNumber", false);
            }
        }
    }

}
