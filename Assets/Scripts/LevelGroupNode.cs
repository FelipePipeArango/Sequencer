using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public class LevelGroupNode : MonoBehaviour
{
    [FormerlySerializedAs("levelName")] [Header("Data")]
    public string levelGroupName;
    [Header("Corresponds to the position of this group (1st, 2nd,etc)")]
    public int levelGroupNumber;

    public GameObject containedLevels;
    [HideInInspector] public int completedLevels;
    // public int difficulty; // use to select an image or color later
    // public Sprite thumbnailSprite;
    // public Sprite difficultySprite;

    [Header("State")]
    public bool isUnlocked = false;
    public bool isCleared = false;

    [Header("Nodes Lists")] public LevelNode[] subLevelNodes; 
    public List<LevelGroupNode> nextNodes;
    
    [Header("Visuals")]
    public TextMeshProUGUI levelTitleText;
    public Image difficultyImage; 
    public Image levelThumbnail;
    public Button levelButton;
    public GameObject lockOverlay; // lock visual — a translucent panel on top
    
    void Start()
    {
        InitializeUI();
        UnlockNextLevels();
    }

    void UnlockNextLevels()
    {
        for (int i = 0; i < subLevelNodes.Length; i++)
        {
            if (subLevelNodes[i].isCleared == false)
            {
                return;
            }
            else if (i == subLevelNodes.Length - 1)
            {
                Debug.Log("this sections is complete");
            } 
        }
    }
    
    void InitializeUI()
    {
        subLevelNodes = containedLevels.GetComponentsInChildren<LevelNode>();
        foreach (var level in subLevelNodes)
        {
            level.parentGroup = this;
        }
        levelTitleText.text = levelGroupName;
        levelButton.interactable = isUnlocked;
        lockOverlay.SetActive(!isUnlocked);
        // Commented for testing. Needs to be uncommented once we have the images
        // difficultyImage.sprite = difficultySprite; 
        // levelThumbnail.sprite = thumbnailSprite; 
        // levelButton.onClick.AddListener(OnLevelSelect);
    }

    // void OnLevelSelect()
    // {
    //     if (!isUnlocked) return;
    //
    //     isCleared = true;
    //     Debug.Log($"{levelGroupName} cleared!");
    //
    //     // Unlock next nodes
    //     foreach (LevelNode node in nextNodes)
    //     {
    //         node.Unlock();
    //     }
    // }
    
    public void Unlock()
    {
        if (!isUnlocked)
        {
            isUnlocked = true;
            levelButton.interactable = true;
            lockOverlay.SetActive(false);
        }
    }

    public void SetCurrentLevelGroup()
    {
        LevelSelectManager.Instance.currentLevelGroup = levelGroupNumber;
    }
    
    public void CheckIfGroupCleared()
    {
        foreach (var level in subLevelNodes)
        {
            if (!level.isCleared)
                return;
        }

        isCleared = true;

        if (nextNodes.Count > 0)
        {
            foreach (var nextGroup in nextNodes)
            {
                nextGroup.Unlock();
            }
        }
        //LevelSelectManager.Instance.ReturnToMainMenu();
    }
}
