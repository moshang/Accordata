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
        audioLoader.OnStyleSelectDeactivation += deactivateToggle;
        audioLoader.OnStyleSelectActivation += activateToggle;
    }

    public void setStyle()
    {
        if (toggle.isOn)
            seqGen.switchStyle(styleIndex);
    }

    private void deactivateToggle()
    {
        toggle.interactable = false;
    }

    private void activateToggle()
    {
        toggle.interactable = true;
    }
}
