using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UndoManager : MonoBehaviour
{
    [SerializeField] Sequencer sequencer;
    List<(GameActions.Actions usedAction, NumberItem usedNumber)> turns = new List<(GameActions.Actions usedAction, NumberItem usedNumber)>();
    //List<(GameActions.Actions usedAction, int usedNumber, int previousNumber)> turns = new List<(GameActions.Actions usedAction, int usedNumber, int previousNumber)>();
    List<Vector3> objectCoordinates = new List<Vector3> ();

    public void ActionHistory(GameActions.Actions usedAction, NumberItem usedNumber)
    {
        var turn = (Action: usedAction, Number: usedNumber);
        turns.Add(turn);

        /*foreach (var step in turns)
        {
            Debug.Log(step);
        }*/
    }

    public void SaveObjectPositions (Vector3 coordinate)
    {
        objectCoordinates.Add(coordinate);
        /*foreach (var xy in objectCoordinates)
        {
            Debug.Log(xy);
        }*/
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
                Debug.Log("deshaciendo carta #" + previousTurn.usedNumber.value);
                Debug.Log("numero de este turno " + thisTurn.usedNumber.value);
                sequencer.UndoCard(previousTurn.usedNumber, thisTurn.usedNumber);
                turns.RemoveAt(turns.Count - 1);
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {

        }
    }


}
