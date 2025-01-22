using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    List<(GameActions.Actions usedAction, int usedNumber, int? coordinates)> turns = new List<(GameActions.Actions usedAction, int usedNumber, int? coordinates)>();

    public void ActionHistory(GameActions.Actions usedAction, int usedNumber, int? cordX = null)
    {
        var turn = (Action: usedAction, Number: usedNumber, Coordinate: cordX);
        turns.Add(turn);

        foreach (var step in turns)
        {
            Debug.Log(step);
        }
    }
}
