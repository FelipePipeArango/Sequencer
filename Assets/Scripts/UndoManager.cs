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

    GameActions.Actions currentAction;

    List<(GameActions.Actions usedAction, NumberItem usedNumber)> turns = new List<(GameActions.Actions usedAction, NumberItem usedNumber)>();
    //List<(Vector3 coordinates, GameObject affectedObject)> objectCoordinates = new List<(Vector3 coordinates, GameObject affectedObject)> ();

    List<GameObject> objectCoordinatesTest = new List<GameObject>();
    Dictionary<GameActions.Actions, List<GameObject>> test = new Dictionary<GameActions.Actions, List<GameObject>>();

    private void OnEnable()
    {
        UnitControler.OnObjectPickUp += Test2;
    }

    private void OnDisable()
    {
        UnitControler.OnObjectPickUp -= Test2;
    }

    public void ActionHistory(GameActions.Actions usedAction, NumberItem usedNumber)
    {
        currentAction = usedAction;
        var turn = (Action: usedAction, Number: usedNumber);
        turns.Add(turn);
        Test2(usedAction,null);
    }

    /*public void SaveObjectPositions(Vector3 coordinates, GameObject affectedObject)
    {
        var ObjectPosition = (Coordinate: coordinates, AffectedObject: affectedObject);
        objectCoordinates.Add(ObjectPosition);
    }*/

    public void Test2(GameActions.Actions usedAction, GameObject affectedObject)
    {
        if (affectedObject != null)
        {
            objectCoordinatesTest.Add(affectedObject);
        }

        if (currentAction != usedAction)
        {
            test.Add(usedAction, objectCoordinatesTest);
        }

        if (test.TryGetValue(usedAction, out var action))
        {
            Debug.Log(action.ToString());
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

                if (test.TryGetValue(thisTurn.usedAction, out var action)) //action is a list of game objects
                {
                    gameManager.CommunicateUndo(action, thisTurn.usedAction, previousTurn.usedNumber, thisTurn.usedNumber);
                }
                else
                {
                    gameManager.CommunicateUndo(null, thisTurn.usedAction, previousTurn.usedNumber, thisTurn.usedNumber);
                }

                /*if (thisTurn.usedAction == GameActions.Actions.Move || thisTurn.usedAction == GameActions.Actions.PickUp)
                {
                    var previousPosition = objectCoordinates[objectCoordinates.Count - 1];
                    objectCoordinates.RemoveAt(objectCoordinates.Count - 1);

                    if (test.TryGetValue(usedAction, out var action))
                    {
                        foreach (var savedObject in action) //each action can affect multiple objects, this is saying to give me the list of objects affected by usedAction
                        {
                            Debug.Log(savedObject.ToString());
                        }
                    }

                    gameManager.CommunicateUndo(previousPosition.coordinates, , thisTurn.usedAction, previousTurn.usedNumber, thisTurn.usedNumber);
                }
                else
                {
                    var ignorePosition = new Vector3 (0,0,0);
                    gameManager.CommunicateUndo(ignorePosition, null, thisTurn.usedAction, previousTurn.usedNumber, thisTurn.usedNumber);
                }*/

                turns.RemoveAt(turns.Count - 1);
            }
        }
    }


}
