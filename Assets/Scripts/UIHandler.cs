using UnityEngine;
using TMPro;


public class UIHandler : MonoBehaviour
{
    public static UIHandler UIHandlerInstance;

    [SerializeField] public CompleteScreen goalSequence;
    [SerializeField] UIMessages uiMessanger;
    [SerializeField] TMP_Text characterMoveUI;
    [SerializeField] GameObject keyItemHUD;

    private bool isHoveringOverInteractable = false;

    void Awake()
    {
        if (UIHandlerInstance != null && UIHandlerInstance != this)
        {
            Destroy(gameObject);
            return;
        }
        UIHandlerInstance = this;
    }

    void Start()
    {
        keyItemHUD.SetActive(false);
        UpdateMovementPointsText(0);
    }

    public void CheckForKeyItem(bool doesPlayerHaveUSB)
    {
        keyItemHUD.SetActive(doesPlayerHaveUSB);
    }

    public void OpenWinScreen()
    {
        goalSequence.OpenCompleteScreen();
    }

    public void SetHoverState(bool isHovering)
    {
        isHoveringOverInteractable = isHovering;
    }

    public void UpdateMovementPointsText(int numberValue)
    {
        if (characterMoveUI)
            characterMoveUI.text = numberValue.ToString();
    }

    public void HideKeyItemHUD()
    {
        if (keyItemHUD != null)
            keyItemHUD.SetActive(false);
    }

    public void TriggerNoItemMessage(bool isThrowAction)
    {
        uiMessanger.RecieveNoItemMessage(isThrowAction);
    }

    public void TriggerNoValidCell()
    {
        uiMessanger.RecieveNoValidCellMessage();
    }

    public void TriggerConfirmClickMessage(bool completed)
    {
        uiMessanger.RecieveConfirmClickMessage(completed);
    }

}
