using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class switchTo72HR : MonoBehaviour
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
            uiControl.setMode(Mode.site72Hr);
        else
            uiControl.setMode(uiControl.lastMapMode);
    }
}
