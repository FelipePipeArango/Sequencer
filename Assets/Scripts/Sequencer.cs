using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static GameActions;
using static GameDirections;
using static GridManager;

public class Sequencer : MonoBehaviour
{
    public static Sequencer sequencer { get; private set; }

    Image cardBackground;
    Image notNextCardCover;

    CardTrigger[] levelCards;

    [Header ("PART OF THE HUD")] 
    [SerializeField] GameObject cardsInLevel;

    public CardTrigger lastCard { get; private set; }
    public bool state { get; private set; }
    public NumberItem lastNumber { get; private set; }
    public Directions lastDirection { get; set; }

    public void Awake()
    {
        if (sequencer != null && sequencer != this)
        {
            Destroy(gameObject);
            return;
        }
        sequencer = this;
        FillCards();
    }

    public void FillCards()
    {
        levelCards = new CardTrigger[cardsInLevel.transform.childCount];

        for (int i = 0; i < levelCards.Length; i++)
        {
            levelCards[i] = cardsInLevel.transform.GetChild(i).GetComponent<CardTrigger>();
            levelCards[i].SetSlotNumber(i + 1);
            levelCards[i].Initialize();
        }
    }

    public void NextCard(int recievedValue)
    {
        for (int i = 0; i < levelCards.Length; i++)
        {
            cardBackground = levelCards[i].gameObject.GetComponentInChildren<Image>();
            notNextCardCover = levelCards[i].notNextCover;

            if (i == recievedValue - 1)
            {  // This is the next card
                levelCards[i].nextInSequence = true;
                notNextCardCover.gameObject.SetActive(false);
            }
            else
            {   // Not the next card
                levelCards[i].nextInSequence = false;

                if (levelCards[i].available)
                {
                    notNextCardCover.gameObject.SetActive(true);
                }
            }
        }
    }

    public void CommunicateAIActions(bool isBefore, Directions direction)
    {
        if (gridManager.AIActions != null)
        {
            gridManager.AIActions.direction = direction;
            gridManager.AIActions.action = AIActions.Move;
            lastDirection = direction;
            gridManager.AIActions.canMove = isBefore;
            gridManager.AIActions.isBefore = isBefore;
            gridManager.AIActions.isIAActive = true;
        }
    }
    //hdalksjdh

    public void HandleStateChange(bool isMoving)
    {
        if (isMoving)
        {
            DisableAll();
        }
        else
        {
            EnableNextCard();
            EnableCard();
        }
        state = isMoving;
    }
    private void EnableCard()
    {
        foreach (var card in levelCards)
        {
            if (card.available || card.isUsed == false)
            {
                card.Enable();
            }
        }
        NextCard(lastNumber.value);
    }
    private void DisableAll()
    {
        foreach (var card in levelCards)
        {
            card.Disable();

            cardBackground = card.gameObject.GetComponentInChildren<Image>();
            if (!card.available)
            {
                // If it's used => alpha = 1
                cardBackground.color = new Color(
                    cardBackground.color.r,
                    cardBackground.color.g,
                    cardBackground.color.b,
                    1f);
            }
            else
            {
                // If it's not used => alpha = 0.5
                cardBackground.color = new Color(
                    cardBackground.color.r,
                    cardBackground.color.g,
                    cardBackground.color.b,
                    0.5f);
            }
        }
    }

    private void EnableNextCard()
    {
        foreach (var card in levelCards)
        {
            if (card.nextInSequence && 
                card != lastCard && 
                card.isUsed != true)
            {
                card.Enable();
            }
        }
        NextCard(lastNumber.value);
    }

    public void AIAfterAction()
    {
        if (gridManager.AIActions != null
            && gridManager.AIActions.action != AIActions.Stay)
        {
            gridManager.AIActions.direction = lastDirection;
            gridManager.AIActions.canMove = true;
        }
    }

    public void CommunicateAction(NumberItem recievedNumber, Actions usedAction)
    {
        for (int i = 0; i < levelCards.Length; i++)
        {
            // Must match the usedAction AND be "inUse == true"
            if (usedAction == levelCards[i].cardAction && levelCards[i].isInUse == true)
            {
                lastCard = levelCards[i];
                lastNumber = recievedNumber;

                if (!levelCards[i].hasArrow)
                {
                    if (levelCards[i].cardAction == Actions.Enable)
                        levelCards[recievedNumber.value - 1].Enable();
                    else
                        levelCards[i].ExecuteAction(recievedNumber);

                    levelCards[i].DisableUsed(recievedNumber);
                    NextCard(recievedNumber.value);
                    break;
                }
                else
                {
                    if (!levelCards[i].isAIBefore)
                    {
                        if (levelCards[i].cardAction == Actions.Enable)
                            levelCards[recievedNumber.value - 1].Enable();
                        else
                            levelCards[i].ExecuteAction(recievedNumber);

                        levelCards[i].DisableUsed(recievedNumber);
                        NextCard(recievedNumber.value);
                        break;
                    }
                    else
                    {
                        // If isAIBefore == true
                        levelCards[i].DisableUsed(recievedNumber);
                        NextCard(recievedNumber.value);
                        break;
                    }
                }
            }
        }
    }

    public void PlayerAfterAction()
    {
        lastCard.ExecuteAction(lastNumber);
    }
}

