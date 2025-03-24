using System;
using UnityEngine;
using UnityEngine.UI;

public enum ArrowTiming
{
    BeforePlayer,
    AfterPlayer
}

public enum ArrowDirection
{
    Up,
    Down,
    Left,
    Right
}

[Serializable]
public class ArrowCard : MonoBehaviour
{
    [Header("Arrow Settings")]
    public Image arrowImage;
    public ArrowTiming arrowTiming;
    public ArrowDirection arrowDirection;

    private void Start()
    {
        ConfigureArrowPosition(arrowTiming);
        ConfigureArrowDirection(arrowDirection);
    }

   
    public void TriggerCard()
    {
        Debug.Log($"ArrowCard triggered, Direction = {arrowDirection}, Timing = {arrowTiming}");
    }

    private void ConfigureArrowPosition(ArrowTiming timing)
    {
        RectTransform arrowRect = arrowImage.GetComponent<RectTransform>();

        if (timing == ArrowTiming.BeforePlayer)
        {
            arrowRect.anchoredPosition = new Vector2(0, 100f);
        }
        else
        {
            arrowRect.anchoredPosition = new Vector2(0, -100f);
        }
    }

    private void ConfigureArrowDirection(ArrowDirection direction)
    {
        float zRotation = 0f;
        switch (direction)
        {
            case ArrowDirection.Up:
                zRotation = -90f;
                break;
            case ArrowDirection.Right:
                zRotation = 180f;
                break;
            case ArrowDirection.Down:
                zRotation = 90f;
                break;
            case ArrowDirection.Left:
                zRotation = 0f;
                break;
        }

        arrowImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, zRotation);
    }
}





