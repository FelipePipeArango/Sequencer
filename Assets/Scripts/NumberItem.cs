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

    float lockedPosition = 0f;
    [SerializeField] Camera UiCamera;
    public Vector3 mousePosition;
    public Vector3 world;
    public Canvas canvas;

    private RectTransform thisRectTransform;

    void Awake()
    {
        numberText.text = value.ToString();
        thisRectTransform = GetComponent<RectTransform>();

        if (UiCamera == null)
        {
            GameObject uiCameraObj = GameObject.Find("UICamera");
            if (uiCameraObj != null)
            {
                UiCamera = uiCameraObj.GetComponent<Camera>();
            }
        }

        if (canvas == null)
        {
            GameObject canvasObj = GameObject.Find("HUD");
            if (canvasObj != null)
            {
                canvas = canvasObj.GetComponent<Canvas>();
            }
        }


    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentTransform = transform.parent;
        transform.SetParent(transform.root);
        /*Vector3 currentPosition = transform.localPosition;
        currentPosition.z = lockedPosition;
        transform.localPosition = currentPosition;*/
        image.raycastTarget = false;
        transform.SetAsLastSibling();

        if (OnDragAction != null)
        {
            OnDragAction(value, true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        /*mousePosition = Input.mousePosition;
        mousePosition.z = lockedPosition;
        world = UiCamera.ScreenToWorldPoint(mousePosition);
        //world.z = lockedPosition;
        transform.position = UiCamera.ScreenToWorldPoint(mousePosition);
        //transform.position = Input.mousePosition;*/

        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle (canvas.transform as RectTransform, eventData.position, UiCamera, out localPosition);

       mousePosition = eventData.position;
        mousePosition.z = UiCamera.nearClipPlane;  // Set the Z position to the near clip plane (or a fixed distance)

        // Convert the screen space position to world space
        Vector3 worldPos = UiCamera.ScreenToWorldPoint(mousePosition);

        Vector3 offset = Vector3.zero;

        // Calculate the offset from the original object position to the mouse position
        if (offset == Vector3.zero)
        {
            offset = transform.position - worldPos;
        }

        // Update the object's position in world space
        thisRectTransform.localPosition = localPosition;
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
