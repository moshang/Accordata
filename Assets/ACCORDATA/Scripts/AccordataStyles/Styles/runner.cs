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
using AccordataStyle;

public class runner : Style
{
    // -> ACCORDATA <-
    private void Reset() // override the values in the parent class
    {
        // Info about the style to display in the app
        StyleNameEng = "Runner";
        StyleNameChTw = "跑步";
        ComposerNameEng = "MoShang";
        ComposerNameChTw = "莫尚";
        StyleInfoEng = "";
        StyleInfoChTw = "";
        // Style settings
        bpm = 175;
        scale = Scale.pentatonic;
        seqLength = 16; // the length of a single bar of the sequence in 16ths
        newValuesEveryXBars = 1; // switch to the next set of values every x bars (either values from the next site, or the next hour for the same site)
        newSeqEveryXBars = 1;           /*  how often should we generate a new sequence?
                                            this value needs to be smaller than the newValuesEveryXBars value, or we will skip over new data
                                            set to 1 to regenerate the sequence with the current data every bar, even if the data hasn't changed
                                            set to more than 1 to repeat the previously generated bar */
    }

    // Style Specific Variables
    int rootnote;

    public override void initStyle(int newSeqBar)
    {
        seq.setBPM(bpm);
        seq.loadEnsemble(1);
        seqGen.scale = scale;
        newSeqAtBar = newSeqBar;
        resetSFX();
        rootnote = Random.Range(48, 60);
        // style specific initializations
    }

    public override void makeSeq(int barNum, int aqiVal, float tempVal, float windVal, float humidityVal, float rainVal)
    {
        if (barNum != newSeqAtBar)
            return;

        seq.clear();

        Debug.Log("Bar:" + barNum);
        Debug.Log("AqiVal:" + aqiVal);
        int vol = 0;

        if (tempVal > 0)
            seq.setBPM((int)utils.map(tempVal, 0, 40, 80, 180));

        for (int i = 0; i < seqLength; i++)
        {
            if (uiCtrl.isActiveAqi) // check if the value is active or muted in the ui
            {
                // value range 0 - 250
                //seq.addNote(0, 0, 0);
                if (i % 4 == 0)
                    vol = Random.Range(90, 100);
                else
                    vol = 85 - i;
                if (aqiVal > 50 || aqiVal < 0)
                    seq.addNote(i, rootnote + (i * ((aqiVal / 50) + 1)), vol);
                else
                    seq.addNote(i, rootnote + seqGen.scales[0, i % 7] + (i / 7) * 12, vol);
            }
            if (uiCtrl.isActiveTemp) // check if the value is active or muted in the ui
            {
                // value range 0 - 40
                //seq.addNote(0, 0, 0);
            }
            if (uiCtrl.isActiveWind) // check if the value is active or muted in the ui
            {
                // value range 0 - 10
                //seq.addNote(0, 0, 0);
            }
            if (uiCtrl.isActiveHumidity) // check if the value is active or muted in the ui
            {
                // value range  0 - 100
                //seq.addNote(0, 0, 0);
            }
            if (uiCtrl.isActiveRain) // check if the value is active or muted in the ui
            {
                // value range 0 - 60
                //seq.addNote(0, 0, 0);
            }
            //}
        }

        // WIND
        if (uiCtrl.isActiveWind)
        {
            if (windVal <= 0)
            {
                setVol("windLightVol", -80, 3);
                setVol("windMediumVol", -80, 3);
                setVol("windHeavyVol", -80, 3);
            }
            else if (windVal <= 6.6f)
            {
                setVol("windLightVol", 10, 3);
                setVol("windMediumVol", -80, 3);
                setVol("windHeavyVol", -80, 3);
            }
            else if (windVal <= 13.2f)
            {
                setVol("windLightVol", 10, 3);
                setVol("windMediumVol", -6, 3);
                setVol("windHeavyVol", -80, 3);
            }
            else
            {
                setVol("windLightVol", 10, 3);
                setVol("windMediumVol", -12, 3);
                setVol("windHeavyVol", -20, 3);
            }
        }
        else
        {
            setVol("windLightVol", -80, 3);
            setVol("windMediumVol", -80, 3);
            setVol("windHeavyVol", -80, 3);
        }

        // HUMIDITY
        if (uiCtrl.isActiveHumidity)
        {
            for (int i = 0; i < 16; i++)
            {
                if (Random.Range(0, 100) < humidityVal)
                {
                    if (humidityVal > 90 && i % 2 == 0)
                        seq.addNote(i, Random.Range(27, 36), Random.Range(70 + (int)humidityVal / 4, 100 + (int)humidityVal / 4));
                    else if (humidityVal > 80 && i % 4 == 0)
                        seq.addNote(i, Random.Range(27, 33), Random.Range(70 + (int)humidityVal / 4, 100 + (int)humidityVal / 4));
                    else if (i % 6 == 0)
                        seq.addNote(i, Random.Range(27, 33), Random.Range(70 + (int)humidityVal / 4, 100 + (int)humidityVal / 4));
                }
            }
        }

        // RAIN
        if (uiCtrl.isActiveRain)
        {
            if (rainVal <= 0)
            {
                setVol("rainLightVol", -80, 5);
                setVol("rainMediumVol", -80, 5);
                setVol("rainHeavyVol", -80, 5);
            }
            else if (rainVal <= 20)
            {
                setVol("rainLightVol", -6, 3);
                setVol("rainMediumVol", -80, 3);
                setVol("rainHeavyVol", -80, 3);
            }
            else if (rainVal <= 40)
            {
                setVol("rainLightVol", -10, 3);
                setVol("rainMediumVol", -16, 3);
                setVol("rainHeavyVol", -80, 3);
            }
            else
            {
                setVol("rainLightVol", -10, 3);
                setVol("rainMediumVol", -20, 3);
                setVol("rainHeavyVol", -25, 3);
            }
        }
        else
        {
            setVol("rainLightVol", -80, 5);
            setVol("rainMediumVol", -80, 5);
            setVol("rainHeavyVol", -80, 5);
        }

        newSeqAtBar += newSeqEveryXBars;
    }
}
