using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Sequencer : MonoBehaviour
{
    //[SerializeField] int recievedValue = 0;
    //[SerializeField] CardSO[] cardArray = new CardSO[] { };
    Image cardBackground;
    [SerializeField] Image nextCardText;
    [SerializeField] UnitControler unitControler;
    [SerializeField] Effect[] availableEffects;
    [SerializeField] CardTrigger[] availableCards;

    enum Effect
    {
        Move, PickUp, Throw, Enable
    }

    public void RecieveInfo(int recievedValue, Enum effect)
    {
        for (int i = 0; i < availableEffects.Length; i++)
        {
            if (effect.ToString() == availableEffects[i].ToString())
            {
                switch (availableEffects[i])
                {
                    case Effect.Move:
                        unitControler.MovementReceiver(recievedValue);
                        break;

                    case Effect.PickUp:
                        unitControler.PickUpReceiver(recievedValue);
                        break;
                    
                    case Effect.Enable:
                        availableCards[recievedValue - 1].Enable();
                        break;

                    case Effect.Throw:
                        unitControler.ThrowReceiver(recievedValue);
                        break;
                }
            }

        }
        NextCard(recievedValue);
    }

    void NextCard(int recievedValue)
    {
        for (int i = 0; i < availableEffects.Length; i++)
        {
            if (i == recievedValue - 1)
            {
                availableCards[i].nextInSequence = true;
                cardBackground = availableCards[i].gameObject.GetComponentInChildren<Image>();
                cardBackground.color = new Color(cardBackground.color.r, cardBackground.color.g, cardBackground.color.b, 1);
            }
            else
            {
                availableCards[i].nextInSequence = false;
                cardBackground = availableCards[i].gameObject.GetComponentInChildren<Image>();
                cardBackground.color = new Color(cardBackground.color.r, cardBackground.color.g, cardBackground.color.b, 0.5f);
            }
        }
    }

    public void ManageSequenceText(int recievedNumber, bool dragging)
    {
        if (dragging == true)
        {
            for (int i = 0; i < availableEffects.Length; i++)
            {
                if (i == recievedNumber - 1)
                {
                    nextCardText.gameObject.SetActive(true);
                    nextCardText.transform.position = new Vector3(availableCards[i].transform.position.x, availableCards[i].transform.position.y + 92, availableCards[i].transform.position.z);
                }
            }
        }
        else
        {
            
            nextCardText.gameObject.SetActive(false);
        }
    }
}
