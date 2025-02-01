using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public Renderer render;

    private Color setColor;
    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<Renderer>();
        setColor = render.material.color;
        Debug.Log($"This tile is {GetColor()}"); 
        Debug.Log($"This tile is {this.isActiveAndEnabled}");
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
