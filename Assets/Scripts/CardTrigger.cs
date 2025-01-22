using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class CardTrigger : MonoBehaviour, IDropHandler
{
    [SerializeField] TextMeshProUGUI usedText;
    [SerializeField] Image slotImage;
    [SerializeField] Image cardBackground;
    [HideInInspector] public bool available = true; //tracks if the card has been used
    [HideInInspector] public bool nextInSequence;

    public GameActions.Actions LevelActions;

    public delegate void GrabActions(int number, bool isGrabing);
    public static event GrabActions OnGrab;

    public delegate void DropActions(int number, GameActions.Actions action);
    public static event DropActions OnDropAction;

    public void OnDrop(PointerEventData eventData)
    {
        if (available == true && nextInSequence == true)
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            draggableItem.parentTransform = transform;
            draggableItem.gameObject.SetActive(false);

            slotImage.gameObject.SetActive(false);
            usedText.gameObject.SetActive(true);

            usedText.text = draggableItem.value.ToString();
            available = false;

            if (OnDropAction != null)
            {
                OnDropAction(draggableItem.value, LevelActions);
            }

            if (OnGrab != null)
            {
                OnGrab(0, false);
            }
        }
    }

    public void Enable()
    {
        if (!available)
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
}
