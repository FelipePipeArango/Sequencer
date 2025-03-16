using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIHandler : MonoBehaviour
{
   
    public TMP_Text numberText;


    public void UpdateNumberText(int numberValue)
    {
        numberText.text = numberValue.ToString();
    }

    void Start()
    {
        UpdateNumberText(0);
    }
}

