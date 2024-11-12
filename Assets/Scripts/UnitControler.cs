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
    public bool hasItem = false; //Could be changed to an int, to make it possible to have multiple items in a level
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
        Collider[] hitColliders = Physics.OverlapBox(transform.position, Vector3.one * range);

        int i = 0;
        while (i < hitColliders.Length) //Bad implementation, fix number collection in next task, maybe with interfaces
        {
            if(range+0.1f <= (int)Mathf.Abs(transform.position.x - hitColliders[i].transform.position.x)+ (int)Mathf.Abs(transform.position.z - hitColliders[i].transform.position.z))
            {
                i++;
                continue;
            }

            Debug.Log("Tried to pick up " + hitColliders[i].gameObject.name);
            switch (hitColliders[i].tag)
            {
                case "Item":
                    hasItem = true;
                    hitColliders[i].gameObject.SetActive(false);
                    break;

                case "Number":
                    hitColliders[i].gameObject.SetActive(false);
                    numberHUD.SetActive(true); 
                    break;

                default:
                    break;
            }

            i++;
        }


        //if (range >= distaceToItem)
        //{
        //    hasItem = true;
        //    item.SetActive(false);
        //}

        //if (range >= distaceToNumber)
        //{
        //    number.SetActive(false);
        //    numberHUD.SetActive(true);
        //}
    }

    void Update()
    {
        if (maxAmount <= 0) return; //Inverted if to reduce nesting

        //Not the best implementation, can be changed with better input mapping in the future to reduce yandere elifs
        if (Input.GetKeyDown(KeyCode.W))
        {
            Movement(Vector2Int.up); //Used constant vectors instead of hard coded numbers, easy to know where the player is going
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Movement(Vector2Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Movement(Vector2Int.right);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Movement(Vector2Int.left);
        }
    }

    //Changed the previous movement implementation to make it more easy to calculate
    void Movement(Vector2Int direction)
    {
        playerPosition.position += new Vector3(direction.x, 0, direction.y);

        maxAmount = maxAmount - 1;

        //Checking collision after moving to know what the player just touched

        Collider[] hitColliders = Physics.OverlapBox(transform.position, transform.localScale/2);

        int i = 0;
        while (i < hitColliders.Length)
        {
            Debug.Log("Collided with " + hitColliders[i].gameObject.name);
            switch (hitColliders[i].tag)
            {
                case "Goal":
                    if (hasItem == true)
                    {
                        Debug.Log("you win");
                    }
                    break;

                case "Item":
                    hasItem = true;
                    item.SetActive(false);
                    break;

                case "Number":
                    number.SetActive(false);
                    numberHUD.SetActive(true); //Bad implementation, fix number collection in next task
                    break;

                default:
                    break;
            }
            
            i++;
        }
    }
}