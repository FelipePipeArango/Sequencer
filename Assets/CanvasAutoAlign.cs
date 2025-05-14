using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CanvasAutoAlign : MonoBehaviour
{
    [System.Serializable]
    public class CanvasPair
    {
        public Canvas canvas;                          // The World Space canvas to align
        public CinemachineVirtualCamera virtualCam;    // The virtual camera to align it with
        public Vector3 offsetPosition;                 // Optional positional offset
        public Vector3 offsetEulerAngles;              // Optional rotational offset
    }

    public CanvasPair[] canvasPairs;

    void Start()
    {
        foreach (var pair in canvasPairs)
        {
            if (pair.canvas != null && pair.virtualCam != null)
            {
                Transform camTransform = pair.virtualCam.transform;
                pair.canvas.transform.position = camTransform.position + pair.offsetPosition;
                pair.canvas.transform.rotation = camTransform.rotation * Quaternion.Euler(pair.offsetEulerAngles);
            }
        }
    }
}

