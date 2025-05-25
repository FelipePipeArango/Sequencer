using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverScaleAnd3DTilt : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Scale")]
    public Vector3 hoverScale = new Vector3(1.4f, 1.4f, 1.4f);
    public float scaleLerpSpeed = 5f;

    [Header("3-D Tilt")]
    public float xAmplitude = 8f;
    public float yAmplitude = 8f;
    public float zAmplitude = 8f;
    public float tiltSpeed = 2f;

    Vector3 baseScale;
    Quaternion baseRotation;
    bool hovering;
    bool dragging;

    void Awake()
    {
        baseScale = transform.localScale;
        baseRotation = transform.localRotation;
    }

    void Update()
    {
       
        Vector3 targetScale = (hovering && !dragging) ? hoverScale : baseScale;
        transform.localScale =
            Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleLerpSpeed);

        
        if (hovering && !dragging)
        {
            
            float t = Time.time * tiltSpeed * Mathf.PI * 2f; 
            float x = Mathf.Sin(t) * xAmplitude;
            float y = Mathf.Sin(t + Mathf.PI * 0.5f) * yAmplitude;
            float z = Mathf.Cos(t) * zAmplitude;

            transform.localRotation = baseRotation * Quaternion.Euler(x, y, z);
        }
        else
        {
            
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation, baseRotation, Time.deltaTime * scaleLerpSpeed);
        }
    }

    

    public void OnPointerEnter(PointerEventData _) { if (!dragging) hovering = true; }
    public void OnPointerExit(PointerEventData _) { hovering = false; }

    public void OnPointerDown(PointerEventData _)
    {
        hovering = false;
        SnapBack();
    }

    public void OnBeginDrag(PointerEventData _)
    {
        dragging = true;
        SnapBack();
    }

    public void OnEndDrag(PointerEventData _)
    {
        dragging = false;
    }

    void SnapBack()
    {
        transform.localScale = baseScale;
        transform.localRotation = baseRotation;
    }
}