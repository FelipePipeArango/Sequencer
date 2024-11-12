using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameActions : ScriptableObject {

    public enum Actions
    {
        Move, PickUp, Throw, Enable
    }

    [SerializeField] List<Actions> actions = new List<Actions>();
}
