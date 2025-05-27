using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameStates;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager Instance;
   
    public GameObject mainLevelMenu;
    private int[] levelGroupTracker;
    public LevelGroupNode[] LevelGroups;
    public GameObject[] subLevelMenus;

    [HideInInspector] public int currentLevelGroup;

    private void OnEnable()
    {
        GameStateManager.stateEvent += LevelCompleted;
    }

    private void OnDisable()
    {

        GameStateManager.stateEvent -= LevelCompleted;
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        LevelGroups = mainLevelMenu.GetComponentsInChildren<LevelGroupNode>();

        mainLevelMenu.SetActive(true);
        /*foreach (var levelMenu in subLevelMenus)
        {
            levelMenu.SetActive(false);
        }*/
    }

    public void FillValues() //back in the LevelSelection scene, each group asks for its progress
    {
        /*for (int i = 0; i < LevelGroups.Length; i++)
        {
            if (currentLevelGroup == i)
            {
                LevelGroups[i].completedLevels = levelGroupTracker[i]; //and it updates the corresponding one
            } 
        }*/
        LevelGroups[currentLevelGroup].completedLevels = levelGroupTracker[currentLevelGroup];
    }

    void LevelCompleted (gameStates completed)
    {
        if (completed == gameStates.Completed) //each time a level is completed
        {
            for (int i = 0; i < levelGroupTracker.Length; i++) //it searches the tracker
            {
                if (i == currentLevelGroup) //and in its correspondent group
                {
                    levelGroupTracker[i] += 1; //the value of completed levels increases
                } 
            }
        }
    }

    public void ReturnToMainMenu()
    {
        /*if (currentLevelMenu != null)
            currentLevelMenu.SetActive(false);*/
        mainLevelMenu.SetActive(true);
        //currentLevelMenu = mainLevelMenu;
    }

}
