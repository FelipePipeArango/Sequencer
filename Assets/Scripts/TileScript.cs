using System;
using UnityEngine;
using static GameTiles;

public class TileScript : MonoBehaviour
{
    public static event Action<TileScript> OnTileArrowClicked;

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
        render = GetComponent<Renderer>();
        render.material.color = color;
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

    public void TileSelectedButton()
    {
        OnTileArrowClicked?.Invoke(this);
    }
}
