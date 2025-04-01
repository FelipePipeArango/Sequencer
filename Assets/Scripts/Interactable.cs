using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverCursorSwap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        UICursorController.Instance.SetCursorState(CursorState.Interact);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UICursorController.Instance.SetCursorState(CursorState.Default);
    }
}



