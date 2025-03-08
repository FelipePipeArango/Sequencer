using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class CardTrigger : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI usedText;
    [SerializeField] Image slotImage;
    [SerializeField] Image cardBackground;
    [SerializeField] private Material dissolveMaterial;

    [HideInInspector] public bool available = true; //tracks if the card has been used
    [HideInInspector] public bool nextInSequence;

    public GameActions.Actions LevelActions;

    public delegate void GrabActions(int number, bool isGrabing);
    public static event GrabActions OnGrab;

    public delegate void DropAction(NumberItem test, GameActions.Actions action);
    public static event DropAction OnDropAction;


    private NumberItem hoveredNumberItem;



    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject hoveredObject = eventData.pointerDrag;

        if (hoveredObject != null && hoveredObject.GetComponent<NumberItem>() != null)
        {
            hoveredNumberItem = hoveredObject.GetComponent<NumberItem>();

            // Display the debug message based on the card type and number value
            if (LevelActions == GameActions.Actions.Move)
            {
                GridGenerator.Instance.MoveDistanceCheck(hoveredNumberItem.value, GameManager.Instance.GetPlayerPos());
            }
            else if (LevelActions == GameActions.Actions.PickUp)
            {
                GridGenerator.Instance.PickUpThrowCheck(hoveredNumberItem.value);
            }
            else if (LevelActions == GameActions.Actions.Throw)
            {
                GridGenerator.Instance.PickUpThrowCheck(hoveredNumberItem.value);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GridGenerator.Instance.Reset();
        // Clear the hovered item reference when leaving the card
        hoveredNumberItem = null;
    }


    public void OnDrop(PointerEventData eventData)
    {
        if (available == true && nextInSequence == true)
        {
            GameObject dropped = eventData.pointerDrag;
            NumberItem draggableItem = dropped.GetComponent<NumberItem>();

            OnDropAction?.Invoke(draggableItem, LevelActions);
            OnGrab?.Invoke(0, false);

            StartDissolve(); // Start the dissolve effect on the card
        }

        if (hoveredNumberItem != null)
        {
            Debug.Log($"Dropped {hoveredNumberItem.value} on {LevelActions} action.");
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

            StartDissolve();
        }
        else
        {
            return;
        }
    }

    public void Enable(bool undo, NumberItem number)
    {
        if (!undo) //this check if a card is being enabled through the Undo function of the game, or thorugh the Enable card action.
        {
            if (!available) //if it's not through undo (therefore, using the Enable action), then it does not return the used numbers.
            {
                available = true;
                usedText.gameObject.SetActive(false);
                slotImage.gameObject.SetActive(true);
            }
            else
            {
                return;
            } 
        }
        else //if it's through the undo system, then it returns the used number
        {
            usedText.gameObject.SetActive(false);
            slotImage.gameObject.SetActive(true);

            number.image.raycastTarget = true;
            number.gameObject.SetActive(true);
            available = true;
        }
    }

    private IEnumerator DissolveEffect()
    {
        float dissolveAmount = 0f;
        float dissolveSpeed = 2.0f;
        slotImage.material = new Material(dissolveMaterial);
        Material mat = slotImage.material;

        while (dissolveAmount < 1.0f)
        {
            dissolveAmount += Time.deltaTime * dissolveSpeed;
            mat.SetFloat("DissolveAmount", dissolveAmount);
            yield return null;
        }

        
    }

    private void StartDissolve()
    {
        if (dissolveMaterial != null)
        {
            StartCoroutine(DissolveEffect());
        }
    }
}
