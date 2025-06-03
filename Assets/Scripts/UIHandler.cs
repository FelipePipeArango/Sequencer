using UnityEngine;
using TMPro;


public class UIHandler : MonoBehaviour
{
    public static UIHandler UIHandlerInstance;

    [SerializeField] public CompleteScreen goalSequence;
    public TMP_Text characterMoveUI;
    public GameObject keyItemHUD;

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

    void Update()
    {
        CheckForKeyItem(); // Check every frame if key item exists
    }

    private void CheckForKeyItem()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");

        if (items.Length == 0)
        {
            keyItemHUD.SetActive(true); // Show USB icon if no items remain
        }
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

}
