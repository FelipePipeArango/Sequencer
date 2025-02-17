using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class TileScript : MonoBehaviour
{
    private Renderer render;
    private Color setColor;
    public GameActions.TileTypes tileType { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<Renderer>();
        setColor = render.material.color;
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
