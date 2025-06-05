using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using static GameStates;

public class GameButtons : MonoBehaviour
{
    [HideInInspector] public string activeScene;

    enum availableBuilds
    {
        WebCredits,
        PcCredits
    }
    [SerializeField] availableBuilds targetBuild;

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Addressables.LoadSceneAsync(SceneHolder.sceneHolderInstance.GetCurrentScene());
        }
    }
    public void PlayGameButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitButton()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
    public void CreditButton()
    {
        if (targetBuild == availableBuilds.PcCredits)
        {
            SceneManager.LoadScene(availableBuilds.PcCredits.ToString()); 
        }
        else if (targetBuild == availableBuilds.WebCredits)
        {
            SceneManager.LoadScene(availableBuilds.WebCredits.ToString());
        }
    }
    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
   
    public void ContinueButton()
    {
        GameStateManager.gameStateManagerInstance.CommunicateStateChange(gameStates.Completed);
    }
    public void ResetButton()
    {
        Addressables.LoadSceneAsync(SceneHolder.sceneHolderInstance.GetCurrentScene());
    }

    public void SettingsButton(GameObject settings)
    {
        settings.GameObject().SetActive(true);
    }

    public void ResumeButton(GameObject settings)
    {
        settings.GameObject().SetActive(false);
    }
}
