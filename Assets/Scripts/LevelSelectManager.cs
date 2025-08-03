using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameStates;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager levelSelectManagerInstance;

    [SerializeField] bool unlockedMode;

    [SerializeField] LevelOrganizer[] levelCollector;

    int currentLevelID;
    int dontReset;
    GameObject mainLevelMenu;
    int[] levelGroupTracker;
    LevelGroupNode[] levelGroups;
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

    public void RecieveCurrentLevel(int currentLevel)
    {
        currentLevelID = currentLevel;
    }

    public int GetCurrentLevel()
    {
        return currentLevelID;
    }

    public string GetNextLevel()
    {
        if (currentLevelID < levelGroups[currentLevelGroup].subLevelNodes.Length)
        {
            currentLevelID++;
            string nextLevel = levelCollector[currentLevelGroup].levelOrder[currentLevelID].AssetGUID;
            return nextLevel;
        }
        else
        {
            return null;
        }
    }

    void GetReferences(string currentScene)
    {
        mainLevelMenu = GameObject.FindGameObjectWithTag("MainLevelMenu");
        if (mainLevelMenu != null)
        {
            levelGroups = mainLevelMenu.GetComponentsInChildren<LevelGroupNode>();

            if (dontReset == 0) levelGroupTracker = new int[levelGroups.Length]; dontReset++;

            for (int i = 0; i < levelGroups.Length; i++)
            {
                levelGroups[i].levelGroupNumber = i;
                if (levelGroups[i].isUnlocked) unlockedGroups.Add(levelGroups[i].levelGroupNumber);

            }

            if (unlockedGroups.Count <= 0)
            {
                unlockedGroups.Add(levelGroups[0].levelGroupNumber);
            }

            if(unlockedMode)
            {
                foreach (var group in unlockedGroups)
                {
                    levelGroups[group].EnableEverything(); 
                }
            }

            FillValues();
        }
    }

    public void FillValues() //back in the LevelSelection scene,
    {
        foreach (var group in unlockedGroups) //every unlocked group
        {
            levelGroups[group].RefillProgress(levelGroupTracker[group], 0); //has it's progress refilled 
        }

        CheckCompletedLevelGroups();
    }

    void CheckCompletedLevelGroups()
    {
        if (levelGroups[currentLevelGroup].IsGroupCompleted()) //if the level group is clear
        {
            UnlockNextGroups(); //Also unlock the ones connected
        }

        foreach (var group in unlockedGroups) //once it returns to the levelScene
        {
            levelGroups[group].isUnlocked = true; //it re-unlocks previously saved levels
            levelGroups[group].UpdateLevelGroupUI(); //and updates their visuals
        }
    }

    void UnlockNextGroups()
    {
        if (levelGroups[currentLevelGroup].nextNodes.Count > 0)
        {
            foreach (var levelGroup in levelGroups[currentLevelGroup].nextNodes) // checks each group that is next to the current one
            {
                if (!unlockedGroups.Contains(levelGroup.levelGroupNumber))
                {
                    unlockedGroups.Add(levelGroup.levelGroupNumber); //And adds them to the list of unlocked groups 
                }
                levelGroup.subLevelNodes[0].isUnlocked = true; //makes sure the first level of each group is unlocked
            } 
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
