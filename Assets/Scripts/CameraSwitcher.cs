using Cinemachine;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera mainMenuCamera;
    public CinemachineVirtualCamera levelSelectCamera;
    public CinemachineVirtualCamera exitCamera;
    public CinemachineVirtualCamera tableCamera;

    public GameObject mainMenuCanvas;
    public GameObject levelSelectCanvas;
    public GameObject exitCanvas;
    public GameObject tableCanvas;

    public void OnPlayClicked()
    {
        SetCamera(mainMenuCamera: false, levelSelectCamera: true, exitCamera: false, tableCamera: false);
    }

    public void OnExitClicked()
    {
        SetCamera(mainMenuCamera: false, levelSelectCamera: false, exitCamera: true, tableCamera: false);
    }

    public void OnBackClicked()
    {
        SetCamera(mainMenuCamera: true, levelSelectCamera: false, exitCamera: false, tableCamera: false);
    }

    public void OnLevelSelectClicked()
    {
        SetCamera(mainMenuCamera: false, levelSelectCamera: false, exitCamera: false, tableCamera: true);
    }

    private void SetCamera(bool mainMenuCamera, bool levelSelectCamera, bool exitCamera, bool tableCamera)
    {
        this.mainMenuCamera.Priority = mainMenuCamera ? 10 : 0;
        this.levelSelectCamera.Priority = levelSelectCamera ? 10 : 0;
        this.exitCamera.Priority = exitCamera ? 10 : 0;
        this.tableCamera.Priority = tableCamera ? 10 : 0;

        this.mainMenuCanvas.SetActive(mainMenuCamera);
        this.levelSelectCanvas.SetActive(levelSelectCamera);
        this.exitCanvas.SetActive(exitCamera);
        this.tableCanvas.SetActive(tableCamera);
    }
}


