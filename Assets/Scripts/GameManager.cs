using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    //Cards manager
    //The cards present in the level
    NumberItem[] levelNumbers;
    int[] numberValues;
    CardTrigger[] levelCards;
    GameActions.Actions[] levelActions;

 
    [Header("MANDATORY OBJECTS IN A LEVEL")]
    [SerializeField]
    //Card Manager
    GameObject cardsInLevel; //hopefully I'll find a way to make this fill itself instead of requiring a serialization
    [SerializeField] GameObject numbersInLevel;

    
    [SerializeField] public GameObject numberHUD;

    [Header("NO NEED TO ASSIGN THIS")]
    [SerializeField]
    Sequencer sequencer;

    //Grid manager


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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // Ensure only one instance exists
            return;
        }

        Instance = this;  // Assign the static instance

        #region NumberFilling

        //This section informs the manager about the numbers in the level
        levelNumbers = new NumberItem[numbersInLevel.transform.childCount];
        numberValues = new int[numbersInLevel.transform.childCount];
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
        
    }
     
    public void ReCalculateBoard(GameActions.Actions usedAction, GameObject affected)
    {
        GridManager.Instance.GoalCheck();
        GridManager.Instance.KeyItemCheck();
        GridManager.Instance.NumberItemCheck();
        GridManager.Instance.PickUpCheck(usedAction, affected);
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
                        //undoManager.SaveBoard(i, board);
                        GridManager.Instance.playerActions.MovementReceiver(recievedNumber.value, GameActions.Actions.Move);
                        break;

                    case GameActions.Actions.PickUp:
                        GridManager.Instance.playerActions.PickUpReceiver(
                            recievedNumber.value, 
                            GridManager.Instance.distaceToItem, 
                            GridManager.Instance.distaceToNumber, 
                            GridManager.Instance.keyItem, 
                            GridManager.Instance.pickUpNumber, numberHUD);
                        break;

                    case GameActions.Actions.Enable:
                        //undoManager.SaveBoard(i, board);
                        levelCards[recievedNumber.value - 1].Enable(false, recievedNumber);
                        break;

                    case GameActions.Actions.Throw:
                        GridManager.Instance.playerActions.ThrowReceiver(
                            recievedNumber.value, 
                            GridManager.Instance.distaceToGoal);
                        break;
                }

                levelCards[i].Disable(recievedNumber);
                sequencer.NextCard(recievedNumber);
                //undoManager.ActionHistory(levelActions[i], recievedNumber);
                break;
            }
        }
    }
}

