using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteScreen : MonoBehaviour
{
    [SerializeField]
    public GameObject CardsInLevel;
    public GameObject NumberSlot;

    bool isPaused = false;
    float seconds;

    public IEnumerator OpenCompleteScreen()
    {
        CardsInLevel.GameObject().SetActive(false);
        NumberSlot.GameObject().SetActive(false);
        this.GameObject().SetActive(true);

        yield return new WaitForSeconds(5.0f);
        
        StartCoroutine(ifPaused());
    }
    private IEnumerator ifPaused()
    {
        if(!isPaused)
        {
            OpenMenuScreen();
        }
        else
        {
            CardsInLevel.GameObject().SetActive(false);
            NumberSlot.GameObject().SetActive(false);
            this.GameObject().SetActive(true);

            yield return null;
        }
    }
    void OpenMenuScreen()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PauseButton()
    {
        isPaused = true;
        Debug.Log("PAUSE!");
    }
}
