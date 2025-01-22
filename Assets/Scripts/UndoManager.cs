using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    [SerializeField] Sequencer sequencer;

    List<(GameActions.Actions usedAction, int usedNumber)> turns = new List<(GameActions.Actions usedAction, int usedNumber)>();
    List<Vector3> objectCoordinates = new List<Vector3> ();

    public void ActionHistory(GameActions.Actions usedAction, int usedNumber)
    {
        var turn = (Action: usedAction, Number: usedNumber);
        turns.Add(turn);

        foreach (var step in turns)
        {
            Debug.Log(step);
        }
    }

    public void SaveObjectPositions (Vector3 coordinate)
    {
        objectCoordinates.Add(coordinate);
        foreach (var xy in objectCoordinates)
        {
            Debug.Log(xy);
        }
    }

    private void Update()
    {
        if (turns.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                var previousTurn = turns[turns.Count - 1];
                Debug.Log(previousTurn);
                sequencer.PreviousCard(previousTurn.usedNumber);
                turns.Remove(previousTurn);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
               
            }
        }
    }
}
