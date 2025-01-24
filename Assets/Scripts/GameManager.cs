using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class GameManager : MonoBehaviour
{
    //The cards present in the level
    NumberItem[] levelNumbers;
    int[] numberValues;
    CardTrigger[] levelCards;
    GameActions.Actions[] levelActions;
    object[,] board;

    [Header("SIZE OF THE BOARD - STARTS FROM 0")]
    [SerializeField] Vector2Int size;

    [Header("MANDATORY OBJECTS IN A LEVEL")]
    [SerializeField] GameObject cardsInLevel; //hopefully I'll find a way to make this fill itself instead of requiring a serialization
    [SerializeField] GameObject numbersInLevel;

    //Make this a prefab, add player under GameManager, and assign player to UnitController
    [SerializeField] GameObject player;
    [SerializeField] GameObject keyItem;
    [SerializeField] GameObject goal;

    [Header("OPTIONAL OBJECTS IN A LEVEL")]
    [SerializeField] GameObject pickUpNumber;
    [SerializeField] GameObject numberHUD;

    [Header("NO NEED TO ASSIGN THIS")]
    [SerializeField] Sequencer sequencer;

    UndoManager undoManager;
    UnitControler playerActions;

    int distaceToItem;
    int distaceToGoal;
    int distaceToNumber;

    private void OnEnable()
    {
        CardTrigger.OnDropAction += CommunicateAction;
        UnitControler.OnMovement += ReCalculateBoard;
        UnitControler.OnObjectPickUp += CommunicateChange;
    }
    
    private void OnDisable()
    {
        CardTrigger.OnDropAction -= CommunicateAction;
        UnitControler.OnMovement -= ReCalculateBoard;
        UnitControler.OnObjectPickUp -= CommunicateChange;
    }

    public void Awake()
    {
        playerActions = player.GetComponent<UnitControler>();
        undoManager = this.GetComponent<UndoManager>();

        #region NumberFilling
        //This section informs the manager about the numbers in the level
        levelNumbers = new NumberItem[numbersInLevel.transform.childCount];
        numberValues = new int [numbersInLevel.transform.childCount];
        for (int i = 0; i < levelNumbers.Length; i++)
        {
            levelNumbers[i] = numbersInLevel.transform.GetChild(i).GetComponentInChildren<NumberItem>();
            numberValues[i] = levelNumbers[i].value;
        }
        #endregion

        #region CardFilling
        //This checks the cards being used in the level and fills the required arrays
        levelCards = new CardTrigger[cardsInLevel.transform.childCount];
        levelActions = new GameActions.Actions[cardsInLevel.transform.childCount];

        for (int i = 0; i < levelCards.Length; i++)
        {
            levelCards[i] = cardsInLevel.transform.GetChild(i).GetComponent<CardTrigger>();
            levelActions[i] = levelCards[i].LevelActions;
        }

        sequencer.FillCards(cardsInLevel);
        #endregion
    }

    private void Start()
    {
        distaceToItem = (int)Mathf.Abs(player.transform.position.x - keyItem.transform.position.x) + (int)Mathf.Abs(player.transform.position.z - keyItem.transform.position.z);
        distaceToGoal = (int)Mathf.Abs(player.transform.position.x - goal.transform.position.x) + (int)Mathf.Abs(player.transform.position.z - goal.transform.position.z);

        if (pickUpNumber != null)
        {
            distaceToNumber = (int)Mathf.Abs(player.transform.position.x - pickUpNumber.transform.position.x) + (int)Mathf.Abs(player.transform.position.z - pickUpNumber.transform.position.z); 
        }

        board = new object[size.x, size.y];

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (player.transform.position.x == i && player.transform.position.z == j)
                {
                    board[i, j] = playerActions.moveAmount;
                }
                if (keyItem.transform.position.x == i && keyItem.transform.position.z == j)
                {
                    board[i, j] = keyItem.tag;
                }
                if (pickUpNumber != null)
                {
                    if (pickUpNumber.transform.position.x == i && pickUpNumber.transform.position.z == j)
                    {
                        board[i, j] = pickUpNumber.tag;
                    } 
                }
            }
        }
        undoManager.InitialState(board);
    }

    void ReCalculateBoard()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (board[i, j] != null)
                {
                    if (board[i, j].GetType() == typeof(int))
                    {
                        board[i, j] = null;
                    } 
                }
                if (player.transform.position.x == i && player.transform.position.z == j) //updates the position of the player in the array
                {
                    board[i, j] = playerActions.moveAmount;
                    Debug.Log("player is in " + board[i, j].GetType() + " on: " + i + "," + j);
                }
            }
        }

        distaceToGoal = (int)Mathf.Abs(player.transform.position.x - goal.transform.position.x) + (int)Mathf.Abs(player.transform.position.z - goal.transform.position.z);

        if (!playerActions.hasItem)
        {
            distaceToItem = (int)Mathf.Abs(player.transform.position.x - keyItem.transform.position.x) + (int)Mathf.Abs(player.transform.position.z - keyItem.transform.position.z);
        }
        else
        {
            distaceToItem = 0;
        }

        if (pickUpNumber != null)
        {
            distaceToNumber = (int)Mathf.Abs(player.transform.position.x - pickUpNumber.transform.position.x) + (int)Mathf.Abs(player.transform.position.z - pickUpNumber.transform.position.z);
        }

        if (distaceToNumber == 0 && !playerActions.hasNumber)
        {
            if (pickUpNumber != null)
            {
                for (int i = 0; i < size.x; i++)
                {
                    for (int j = 0; j < size.y; j++)
                    {
                        if (board[i, j] != null)
                        {
                            if (pickUpNumber.transform.position.x == i && pickUpNumber.transform.position.z == j)
                            {
                                board[i, j] = playerActions.moveAmount;
                            }
                        }
                    }
                } 
            }
            CommunicateChange(GameActions.Actions.Move, pickUpNumber);
            pickUpNumber.SetActive(false);
            numberHUD.SetActive(true);
            playerActions.hasNumber = true;
        }

        if (distaceToGoal == 0 && playerActions.hasItem)
        {
            Debug.Log("you win");
        }

        if (distaceToItem == 0 && !playerActions.hasItem)
        {
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    if (board[i, j] != null)
                    {
                        if (keyItem.transform.position.x == i && keyItem.transform.position.z == j)
                        {
                            board[i, j] = playerActions.moveAmount;
                        } 
                    }
                }
            }
            CommunicateChange(GameActions.Actions.Move, keyItem);
            playerActions.hasItem = true;
            keyItem.SetActive(false);
        }
    }

    void CommunicateChange(GameActions.Actions action, GameObject @object) //This lets the UndoManager know what action modified the objects in the board
    {
        for (int i = 0; i < levelActions.Length; i++) //this translates from action to its corresponding array slot, to allow for duplicate actions in the same level
        {
            if (action == levelActions[i])
            {
                //undoManager.AddToSaveHistory(i, @object, @object.transform.position);
                undoManager.SaveBoard(i, board);
            }
        }
    }

    void CommunicateAction(NumberItem recievedNumber, GameActions.Actions usedAction)
    {
        for (int i = 0; i < levelActions.Length; i++)
        {
            if (usedAction == levelActions[i])
            {
                switch (levelActions[i])
                {
                    case GameActions.Actions.Move:
                        //undoManager.AddToSaveHistory(i, player, player.transform.position);
                        //undoManager.SaveBoard(i, board);
                        playerActions.MovementReceiver(recievedNumber.value, GameActions.Actions.Move);
                        break;

                    case GameActions.Actions.PickUp:
                        playerActions.PickUpReceiver(recievedNumber.value, distaceToItem, distaceToNumber, keyItem, pickUpNumber, numberHUD);
                        break;

                    case GameActions.Actions.Enable:
                        levelCards[recievedNumber.value - 1].Enable(false, recievedNumber);
                        break;

                    case GameActions.Actions.Throw:
                        playerActions.ThrowReceiver(recievedNumber.value, GameActions.Actions.Throw, distaceToGoal);
                        break;
                }
                levelCards[i].Disable(recievedNumber);
                sequencer.NextCard(recievedNumber);
                undoManager.ActionHistory(levelActions[i], recievedNumber);
            }
        }
    }

    /*public void CommunicateUndo(List<(GameObject gameObject, Vector3 position)> gameObjects, GameActions.Actions undoneAction, NumberItem previousNumber, NumberItem currentNumber)
    {
        sequencer.UndoSequence(previousNumber, currentNumber);
        switch (undoneAction)
        {
            case GameActions.Actions.Move:
                foreach(var @object in gameObjects)
                {
                    Debug.Log(@object.gameObject.tag);
                    if (@object.gameObject.tag == player.tag)
                    {
                        Debug.Log("retrocediendo jugador a posicion: " + @object.position);
                        playerActions.UndoMovement(@object.position);
                    }

                    if (@object.gameObject.tag == keyItem.tag)
                    {
                        playerActions.UndoPickUps(@object.gameObject);
                    }

                    if (@object.gameObject.tag == pickUpNumber.tag)
                    {
                        Debug.Log("retrocediendo numero");
                        numberHUD.SetActive(false);
                        playerActions.UndoPickUps(@object.gameObject);
                    }
                }
                break;

            case GameActions.Actions.PickUp:
                //playerActions.UndoPickUp(keyItem, pickUpNumber, numberHUD);
                break;

            case GameActions.Actions.Enable:

                break;

            case GameActions.Actions.Throw:

                break;
        }
    }*/

    public void CommunicateUndo(object[,] newBoard, GameActions.Actions undoneAction, NumberItem previousNumber, NumberItem currentNumber)
    {
        sequencer.UndoSequence(previousNumber, currentNumber);

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (newBoard[i, j] != null)
                {
                    Debug.Log("going back to board with: " + newBoard[i, j].GetType() + " on: " + i + "," + j);
                }
            }
        }

        /*switch (undoneAction)
        {
            case GameActions.Actions.Move:
                foreach (var @object in gameObjects)
                {
                    Debug.Log(@object.gameObject.tag);
                    if (@object.gameObject.tag == player.tag)
                    {
                        Debug.Log("retrocediendo jugador a posicion: " + @object.position);
                        playerActions.UndoMovement(@object.position);
                    }

                    if (@object.gameObject.tag == keyItem.tag)
                    {
                        playerActions.UndoPickUps(@object.gameObject);
                    }

                    if (@object.gameObject.tag == pickUpNumber.tag)
                    {
                        Debug.Log("retrocediendo numero");
                        numberHUD.SetActive(false);
                        playerActions.UndoPickUps(@object.gameObject);
                    }
                }
                break;

            case GameActions.Actions.PickUp:
                //playerActions.UndoPickUp(keyItem, pickUpNumber, numberHUD);
                break;

            case GameActions.Actions.Enable:

                break;

            case GameActions.Actions.Throw:

                break;
        }*/
    }
}
