using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static Difficulties;

[System.Serializable]
public class LevelGroupNode : MonoBehaviour
{
    string levelGroupName;
    
    [HideInInspector] public int levelGroupNumber;

    [Header("DON'T MODIFY")]
    public GameObject containedLevels;
    int completedLevels;

    [Header("State")]
    public bool isUnlocked;
    [HideInInspector] public bool isGroupClear = false;

    [Header("Visual Color")]
    [SerializeField] Color groupColor;

    [Header("Difficulty of the node")]
    [SerializeField] difficultyLevel nodeDifficulty;
    [SerializeField] Difficulties difficultyDB;

    [HideInInspector] public LevelNode[] subLevelNodes;

    [Header("Groups connected to this")]
    public List<LevelGroupNode> nextNodes;
    
    [Header("Visuals - don't modify")]
    public TextMeshProUGUI levelTitleText;
    public Image hoverImage; 
    public Image levelThumbnail;
    public Button levelButton;
    [SerializeField] Image difficultyImage;


    private void Awake()
    {
        Color nodeColor;
        subLevelNodes = containedLevels.GetComponentsInChildren<LevelNode>();
        levelThumbnail.color = groupColor;
        hoverImage.color = groupColor;
        nodeColor = difficultyDB.AssignColorDifficulty(nodeDifficulty);
        difficultyImage.color = nodeColor;
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
        Debug.Log("first");
        if (isUnlocked)
        {
            Debug.Log("second");
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
    }

    void UnlockInternalLevels()
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
    void Unlock(LevelNode level)
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
        SetCurrentLevelGroup();
    }
    void SetCurrentLevelGroup() //Since the player can click and change the level group, this is intended to set it
    {
        LevelSelectManager.levelSelectManagerInstance.currentLevelGroup = levelGroupNumber; //This changes the currentGroup being tracked
    }

    public void GoBackButton() //Called by a button
    {
        containedLevels.transform.SetParent(this.gameObject.transform);
        containedLevels.SetActive(false);
    }
}
