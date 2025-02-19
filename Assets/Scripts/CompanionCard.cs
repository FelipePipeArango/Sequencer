//// Online C# Editor for free
//// Write, Edit and Run your C# code using C# Online Compiler

//using System;
//using UnityEngine;

//public class CompanionCard : MonoBehavior
//{
//    //ArrowSettings
//    public Image arrowImage;
//    public CompanionMoveTiming moveTiming;
//    public CompanionDirection moveDirection;

//    //MovementSettings
//    public float moveDistance


//    private void Start()
//    {
//        ConfigureArrowPosition(moveTiming)
//    }

//    public void ExecuteCompanionMove(Transform companion)
//    {
//        Vector3 movement = Vector3.zero


//        switch (moveDirection)
//        {
//            case CompanionDirection.Up:
//                movement = Vector3.up * moveDistance;
//                break;

//            case CompanionDirection.Down:
//                movement = Vector3.down * moveDistance;
//                break;

//            case CompanionDirection.Left:
//                movement = Vector3.left * moveDistance;
//                break;

//            case CompanionDirection.Right:
//                movement = Vector3.right * moveDistance;
//                break;
//        }

//        companion.position += movement;

//    }

//    private void ConfigureArrowPosition(CompanionMoveTiming timing)
//    {
//        RectTransform arrowRect = arrowImage.GetComponent<RectTransform>();

//        if (timing == CompanionMoveTiming.BeforePlayer)
//        {
//            arrow.Rect.anchoredPosition = new Vector2(0, 50f);
//        }
//        else
//        {
//            arrowRect.anchoredPosition = new Vector2(0, -50f);
//        }
//    }

//    private void ConfigureArrowDirection(CompanionDirection direction)
//    {
//        float zRotation = 0f;
//        switch (direction)
//        {
//            case CompanionDirection.Up:
//                zRotation = 0f;
//                break;

//            case CompanionDirection.Right:
//                zRotation = -90f;
//                break;


//            case CompanionDirection.Down:
//                zRotation = 180f;
//                break;


//            case CompanionDirection.Left:
//                zRotation = 90f;
//                break;
//        }

//        arrowImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, zRotation);
//    }
//}

























