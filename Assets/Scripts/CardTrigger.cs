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
    [SerializeField] Image usedBackground;
    [SerializeField] private Material dissolveMaterial;

    [HideInInspector] public bool available = true; //tracks if the card has been used
    [HideInInspector] public bool nextInSequence;

    public GameActions.Actions LevelActions;

    public delegate void GrabActions(int number, bool isGrabing);
    public static event GrabActions OnGrab;

    public delegate void DropAction(NumberItem test, GameActions.Actions action);
    public static event DropAction OnDropAction;


    private NumberItem hoveredNumberItem;


    private void Awake()
    {
        if (usedBackground != null)
        {
            Color bgColor = usedBackground.color;
            usedBackground.color = new Color(bgColor.r, bgColor.g, bgColor.b, 0f);
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

            

            StartDissolve();
            StartCoroutine(FadeInUsedBackground(0.3f));

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

        if (usedBackground != null)
        {
            StartCoroutine(FadeOutUsedBackground());
        }

        //Reset cardBackground
        if (cardBackground != null)
        {
            cardBackground.material = null; // Remove dissolve shader
            cardBackground.color = new Color(cardBackground.color.r, cardBackground.color.g, cardBackground.color.b, 1f); // Fully opaque again
        }
    }

    private IEnumerator DissolveEffect()
    {
        float dissolveAmount = 0f;
        float dissolveSpeed = 1f;

        // Assign dissolve material to the foreground (cardBackground)
        cardBackground.material = new Material(dissolveMaterial);
        Material mat = cardBackground.material;
        Color originalColor = cardBackground.color;

        while (dissolveAmount < 1.0f)
        {
            dissolveAmount += Time.deltaTime * dissolveSpeed;
            mat.SetFloat("_DissolveAmount", dissolveAmount);
            yield return null;
            cardBackground.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f - dissolveAmount);
        }

        
        cardBackground.color = new Color(cardBackground.color.r, cardBackground.color.g, cardBackground.color.b, 0f);
    }

    private void StartDissolve()
    {
        if (dissolveMaterial != null && cardBackground != null)
        {
            StartCoroutine(DissolveEffect());
        }
    }

    private IEnumerator FadeInUsedBackground(float delay)
    {
        yield return new WaitForSeconds(delay); // Delay before fading in

        float duration = 0.5f;
        Color bgColor = usedBackground.color;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, t / duration);
            usedBackground.color = new Color(bgColor.r, bgColor.g, bgColor.b, alpha);
            yield return null;
        }

        usedBackground.color = new Color(bgColor.r, bgColor.g, bgColor.b, 1f);
    }

    private IEnumerator FadeOutUsedBackground()
    {
        float duration = 0.5f;
        float elapsedTime = 0f;
        Color bgColor = usedBackground.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            usedBackground.color = new Color(bgColor.r, bgColor.g, bgColor.b, alpha);
            yield return null;
        }

        usedBackground.color = new Color(bgColor.r, bgColor.g, bgColor.b, 0f); // Fully hidden
    }
}

