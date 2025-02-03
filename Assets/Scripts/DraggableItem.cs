using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Image image;
    public int value;
    [HideInInspector] public Transform parentTransform;
    [SerializeField] Sequencer sequence;
    [SerializeField] TextMeshProUGUI numberText;

    void Awake()
    {
        numberText.text = value.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentTransform = transform.parent;
        transform.SetParent(transform.root);
        image.raycastTarget = false;
        transform.SetAsLastSibling();

        sequence.ManageSequenceText(value, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentTransform);
        image.raycastTarget = true;

        sequence.ManageSequenceText(value, false);
    }
}
