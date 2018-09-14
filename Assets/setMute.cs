using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum valueType { aqi, temperature, windspeed, humidity, rainfall };
public class setMute : MonoBehaviour
{
    public uiController uiCtrl;
    private Toggle toggle;
    public valueType valType;

    private void OnEnable()
    {
        toggle = GetComponent<Toggle>();
        updateToggle();
    }

    public void updateToggle()
    {
        bool toggleState = false;
        switch (valType)
        {
            case valueType.aqi:
                toggleState = uiCtrl.isActiveAqi;
                break;
            case valueType.temperature:
                toggleState = uiCtrl.isActiveTemp;
                break;
            case valueType.windspeed:
                toggleState = uiCtrl.isActiveWind;
                break;
            case valueType.humidity:
                toggleState = uiCtrl.isActiveHumidity;
                break;
            case valueType.rainfall:
                toggleState = uiCtrl.isActiveRain;
                break;
        }
        if (toggle.isOn != toggleState)
            toggle.isOn = toggleState;
    }

    public void setMuteToggleLocal()
    {
        uiCtrl.setMuteToggleRemote(valType, toggle.isOn);
    }
}
