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

    [Header("SIZE OF THE BOARD - STARTS FROM 1")]
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
        UnitControler.OnObjectPickUp += ReCalculateBoard;
    }
    
    private void OnDisable()
    {
        CardTrigger.OnDropAction -= CommunicateAction;
        UnitControler.OnMovement -= ReCalculateBoard;
        UnitControler.OnObjectPickUp -= ReCalculateBoard;
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

        board [(int) player.transform.position.x, (int) player.transform.position.z] = playerActions.moveAmount;
        board[(int)keyItem.transform.position.x, (int)keyItem.transform.position.z] = keyItem.tag;
        if (pickUpNumber != null)
        {
            board[(int)pickUpNumber.transform.position.x, (int)pickUpNumber.transform.position.z] = pickUpNumber.tag; 
        }

        undoManager.InitialState(board);
    }

    void ReCalculateBoard(GameActions.Actions usedAction, GameObject affected)
    {
        //The array board is mostly used for the undo system of the game, keeping the player updated in that array is here.
        #region Updates player position in the array board
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
                }
            }
        }
        #endregion

        //Goal distance measuring and it's interactios
        #region Goal Checks
        distaceToGoal = (int)Mathf.Abs(player.transform.position.x - goal.transform.position.x) + (int)Mathf.Abs(player.transform.position.z - goal.transform.position.z);

        if (distaceToGoal == 0 && playerActions.hasItem)
        {
            Debug.Log("you win");
        }
        #endregion

        //KeyItem distance measuring and it's interactions
        #region KeyItem Checks
        if (!playerActions.hasItem)
        {
            distaceToItem = (int)Mathf.Abs(player.transform.position.x - keyItem.transform.position.x) + (int)Mathf.Abs(player.transform.position.z - keyItem.transform.position.z);
        }

        if (distaceToItem == 0 && !playerActions.hasItem) //this updates the board array if the player picks the item up manually
        {
            board[(int) player.transform.position.x, (int)player.transform.position.z] = playerActions.moveAmount; //the array where the item was is now updated with the player
            playerActions.hasItem = true;
            keyItem.SetActive(false);
        }
        #endregion

        //NumberItem distance measuring and it's interactions
        #region NumberItem  Checks
        if (pickUpNumber != null)
        {
            distaceToNumber = (int)Mathf.Abs(player.transform.position.x - pickUpNumber.transform.position.x) + (int)Mathf.Abs(player.transform.position.z - pickUpNumber.transform.position.z);

            if (distaceToNumber == 0 && !playerActions.hasNumber) //this updates the board array if the player picks the number up manually
            {
                board[(int)player.transform.position.x, (int)player.transform.position.z] = playerActions.moveAmount; //the array where the number was is now updated with the player
                pickUpNumber.SetActive(false);
                numberHUD.SetActive(true);
                playerActions.hasNumber = true;
            }
        }
        else
        {
            distaceToNumber = 0;
        }
        #endregion

        //The PickUp action may affect objects in game, therefore, the array board must be updated
        #region PickUp checks
        if (usedAction == GameActions.Actions.PickUp && affected != null)
        {
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    if (board[i, j] != null && board[i, j].GetType() != typeof(int))
                    {
                        if (affected.CompareTag(board[i, j].ToString()))
                        {
                            board[i, j] = null;
                        }
                    }
                }
            }
        }
        #endregion
        CommunicateChange(usedAction);
    }

    void CommunicateChange(GameActions.Actions action) //This lets the UndoManager know that a board changing action has been used
    {
        for (int i = 0; i < levelActions.Length; i++) //this translates from action to its corresponding array slot, to allow for duplicate actions in the same level
        {
            if (action == levelActions[i])
            {
                undoManager.SaveBoard(i, board);
            }
        }
    }

    void CommunicateAction(NumberItem recievedNumber, GameActions.Actions usedAction)
    {
        for (int i = 0; i < levelActions.Length; i++)
        {
            if (usedAction == levelActions[i] //The card slot that's equal to the recieved number
                && levelCards[i].available == true) //allows for multiple cards of the same type
            {
                switch (levelActions[i])
                {
                    case GameActions.Actions.Move:
                        undoManager.SaveBoard(i, board);
                        playerActions.MovementReceiver(recievedNumber.value, GameActions.Actions.Move);
                        break;

                    case GameActions.Actions.PickUp:
                        playerActions.PickUpReceiver(recievedNumber.value, distaceToItem, distaceToNumber, keyItem, pickUpNumber, numberHUD);
                        break;

                    case GameActions.Actions.Enable:
                        undoManager.SaveBoard(i, board);
                        levelCards[recievedNumber.value - 1].Enable(false, recievedNumber);
                        break;

                    case GameActions.Actions.Throw:
                        playerActions.ThrowReceiver(recievedNumber.value, GameActions.Actions.Throw, distaceToGoal);
                        break;
                }
                levelCards[i].Disable(recievedNumber);
                sequencer.NextCard(recievedNumber);
                undoManager.ActionHistory(levelActions[i], recievedNumber);
                break;
            }
        }
    }

    public void CommunicateUndo(object[,] newBoard, GameActions.Actions undoneAction, NumberItem previousNumber, NumberItem currentNumber)
    {
        sequencer.UndoSequence(previousNumber, currentNumber);

        if (undoneAction == GameActions.Actions.Throw)
        {
            if (playerActions.hasItem == true)
            {
                playerActions.UndoThrow(true);
            }
            else
            {
                playerActions.UndoThrow(false);
            }
        }

        if (undoneAction == GameActions.Actions.Enable)
        {
            levelCards[currentNumber.value - 1].Disable(previousNumber);
        }

        if (newBoard != null)
        {
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    if (newBoard[i, j] != null)
                    {
                        if(undoneAction == GameActions.Actions.Move)
                        {
                            if (newBoard[i, j].GetType() == typeof(int))
                            {
                                playerActions.UndoMovement(i, j, (int) newBoard[i, j]);
                            }
                            if (newBoard[i, j].GetType() != typeof(int))
                            {
                                if (keyItem.CompareTag(newBoard[i, j].ToString()))
                                {
                                    keyItem.SetActive(true);
                                    playerActions.hasItem = false;
                                }

                                if (pickUpNumber != null)
                                {
                                    if (pickUpNumber.CompareTag(newBoard[i, j].ToString()))
                                    {
                                        playerActions.hasNumber = false;
                                        pickUpNumber.SetActive(true);
                                    } 
                                }
                            }
                        }

                        if(undoneAction == GameActions.Actions.PickUp)
                        {
                            if (newBoard[i, j].GetType() != typeof(int))
                            {
                                if (keyItem.CompareTag(newBoard[i, j].ToString()))
                                {
                                    playerActions.UndoPickUps(keyItem);
                                }
                                if (pickUpNumber != null)
                                {
                                    if (pickUpNumber.CompareTag(newBoard[i, j].ToString()))
                                    {
                                        playerActions.UndoPickUps(pickUpNumber);
                                    } 
                                }
                            }
                        }
                    }
                }
            }


        }
    }
}
