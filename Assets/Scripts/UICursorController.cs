using UnityEngine;
using UnityEngine.UI;

public enum CursorState
{
    Default,
    Interact,
    HoverDraggable
}

public class UICursorController : MonoBehaviour
{
    public static UICursorController Instance { get; private set; }

    public RectTransform cursorImage;
    public Canvas canvas;
    public Sprite defaultCursorSprite;
    public Sprite interactCursorSprite;
    public Sprite hoverDraggableSprite;

    [SerializeField] private Vector2 hotspotOffset = new Vector2(15, -30);
    private Image cursorImageComponent;

    private CursorState currentHoverState = CursorState.Default;
    private bool isClicking = false;

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
        cursorImageComponent.raycastTarget = false;
        SetCursorVisual(CursorState.Default);
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

        // Detect click or hold
        if (Input.GetMouseButtonDown(0))
        {
            isClicking = true;
            SetCursorVisual(CursorState.Interact); // show click sprite
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isClicking = false;
            SetCursorVisual(currentHoverState); // revert to hover state
        }
    }

    void LateUpdate()
    {
        cursorImage.SetAsLastSibling();
    }

    public void SetCursorState(CursorState newHoverState)
    {
        currentHoverState = newHoverState;

        if (!isClicking)
        {
            SetCursorVisual(newHoverState);
        }
    }

    private void SetCursorVisual(CursorState state)
    {
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
            case CursorState.HoverDraggable:
                cursorImageComponent.sprite = hoverDraggableSprite;
                cursorImage.sizeDelta = new Vector2(56, 56);
                break;
        }
    }
}

