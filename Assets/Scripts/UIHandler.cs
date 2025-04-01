using UnityEngine;
using TMPro;


public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance { get; private set; }

    public TMP_Text numberText;
    public GameObject keyItemHUD;

    private bool isHoveringOverInteractable = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        keyItemHUD.SetActive(false);
        UpdateNumberText(0);
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

    public void SetHoverState(bool isHovering)
    {
        isHoveringOverInteractable = isHovering;
    }

    public void UpdateNumberText(int numberValue)
    {
        if (numberText)
            numberText.text = numberValue.ToString();
    }
}









