using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "Card", order = 1)]

public class CardSO: ScriptableObject
{
    [SerializeField] enum Dropdown
    {
        Move, PickUp, Throw, Enable
    }
    [SerializeField] Dropdown dropdown;
}
