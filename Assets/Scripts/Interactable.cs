using UnityEngine;

public class Interactable : MonoBehaviour
{
    void OnMouseEnter()
    {
        if (UIHandler.Instance != null)
        {
            UIHandler.Instance.SetHoverState(true);
        }
    }

    void OnMouseExit()
    {
        if (UIHandler.Instance != null)
        {
            UIHandler.Instance.SetHoverState(false);
        }
    }
}

