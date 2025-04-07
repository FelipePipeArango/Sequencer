using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverCursorSwap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CursorState CursorState = CursorState.Interact;

    public void OnPointerEnter(PointerEventData eventData)
    {
        UICursorController.Instance.SetCursorState(CursorState);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UICursorController.Instance.SetCursorState(CursorState.Default);
    }
}


