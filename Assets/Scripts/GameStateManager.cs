using UnityEngine;
using static GameStates;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager gameStateManagerInstance;

    public delegate void StateChangeEvent(gameStates state);
    public static event StateChangeEvent stateEvent;

    gameStates currentState;

    private void Awake()
    {
        if (gameStateManagerInstance == null)
            gameStateManagerInstance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        currentState = gameStates.Start;
        CommunicateStateChange (currentState);
    }

    public void CommunicateStateChange(gameStates newState)
    {
        currentState = newState;
        if (stateEvent != null)
            stateEvent(currentState); //Calls any script that to acts depending on the game state;
    }
}
