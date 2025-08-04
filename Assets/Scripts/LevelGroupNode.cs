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
    bool unlockEverything;
    string levelGroupName;
    
    [HideInInspector] public int levelGroupNumber;

    [Header("DON'T MODIFY")]
    public GameObject containedLevels;
    int completedLevels = 0;

    [Header("State")]
    public bool isUnlocked;
    [HideInInspector] public bool isGroupClear = false;

    [Header("Visual Color")]
    [SerializeField] Color groupColor;

    [Header("Assigned Group")]
    [SerializeField] LevelOrganizer levelOrder;

    [HideInInspector] public LevelNode[] subLevelNodes;

    [Header("Groups connected to this")]
    public List<LevelGroupNode> nextNodes;
    
    [Header("Visuals - don't modify")]
    public TextMeshProUGUI levelTitleText;
    public Image hoverImage; 
    public Image levelThumbnail;
    public Button levelButton;
    [SerializeField] GameObject completedImage;
    [SerializeField] TextMeshProUGUI groupLevelsText;
    [SerializeField] TextMeshProUGUI completedGroupLevelsText;
    [SerializeField] GameObject completedGroup;


    private void Awake()
    {
        //Color nodeColor;
        subLevelNodes = containedLevels.GetComponentsInChildren<LevelNode>();
        groupLevelsText.text = " | " + subLevelNodes.Length.ToString();
        levelThumbnail.color = groupColor;
        hoverImage.color = groupColor;
        //nodeColor = difficultyDB.AssignColorDifficulty(nodeDifficulty);
        //difficultyImage.color = nodeColor;
    }

    private void Start()
    {
        for (int i = 0; i < subLevelNodes.Length; i++)
        {
            subLevelNodes[i].gameObject.GetComponent<SceneLoader>().scene = levelOrder.levelOrder[i];
            subLevelNodes[i].levelID = i;
        }
        UnlockInternalLevels();
    }

    public void RefillProgress(int levelsCleared)
    {
        completedLevels++;
        completedGroupLevelsText.text = completedLevels.ToString();
        if (completedLevels == subLevelNodes.Length)
        {
            completedImage.SetActive(true); 
            completedGroup.SetActive(true);
        }

        subLevelNodes[levelsCleared].isCleared = true;
    }

    public void UpdateLevelGroupUI()
    {
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
    }

    public void EnableEverything()
    {
        unlockEverything = true;
    }

    void UnlockInternalLevels()
    {
        if (!unlockEverything)
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
        else
        {
            foreach (var level in subLevelNodes)
            {
                Unlock(level);
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
