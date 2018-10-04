/* Copyright (c) Jean Marais / MoShang 2018. Licensed under GPLv3.
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

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
