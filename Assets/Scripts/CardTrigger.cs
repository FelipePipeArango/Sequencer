using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class CardTrigger : MonoBehaviour, IDropHandler
{
    [SerializeField] TextMeshProUGUI usedText;
    [SerializeField] Image slotImage;
    [SerializeField] Image cardBackground;
    bool available = true; //tracks if the card has been used
    [HideInInspector] public bool nextInSequence;

    public GameActions.Actions LevelActions;

    public delegate void GrabActions(int number, bool isGrabing);
    public static event GrabActions OnGrab;

    public delegate void GrabTest(NumberItem test, GameActions.Actions action);
    public static event GrabTest OnTest;

    public delegate void DropActions(int number, GameActions.Actions action);
    public static event DropActions OnDropAction;

    public void OnDrop(PointerEventData eventData)
    {
        if (available == true && nextInSequence == true)
        {
            GameObject dropped = eventData.pointerDrag;
            NumberItem draggableItem = dropped.GetComponent<NumberItem>();
            //draggableItem.parentTransform = transform;
            if (OnTest != null)
            {
                OnTest (draggableItem, LevelActions);
            }

            //slotImage.gameObject.SetActive(false);
            //usedText.gameObject.SetActive(true);

            //usedText.text = draggableItem.value.ToString();
            //available = false;

            if (OnDropAction != null)
            {
                OnDropAction(draggableItem.value, LevelActions); //Tells the manager the value and action used, with the refactor this should no longer be necesary
            }

            if (OnGrab != null)
            {
                OnGrab(0, false); //Communicates with the sequencer whengrabing a number.
            }
        }
    }

    public void Disable(NumberItem number)
    {
        if (available)
        {
            usedText.gameObject.SetActive(true);
            slotImage.gameObject.SetActive(false);
            usedText.text = number.value.ToString();
            number.transform.SetParent(number.parentTransform);
            number.gameObject.SetActive(false);
            available = false;
        }
        else
        {
            return;
        }
    }

    public void Enable(bool undo, NumberItem number)
    {
        if (!undo) //this check if a card is being enabled through the Undo function of the game, or thorugh the Enable card action.
        {
            if (!available) //if it's not through undo (therefore, using the Enable action), then it does not return the used numbers.
            {
                available = true;
                usedText.gameObject.SetActive(false);
                slotImage.gameObject.SetActive(true);
            }
            else
            {
                return;
            } 
        }
        else
        {
            usedText.gameObject.SetActive(false);
            slotImage.gameObject.SetActive(true);
            //number.transform.SetParent(number.parentTransform);
            number.image.raycastTarget = true;
            number.gameObject.SetActive(true);
            available = true;
        }
    }
}
