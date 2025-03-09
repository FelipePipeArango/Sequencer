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

    [SerializeField] 
    Image nextCardText;
    Image cardBackground;

    CardTrigger[] levelCards;
    GameObject[] allCards;

    public CardTrigger lastCard { get; private set; }
    public NumberItem lastNumber { get; private set; }
    public Directions lastDirection { get; set; }

    //Makes sure that only the next card in the sequence is considered as "next". Cards that are not next in the sequence are not avaiable to use.

    private void OnEnable()
    {      
        AICompanion.OnMove += HandleAIStateChanged;
        CardTrigger.OnGrab += ManageSequenceText;
        NumberItem.OnDragAction += ManageSequenceText;
    }

    private void OnDisable()
    {
        AICompanion.OnMove -= HandleAIStateChanged;
        CardTrigger.OnGrab -= ManageSequenceText;
        NumberItem.OnDragAction -= ManageSequenceText;
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
        allCards = GameObject.FindGameObjectsWithTag("Card");

        if (allCards != null)
        {
            CardTrigger[] cardPlaceholder = new CardTrigger[allCards.Length];

            for (int i = 0; i < allCards.Length; i++)
                cardPlaceholder[i] = allCards[i].GetComponent<CardTrigger>();

            levelCards = new CardTrigger[allCards.Length];

            for (int i = 0; i < levelCards.Length; i++)
            {
                levelCards[cardPlaceholder[i].numberInQueue] = cardPlaceholder[i];
                levelCards[cardPlaceholder[i].numberInQueue].Initialize();
            }
        }
    }

    public void NextCard(int recievedValue)
    {
        for (int i = 0; i < levelCards.Length; i++)
        {
            if (i == recievedValue - 1)
            {
                levelCards[i].nextInSequence = true;
                cardBackground = levelCards[i].gameObject.GetComponentInChildren<Image>();

                cardBackground.color = new Color(
                    cardBackground.color.r,
                    cardBackground.color.g,
                    cardBackground.color.b,
                    1);
            }
            else
            {
                levelCards[i].nextInSequence = false;
                cardBackground = levelCards[i].gameObject.GetComponentInChildren<Image>();

                cardBackground.color = new Color(
                    cardBackground.color.r,
                    cardBackground.color.g,
                    cardBackground.color.b,
                    0.5f);
            }
        }
    }
    //Manages the "next" text on the actions.
    public void ManageSequenceText(int recievedNumber, bool dragging)
    {
        if (dragging == true)
        {
            for (int i = 0; i < levelCards.Length; i++)
            {
                if (i == recievedNumber - 1)
                {
                    nextCardText.gameObject.SetActive(true);
                    nextCardText.transform.position =
                        new Vector3(
                            levelCards[i].transform.position.x,
                            levelCards[i].transform.position.y + 92, 
                            levelCards[i].transform.position.z);
                }
            }
        }
        else
        {
            nextCardText.gameObject.SetActive(false);
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
            cardBackground.color = new Color(cardBackground.color.r, cardBackground.color.g, cardBackground.color.b, 0.5f);
        }
    }
    private void EnableNextCard()
    {
        foreach (var card in levelCards)
        {
            if (card.nextInSequence && 
                card != lastCard)
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
            if (usedAction == levelCards[i].LevelActions //The card slot that's equal to the recieved number
                && levelCards[i].isInUse == true) //allows for multiple cards of the same type
            {
                lastCard = levelCards[i];
                lastNumber = recievedNumber;

                if (levelCards[i].hasArrow != true)
                {
                    if (levelCards[i].LevelActions == Actions.Enable)
                        levelCards[recievedNumber.value - 1].Enable();
                    else
                        levelCards[i].ExecuteAction(recievedNumber);

                    levelCards[i].DisableUsed(recievedNumber);
                    NextCard(recievedNumber.value);
                   
                    break;
                }
                else
                {
                    if (levelCards[i].isAIBefore != true)
                    {
                        if (levelCards[i].LevelActions == Actions.Enable)
                            levelCards[recievedNumber.value - 1].Enable();
                        else
                            levelCards[i].ExecuteAction(recievedNumber);

                        levelCards[i].DisableUsed(recievedNumber);
                        NextCard(recievedNumber.value);
                        
                        break;
                    }
                    else
                    {
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
