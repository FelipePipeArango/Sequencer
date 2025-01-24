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
    }
    
    private void OnDisable()
    {
        CardTrigger.OnDropAction -= CommunicateAction;
        UnitControler.OnMovement -= ReCalculateBoard;
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
    }

    void ReCalculateBoard()
    {
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
            undoManager.Test2(GameActions.Actions.Move, pickUpNumber);
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
            undoManager.Test2(GameActions.Actions.Move, keyItem);
            playerActions.hasItem = true;
            keyItem.SetActive(false);
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
                        //undoManager.SaveObjectPositions(player.transform.position, pickUpNumber);
                        playerActions.MovementReceiver(recievedNumber.value, GameActions.Actions.Move);
                        break;

                    case GameActions.Actions.PickUp:
                        //undoManager.SaveObjectPositions(keyItem.transform.position, keyItem);
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

    public void CommunicateUndo(List<GameObject> gameObjects, GameActions.Actions undoneAction, NumberItem previousNumber, NumberItem currentNumber)
    {
        switch (undoneAction)
        {
            case GameActions.Actions.Move:
                foreach(var @object in gameObjects)
                {
                    if (@object == player)
                    {
                        playerActions.UndoMovement(@object.transform.position);
                    }

                    if (@object == keyItem)
                    {
                        playerActions.UndoPickUps(@object);
                    }

                    if (@object == pickUpNumber)
                    {
                        numberHUD.SetActive(false);
                        playerActions.UndoPickUps(@object);
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
        sequencer.UndoSequence(previousNumber, currentNumber);
    }
}
