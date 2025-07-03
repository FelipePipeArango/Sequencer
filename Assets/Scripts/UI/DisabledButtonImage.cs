using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonWithDisabledImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image disabledImage; // Image to show when the button is disabled

    [SerializeField] Button targetButton;
    [SerializeField] private Image highlightedImage;

    IEnumerator Start()
    {
        yield return WaitForFrames (4); //While not ideal, this ensures the nodes are updated fist
        UpdateDisabledImage();
    }

    IEnumerator WaitForFrames(int frameCount)
    {
        while (frameCount > 0)
        {
            frameCount--;
            yield return null; // waits for the next frame
        }
    }

    private void UpdateDisabledImage()
    {
        if (targetButton != null)
        {
            disabledImage.gameObject.SetActive(!targetButton.interactable);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetButton != null) highlightedImage.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetButton != null) highlightedImage.gameObject.SetActive(false);
    }
}

