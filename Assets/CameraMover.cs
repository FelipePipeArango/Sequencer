using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public Transform cameraTarget;
    public Vector3 playPositionOffset = new Vector3(10f, 0f, 0f);
    public Vector3 exitPositionOffset = new Vector3(-10f, 0f, 0f);

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = cameraTarget.position;
    }

    public void MoveToPlay()
    {
        cameraTarget.position = originalPosition + playPositionOffset;
    }

    public void MoveToExit()
    {
        cameraTarget.position = originalPosition + exitPositionOffset;
    }
}

