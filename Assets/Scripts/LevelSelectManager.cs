using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameStates;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager levelSelectManagerInstance;

    int dontReset;
    GameObject mainLevelMenu;
    int[] levelGroupTracker;
    LevelGroupNode[] LevelGroups;
    HashSet<int> unlockedGroups = new HashSet<int>();

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

            if (dontReset == 0) levelGroupTracker = new int[LevelGroups.Length]; dontReset++;

            for (int i = 0; i < LevelGroups.Length; i++)
            {
                LevelGroups[i].levelGroupNumber = i;
            }

            if (unlockedGroups.Count <= 0)
            {
                unlockedGroups.Add(LevelGroups[0].levelGroupNumber);
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

        foreach (var group in unlockedGroups) //once it returns to the levelScene
        {
            LevelGroups[group].isUnlocked = true; //it re-unlocks previously saved levels
            LevelGroups[group].UpdateLevelGroupUI(); //and updates their visuals
        }
    }

    void UnlockNextGroups()
    {
        foreach (var levelGroup in LevelGroups[currentLevelGroup].nextNodes) // each group that is next to the current one
        {
            if (!unlockedGroups.Contains(levelGroup.levelGroupNumber))
            {
                unlockedGroups.Add(levelGroup.levelGroupNumber); //And add it to the list of unlocked groups 
            }
            levelGroup.subLevelNodes[0].isUnlocked = true; //makes sure the first level of each group is unlocked
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
                        levelGroupTracker[i]++; //the value of completed levels increases
                    }
                }
                trackLevelCompletion = false;
            } 
        }
    }
}
