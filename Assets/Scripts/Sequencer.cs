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
    [SerializeField] UnitControler unitControler;

    CardTrigger[] levelCards;
    GameActions.Actions[] levelActions;

    [SerializeField] GameObject cardParent;

    public void Awake()
    {
        #region CardFilling
        //This checks the cards being used in the level and fills the required arrays
        levelCards = new CardTrigger[cardParent.transform.childCount];
        levelActions = new GameActions.Actions[cardParent.transform.childCount];

        for (int i = 0; i < levelCards.Length; i++)
        {
            levelCards[i] = cardParent.transform.GetChild(i).GetComponent<CardTrigger>();
            levelActions[i] = levelCards[i].actionsTest;
        }
        #endregion
    }

    public void RecieveInfo(int recievedValue, Enum effect)
    {
        for (int i = 0; i < levelActions.Length; i++)
        {
            if (effect.ToString() == levelActions[i].ToString())
            {
                switch (levelActions[i])
                {
                    case GameActions.Actions.Move:
                        unitControler.MovementReceiver(recievedValue);
                        break;

                    case GameActions.Actions.PickUp:
                        unitControler.PickUpReceiver(recievedValue);
                        break;

                    case GameActions.Actions.Enable:
                        levelCards[recievedValue - 1].Enable();
                        break;

                    case GameActions.Actions.Throw:
                        unitControler.ThrowReceiver(recievedValue);
                        break;
                }
            }
        }
        NextCard(recievedValue);
    }

    void NextCard(int recievedValue)
    {
        for (int i = 0; i < levelActions.Length; i++)
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
