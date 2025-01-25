using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class UndoManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] Sequencer sequencer;

    List<(GameActions.Actions usedAction, NumberItem usedNumber)> turns = new List<(GameActions.Actions usedAction, NumberItem usedNumber)>();

    List<object[,]> boardHistory = new List<object[,]> ();
    int currentAction = -1;
    object[,] savedBoard;

    public void ActionHistory(GameActions.Actions usedAction, NumberItem usedNumber)
    {
        var turn = (Action: usedAction, Number: usedNumber);
        turns.Add(turn);
    }

    public void InitialState(object[,] board)
    {
        savedBoard = new object[board.GetLength(0), board.GetLength(1)];

        for (int i = 0; i < savedBoard.GetLength(0); i++)
        {
            for (int j = 0; j < savedBoard.GetLength(1); j++)
            {
                savedBoard[i, j] = board[i, j];
            } 
        }

        boardHistory.Add(savedBoard);
    }

    public void SaveBoard(int actionSlot, object[,] board)
    {
        savedBoard = new object[board.GetLength(0), board.GetLength(1)];
        for (int i = 0; i < savedBoard.GetLength(0); i++)
        {
            for (int j = 0; j < savedBoard.GetLength(1); j++)
            {
                if (board[i, j] != null)
                {
                    savedBoard[i, j] = board[i, j]; 
                }
            }
        }
        if (currentAction == -1) //if its the first action to have an effect on the board
        {
            currentAction = actionSlot;
            boardHistory.Add(savedBoard);

            return; //makes sure it doesn't go into the next if, that said, a switch could have done the same, huh?
        }

        if (currentAction == actionSlot) //if the same action affected multiple items, update the boardHistory with the new version of the board.
        {
            boardHistory.RemoveAt(boardHistory.Count - 1);
            boardHistory.Add(savedBoard);
        }
        else
        {
            currentAction = actionSlot;
            boardHistory.Add(savedBoard);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (turns.Count == 1)
            {
                string currentScene = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentScene);
            }

            else if (turns.Count > 1)
            {
                var previousTurn = turns[turns.Count - 2];
                var thisTurn = turns[turns.Count - 1];

                Debug.Log("deshaciendo accion: " + thisTurn.usedAction + " que equivale a carta #" + previousTurn.usedNumber.value);
                Debug.Log("numero de este turno " + thisTurn.usedNumber.value);

                if (thisTurn.usedAction == GameActions.Actions.Move || thisTurn.usedAction == GameActions.Actions.PickUp)
                {
                    gameManager.CommunicateUndo(boardHistory[boardHistory.Count - 2], thisTurn.usedAction, previousTurn.usedNumber, thisTurn.usedNumber);
                    boardHistory.RemoveAt(boardHistory.Count - 1);
                    currentAction = -1; //makes sure that the current action doesn't counts as a new action, since the previous one has been undone
                }
                else
                {
                    gameManager.CommunicateUndo(null, thisTurn.usedAction, previousTurn.usedNumber, thisTurn.usedNumber);
                }
                turns.RemoveAt(turns.Count - 1);
            }
        }
    }


}
