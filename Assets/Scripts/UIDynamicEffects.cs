using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDynamicEffects : MonoBehaviour
{
    private Vector3 originalPos;
    private Vector3 originalScale;
    private Quaternion originalRot;
    private Image image;

    [Header("Float")]
    public bool enableFloat = true;
    public float floatAmplitude = 10f;
    public float floatSpeed = 2f;

    [Header("Pulse")]
    public bool enablePulse = true;
    public float pulseAmplitude = 0.05f;
    public float pulseSpeed = 2f;

    [Header("Idle Rotation")]
    public bool enableIdleRotation = true;
    public float rotationSpeed = 15f;

    [Header("Wiggle")]
    public bool enableWiggle = true;
    public float wiggleAmount = 10f;
    public float wiggleSpeed = 3f;

    [Header("Color Shift")]
    public bool enableColorShift = true;
    public Color colorA = Color.white;
    public Color colorB = Color.cyan;
    public float colorLerpSpeed = 2f;

    [Header("Parallax")]
    public bool enableParallax = true;
    public float parallaxStrength = 10f;

    void Start()
    {
        originalPos = transform.localPosition;
        originalScale = transform.localScale;
        originalRot = transform.localRotation;

        image = GetComponent<Image>();
    }

    void Update()
    {
        if (enableFloat)
        {
            float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.localPosition = originalPos + new Vector3(0, yOffset, 0);
        }

        if (enablePulse)
        {
            float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
            transform.localScale = originalScale * scale;
        }

        if (enableIdleRotation)
        {
            transform.localRotation = originalRot * Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 0.25f) * rotationSpeed);
        }

        if (enableWiggle)
        {
            float wiggle = Mathf.Sin(Time.time * wiggleSpeed) * wiggleAmount;
            transform.localRotation = originalRot * Quaternion.Euler(0, wiggle, 0);
        }

        if (enableColorShift && image != null)
        {
            image.color = Color.Lerp(colorA, colorB, Mathf.PingPong(Time.time * colorLerpSpeed, 1));
        }

        if (enableParallax)
        {
            Vector3 mousePos = Input.mousePosition;
            float x = (mousePos.x / Screen.width - 0.5f) * parallaxStrength;
            float y = (mousePos.y / Screen.height - 0.5f) * parallaxStrength;
            transform.localPosition = originalPos + new Vector3(x, y, 0);
        }
    }
}

