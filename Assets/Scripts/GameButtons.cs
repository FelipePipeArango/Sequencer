using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameButtons : MonoBehaviour
{
    enum availableBuilds
    {
        WebCredits,
        PcCredits
    }

    [SerializeField] availableBuilds targetBuild;

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
    public void BackButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
   
    public void ContinueButton()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
    public void ResetButton()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene); 
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
