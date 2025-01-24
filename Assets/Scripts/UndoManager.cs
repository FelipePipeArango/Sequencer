using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UndoManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] Sequencer sequencer;

    List<(GameActions.Actions usedAction, NumberItem usedNumber)> turns = new List<(GameActions.Actions usedAction, NumberItem usedNumber)>();

    List<(GameObject gameObject, Vector3 position)> SavedObjects = new List<(GameObject gameObject, Vector3 position)>();
    Dictionary<int, List<(GameObject gameObject, Vector3 position)>> ActionResposibleForObject = new Dictionary<int, List<(GameObject gameObject, Vector3 position)>>();

    public void ActionHistory(GameActions.Actions usedAction, NumberItem usedNumber)
    {
        var turn = (Action: usedAction, Number: usedNumber);
        turns.Add(turn);
    }

    public void AddToSaveHistory (int actionSlot, GameObject affectedObject, Vector3 position) //Adds a list of objects that were affected by the current action to the dictionary
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
                SavedObjects = new List<(GameObject gameObject, Vector3 position)>(); //makes sure that different actions donLt get assigned GameObjects from past actions
                SavedObjects.Add(thisturn);
                ActionResposibleForObject.Add(actionSlot, SavedObjects);
            }
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

                if (ActionResposibleForObject.TryGetValue(previousTurn.usedNumber.value - 1, out var affected)) //affected is a list of game objects and their positions
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
                }
                turns.RemoveAt(turns.Count - 1);
            }
        }
    }


}
