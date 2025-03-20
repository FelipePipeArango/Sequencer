using UnityEngine;
using TMPro;

public enum CursorState
{
    Default,
    Interact
}

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance { get; private set; }

    public TMP_Text numberText;
    public GameObject keyItemHUD;
    public Texture2D defaultCursor;
    public Texture2D interactCursor;

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
        keyItemHUD.SetActive(false); // Start with USB icon hidden
        SetCursorState(CursorState.Default);
        UpdateNumberText(0);
    }

    void Update()
    {
        if (isHoveringOverInteractable)
        {
            SetCursorState(CursorState.Interact);
        }
        else
        {
            SetCursorState(CursorState.Default);
        }

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

    public void SetCursorState(CursorState state)   //Adjust cursor
    {
        Texture2D cursorTexture = defaultCursor;
        Vector2 hotspot = Vector2.zero;

        switch (state)
        {
            case CursorState.Default:
                cursorTexture = defaultCursor;
                hotspot = new Vector2(5, 5);
                break;
            case CursorState.Interact:
                cursorTexture = interactCursor;
                hotspot = new Vector2(5, 5); 
                break;
        }

        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }

}







