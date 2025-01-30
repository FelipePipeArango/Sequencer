using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class UnitControler : MonoBehaviour
{
    public delegate void PickUpObject(GameActions.Actions action, GameObject affected);
    public static event PickUpObject OnObjectPickUp;

    public delegate void MoveAction(GameActions.Actions usedAction, GameObject affected);
    public static event MoveAction OnMovement;

    private Vector2Int playerPreviousPosition;

    public int moveAmount = 0;
    [HideInInspector] public bool hasItem = false;
    [HideInInspector] public bool hasNumber = false;


    private void Start() { }

    public void MovementReceiver(int recievedNumber/*, GameActions.Actions usedAction*/)
    {
        moveAmount = recievedNumber;
    }


    public void ThrowReceiver(int recievedNumber, /* GameActions.Actions usedAction, */int distanceToGoal)
    {
        if (hasItem)
        {
            hasItem = false;
            if (recievedNumber >= distanceToGoal) Debug.Log("you win");
        }
    }

    //if the player HAD the item before throwing, undoing the action should return the item to him.
    //but if he did NOT, the item should NOT be given to him when undoing the action.

    //no need for if statement
    public void UndoThrow(bool playedHadItem)
    {
        hasItem = playedHadItem;
    }

    //Manages the PickUp action
    public void PickUpReceiver
        (int recievedNumber, int distanceToItem, int distanceToNumber,
            GameObject item, GameObject pickUpNumber, GameObject numberHUD)
    {
        if (!hasItem)
        {
            if (recievedNumber >= distanceToItem)
            {
                hasItem = true;
                item.SetActive(false);

                if (OnObjectPickUp != null)
                    OnObjectPickUp(GameActions.Actions.PickUp, item);

            }
        }
        else
        {
            if (OnObjectPickUp != null)
                OnObjectPickUp(GameActions.Actions.PickUp, null);

        }

        if (!hasNumber)
        {
            if (recievedNumber >= distanceToNumber)
            {
                pickUpNumber.SetActive(false);
                numberHUD.SetActive(true);
                hasNumber = true;

                if (OnObjectPickUp != null)
                    OnObjectPickUp(GameActions.Actions.PickUp, pickUpNumber);
            }
        }
        else
        {
            if (OnObjectPickUp != null)
                OnObjectPickUp(GameActions.Actions.PickUp, null);

        }
    }

    public void UndoPickUps(GameObject @object)
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
            //Used constant vectors instead of hard coded numbers
            if (Input.GetKeyDown(KeyCode.W)) Movement(Vector2Int.up);

            if (Input.GetKeyDown(KeyCode.S)) Movement(Vector2Int.down);

            if (Input.GetKeyDown(KeyCode.D)) Movement(Vector2Int.right);

            if (Input.GetKeyDown(KeyCode.A)) Movement(Vector2Int.left);

        }
    }

    //Changed the previous movement implementation to make it more easy to calculate
    void Movement(Vector2Int direction)
    {
        playerPreviousPosition = new Vector2Int(
            Mathf.FloorToInt(this.transform.position.x),
            Mathf.FloorToInt(this.transform.position.z)
            );

        this.transform.position += new Vector3(direction.x, 0, direction.y);
        moveAmount--;

        if (OnMovement != null)
        {
            OnMovement(GameActions.Actions.Move, null);
        }
    }
    public Vector2Int GetPreviousPosition()
    {
        return playerPreviousPosition;
    }
    public void UndoMovement(int previousPositionX, int previousPositionY, int move)
    {
        this.transform.position = new Vector3(previousPositionX, 1, previousPositionY);
        moveAmount = move;
    }


}
