using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GameTiles;


public class TileScript : MonoBehaviour
{
    private Renderer render;
    private Color setColor;
    public bool isHighLight;
    public TileTypes tileType { get; set; }

    //Debug
    [SerializeField] public TileTypes tile;
    
    void Start()
    {
        render = GetComponent<Renderer>();
        setColor = render.material.color;
        isHighLight = false;
    }
    public void SetColor(Color color)
    {
        render = GetComponent<Renderer>();
        this.render.material.color = color;
    }
    public Color GetColor()
    {
        render = GetComponent<Renderer>();
        return render.material.color;
    }
    public void ResetColor()
    {
        SetColor(setColor);
    }

    
}
