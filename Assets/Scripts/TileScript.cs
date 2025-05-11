using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GameTiles;

public class TileScript : MonoBehaviour
{
    [HideInInspector] public TileTypes tileType { get; set; }
    public GameObject pointer;

    private Renderer render;
    private Color setColor;

    [HideInInspector] public bool isHighLight;
    
    void Start()
    {
        render = GetComponent<Renderer>();
        setColor = render.material.color;
        isHighLight = false;
    }
    public void SetColor(Color color)
    {
        render.material.color = color;
    }
    public Color GetColor()
    {
        return render.material.color;
    }
    public void ResetColor()
    {
        SetColor(setColor);
    }
}
