using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public class LevelGroupNode : MonoBehaviour
{
    [FormerlySerializedAs("levelName")][Header("Data")]
    string levelGroupName;
    [Header("Corresponds to the position of this group (1st, 2nd,etc)")]
    public int levelGroupNumber;

    public GameObject containedLevels;
    int completedLevels;
    // public int difficulty; // use to select an image or color later
    // public Sprite thumbnailSprite;
    // public Sprite difficultySprite;

    [Header("State")]
    public bool isUnlocked;
    public bool isGroupClear = false;

    [Header("Nodes Lists")]
    [HideInInspector] public LevelNode[] subLevelNodes; 
    public List<LevelGroupNode> nextNodes;
    
    [Header("Visuals")]
    public TextMeshProUGUI levelTitleText;
    public Image difficultyImage; 
    public Image levelThumbnail;
    public Button levelButton;
    public GameObject lockOverlay; // lock visual — a translucent panel on top

    private void Awake()
    {
        subLevelNodes = containedLevels.GetComponentsInChildren<LevelNode>();
    }

    public void RefillProgress(int amountLevelsCompleted)
    {
        completedLevels = amountLevelsCompleted;
        for (int i = 0; i < amountLevelsCompleted; i++)
        {
            subLevelNodes[i].isCleared = true;
        }
        UnlockInternalLevels();
    }

    public void UpdateLevelGroupUI()
    {
        Debug.Log(isUnlocked);
        if (isUnlocked)
        {
            levelGroupName = this.name;
            int i = 0;
            foreach (var level in subLevelNodes)
            {
                level.levelName = this.name + ": file_" + i;
                i++;
            }
            levelTitleText.text = levelGroupName;
            levelButton.interactable = isUnlocked; 
        }
        // Commented for testing. Needs to be uncommented once we have the images
        // difficultyImage.sprite = difficultySprite; 
        // levelThumbnail.sprite = thumbnailSprite; 
        // levelButton.onClick.AddListener(OnLevelSelect);
    }

    public void UnlockInternalLevels()
    {
        foreach (var level in subLevelNodes)
        {
            if (completedLevels >= 0)
            {
                Unlock(level);
                completedLevels--;
            }
        }
    }
    public void Unlock(LevelNode level)
    {
        level.isUnlocked = true;
        level.levelButton.interactable = true;
    }

    public bool IsGroupCompleted()
    {
        foreach (var level in subLevelNodes)
        {
            if (!level.isCleared)
                return false;
        }

        isGroupClear = true;
        return true;
    }

    public void OpenSubGroupButton() //Called by a button
    {
        containedLevels.transform.SetParent(transform.root);

        RectTransform rectTransform = containedLevels.GetComponent<RectTransform>();

        // Set anchor to middle
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

        // Reset position and pivot
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        containedLevels.SetActive(true);
        if (levelGroupNumber != 0)
        {
            SetCurrentLevelGroup();
        }
        else
        {
            Debug.LogWarning("This level has no number");
        }
    }
    public void SetCurrentLevelGroup() //Since the player can click and change the level group, this is intended to set it
    {
        LevelSelectManager.levelSelectManagerInstance.currentLevelGroup = levelGroupNumber - 1; //This changes the currentGroup being tracked
    }

    public void GoBackButton() //Called by a button
    {
        containedLevels.transform.SetParent(this.gameObject.transform);
        containedLevels.SetActive(false);
    }
}
