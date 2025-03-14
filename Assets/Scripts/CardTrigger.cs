using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static GridManager;
using static GameActions;
using static GameDirections;

public class CardTrigger : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI usedText;
    [SerializeField] private Image slotImage;
    [SerializeField] private Image cardBackground;
    [SerializeField] private Image usedBackground;
    [SerializeField] private Material dissolveMaterial;

    public bool nextInSequence;
    public bool isInUse = false;
    public bool available { get; set; } = true;
    public Actions LevelActions;

    public delegate void GrabActions(int number, bool isGrabbing);
    public static event GrabActions OnGrab;

    public delegate void DropAction(NumberItem item, Actions action);
    public static event DropAction OnDropAction;

    private Action<NumberItem> executeAction;
    private NumberItem hoveredNumberItem;

    public bool hasArrow;
    public Image arrowImage;
    public bool isAIBefore;
    public Directions arrowDirection;

    void Awake()
    {
        if (usedBackground != null)
        {
            usedBackground.enabled = false;
            Color bgColor = usedBackground.color;
            usedBackground.color = new Color(bgColor.r, bgColor.g, bgColor.b, 0f);
        }
    }

    void Start()
    {
        if (gridManager.AIActions != null && hasArrow && arrowImage != null)
        {
            arrowImage.gameObject.SetActive(true);
            ConfigureArrowPosition(isAIBefore);
            ConfigureArrowDirection(arrowDirection);
        }
    }

    void ConfigureArrowPosition(bool isBefore)
    {
        if (arrowImage == null) return;
        RectTransform arrowRect = arrowImage.GetComponent<RectTransform>();
        if (isBefore) arrowRect.anchoredPosition = new Vector2(0, 100f);
        else arrowRect.anchoredPosition = new Vector2(0, -100f);
    }

    void ConfigureArrowDirection(Directions direction)
    {
        if (arrowImage == null) return;
        float zRotation = 0f;
        if (direction == Directions.Forward) zRotation = -90f;
        else if (direction == Directions.Right) zRotation = 180f;
        else if (direction == Directions.Back) zRotation = 90f;
        else if (direction == Directions.Left) zRotation = 0f;
        arrowImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, zRotation);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject hoveredObject = eventData.pointerDrag;
        if (hoveredObject != null)
        {
            hoveredNumberItem = hoveredObject.GetComponent<NumberItem>();
            if (hoveredNumberItem != null)
            {
                if (LevelActions == Actions.Move) gridManager.TurnOnHighlight(true, hoveredNumberItem.value);
                else if (LevelActions == Actions.PickUp) gridManager.TurnOnHighlight(false, hoveredNumberItem.value);
                else if (LevelActions == Actions.Throw) gridManager.TurnOnHighlight(false, hoveredNumberItem.value);
                isInUse = true;
            }
        }
        if (cardBackground != null)
        {
            cardBackground.material = null;
            Color cbColor = cardBackground.color;
            cardBackground.color = new Color(cbColor.r, cbColor.g, cbColor.b, 1f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gridManager.TurnOffHighlight();
        hoveredNumberItem = null;
        isInUse = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!available || !nextInSequence) return;

        GameObject droppedObj = eventData.pointerDrag;
        NumberItem draggableItem = droppedObj != null ? droppedObj.GetComponent<NumberItem>() : null;
        if (draggableItem == null) return;

        OnDropAction?.Invoke(draggableItem, LevelActions);

        if (hasArrow && arrowImage != null)
        {
            Sequencer.sequencer.CommunicateAIActions(isAIBefore, arrowDirection);
        }

        Sequencer.sequencer.CommunicateAction(draggableItem, LevelActions);
        OnGrab?.Invoke(0, false);

       
        available = false;
        if (usedBackground != null)
        {
            usedBackground.enabled = true;
            Color bgColor = usedBackground.color;
            usedBackground.color = new Color(bgColor.r, bgColor.g, bgColor.b, 1f);
            usedBackground.transform.SetAsFirstSibling();
        }
        if (usedText != null) usedText.gameObject.SetActive(false);
        StartDissolve();

        if (hoveredNumberItem != null)
        {
            Debug.Log("Dropped " + hoveredNumberItem.value + " on " + LevelActions + " action.");
        }
    }

    IEnumerator DissolveEffect()
    {
        float dissolveAmount = 0f;
        float dissolveSpeed = 1f;

        cardBackground.material = new Material(dissolveMaterial);
        Material mat = cardBackground.material;
        Color originalColor = cardBackground.color;

        while (dissolveAmount < 1f)
        {
            dissolveAmount += Time.deltaTime * dissolveSpeed;
            mat.SetFloat("_DissolveAmount", dissolveAmount);
            cardBackground.color = new Color(
                originalColor.r,
                originalColor.g,
                originalColor.b,
                1f - dissolveAmount
            );
            yield return null;
        }

        cardBackground.color = new Color(
            cardBackground.color.r,
            cardBackground.color.g,
            cardBackground.color.b,
            0f
        );
    }

    void StartDissolve()
    {
        if (dissolveMaterial != null && cardBackground != null)
        {
            StartCoroutine(DissolveEffect());
        }
    }

    public void Initialize()
    {
        if (LevelActions == Actions.Move) executeAction = ExecuteMoveAction;
        else if (LevelActions == Actions.PickUp) executeAction = ExecutePickUpAction;
        else if (LevelActions == Actions.Throw) executeAction = ExecuteThrowAction;
        else executeAction = DefaultAction;
    }

    public void ExecuteAction(NumberItem numberItem)
    {
        executeAction?.Invoke(numberItem);
    }

    void ExecuteMoveAction(NumberItem numberItem)
    {
        gridManager.playerActions.MovementReceiver(numberItem.value);
    }

    void ExecutePickUpAction(NumberItem numberItem)
    {
        gridManager.playerActions.PickUpReceiver(numberItem.value);
    }

    void ExecuteThrowAction(NumberItem numberItem)
    {
        gridManager.playerActions.ThrowReceiver(numberItem.value);
    }

    void DefaultAction(NumberItem numberItem)
    {
        Debug.Log("No action assigned for " + LevelActions);
    }

    public void Enable()
    {
        if (!available)
        {
            available = true;

            if (usedBackground != null)
            {
                usedBackground.enabled = false;
                Color bgColor = usedBackground.color;
                usedBackground.color = new Color(bgColor.r, bgColor.g, bgColor.b, 0f);
            }

            if (cardBackground != null)
            {
                cardBackground.material = null;
                Color cbColor = cardBackground.color;
                cardBackground.color = new Color(cbColor.r, cbColor.g, cbColor.b, 1f);
            }

            if (usedText != null) usedText.gameObject.SetActive(false);
            if (slotImage != null) slotImage.gameObject.SetActive(true);
        }
    }

    public void DisableUsed(NumberItem number)
    {
        if (available)
        {
            if (usedText != null)
            {
                usedText.gameObject.SetActive(true);
                usedText.text = number.value.ToString();
            }
            if (slotImage != null) slotImage.gameObject.SetActive(false);

            number.transform.SetParent(number.parentTransform);
            number.gameObject.SetActive(false);

            available = false;
            if (usedBackground != null)
            {
                usedBackground.enabled = true;
                Color bgColor = usedBackground.color;
                usedBackground.color = new Color(bgColor.r, bgColor.g, bgColor.b, 1f);
                usedBackground.transform.SetAsFirstSibling();
            }
            StartDissolve();
        }
    }

    public void Disable()
    {
        if (usedText != null) usedText.gameObject.SetActive(true);
        if (slotImage != null) slotImage.gameObject.SetActive(false);

        available = false;
        if (usedBackground != null)
        {
            usedBackground.enabled = true;
            Color bgColor = usedBackground.color;
            usedBackground.color = new Color(bgColor.r, bgColor.g, bgColor.b, 1f);
            usedBackground.transform.SetAsFirstSibling();
        }
        StartDissolve();
    }
}



