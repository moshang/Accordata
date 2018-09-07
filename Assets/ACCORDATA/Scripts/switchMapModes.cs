using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class switchMapModes : MonoBehaviour
{
    Toggle toggle;
    public uiController uiControl;
    // Use this for initialization
    void Start()
    {
        toggle = GetComponent<Toggle>();
    }

    public void changeMode()
    {
        if (toggle.isOn)
            uiControl.setMode(Mode.mapCounty);
        else
            uiControl.setMode(Mode.mapAll);
    }
}
