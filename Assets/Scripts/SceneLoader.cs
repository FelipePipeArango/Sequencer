using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
public class SceneLoader : MonoBehaviour
{
    public AssetReference scene; //the scene to be loaded, set through the inspector

    string sceneID;

    public static event Action<string> OnSceneLoad;

    public void LoadScene()
    {
        sceneID = scene.AssetGUID;
        scene.LoadSceneAsync().Completed += OnLoadCallback;
    }

    public void LoadNextScene()
    {
        sceneID = LevelSelectManager.levelSelectManagerInstance.GetNextLevel();

        if (sceneID == null)
        {
            sceneID = scene.AssetGUID;
        }

        Addressables.LoadSceneAsync(sceneID).Completed += OnLoadCallback;
    }

    void OnLoadCallback(AsyncOperationHandle<SceneInstance> targetScene)
    {
        if (targetScene.Status == AsyncOperationStatus.Succeeded)
        {
            OnSceneLoad?.Invoke(sceneID);
        }
    }
}