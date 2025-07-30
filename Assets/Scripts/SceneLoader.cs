using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
public class SceneLoader : MonoBehaviour
{
    [SerializeField] AssetReference scene; //the scene to be loaded, set through the inspector

    public static event Action<string> OnSceneLoad;

    public void LoadScene()
    {
        scene.LoadSceneAsync().Completed += OnLoadCallback;
    }

    void OnLoadCallback(AsyncOperationHandle<SceneInstance> targetScene)
    {
        if (targetScene.Status == AsyncOperationStatus.Succeeded)
        {
            OnSceneLoad?.Invoke(scene.AssetGUID);
        }
    }
}