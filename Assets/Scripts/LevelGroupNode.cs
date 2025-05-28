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
    [HideInInspector] public int completedLevels;
    // public int difficulty; // use to select an image or color later
    // public Sprite thumbnailSprite;
    // public Sprite difficultySprite;

    [Header("State")]
    public bool isUnlocked = false;
    public bool groupClear = false;

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
        InitializeUI();
    }

    void Start()
    {
        LevelSelectManager.Instance.FillValues();
        levelGroupName = this.name;
        UnlockInternalLevels();
        CheckGroupCompleted();
    }
    
    void InitializeUI()
    {
        int i = 0;
        subLevelNodes = containedLevels.GetComponentsInChildren<LevelNode>();
        foreach (var level in subLevelNodes)
        {
            level.parentGroup = this;
            level.levelName = this.name + ": file_" + i;
            i++;
        }
        levelTitleText.text = levelGroupName;
        levelButton.interactable = isUnlocked;
        lockOverlay.SetActive(!isUnlocked);
        // Commented for testing. Needs to be uncommented once we have the images
        // difficultyImage.sprite = difficultySprite; 
        // levelThumbnail.sprite = thumbnailSprite; 
        // levelButton.onClick.AddListener(OnLevelSelect);
    }

    public void UnlockInternalLevels()
    {
        if (completedLevels == 0)
        {
            Unlock(subLevelNodes[0]);
        }
        else
        {
            for (int i = 1; i <= completedLevels; i++)
            {
                Unlock(subLevelNodes[i - 1]);
            } 
        }
    }
    public void Unlock(LevelNode level)
    {
        level.isUnlocked = true;
        level.levelButton.interactable = true;
        level.lockOverlay.SetActive(false);
    }

    public bool CheckGroupCompleted()
    {
        foreach (var level in subLevelNodes)
        {
            if (!level.isCleared)
                return false;
        }
        groupClear = true;
        return true;
    }

    public void SetCurrentLevelGroup() //Since the player can click and change the level group, this is intended to set it
    {
        LevelSelectManager.Instance.currentLevelGroup = levelGroupNumber - 1; //This changes the currentGroup being tracked
    }

    public void OpenSubGroup() //Called by a button
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
    public void GoBack() //Called by a button
    {
        containedLevels.transform.SetParent(this.gameObject.transform);
        containedLevels.SetActive(false);
    }
}
