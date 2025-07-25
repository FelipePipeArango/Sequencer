using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDAnimator : MonoBehaviour
{
    [SerializeField] GameObject cardsInLevel;
    Animator[] cardAnimator;

    [SerializeField] GameObject numbersInLevel;
    public Animator[] numberAnimator;
    public NumberItem[] numberItems;

    private void OnEnable()
    {
        NumberItem.DragingNumber += AnimateNumberSlot;
        NumberItem.DragingNumber += BlinkNumber;
        CardTrigger.DroppedNumber += AnimateDrop;
        CardTrigger.DroppedNumber += NumberDrop;
    }

    private void OnDisable()
    {
        NumberItem.DragingNumber -= AnimateNumberSlot;
        NumberItem.DragingNumber -= BlinkNumber;
        CardTrigger.DroppedNumber -= AnimateDrop;
        CardTrigger.DroppedNumber -= NumberDrop;
    }

    void Start()
    {
        cardAnimator = new Animator[cardsInLevel.transform.childCount];
        for (int i = 0; i < cardAnimator.Length; i++)
        {
            cardAnimator[i] = cardsInLevel.transform.GetChild(i).GetComponent<Animator>();
        }

        numberItems = new NumberItem [numbersInLevel.transform.childCount];
        numberAnimator = new Animator[numbersInLevel.transform.childCount];
        for (int i = 0; i < numberAnimator.Length; i++)
        {
            numberItems[i] = numbersInLevel.transform.GetChild(i).GetComponent<NumberItem>();
            numberAnimator[i] = numbersInLevel.transform.GetChild(i).GetComponent<Animator>();
        }
    }

    void AnimateNumberSlot(int value, bool isDragging)
    {
        if (isDragging == true)
        {
            for (int i = 0; i < cardAnimator.Length; i++)
            {
                cardAnimator[value - 1].SetBool("isNext", true);
                cardAnimator[i].SetBool("isGrabbingNumber", isDragging);
            } 
        }
        else
        {
            for (int i = 0; i < cardAnimator.Length; i++)
            {
                cardAnimator[i].SetBool("isNext", false);
                cardAnimator[i].SetBool("isGrabbingNumber", isDragging);
            }
        }
    }

    void BlinkNumber(int value, bool isDragging)
    {
        if (isDragging == true)
        {
            for (int i = 0; i < numberItems.Length; i++)
            {
                if (numberItems[i].isActiveAndEnabled)
                {
                    numberAnimator[i].SetBool("isPlayerGrabbing", true);
                    if (numberItems[i].value == value)
                    {
                        numberAnimator[i].SetBool("isGrabbed", true);
                    }
                    else
                    {
                        numberAnimator[i].SetBool("isGrabbed", false);
                    } 
                }
            }
        }
        if (isDragging == false)
        {
            for (int i = 0; i < numberItems.Length; i++)
            {
                if (numberItems[i].isActiveAndEnabled)
                {
                    numberAnimator[i].SetBool("isPlayerGrabbing", false); 
                }
            }
        }
    }

    void NumberDrop(int value, bool isSlotted)
    {
        if (isSlotted == true)
        {
            for (int i = 0; i < numberAnimator.Length; i++)
            {
                if (numberAnimator[i].isActiveAndEnabled)
                {
                    numberAnimator[i].SetBool("isPlayerGrabbing", false);
                }
            }
        }
    }

    void AnimateDrop (int value, bool isSlotted)
    {
        if (isSlotted == true)
        {
            for (int i = 0; i < cardAnimator.Length; i++)
            {
                cardAnimator[value - 1].SetBool("isNext", false);
                cardAnimator[i].SetBool("isGrabbingNumber", false);
                cardAnimator[value - 1].SetBool("isSloted", true);
            }
        }
        else if (isSlotted == false) {
            cardAnimator[value - 1].SetBool("isSloted", false);
        }
    }

}
