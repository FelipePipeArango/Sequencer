using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameActions : ScriptableObject 
{
    public enum Actions
    {
        Move,
        Pick_Up, 
        Throw, 
        Enable
    }

    public enum AIActions
    {
        Stay,
        Move
    }
}

