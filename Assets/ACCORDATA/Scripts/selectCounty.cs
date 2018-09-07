using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selectCounty : MonoBehaviour
{
    [Header("-> ACCORDATA <-")]
    public uiController uiControl;
    public int countyIndex;

    public void setCounty()
    {
        uiControl.currentCountyIndex = countyIndex;
        uiControl.updateCountyName();
    }
}
