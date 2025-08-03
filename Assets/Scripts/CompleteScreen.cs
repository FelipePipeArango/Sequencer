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

    public void OpenCompleteScreen()
    {
        CardsInLevel.SetActive(false);
        NumberSlot.SetActive(false);
        HUDIcons.SetActive(false);
        this.GameObject().SetActive(true);

        ifPaused();
    }
    private void ifPaused()
    {
        CardsInLevel.SetActive(false);
        NumberSlot.SetActive(false);
        HUDIcons.SetActive(false);
        this.GameObject().SetActive(true);
    }
}
