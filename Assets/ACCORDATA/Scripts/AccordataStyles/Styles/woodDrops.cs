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

public class woodDrops : Style
{
    // -> ACCORDATA <-
    private void Reset() // override the values in the parent class
    {
        // Info about the style to display in the app
        StyleNameEng = "WoodDrops";
        StyleNameChTw = "木滴";
        ComposerNameEng = "MoShang";
        ComposerNameChTw = "莫尚";
        StyleInfoEng = "";
        StyleInfoChTw = "";
        // Style settings
        bpm = 90;
        scale = Scale.major;
        seqLength = 16; // the length of a single bar of the sequence in 16ths
        newValuesEveryXBars = 2; // switch to the next set of values every x bars (either values from the next site, or the next hour for the same site)
        newSeqEveryXBars = 1;           /*  how often should we generate a new sequence?
                                            this value needs to be smaller than the newValuesEveryXBars value, or we will skip over new data
                                            set to 1 to regenerate the sequence with the current data every bar, even if the data hasn't changed
                                            set to more than 1 to repeat the previously generated bar */
    }

    // Style Specific Variables
    public int originalRootNote = 48;
    private int rootNote;
    private int currentSeqGenScale;
    //private int newScaleOnBar;

    public override void initStyle(int newSeqBar)
    {
        seq.setBPM(bpm);
        seq.loadEnsemble(2);
        seqGen.scale = scale;
        newSeqAtBar = newSeqBar;

        // style specific initializations
        rootNote = originalRootNote;
        currentSeqGenScale = 0;
        //newScaleOnBar += newValuesEveryXBars;
        resetSFX();
    }

    public override void makeSeq(int barNum, int aqiVal, float tempVal, float windVal, float humidityVal, float rainVal)
    {
        /*
        if (barNum == newScaleOnBar)
        {
            currentSeqGenScale = (currentSeqGenScale + 1) % seqGen.numScales;
            seqGen.scale = (Scale)currentSeqGenScale;
            newScaleOnBar += newValuesEveryXBars;
        }
        */
        if (barNum != newSeqAtBar)
            return;

        seq.clear();

        int notesInArp = Mathf.Clamp((int)utils.map(aqiVal % 100, 0, 50, 2, 6), 2, 5);

        // AQI determines the scale
        if (aqiVal <= 50)
            currentSeqGenScale = 0;
        else if (aqiVal <= 100)
            currentSeqGenScale = 1;
        else
            currentSeqGenScale = 2;

        seqGen.scale = (Scale)currentSeqGenScale;

        // AQI ARP
        if (uiCtrl.isActiveAqi)
        {
            int scaleIndex = 0;
            for (int i = 0; i < seqLength; i++)
            {
                int nnToAdd = rootNote + 36 + seqGen.scales[(int)seqGen.scale, seqGen.triad[scaleIndex % notesInArp]];
                int velo = 0;
                if (i % 8 == 0)
                    velo = 127;
                else if (i % 4 == 0)
                    velo = 90;
                else
                    velo = 45 + Random.Range(0, 10) + (int)utils.map(aqiVal, 0, 250, 0, 30);
                seq.addNote(i, nnToAdd, velo);
                scaleIndex++;
            }
        }

        // TEMPERATURE
        if (uiCtrl.isActiveTemp)
        {
            // MELODY
            int scaleIndex = 0;
            int lastNoteIndex = UnityEngine.Random.Range(0, seqGen.scales.GetLength(1));

            while (scaleIndex < seqLength)
            {
                int newNoteIndex = UnityEngine.Random.Range(0, 5);
                seq.addNote(scaleIndex, rootNote + 12 + seqGen.scales[(int)seqGen.scale, newNoteIndex], Random.Range(30, 127));
                lastNoteIndex = newNoteIndex;
                scaleIndex += UnityEngine.Random.Range(1, 3);
            }
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

        // HUMIDITY (top 2 notes)
        if (uiCtrl.isActiveHumidity)
        {
            int repetition = 0;
            int triadIndex = 0;

            if (humidityVal <= 70)
            {
                triadIndex = 0;
                repetition = 4;
            }
            else if (humidityVal <= 80)
            {
                triadIndex = 1;
                repetition = 3;
            }
            else if (humidityVal <= 90)
            {
                triadIndex = 1;
                repetition = 2;
            }
            else
            {
                triadIndex = 2;
                repetition = 2;
            }
            for (int i = 0; i < seqLength; i++)
            {
                if (i % repetition == 0)
                {
                    seq.addNote(i, rootNote + seqGen.scales[(int)seqGen.scale, seqGen.triad[triadIndex]] + 48, Random.Range(90, 127));
                    seq.addNote(i, rootNote + seqGen.scales[(int)seqGen.scale, seqGen.triad[triadIndex + 1]] + 48, Random.Range(90, 127));
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

        // schedule the next bar to create a new sequence on
        newSeqAtBar += newSeqEveryXBars;
    }
}
