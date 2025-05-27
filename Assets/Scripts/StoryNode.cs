using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryNode")]
public class StoryNode : ScriptableObject
{
    [Header("Add story text here. The text will be displayed at runtime.")]
    [TextArea(2, 10)] public string storyText;
    
    [Header("Add story options here. These options will be visible at runtime. The 'Next Node' wil contain the StoryNode object that will be displayed when the players selects a particular option.")]
    public List<StoryOption> options;
}

[System.Serializable]
public class StoryOption
{
    public string optionText;
    public StoryNode nextNode;
}