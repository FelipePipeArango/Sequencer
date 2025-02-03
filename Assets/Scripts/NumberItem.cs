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
    public float proximityThreshold = 0.5f;  // The distance at which the event triggers
    public UnityEvent onProximityReached;  // The event to trigger

    private bool eventTriggered = false;  // To ensure the event only triggers once

    


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
        //Debug.Log($"This is the position {transform.position}");
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
