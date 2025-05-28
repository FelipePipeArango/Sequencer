using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameStates;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager Instance;
   
    public GameObject mainLevelMenu;
    public int[] levelGroupTracker;
    public LevelGroupNode[] LevelGroups;

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
        LevelGroups = mainLevelMenu.GetComponentsInChildren<LevelGroupNode>();
        //levelGroupTracker = new int[LevelGroups.Length];
    }

    public void FillValues() //back in the LevelSelection scene, each group asks for its progress
    {
        LevelGroups[currentLevelGroup].completedLevels = levelGroupTracker[currentLevelGroup]; //and it updates the corresponding one

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

    void LevelCompleted (gameStates completed) //This wonÅLt identy if the player repeats a level!
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
        }
    }
}
