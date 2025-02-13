using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    //Cards manager
    //The cards present in the level
    NumberItem[] levelNumbers;
    int[] numberValues;
    CardTrigger[] levelCards;
    GameActions.Actions[] levelActions;

    [SerializeField] GameObject tile;

    //Grid manager
    object[,] board;
    [Header("SIZE OF THE BOARD - STARTS FROM 1")]
    [SerializeField] public Vector2Int size;
    [Header("MANDATORY OBJECTS IN A LEVEL")]
    [SerializeField]
    //Card Manager
    GameObject cardsInLevel; //hopefully I'll find a way to make this fill itself instead of requiring a serialization
    [SerializeField] GameObject numbersInLevel;

    

    //Grid manager
    //Make this a prefab, add player under GameManager, and assign player to UnitController
    [SerializeField] GameObject player;
    [SerializeField] GameObject keyItem;
    [SerializeField] GameObject goal;

    [Header("OPTIONAL OBJECTS IN A LEVEL")]
    [SerializeField]

    GameObject pickUpNumber;

    [SerializeField] GameObject numberHUD;

    [Header("NO NEED TO ASSIGN THIS")]
    [SerializeField]
    Sequencer sequencer;
    //Managers
    UndoManager undoManager;
   public UnitControler playerActions;

    //Grid manager
    private List<Vector2Int> affectedCells = new List<Vector2Int>();
    private int distaceToItem, distaceToGoal, distaceToNumber;


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

    public Vector3 GetPlayerPos()
    {
        return playerActions.transform.position;
    }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // Ensure only one instance exists
            return;
        }

        Instance = this;  // Assign the static instance

        playerActions = player.GetComponent<UnitControler>();
        undoManager = this.GetComponent<UndoManager>();
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
        
        distaceToItem = CalculateDistance(keyItem);
        distaceToGoal = CalculateDistance(goal);

        if (pickUpNumber != null)
            distaceToNumber = CalculateDistance(pickUpNumber);
         

        board = new object[size.x, size.y];
        Vector2Int PlayerPosition = PosConverter(player.transform.position);
        // Clamp player position to ensure it fits within the board size
        board[PlayerPosition.x, PlayerPosition.y] = playerActions.moveAmount;

        Vector2Int keyPosition = PosConverter(keyItem.transform.position);
        board[keyPosition.x, keyPosition.y] = keyItem.tag;

        if (pickUpNumber != null)
        {
            Vector2Int PickUpNumberPosition = PosConverter(pickUpNumber.transform.position);
            board[PickUpNumberPosition.x, PickUpNumberPosition.y] = pickUpNumber.tag;
        }

        undoManager.InitialState(board);
    }

    public Vector2Int PosConverter(Vector3 converted)
    {
        Vector2Int PosConverted = new Vector2Int(
            Mathf.Clamp(Mathf.FloorToInt(converted.x), 0, size.x - 1),
            Mathf.Clamp(Mathf.FloorToInt(converted.z), 0, size.y - 1));
        return PosConverted;
    }
    // takes game object and calculates distance 
    // redundant(but still in use), needs rewriting  
    
    public int CalculateDistance(GameObject target)
    {
        Vector2Int currentPosition = new Vector2Int(
            Mathf.FloorToInt(playerActions.transform.position.x),
            Mathf.FloorToInt(playerActions.transform.position.z)
        );
        Vector2Int targetPosition = new Vector2Int(
            Mathf.FloorToInt(target.transform.position.x),
            Mathf.FloorToInt(target.transform.position.z)
        );
        int distance = Mathf.Abs(currentPosition.x - targetPosition.x) +
                       Mathf.Abs(currentPosition.y - targetPosition.y);
        return distance;
    }
    // Takes position and calculates the distance 
    public int CalculateDistance(Vector3 position)
    {
        Vector2Int currentPosition = new Vector2Int(
            Mathf.FloorToInt(playerActions.transform.position.x),
            Mathf.FloorToInt(playerActions.transform.position.z)
        );
        Vector2Int targetPosition = new Vector2Int(
            Mathf.FloorToInt(position.x),
            Mathf.FloorToInt(position.z)
        );
        int distance = Mathf.Abs(currentPosition.x - targetPosition.x) +
                       Mathf.Abs(currentPosition.y - targetPosition.y);
        return distance;
    }

    void UpdatePlayerPosition(Vector2Int previousPosition)
    {
        
        previousPosition = new Vector2Int(
            Mathf.Clamp(playerActions.GetPreviousPosition().x, 0, size.x - 1),
            Mathf.Clamp(playerActions.GetPreviousPosition().y, 0, size.y - 1));

        // Add the previous position to the affected cells
        affectedCells.Add(previousPosition);

        // Clear the previous position on the grid
        if (previousPosition.x >= 0 && previousPosition.x < size.x &&
            previousPosition.y >= 0 && previousPosition.y < size.y)
        {
            board[previousPosition.x, previousPosition.y] = null;
        }
        else
            Debug.LogWarning($"Previous Position {previousPosition} is out of bounds!");


        // Calculate the new position
        Vector2Int newPosition = PosConverter(player.transform.position);

        // Add the new position to the affected cells
        affectedCells.Add(newPosition);

        // Update the new position on the grid
        if (newPosition.x >= 0 && newPosition.x < size.x &&
            newPosition.y >= 0 && newPosition.y < size.y)
        {
            board[newPosition.x, newPosition.y] = playerActions.moveAmount;
        }
        else
            Debug.LogWarning($"New Position {newPosition} is out of bounds!");

    }

    void GoalCheck()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        distaceToGoal = CalculateDistance(goal);

        if (distaceToGoal == 0 && playerActions.hasItem)
        {
            if(nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
    }

    void KeyItemCheck()
    {
        if (!playerActions.hasItem)
        {
            distaceToItem = CalculateDistance(keyItem);
        }

        //this updates the board array if the player picks the item up manually
        if (distaceToItem == 0 && !playerActions.hasItem)
        {
            Vector2Int currentPosition = PosConverter(player.transform.position);
            // Add the new position to the affected cells
            affectedCells.Add(currentPosition);

            //the array where the item was is now updated with the player
            board[currentPosition.x, currentPosition.y] = playerActions.moveAmount;
            playerActions.hasItem = true;
            keyItem.SetActive(false);
        }
    }

    void NumberItemCheck()
    {
        if (pickUpNumber != null)
        {
            distaceToNumber = CalculateDistance(pickUpNumber);
        }

        if (pickUpNumber != null)
        {
            //this updates the board array if the player picks the item up manually
            if (distaceToNumber == 0 && !playerActions.hasNumber)
            {
                Vector2Int currentPosition = PosConverter(player.transform.position);
                // Add the new position to the affected cells
                affectedCells.Add(currentPosition);
                //the array where the item was is now updated with the player
                board[currentPosition.x, currentPosition.y] = playerActions.moveAmount;

                pickUpNumber.SetActive(false);
                numberHUD.SetActive(true);
                playerActions.hasNumber = true;
            }
        }
    }

    void PickUpCheck(GameActions.Actions usedAction, GameObject affected)
    {
        //No need for "for" loop as we know the position of everything
        if (pickUpNumber == null && affected == pickUpNumber)
        {
            Debug.Log("Pick Up Number is null");
        }
        else
        {
            if (usedAction == GameActions.Actions.PickUp && affected != null)

            {
                Vector2Int affectedPosition = PosConverter(affected.transform.position);
                // Add the item's position to the affected cells
                affectedCells.Add(affectedPosition);

                if (board[affectedPosition.x, affectedPosition.y] != null)
                {
                    board[affectedPosition.x, affectedPosition.y] = null;
                }
            }
        }

    }


    void ReCalculateBoard(GameActions.Actions usedAction, GameObject affected)
    {
        //Updates player position in the array board
        UpdatePlayerPosition(playerActions.GetPreviousPosition());

        //Goal distance measuring and it's interactios
        //Goal Checks
        GoalCheck();

        //KeyItem distance measuring and it's interactions
        //KeyItem Checks
        KeyItemCheck();

        //NumberItem distance measuring and it's interactions
        //NumberItem  Checks
        NumberItemCheck();

        //The PickUp action may affect objects in game, therefore, the array board must be updated
        //PickUp checks
        PickUpCheck(usedAction, affected);


        CommunicateChange(usedAction);
    }




    //This lets the UndoManager know that a board changing action has been used
    void CommunicateChange(GameActions.Actions action)
    {
        //this translates from action to its corresponding array slot, to allow for duplicate actions in the same level
        for (int i = 0; i < levelActions.Length; i++)
        {
            if (action == levelActions[i]) undoManager.SaveBoard(i, board);
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
                        playerActions.MovementReceiver(recievedNumber.value /*, GameActions.Actions.Move*/);
                        break;

                    case GameActions.Actions.PickUp:
                        playerActions.PickUpReceiver(
                            recievedNumber.value, distaceToItem, distaceToNumber, keyItem, pickUpNumber, numberHUD);
                        break;

                    case GameActions.Actions.Enable:
                        undoManager.SaveBoard(i, board);
                        levelCards[recievedNumber.value - 1].Enable(false, recievedNumber);
                        break;

                    case GameActions.Actions.Throw:
                        playerActions.ThrowReceiver(recievedNumber.value, /*GameActions.Actions.Throw,*/ distaceToGoal);
                        break;
                }

                levelCards[i].Disable(recievedNumber);
                sequencer.NextCard(recievedNumber);
                undoManager.ActionHistory(levelActions[i], recievedNumber);
                break;
            }
        }
    }

    public void CommunicateUndo(
        object[,] newBoard, GameActions.Actions undoneAction,
        NumberItem previousNumber, NumberItem currentNumber)
    {

        sequencer.UndoSequence(previousNumber, currentNumber);

        if (undoneAction == GameActions.Actions.Throw)
            playerActions.UndoThrow(playerActions.hasItem);


        if (undoneAction == GameActions.Actions.Enable)
            levelCards[currentNumber.value - 1].Disable(previousNumber);

        //No need to go through the whole grid
        //Created new variable for affected cells
        if (newBoard != null)
        {
            foreach (Vector2Int cell in affectedCells)
            {
                if (newBoard[cell.x, cell.y] != null)
                {
                    if (undoneAction == GameActions.Actions.Move)
                    {
                        if (newBoard[cell.x, cell.y].GetType() == typeof(int))
                        {
                            playerActions.UndoMovement(cell.x, cell.y, (int)newBoard[cell.x, cell.y]);
                        }

                        if (newBoard[cell.x, cell.y].GetType() != typeof(int))
                        {
                            if (keyItem.CompareTag(newBoard[cell.x, cell.y].ToString()))
                            {
                                keyItem.SetActive(true);
                                playerActions.hasItem = false;
                            }

                            if (pickUpNumber != null)
                            {
                                if (pickUpNumber.CompareTag(newBoard[cell.x, cell.y].ToString()))
                                {
                                    pickUpNumber.SetActive(true);
                                    playerActions.hasNumber = false;
                                }
                            }
                        }
                    }

                    if (undoneAction == GameActions.Actions.PickUp)
                    {
                        if (newBoard[cell.x, cell.y].GetType() != typeof(int))
                        {
                            if (keyItem.CompareTag(newBoard[cell.x, cell.y].ToString()))
                            {
                                playerActions.UndoPickUps(keyItem);
                            }
                            if (pickUpNumber != null)
                            {
                                if (pickUpNumber.CompareTag(newBoard[cell.x, cell.y].ToString()))
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

