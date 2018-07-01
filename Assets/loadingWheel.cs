using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadingWheel : MonoBehaviour
{
    public Image[] spokes;
    private int startSpoke = 12;
    private Color32 startColor = new Color32(255, 255, 255, 255);

    private void OnEnable()
    {
        InvokeRepeating("updateColors", 0, 0.083f);
    }

    private void updateColors()
    {
        startSpoke = (startSpoke - 1 >= 0) ? startSpoke - 1 : 11;
        for (int i = 0; i < 12; i++)
        {
            Color32 newColor = startColor;
            newColor.a = (byte)(255 - (i * 15));
            spokes[(startSpoke + i) % 12].color = newColor;
        }
    }
}
