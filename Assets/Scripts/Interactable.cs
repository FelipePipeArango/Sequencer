using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverCursorSwap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CursorState cursorStateOnHover = CursorState.Interact;

    public CursorState CursorState => cursorStateOnHover;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!UICursorController.Instance.isDragging)
        {
            UICursorController.Instance.SetCursorState(cursorStateOnHover);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!UICursorController.Instance.isDragging)
        {
            UICursorController.Instance.SetCursorState(CursorState.Default);
        }
    }
}