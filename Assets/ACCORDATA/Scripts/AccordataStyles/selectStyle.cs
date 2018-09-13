using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class selectStyle : MonoBehaviour
{
    // -> ACCORDATA <-
    public int styleIndex; // needs to be assigned when the selector is instantiated in seqGenerator

    private seqGenerator seqGen;
    private Toggle toggle;

    private void OnEnable()
    {
        toggle = GetComponent<Toggle>();
        seqGen = GameObject.Find("AccordataController").GetComponent<seqGenerator>();
    }

    public void setStyle()
    {
        if (toggle.isOn)
            seqGen.switchStyle(styleIndex);
    }
}
