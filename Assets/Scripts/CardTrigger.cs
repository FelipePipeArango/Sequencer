using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class CardTrigger : MonoBehaviour, IDropHandler
{
    [SerializeField] Sequencer cardManager;
    [SerializeField] TextMeshProUGUI usedText;
    [SerializeField] Image slotImage;
    [SerializeField] Image cardBackground;
    [HideInInspector] public bool available = true; //tracks if the card has been used
    [HideInInspector] public bool nextInSequence;

    enum Dropdown
    {
        Move, PickUp, Throw, Enable
    }
    [SerializeField] Dropdown action;

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
            //cardBackground.color = new Color(cardBackground.color.r, cardBackground.color.g, cardBackground.color.b, 0.5f);
            usedText.text = draggableItem.value.ToString();
            cardManager.RecieveInfo(draggableItem.value, action);
            available = false;

            cardManager.ManageSequenceText(0, false);
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
