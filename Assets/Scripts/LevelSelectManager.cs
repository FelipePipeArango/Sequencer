using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static GameStates;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager levelSelectManagerInstance;
   
    GameObject mainLevelMenu;
    public int[] levelGroupTracker;
    public LevelGroupNode[] LevelGroups;

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
        GetReferences("Level_Selector"); //This will change once the main menu loads the LevelSelector
    }

    void GetReferences(string currentScene)
    {
        mainLevelMenu = GameObject.FindGameObjectWithTag("MainLevelMenu");
        if (mainLevelMenu != null)
        {
            LevelGroups = mainLevelMenu.GetComponentsInChildren<LevelGroupNode>();
            FillValues();
            //levelGroupTracker = new int[LevelGroups.Length]; //ESTO ES CORRECTO, comentado para probar 
        }
    }

    public void FillValues() //back in the LevelSelection scene, each group asks for its progress
    {
        foreach (var item in LevelGroups)
        {
            LevelGroups[currentLevelGroup].UpdateProgress(levelGroupTracker[currentLevelGroup]); //and it updates the corresponding one
        } 

        if (LevelGroups[currentLevelGroup].CheckGroupCompleted()) //if the level group is clear
        {
            UnlockNextGroups();
        }
    }

    void UnlockNextGroups()
    {
        foreach (var levelGroup in LevelGroups[currentLevelGroup].nextNodes) // each group that is next to the current one
        {
            levelGroup.isUnlocked = true; //unlock it
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
