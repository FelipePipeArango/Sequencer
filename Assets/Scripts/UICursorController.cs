using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public bool isDragging = false;

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
        cursorImageComponent.raycastTarget = false; // Prevent blocking UI clicks
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

    void LateUpdate()
    {
        cursorImage.SetAsLastSibling(); // Ensures cursor always renders on top
    }

    public void SetCursorState(CursorState state)
    {
        if (isDragging && state != CursorState.Interact)
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

            case CursorState.HoverDraggable:
                cursorImageComponent.sprite = hoverDraggableSprite;
                cursorImage.sizeDelta = new Vector2(56, 56);
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
        StartCoroutine(RefreshCursorAfterDrag());
    }

    private IEnumerator RefreshCursorAfterDrag()
    {
        yield return null; // Wait one frame

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            var swap = result.gameObject.GetComponent<UIHoverCursorSwap>();
            if (swap != null)
            {
                SetCursorState(swap.CursorState);
                yield break;
            }
        }

        // Nothing under the cursor
        SetCursorState(CursorState.Default);
    }
}
