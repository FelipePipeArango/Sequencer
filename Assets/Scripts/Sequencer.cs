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

    CardTrigger[] levelCards;

    [Header ("PART OF THE HUD")] 
    [SerializeField] GameObject cardsInLevel;

    public CardTrigger lastCard { get; private set; }
    public NumberItem lastNumber { get; private set; }
    public Directions lastDirection { get; set; }

    private void OnEnable()
    {
        AICompanion.OnMove += HandleAIStateChanged;
    }

    private void OnDisable()
    {
        AICompanion.OnMove -= HandleAIStateChanged;
    }

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
            levelCards[i].slot = i + 1; //this lets the card know what slot it is occupying, it's i + 1 becasue array starts from 0, while slots start from 1.
            levelCards[i].Initialize();
        }
    }

    public void NextCard(int recievedValue)
    {
        for (int i = 0; i < levelCards.Length; i++)
        {
            cardBackground = levelCards[i].gameObject.GetComponentInChildren<Image>();

            if (i == recievedValue - 1)
            {
                        // This is the next card
                levelCards[i].nextInSequence = true;
                cardBackground.color = new Color(
                    cardBackground.color.r,
                    cardBackground.color.g,
                    cardBackground.color.b,
                    1f);
            }
            else
            {
                        // Not the next card
                levelCards[i].nextInSequence = false;

                if (!levelCards[i].available)
                {
                        // USED => keep alpha = 1
                    cardBackground.color = new Color(
                        cardBackground.color.r,
                        cardBackground.color.g,
                        cardBackground.color.b,
                        1f);
                }
                else
                {
                        // NOT used, NOT next => alpha = 0.5
                    cardBackground.color = new Color(
                        cardBackground.color.r,
                        cardBackground.color.g,
                        cardBackground.color.b,
                        0.5f);
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
        }
    }

    public void HandleAIStateChanged(bool isMoving)
    {
        if (isMoving)
        {
            DisableAll();
        }
        else
        {
            EnableNextCard();
        }
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
            if (card.nextInSequence && card != lastCard)
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
            gridManager.AIActions.isBefore = false;
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

