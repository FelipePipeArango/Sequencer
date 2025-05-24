using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager Instance { get; private set; }

    
    public GameObject mainLevelMenu;
    public GameObject currentLevelMenu;
    public GameObject[] subLevelMenus;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        mainLevelMenu.SetActive(true);
        foreach (var levelMenu in subLevelMenus)
        {
            levelMenu.SetActive(false);
        }
    }
    
    public void OpenSubGroup(GameObject subLevelMenu)
    {
        mainLevelMenu.SetActive(false);
        subLevelMenu.SetActive(true);
        currentLevelMenu = subLevelMenu;
    }

    public void ReturnToMainMenu()
    {
        if (currentLevelMenu != null)
            currentLevelMenu.SetActive(false);
        mainLevelMenu.SetActive(true);
        currentLevelMenu = mainLevelMenu;
    }

}
