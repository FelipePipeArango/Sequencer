using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LevelNode : MonoBehaviour
{
    [HideInInspector] public string levelName;

    [HideInInspector] public bool isUnlocked;
    [HideInInspector] public bool isCleared = false;

    [Header("Visuals - don't modify")]
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
    }

    public void OnLevelSelect()
    {
        if (!isUnlocked) return;

        if (!isCleared) LevelSelectManager.levelSelectManagerInstance.trackLevelCompletion = true;

        //scene loading is called by the button
    }
}
