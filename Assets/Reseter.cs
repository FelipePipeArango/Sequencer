using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Reseter : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SceneManager.LoadScene(0);
        }
    }
}
