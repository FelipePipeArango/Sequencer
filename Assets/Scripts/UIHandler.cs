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
    public TMP_Text numberText;
    public Texture2D defaultCursor;
    public Texture2D interactCursor;
    public Texture2D holdingCursor;
    public Vector2 cursorHotspot = Vector2.zero;
    public GameObject keyItemHUD;
    private GameObject keyItem;

    void Start()
    {
        keyItem = GameObject.FindGameObjectWithTag("Item");
        SetKeyItemHUD(false);
        UpdateNumberText(0);
        SetCursorState(CursorState.Default);
    }

    void Update()
    {
        if (keyItem && !keyItem.activeInHierarchy)
        {
            keyItem = null;
            SetKeyItemHUD(true);
            enabled = false;
        }
    }

    public void UpdateNumberText(int numberValue)
    {
        numberText.text = numberValue.ToString();
    }

    public void SetCursorState(CursorState state)
    {
        switch (state)
        {
            case CursorState.Default:
                Cursor.SetCursor(defaultCursor, cursorHotspot, CursorMode.Auto);
                break;
            case CursorState.Interact:
                Cursor.SetCursor(interactCursor, cursorHotspot, CursorMode.Auto);
                break;
            case CursorState.Holding:
                Cursor.SetCursor(holdingCursor, cursorHotspot, CursorMode.Auto);
                break;
        }
    }

    public void SetKeyItemHUD(bool hasKey)
    {
        if (keyItemHUD)
            keyItemHUD.SetActive(hasKey);
    }
}
