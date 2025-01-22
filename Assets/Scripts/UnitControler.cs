using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControler : MonoBehaviour
{
    public delegate void MoveAction();
    public static event MoveAction OnMovement;

    Transform playerPosition;

    int maxAmount = 0;
    [HideInInspector] public bool hasItem = false;
    [HideInInspector] public bool hasNumber = false;

    public Event OnPlayerMovement;

    private void Start()
    {
        playerPosition = this.gameObject.transform;
    }

    public void MovementReceiver(int recievedNumber, GameActions.Actions usedAction)
    {
        maxAmount = recievedNumber;
    }
    public void ThrowReceiver(int recievedNumber, GameActions.Actions usedAction, int distanceToGoal)
    {
        if (hasItem)
        {
            if (recievedNumber >= distanceToGoal)
            {
                Debug.Log("you win");
            }
        }
    }
    //Manages the PickUp action
    public void PickUpReceiver(int recievedNumber, GameActions.Actions usedAction, int distanceToItem, int distanceToNumber, GameObject item, GameObject number, GameObject numberHUD)
    {
        if (!hasItem)
        {
            if (recievedNumber >= distanceToItem)
            {
                hasItem = true;
                item.SetActive (false);
            }
        }
        
        if(!hasNumber)
        {
            if (recievedNumber >= distanceToNumber)
            {
                number.SetActive(false);
                numberHUD.SetActive(true);
                hasNumber = true;
            }
        }
    }
    void Update()
    {
        if (maxAmount > 0)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                Movement(Vector2Int.up); //Used constant vectors instead of hard coded numbers
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                Movement(Vector2Int.down);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                Movement(Vector2Int.right);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                Movement(Vector2Int.left);
            } 
        }
    }

    //Changed the previous movement implementation to make it more easy to calculate
    void Movement(Vector2Int direction)
    {
        playerPosition.position += new Vector3(direction.x, 0, direction.y);

        maxAmount = maxAmount - 1;
        if (OnMovement != null)
        {
            OnMovement();
        }
    }
}