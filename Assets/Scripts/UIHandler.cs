using UnityEngine;
using TMPro;

public enum CursorState
{
    Default,
    Interact,
    Holding
}

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance { get; private set; }

    public TMP_Text numberText;
    public GameObject keyItemHUD;

    public Texture2D defaultCursor;
    public Texture2D interactCursor;
    public Texture2D holdingCursor;

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
        SetCursorState(CursorState.Default);
        SetKeyItemHUD(false);
        UpdateNumberText(0);
    }

    void Update()
    {
        if (!isHoveringOverInteractable)
        {
            SetCursorState(CursorState.Default);
            return;
        }

        if (Input.GetMouseButton(0))
        {
            SetCursorState(CursorState.Holding);
        }
        else
        {
            SetCursorState(CursorState.Interact);
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

    public void SetKeyItemHUD(bool hasKey)
    {
        if (keyItemHUD)
            keyItemHUD.SetActive(hasKey);
    }

    public void SetCursorState(CursorState state)
    {
        Texture2D cursorTexture = defaultCursor;

        switch (state)
        {
            case CursorState.Default:
                cursorTexture = defaultCursor;
                break;
            case CursorState.Interact:
                cursorTexture = interactCursor;
                break;
            case CursorState.Holding:
                cursorTexture = holdingCursor;
                break;
        }

        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }
}



