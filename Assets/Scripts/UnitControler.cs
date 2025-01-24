using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class UnitControler : MonoBehaviour
{
    public delegate void PickUpObject(GameActions.Actions action, GameObject affectedObject);
    public static event PickUpObject OnObjectPickUp;

    public delegate void MoveAction();
    public static event MoveAction OnMovement;

    Transform playerPosition;

    public int moveAmount = 0;
    [HideInInspector] public bool hasItem = false;
    [HideInInspector] public bool hasNumber = false;

    private void Start()
    {
        playerPosition = this.gameObject.transform;
    }

    public void MovementReceiver(int recievedNumber, GameActions.Actions usedAction)
    {
        moveAmount = recievedNumber;
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
    public void PickUpReceiver(int recievedNumber, int distanceToItem, int distanceToNumber, GameObject item, GameObject number, GameObject numberHUD)
    {
        if (!hasItem)
        {
            if (recievedNumber >= distanceToItem)
            {
                if (OnObjectPickUp != null)
                {
                    OnObjectPickUp(GameActions.Actions.PickUp, item);
                }

                hasItem = true;
                item.SetActive (false);
            }
        }
        
        if(!hasNumber)
        {
            if (recievedNumber >= distanceToNumber)
            {
                if (OnObjectPickUp != null)
                {
                    OnObjectPickUp(GameActions.Actions.PickUp, number);
                }
                number.SetActive(false);
                numberHUD.SetActive(true);
                hasNumber = true;
            }
        }
    }

    public void UndoPickUps (GameObject @object)
    {
        if (@object.tag == "Item")
        {
            hasItem = false;
            @object.SetActive(true);
        }

        if (@object.tag == "NumberItem")
        {
            @object.SetActive(true);
            hasNumber = false;
        }
    }
    void Update()
    {
        if (moveAmount > 0)
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

        moveAmount = moveAmount - 1;
        if (OnMovement != null)
        {
            OnMovement();
        }
    }

    public void UndoMovement(Vector3 previousPosition)
    {
        playerPosition.position = new Vector3(previousPosition.x, 1, previousPosition.z);
        moveAmount = 0;
    }
}