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

    //List<(GameObject gameObject, Vector3 position)> SavedObjects = new List<(GameObject gameObject, Vector3 position)>();
    //Dictionary<int, List<(GameObject gameObject, Vector3 position)>> ActionResposibleForObject = new Dictionary<int, List<(GameObject gameObject, Vector3 position)>>();

    List<object[,]> boardHistory = new List<object[,]> ();
    int currentAction = -1;

    public void ActionHistory(GameActions.Actions usedAction, NumberItem usedNumber)
    {
        var turn = (Action: usedAction, Number: usedNumber);
        turns.Add(turn);
    }

    public void InitialState(object[,] board)
    {
        boardHistory.Add(board);
    }

    /*public void AddToSaveHistory (int actionSlot, GameObject affectedObject, Vector3 position) //Adds a list of objects that were affected by the current action to the dictionary
    {
        Debug.Log("numero del undo: " + actionSlot);
        if (affectedObject != null)
        {
            var thisturn = (affectedObject, position);
            SavedObjects.Add(thisturn);
            ActionResposibleForObject[actionSlot] = SavedObjects;
            if (ActionResposibleForObject.ContainsKey(actionSlot))
            {
                foreach (var obj in SavedObjects)
                {
                    Debug.Log("objetos en la lista: " + obj.gameObject.tag);
                }
                return;
            }
            else
            {
                SavedObjects = new List<(GameObject gameObject, Vector3 position)>(); //makes sure that different actions donÅLt get assigned GameObjects from past actions
                SavedObjects.Add(thisturn);
                ActionResposibleForObject.Add(actionSlot, SavedObjects);
            }
        }
    }*/

    public void SaveBoard(int actionSlot, object[,] board)
    {
        //board = new object[board.GetLength(0), board.GetLength(1)];  
        if(currentAction == -1) //if its the first action to have an effect on the board
        {
            currentAction = actionSlot;
            boardHistory.Add(board);

            foreach (var obj in boardHistory)
            {
                for (int i = 0; i < obj.GetLength(0); i++)
                {
                    for (int j = 0; j < obj.GetLength(1); j++)
                    {
                        if (obj[i, j] != null)
                        {
                            Debug.Log("" + obj[i, j].GetType() + " esta en: " + i + "," + j);
                        }
                    }
                }
            }
            return;
        }
        Debug.Log("return did not work");
        if (currentAction == actionSlot) //if the same action affected multiple items, update the boardHistory with the new version of the board, but leave the player unafected.
        {
            boardHistory.RemoveAt(boardHistory.Count - 1);
            boardHistory.Add(board);
            currentAction = actionSlot;

            foreach (var obj in boardHistory)
            {
                for (int i = 0; i < obj.GetLength(0); i++)
                {
                    for (int j = 0; j < obj.GetLength(1); j++)
                    {
                        if (obj[i, j] != null)
                        {
                            if (board[i, j].GetType() == typeof(int))
                            {
                                Debug.Log("" + obj[i, j].GetType() + "esta en: " + i + "," + j);  
                            }
                        }
                    }
                }
            }
        }
        else
        {
            boardHistory.Add(board);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (turns.Count == 1)
            {
                SceneManager.LoadScene(0); //change this to make it current scene
            }

            else if (turns.Count > 1)
            {
                var previousTurn = turns[turns.Count - 2];
                var thisTurn = turns[turns.Count - 1];

                Debug.Log("deshaciendo accion: " + thisTurn.usedAction + " que equivale a carta #" + previousTurn.usedNumber.value);
                Debug.Log("numero de este turno " + thisTurn.usedNumber.value);

                /*if (ActionResposibleForObject.TryGetValue(previousTurn.usedNumber.value - 1, out var affected)) //affected is a list of game objects and their positions
                {
                    foreach (var item in affected)
                    {
                        Debug.Log("accion: " + thisTurn.usedAction + " y objeto: " + item); 
                    }
                    gameManager.CommunicateUndo(affected, thisTurn.usedAction, previousTurn.usedNumber, thisTurn.usedNumber);
                }
                else
                {
                    Debug.Log("no GameObject affected");
                    gameManager.CommunicateUndo(null, thisTurn.usedAction, previousTurn.usedNumber, thisTurn.usedNumber);
                }*/
                if (thisTurn.usedAction == GameActions.Actions.Move || thisTurn.usedAction == GameActions.Actions.PickUp)
                {
                    foreach (var item in boardHistory)
                    {
                        Debug.Log("oe");
                        gameManager.CommunicateUndo(boardHistory[boardHistory.Count - 1], thisTurn.usedAction, previousTurn.usedNumber, thisTurn.usedNumber);
                        for (int i = 0; i < item.GetLength(0); i++)
                        {
                            for (int j = 0; j < item.GetLength(1); j++)
                            {
                                if (item[i, j] != null)
                                {
                                    Debug.Log("going back to board with: " + item[i, j].GetType() + " on: " + i + "," + j);
                                }
                            }
                        }
                    }
                }
                boardHistory.RemoveAt(boardHistory.Count - 1);
                turns.RemoveAt(turns.Count - 1);
            }
        }
    }


}
