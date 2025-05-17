using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerStates;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager playerManagerInstance;

    public delegate void PlayerDidSomething (playerStates playerState);
    public static event PlayerDidSomething playerDidSomething;

    playerStates currentPlayerState;

    private void Awake()
    {
        if (playerManagerInstance == null)
            playerManagerInstance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        currentPlayerState = playerStates.None;
    }

    //Observes Player win
    public void PlayerWon()
    {
        Debug.Log("GOAL");
        currentPlayerState = playerStates.Celebrating;

        if (playerDidSomething != null)
            playerDidSomething(currentPlayerState);

        UIHandler.UIHandlerInstance.OpenWinScreen();        
    }

    //Observes player move
    public void PlayerMoved(int movePoints)
    {
        currentPlayerState = playerStates.Moving;

        if (playerDidSomething != null)
            playerDidSomething(currentPlayerState);

        UIHandler.UIHandlerInstance.UpdateMovementPointsText(movePoints);
    }

    //Observes player gain key item
    public void PlayerPickedUp()
    {
        currentPlayerState = playerStates.PickingUp;

        if (playerDidSomething != null)
            playerDidSomething(currentPlayerState);
    }

    //Observes player gain numbe ritem
}
