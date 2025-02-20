using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static GameActions;

public enum ArrowTiming
{
    BeforePlayer,
    AfterPlayer
}

public class ArrowCard : MonoBehaviour
{
    [Header("Arrow Settings")]
    public GameObject aiCompanion;
    public Image arrowImage;
    public bool isBefore;
    public AIActions arrowDirection;
    AICompanion companion;
    private void Start()
    {
        ConfigureArrowPosition(isBefore);
        ConfigureArrowDirection(arrowDirection);
        companion = aiCompanion.GetComponent<AICompanion>();
    }

    public void TriggerCard()
    {
        Debug.Log($"ArrowCard triggered, Direction = {arrowDirection}, Timing = {isBefore}");
        companion.action = arrowDirection;
        companion.canMove = isBefore;
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

    private void ConfigureArrowDirection(AIActions direction)
    {
        float zRotation = 0f;
        switch (direction)
        {
            case AIActions.Forward:
                zRotation = -90f;
                break;
            case AIActions.Right:
                zRotation = 180f;
                break;
            case AIActions.Back:
                zRotation = 90f;
                break;
            case AIActions.Left:
                zRotation = 0f;
                break;
        }

        arrowImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, zRotation);
    }
}





