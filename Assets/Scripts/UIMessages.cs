using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMessages : MonoBehaviour
{
    [SerializeField] GameObject usbMissingMessage;
    [SerializeField] GameObject clickToConfirmMessage;
    [SerializeField] GameObject noUSBInRangeMessage;

    public void RecieveNoItemMessage(bool isThrow)
    {
        if (isThrow) NoItemToThrowMessage();
        else ShowNoItemToPickUpMessage();
    }

    public void RecieveConfirmClickMessage(bool isComplete)
    {
        if (!isComplete) ShowClickToConfirmMessage();
        else HideClickToConfirmMessage();
    }

    private void NoItemToThrowMessage()
    {
        if (usbMissingMessage != null)
        {
            usbMissingMessage.SetActive(true);
            StartCoroutine(HideMessage(usbMissingMessage, 2f));
        }
    }

    private void ShowClickToConfirmMessage()
    {
        if (clickToConfirmMessage != null)
            clickToConfirmMessage.SetActive(true);
    }

    private void HideClickToConfirmMessage()
    {
        if (clickToConfirmMessage != null)
            clickToConfirmMessage.SetActive(false);
    }

    private void ShowNoItemToPickUpMessage()
    {
        if (noUSBInRangeMessage != null)
        {
            noUSBInRangeMessage.SetActive(true);
            StartCoroutine(HideMessage(noUSBInRangeMessage, 0.1f));
        }
    }

    private IEnumerator HideMessage(GameObject messageObj, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (messageObj != null)
            messageObj.SetActive(false);
    }
}
