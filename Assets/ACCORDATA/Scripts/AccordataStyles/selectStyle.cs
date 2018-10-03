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

        if (toggle == null)
            toggle = GetComponent<Toggle>();
        if (seqGen == null)
            seqGen = GameObject.Find("AccordataController").GetComponent<seqGenerator>();
    }

    public void setStyle()
    {
        seqGen.switchStyle(styleIndex);
    }
}
