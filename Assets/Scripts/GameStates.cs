using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStates : ScriptableObject
{
   public enum gameStates
    {
        Start,
        Playing,
        Completed,
        Paused,
        DeadEnd,
    }
}
