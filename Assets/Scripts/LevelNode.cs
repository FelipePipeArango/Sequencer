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
    public bool isUnlocked;
    public bool isCleared = false;
    
    [Header("Visuals")]
    public TextMeshProUGUI levelTitleText;
    public Image difficultyImage; 
    public Image levelThumbnail;
    public Button levelButton;

    void Start()
    {
        InitializeUI();
    }
    
    void InitializeUI()
    {
        levelTitleText.text = levelName;
        levelButton.interactable = isUnlocked;
        // Commented for testing. Needs to be uncommented once we have the images
        // difficultyImage.sprite = difficultySprite; 
        // levelThumbnail.sprite = thumbnailSprite; 
        //levelButton.onClick.AddListener(OnLevelSelect);
    }

    public void OnLevelSelect()
    {
        if (!isUnlocked) return;

        if (!isCleared) LevelSelectManager.levelSelectManagerInstance.trackLevelCompletion = true;

        //scene loading is called by the button
    }
}
