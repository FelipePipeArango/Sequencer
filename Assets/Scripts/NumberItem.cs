using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class NumberItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Image image;
    public int value;
    [HideInInspector] public Transform parentTransform;
    [SerializeField] TextMeshProUGUI numberText;

    public delegate void DragActions(int number, bool isGrabing);
    public static event DragActions OnDragAction;

    public Transform target;  // The target object the player or item approaches
    
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

        if (OnDragAction != null)
        {
            OnDragAction(value, true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentTransform);
        image.raycastTarget = true;

        if (OnDragAction != null)
        {
            OnDragAction(value, false);
        }
    }
}
