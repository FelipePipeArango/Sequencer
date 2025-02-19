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

    [Header("Movement Settings")]
    public float moveDistance;

    private void Start()
    {
        ConfigureArrowPosition(arrowTiming);
        ConfigureArrowDirection(arrowDirection);
    }

    public void ExecuteCompanionMove(Transform companion)
    {
        Vector3 movement = Vector3.zero;

        switch (arrowDirection)
        {
            case ArrowDirection.Up:
                movement = Vector3.up * moveDistance;
                break;

            case ArrowDirection.Down:
                movement = Vector3.down * moveDistance;
                break;

            case ArrowDirection.Left:
                movement = Vector3.left * moveDistance;
                break;

            case ArrowDirection.Right:
                movement = Vector3.right * moveDistance;
                break;
        }

        companion.position += movement;
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


