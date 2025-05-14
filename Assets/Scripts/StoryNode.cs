using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryNode")]
public class StoryNode : ScriptableObject
{
    [TextArea(2, 10)] public string storyText;
    public List<StoryOption> options;
}

[System.Serializable]
public class StoryOption
{
    public string optionText;
    public StoryNode nextNode;
}