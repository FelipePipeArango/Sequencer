using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    Renderer render;

    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<Renderer>();
        Debug.Log(this.isActiveAndEnabled); 
    }

    public void SetColor(Color color)
    {

        render.material.color = color;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
