using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LevelNode : MonoBehaviour
{
    [Header("Data")]
    [HideInInspector] public string levelName;
    // public int difficulty; // use to select an image or color later
    // public Sprite thumbnailSprite;
    // public Sprite difficultySprite;

    [Header("State")]
    public bool isUnlocked = false;
    public bool isCleared = false;
    
    [Header("Next Nodes")]
    public List<LevelNode> nextNodes;
    
    [Header("Visuals")]
    public TextMeshProUGUI levelTitleText;
    public Image difficultyImage; 
    public Image levelThumbnail;
    public Button levelButton;
    public GameObject lockOverlay; // lock visual — a translucent panel on top
    
    public LevelGroupNode parentGroup;
    void Start()
    {
        InitializeUI();
    }
    
    void InitializeUI()
    {
        levelTitleText.text = levelName;
        levelButton.interactable = isUnlocked;
        lockOverlay.SetActive(!isUnlocked);
        // Commented for testing. Needs to be uncommented once we have the images
        // difficultyImage.sprite = difficultySprite; 
        // levelThumbnail.sprite = thumbnailSprite; 
        //levelButton.onClick.AddListener(OnLevelSelect);
        
    }

    public void OnLevelSelect()
    {
        if (!isUnlocked) return;

        isCleared = true;
        parentGroup.UnlockInternalLevels(); //this should be removed once it is connected to levels proper
        // Unlock next nodes
        /*if(nextNodes.Count > 0)
        {
            foreach (LevelNode node in nextNodes)
            {
                //node.Unlock();
            }
        }
        
        if (parentGroup != null)
        {
            parentGroup.CheckIfGroupCleared();
        }*/
    }
    
    public void Unlock()
    {
        isUnlocked = true;
        RefreshUI();
    }
    
    void RefreshUI()
    {
        levelButton.interactable = isUnlocked;
        lockOverlay.SetActive(!isUnlocked);
    }
}
