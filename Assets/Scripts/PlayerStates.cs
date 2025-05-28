using UnityEngine;

public class PlayerStates : ScriptableObject
{
    public enum playerStates
    {
        None,
        Idle,
        Celebrating,
        PreMove,
        Moving,
        Throwing,
        PickingUp
    }
}
