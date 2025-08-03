using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "LevelGroup", menuName = "ScriptableObjects/LevelGroup", order = 1)]
public class LevelOrganizer : ScriptableObject
{
    public AssetReference[] levelOrder;
}
