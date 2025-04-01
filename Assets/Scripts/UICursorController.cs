using UnityEngine;
using UnityEngine.UI;

public enum CursorState
{
    Default,
    Interact
}

public class UICursorController : MonoBehaviour
{
    public static UICursorController Instance { get; private set; }

    public RectTransform cursorImage;
    public Canvas canvas;
    public Sprite defaultCursorSprite;
    public Sprite interactCursorSprite;

    private bool isDragging = false;

    [SerializeField] private Vector2 hotspotOffset = new Vector2(15, -30);
    private Image cursorImageComponent;

    private void Awake()
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
        Cursor.visible = false;
        cursorImageComponent = cursorImage.GetComponent<Image>();
        SetCursorState(CursorState.Default);
    }

    void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out localPoint
        );

        cursorImage.localPosition = localPoint + hotspotOffset;
    }

    public void SetCursorState(CursorState state)
    {
        if (isDragging && state != CursorState.Default)
            return;

        if (cursorImageComponent == null) return;

        switch (state)
        {
            case CursorState.Default:
                cursorImageComponent.sprite = defaultCursorSprite;
                cursorImage.sizeDelta = new Vector2(64, 64);
                break;

            case CursorState.Interact:
                cursorImageComponent.sprite = interactCursorSprite;
                cursorImage.sizeDelta = new Vector2(48, 48);
                break;
        }
    }
    public void BeginDragCursor()
    {
        isDragging = true;
        SetCursorState(CursorState.Interact);
    }

    public void EndDragCursor()
    {
        isDragging = false;
        SetCursorState(CursorState.Default);
    }

}





