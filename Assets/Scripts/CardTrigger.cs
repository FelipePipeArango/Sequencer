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

            if (usedBackground != null)
            {
                Color bgColor = usedBackground.color;
                usedBackground.color = new Color(bgColor.r, bgColor.g, bgColor.b, 1f);
                usedBackground.transform.SetAsFirstSibling();
            }
        }

        if (usedText != null)
        {
            usedText.gameObject.SetActive(false);
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
        if (!undo)
        {
            if (!available)
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
        else
        {
            usedText.gameObject.SetActive(false);
            slotImage.gameObject.SetActive(true);

            number.image.raycastTarget = true;
            number.gameObject.SetActive(true);
            available = true;
        }

        if (cardBackground != null)
        {
            cardBackground.material = null;
            cardBackground.color = new Color(cardBackground.color.r, cardBackground.color.g, cardBackground.color.b, 1f);
        }
    }

    private IEnumerator DissolveEffect()
    {
        float dissolveAmount = 0f;
        float dissolveSpeed = 1f;

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
}


