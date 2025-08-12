using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneHolder : MonoBehaviour
{
    public static SceneHolder sceneHolderInstance;
    string currenteScene;

    private void OnEnable()
    {
        SceneLoader.OnSceneLoad += HoldCurrentScene;
    }

    private void OnDisable()
    {
        SceneLoader.OnSceneLoad -= HoldCurrentScene;
    }

    void Start()
    {
        if (sceneHolderInstance != null && sceneHolderInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            sceneHolderInstance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void HoldCurrentScene(string scene)
    {
        currenteScene = scene;
    }

    public string GetCurrentScene()
    {
        return currenteScene;
    }
}
