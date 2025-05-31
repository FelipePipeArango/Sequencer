using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameStates;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager levelSelectManagerInstance;
   
    GameObject mainLevelMenu;
    int[] levelGroupTracker;
    public LevelGroupNode[] LevelGroups;
    public List<int> unlockedGroups = new List<int>();

    [HideInInspector] public bool trackLevelCompletion;

    [HideInInspector] public int currentLevelGroup;

    private void OnEnable()
    {
        GameStateManager.stateEvent += LevelCompleted;
        SceneLoader.OnSceneLoad += GetReferences;
    }

    private void OnDisable()
    {
        GameStateManager.stateEvent -= LevelCompleted;
        SceneLoader.OnSceneLoad -= GetReferences;
    }

    void Awake()
    {
        if (levelSelectManagerInstance != null && levelSelectManagerInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            levelSelectManagerInstance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        GetReferences("Level_Selector"); //This will change once the main menu loads the LevelSelector
    }

    void GetReferences(string currentScene)
    {
        mainLevelMenu = GameObject.FindGameObjectWithTag("MainLevelMenu");
        if (mainLevelMenu != null)
        {
            LevelGroups = mainLevelMenu.GetComponentsInChildren<LevelGroupNode>();
            levelGroupTracker = new int[LevelGroups.Length];
            if (unlockedGroups.Count <= 0)
            {
                unlockedGroups.Add(LevelGroups[0].levelGroupNumber - 1);
            }

            FillValues();
        }
    }

    public void FillValues() //back in the LevelSelection scene,
    {
        foreach (var group in unlockedGroups) //every unlocked group
        {
            LevelGroups[group].RefillProgress(levelGroupTracker[group]); //has it's progress refilled
        }

        CheckCompletedLevelGroups();
    }

    void CheckCompletedLevelGroups()
    {
        if (LevelGroups[currentLevelGroup].IsGroupCompleted()) //if the level group is clear
        {
            UnlockNextGroups(); //Also unlock the ones connected
        }

        foreach (var group in unlockedGroups)
        {
            LevelGroups[group].UpdateLevelGroupUI();
        }
    }

    void UnlockNextGroups()
    {
        foreach (var levelGroup in LevelGroups[currentLevelGroup].nextNodes) // each group that is next to the current one
        {
            levelGroup.isUnlocked = true; //unlock it
            unlockedGroups.Add(levelGroup.levelGroupNumber - 1); //And add it to the list of unlocked groups
            levelGroup.subLevelNodes[0].isUnlocked = true;
        }
    }

    void LevelCompleted (gameStates completed) 
    {
        if (trackLevelCompletion == true) //checks is the level hasn't already been completed
        {
            if (completed == gameStates.Completed) //each time a level is completed
            {
                for (int i = 0; i < levelGroupTracker.Length; i++) //it searches for all the levelGroups
                {
                    if (i == currentLevelGroup) //and in its correspondent group
                    {
                        levelGroupTracker[i] += 1; //the value of completed levels increases
                    }
                }
                trackLevelCompletion = false;
            } 
        }
    }
}
