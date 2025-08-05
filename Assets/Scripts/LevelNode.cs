using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LevelNode : MonoBehaviour
{
    [HideInInspector] public string levelName;

    [HideInInspector] public bool isUnlocked =true;
    [HideInInspector] public bool isCleared = false;

    public int levelID;

    [Header("Visuals - don't modify")]
    public TextMeshProUGUI levelTitleText;
    public GameObject completeText; 
    public Sprite levelThumbnail;
    public Button levelButton;

    void Start()
    {
        InitializeUI();
    }
    
    void InitializeUI()
    {
        levelTitleText.text = levelName;
        levelButton.interactable = isUnlocked;

        if (isCleared) completeText.SetActive(true);
    }

    public void OnLevelSelect()
    {
        LevelSelectManager.levelSelectManagerInstance.RecieveCurrentLevel(levelID);

        if (!isUnlocked) return;

        if (!isCleared) LevelSelectManager.levelSelectManagerInstance.trackLevelCompletion = true;

        //scene loading is called by the button
    }
}
