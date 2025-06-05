using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteScreen : MonoBehaviour
{
    [SerializeField]
    public GameObject CardsInLevel;
    public GameObject NumberSlot;
    public GameObject HUDIcons;

    bool isPaused = false;
    float seconds;

    public void OpenCompleteScreen()
    {
        CardsInLevel.GameObject().SetActive(false);
        NumberSlot.GameObject().SetActive(false);
        HUDIcons.GameObject().SetActive(false);
        this.GameObject().SetActive(true);
        
        StartCoroutine(ifPaused());
    }
    private IEnumerator ifPaused()
    {
        if(!isPaused)
        {
            NextScene();
        }
        else
        {
            CardsInLevel.GameObject().SetActive(false);
            NumberSlot.GameObject().SetActive(false);
            HUDIcons.GameObject().SetActive(false);
            this.GameObject().SetActive(true);

            yield return null;
        }
    }

   

    void NextScene()
    {
        /*int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }*/
    }

    public void PauseButton()
    {
        isPaused = true;
        Debug.Log("PAUSE!");
    }
}
