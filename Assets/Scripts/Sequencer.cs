using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static GameActions;
using static GridManager;

public class Sequencer : MonoBehaviour
{
    public static Sequencer sequencer { get; private set; }

    Image cardBackground;
    [SerializeField] Image nextCardText;

    CardTrigger[] levelCards;
    //GameActions.Actions[] levelActions;
    public int[] queue;
    GameObject[] allCards;

    //Makes sure that only the next card in the sequence is considered as "next". Cards that are not next in the sequence are not avaiable to use.
   
    private void OnEnable()
    {
        CardTrigger.OnGrab += ManageSequenceText;
        NumberItem.OnDragAction += ManageSequenceText;
    }

    private void OnDisable()
    {
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
            levelCards = new CardTrigger[allCards.Length];
           
            for (int i = 0; i < allCards.Length; i++)            
                levelCards[i] = allCards[i].GetComponent<CardTrigger>();

            CardTrigger[] cardPlaceholder = new CardTrigger[levelCards.Length];

            for (int i = 0; i < levelCards.Length; i++)
                cardPlaceholder[levelCards[i].numberInQueue] = levelCards[i];

            levelCards = cardPlaceholder;
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
                cardBackground.color = new Color(cardBackground.color.r, cardBackground.color.g, cardBackground.color.b, 1);
            }
            else
            {
                levelCards[i].nextInSequence = false;
                cardBackground = levelCards[i].gameObject.GetComponentInChildren<Image>();
                cardBackground.color = new Color(cardBackground.color.r, cardBackground.color.g, cardBackground.color.b, 0.5f);
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
                        new Vector3(levelCards[i].transform.position.x
                            , levelCards[i].transform.position.y + 92
                            , levelCards[i].transform.position.z);
                }
            }
        }
        else
        {
            nextCardText.gameObject.SetActive(false);
        }
    }

    public void TriggerCard(bool isBefore, GameActions.AIActions direction)
    {
        gridManager.AIActions.action = direction;
        gridManager.AIActions.canMove = isBefore;
        gridManager.AIActions.isBefore = isBefore;
    }

    public void CommunicateAction(NumberItem recievedNumber, GameActions.Actions usedAction)
    {
        //its supposed to be here but where exectly and what do I need for it to work
        for (int i = 0; i < levelCards.Length; i++)
        {
            //BUG found it does not allow for multiple cards for the first action
            //TODO need to make the clear which card is in use

                if (usedAction == levelCards[i].LevelActions //The card slot that's equal to the recieved number
                    && levelCards[i].available == true) //allows for multiple cards of the same type
                {
                    if (usedAction == GameActions.Actions.Enable)
                        levelCards[recievedNumber.value - 1].Enable(false, recievedNumber);                                            
                    else
                        PlayerAfterAction(recievedNumber.value, usedAction);
                    
                    levelCards[i].Disable(recievedNumber);
                    NextCard(recievedNumber.value);

                    break;
                }
        }
    }
    public void PlayerAfterAction(int recievedNumber,GameActions.Actions usedAction)
    {
        switch (usedAction)
        {
            case GameActions.Actions.Move:
                gridManager.playerActions.MovementReceiver(recievedNumber, usedAction);
                break;

            case GameActions.Actions.PickUp:
                gridManager.playerActions.PickUpReceiver(recievedNumber, usedAction);
                break;

            case GameActions.Actions.Throw:
                gridManager.playerActions.ThrowReceiver(recievedNumber, usedAction);
                break;
        }
    }
}
