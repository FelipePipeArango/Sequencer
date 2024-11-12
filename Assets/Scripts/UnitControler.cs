using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControler : MonoBehaviour
{
    [SerializeField] GridManager gridManager;

    [SerializeField] Transform playerPosition;
    [SerializeField] Vector2Int itemPosition;
    [SerializeField] Vector2Int goalPosition;
    [SerializeField] Vector2Int numberPosition;

    int maxAmount = 0;
    public bool hasItem = false;
    int distaceToItem;
    int distaceToGoal;
    int distaceToNumber;

    [SerializeField] GameObject item;
    [SerializeField] GameObject goal;
    [SerializeField] GameObject number;
    [SerializeField] GameObject numberHUD;

    private void Start()
    {
        distaceToItem = (int)Mathf.Abs(playerPosition.position.x - itemPosition.x) + (int)Mathf.Abs(playerPosition.position.z - itemPosition.y);
        distaceToGoal = (int) Mathf.Abs(playerPosition.position.x - goalPosition.x) + (int) Mathf.Abs(playerPosition.position.z - goalPosition.y);
        distaceToNumber = (int)Mathf.Abs(playerPosition.position.x - numberPosition.x) + (int)Mathf.Abs(playerPosition.position.z - numberPosition.y);
    }

    public void MovementReceiver(int amount)
    {
        maxAmount = amount;
    }
    public void ThrowReceiver(int range)
    {
        if (hasItem )
        {
            if (range >= distaceToGoal)
            {
                Debug.Log("you win");
            }
        }
    }
    public void PickUpReceiver(int range)
    {
        if (!hasItem)
        {
            if (range >= distaceToItem)
            {
                hasItem = true;
                item.SetActive (false);
            }

            if (range >= distaceToNumber)
            {
                number.SetActive (false);
                numberHUD.SetActive (true);
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

        //Change the rest of the function to detect the distance to items with collisions instead

        //With collisions there is no need to make this calculations
        distaceToItem = (int)Mathf.Abs(playerPosition.position.x - itemPosition.x) + (int)Mathf.Abs(playerPosition.position.z - itemPosition.y);
        distaceToGoal = (int)Mathf.Abs(playerPosition.position.x - goalPosition.x) + (int)Mathf.Abs(playerPosition.position.z - goalPosition.y);
        distaceToNumber = (int)Mathf.Abs(playerPosition.position.x - numberPosition.x) + (int)Mathf.Abs(playerPosition.position.z - numberPosition.y);

        if (distaceToNumber == 0)
        {
            PickUpReceiver(0);
        }

        if (distaceToGoal == 0 && hasItem == true)
        {
            Debug.Log("you win");
        }

        if (distaceToItem == 0)
        {
            PickUpReceiver(0);
        }
    }
}