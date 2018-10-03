using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class styleToggle : MonoBehaviour
{
    public Toggle toggle;
    public Toggle playToggle;
    public clock clk;

    public void doStyleToggle()
    {
        if (toggle.isOn && clock.isRunning)
        {
            clk.stopPlayback();
            playToggle.isOn = false;
        }
    }
}
