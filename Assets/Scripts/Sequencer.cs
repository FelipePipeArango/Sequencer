using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Sequencer : MonoBehaviour
{
    Image cardBackground;
    [SerializeField] Image nextCardText;

    CardTrigger[] levelCards;
    GameActions.Actions[] levelActions;

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

    public void FillCards(GameObject cardsInLevel)
    {
        #region CardFilling
        //This checks the cards being used in the level and fills the required arrays
        levelCards = new CardTrigger[cardsInLevel.transform.childCount];
        levelActions = new GameActions.Actions[cardsInLevel.transform.childCount];

        for (int i = 0; i < levelCards.Length; i++)
        {
            levelCards[i] = cardsInLevel.transform.GetChild(i).GetComponent<CardTrigger>();
            levelActions[i] = levelCards[i].LevelActions;
        }
        #endregion
    }
    public void UndoSequence(NumberItem previousValue, NumberItem currentValue)
    {
        for (int i = 0; i < levelActions.Length; i++)
        {
            if (i == previousValue.value - 1) //returns to the action that was just used, the -1 is only because the array starts at 0, while the used number starts at 1
            {
                Debug.Log("numbero del turno anterior: " + previousValue.value);
                levelCards[i].nextInSequence = true;
                levelCards[i].Enable(true, currentValue);
                cardBackground = levelCards[i].gameObject.GetComponentInChildren<Image>();
                cardBackground.color = new Color(cardBackground.color.r, cardBackground.color.g, cardBackground.color.b, 1f);
            }
            else
            {
                levelCards[i].nextInSequence = false;
                cardBackground = levelCards[i].gameObject.GetComponentInChildren<Image>();
                cardBackground.color = new Color(cardBackground.color.r, cardBackground.color.g, cardBackground.color.b, 0.5f);
            }
        }
    }

    public void NextCard(NumberItem recievedValue)
    {
        for (int i = 0; i < levelActions.Length; i++)
        {
            if (i == recievedValue.value - 1)
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
            for (int i = 0; i < levelActions.Length; i++)
            {
                if (i == recievedNumber - 1)
                {
                    nextCardText.gameObject.SetActive(true);
                    nextCardText.transform.position = new Vector3(levelCards[i].transform.position.x, levelCards[i].transform.position.y + 92, levelCards[i].transform.position.z);
                }
            }
        }
        else
        {
            nextCardText.gameObject.SetActive(false);
        }
    }
}
