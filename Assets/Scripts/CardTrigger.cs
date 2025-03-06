using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using static GridManager;
using static GameActions;
using static GameDirections;
using System;

public class CardTrigger : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI usedText;
    [SerializeField] Image slotImage;
    [SerializeField] Image cardBackground;
    [HideInInspector] public bool nextInSequence;

    public int numberInQueue;
    public bool available { get; protected set; } = true;
    public bool isInUse = false;
    public GameActions.Actions LevelActions { get; set; }

    public delegate void GrabActions(int number, bool isGrabing);
    public static event GrabActions OnGrab;

    private Action<NumberItem> executeAction;
   
    private NumberItem hoveredNumberItem;

    [Header("Arrow Settings")]
    [Header("Check hasArrow to true only if you have Companion in the scene")]
    public bool hasArrow;
    
    public Image arrowImage;
    public bool isAIBefore = false;
    public Directions arrowDirection;


   


    // Delegate to hold the execution logic

    // Set the execution method dynamically based on LevelActions

    private void Start()
    {
        if (gridManager.AIActions != null)
        {
            if (hasArrow == true)
            {
                arrowImage.gameObject.SetActive(true);
                ConfigureArrowPosition(isAIBefore);
                ConfigureArrowDirection(arrowDirection);
            }
        }
    }

    private void ConfigureArrowPosition(bool isbefore)
    {
        RectTransform arrowRect = arrowImage.GetComponent<RectTransform>();

        if (isbefore == true)
        {
            arrowRect.anchoredPosition = new Vector2(0, 100f);
        }
        else
        {
            arrowRect.anchoredPosition = new Vector2(0, -100f);
        }
    }
    private void ConfigureArrowDirection(Directions direction)
    {
        float zRotation = 0f;
        switch (direction)
        {
            case Directions.Forward:
                zRotation = -90f;
                break;
            case Directions.Right:
                zRotation = 180f;
                break;
            case Directions.Back:
                zRotation = 90f;
                break;
            case Directions.Left:
                zRotation = 0f;
                break;
        }

        arrowImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, zRotation);
    }


    public void Enable()
    {
        if (!available) //if it's not through undo (therefore, using the Enable action), then it does not return the used numbers.
        {
            available = true;
            usedText.gameObject.SetActive(false);
            slotImage.gameObject.SetActive(true);
        }
    }
    public void Disable(NumberItem number)
    {
        if (available)
        {
            usedText.gameObject.SetActive(true);
            slotImage.gameObject.SetActive(false);
            usedText.text = number.value.ToString();
            number.transform.SetParent(number.parentTransform);
            number.gameObject.SetActive(false);
            available = false;
        }
        else
        {
            return;
        }
    }
    public void ExecuteAction(NumberItem numberItem)
    {
        executeAction?.Invoke(numberItem);
    }
    public void Initialize(GameActions.Actions action)
    {
        LevelActions = action;
        switch (action)
        {
            case GameActions.Actions.Move:
                executeAction = ExecuteMoveAction;
               
                break;
            case GameActions.Actions.PickUp:
                executeAction = ExecutePickUpAction;
               
                break;
            case GameActions.Actions.Throw:
                executeAction = ExecuteThrowAction;
               
                break;
            default:
                executeAction = DefaultAction;
                break;
        }
    }

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject hoveredObject = eventData.pointerDrag;

        if (hoveredObject != null && hoveredObject.GetComponent<NumberItem>() != null)
        {
            hoveredNumberItem = hoveredObject.GetComponent<NumberItem>();

            // Display the debug message based on the card type and number value
            if (LevelActions == GameActions.Actions.Move)
                gridManager.TurnOnHighlight(true, hoveredNumberItem.value);

            else if (LevelActions == GameActions.Actions.PickUp)
                gridManager.TurnOnHighlight(false, hoveredNumberItem.value);

            if (LevelActions == GameActions.Actions.Throw)
                gridManager.TurnOnHighlight(false, hoveredNumberItem.value);
            isInUse = true;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        gridManager.TurnOffHighlight();
        // Clear the hovered item reference when leaving the card
        hoveredNumberItem = null;
        isInUse = false;
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (available == true && nextInSequence == true)
        {
            GameObject dropped = eventData.pointerDrag;
            NumberItem draggableItem = dropped.GetComponent<NumberItem>();

            if (hasArrow == true)
                Sequencer.sequencer.CommunicateAIActions(isAIBefore, arrowDirection);

            Sequencer.sequencer.CommunicateAction(draggableItem, LevelActions);
            //Sequencer.sequencer.ManageSequenceText(0, false);

            if (OnGrab != null)
                OnGrab(0, false); //Communicates with the sequencer whengrabing a number.
        }
        if (hoveredNumberItem != null)
            Debug.Log($"Dropped {hoveredNumberItem.value} on {LevelActions} action.");
            // Handle drop logic (like executing the action or snapping the item to the card)
        
    }
     
    
    private void ExecuteMoveAction(NumberItem numberItem)
    {
        gridManager.playerActions.MovementReceiver(numberItem.value, LevelActions);
        Debug.Log($"Executing Move action with value {numberItem.value}");
    }
    private void ExecutePickUpAction(NumberItem numberItem)
    {
        gridManager.playerActions.PickUpReceiver(numberItem.value, LevelActions);
        Debug.Log($"Executing PickUp action with value {numberItem.value}");
    }
    private void ExecuteThrowAction(NumberItem numberItem)
    {
        gridManager.playerActions.ThrowReceiver(numberItem.value, LevelActions);
        Debug.Log($"Executing Throw action with value {numberItem.value}");
    }
    private void DefaultAction(NumberItem numberItem)
    {
        Debug.Log($"No action assigned for {LevelActions}");
    }
}
