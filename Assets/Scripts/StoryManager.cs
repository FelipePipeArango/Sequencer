using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI storyText;
    public Button[] optionButtons; 
    
    [Header("Starting Node")]
    public StoryNode startingNode;
    
    private StoryNode currentNode;
    

    private void Start()
    {
        DisplayNode(startingNode);
    }

    public void DisplayNode(StoryNode node)
    {
        currentNode = node;
        storyText.text = node.storyText;
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < node.options.Count)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = node.options[i].optionText;

                int index = i; // Store index separately for lambda 
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => SelectOption(index));
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void SelectOption(int index)
    {
        var nextNode = currentNode.options[index].nextNode;
        if (nextNode != null)
        {
            DisplayNode(nextNode);
        }
        
    }
}
