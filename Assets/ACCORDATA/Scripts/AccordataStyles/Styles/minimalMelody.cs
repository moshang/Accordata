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

public class minimalMelody : Style
{
    // -> ACCORDATA <-
    private void Reset()
    {
        // Info about the style to display in the app
        StyleNameEng = "Minimal Melody";
        StyleNameChTw = "最小的旋律";
        ComposerNameEng = "MoShang";
        ComposerNameChTw = "莫尚";
        StyleInfoEng = "";
        StyleInfoChTw = "";
        // Style settings
        bpm = 85;
        scale = Scale.pentatonic;
        seqLength = 16; // the length of a single bar of the sequence in 16ths
        newValuesEveryXBars = 2; // switch to the next set of values every x bars (either values from the next site, or the next hour for the same site)
        newSeqEveryXBars = 2;           /*  how often should we generate a new sequence?
                                            this value needs to be smaller than the newValuesEveryXBars value, or we will skip over new data
                                            setting to 1 to regenerates the sequence with the current data every bar, even if the data hasn't changed
                                            setting to more than 1 repeats the previously generated bar */
        // Accoplayer  Global FX (ie. affects everything on the accoplayer audiosource, but not on other Unity mixer channels)
        useReverb = true;
        revrbPreset = AudioReverbPreset.Arena;
        useDelay = false;
        delayTimeMS = 500;
        delayDecay = 0.5f;
        delayMix = 0.8f;
    }

    // Style Specific Variables
    private int noteDensity = 25; // likelyhood of this 16th note containing a note
    private int[] rmNoteNum;
    private bool melodyExists = false;
    public AudioClip windLight;

    public override void initStyle(int newSeqBar)
    {
        seq.setBPM(bpm);
        seq.loadEnsemble(1);
        seqGen.scale = scale;
        newSeqAtBar = newSeqBar;
        resetSFX();
        seqGen.windLightAS.clip = windLight;
        seqGen.windLightAS.Play();
    }

    public override void makeSeq(int barNum, int aqiVal, float tempVal, float windVal, float humidityVal, float rainVal)
    {
        if (barNum != newSeqAtBar)
            return;
        seq.clear();
        Debug.Log("Bar:" + barNum);
        Debug.Log("AqiVal:" + aqiVal);
        melodyExists = false;

        // AQI
        if (uiCtrl.isActiveAqi)
        {
            // seq_0
            if (!melodyExists)
            {
                rmNoteNum = new int[seqLength];
                for (int i = 0; i < seqLength; i++)
                {
                    int noteNum = 0;
                    if (i == 0 || Random.Range(0, 100) <= 20)  // use a fixed low value for the bass line // noteDensity)
                    {
                        noteNum = seqGen.originalRootNote + seqGen.scales[(int)seqGen.scale, Random.Range(0, 7)];
                        if (aqiVal <= 100)
                            seq.addNote(i, noteNum, 0); // add it but silent
                        else if (aqiVal <= 150)
                            seq.addNote(i, noteNum, Random.Range(60, 127));

                        if (aqiVal > 200)
                            seq.addNote(i, noteNum - 24, Random.Range(60, 127)); // add bass 2 octave below
                        else if (aqiVal > 150)
                            seq.addNote(i, noteNum - 12, Random.Range(60, 127)); // add bass 1 octave below
                    }
                    rmNoteNum[i] = noteNum;
                }
                melodyExists = true;
            }

            else
            {
                for (int i = 0; i < seqLength; i++)
                {
                    if (rmNoteNum[i] != 0)
                    {
                        if (aqiVal <= 100)
                            seq.addNote(i, rmNoteNum[i], Random.Range(60, 127)); // add it but silent
                        else if (aqiVal <= 150)
                            seq.addNote(i, rmNoteNum[i], Random.Range(60, 127));

                        if (aqiVal > 200)
                            seq.addNote(i, rmNoteNum[i] - 24, Random.Range(60, 127)); // add bass 2 octave below
                        else if (aqiVal > 150)
                            seq.addNote(i, rmNoteNum[i] - 12, Random.Range(60, 127)); // add bass 1 octave below
                    }
                }
            }

            noteDensity = (int)utils.map(aqiVal % 50, 0, 50, 40, 90);

            // seq_1
            int melodyIndex = 0;
            for (int i = 0; i < seqLength; i++)
            {
                // place a note on this step?
                if (Random.Range(0, 100) <= noteDensity)
                {
                    while (rmNoteNum[melodyIndex] == 0)
                        melodyIndex = (melodyIndex + 1) % seqLength;
                    seq.addNote(i, rmNoteNum[melodyIndex] + 12, Random.Range(60, 127));
                    melodyIndex = (melodyIndex + 1) % seqLength; // advance melodyIndex, otherwise it will keep returning true and not go to the next note
                }
            }

            if (aqiVal > 50)
            {
                // seq_2
                melodyIndex = 0;
                for (int i = 0; i < seqLength; i++)
                {
                    // place a note on this step?
                    if (Random.Range(0, 100) <= noteDensity)
                    {
                        while (rmNoteNum[melodyIndex] == 0)
                            melodyIndex = (melodyIndex + 1) % seqLength;
                        seq.addNote(i, rmNoteNum[melodyIndex] + 17, Random.Range(60, 127));
                        melodyIndex = (melodyIndex + 1) % seqLength; // advance melodyIndex, otherwise it will keep returning true and not go to the next note
                    }
                }
            }

            if (aqiVal > 100)
            {
                // seq_3
                melodyIndex = 0;
                for (int i = 0; i < seqLength; i++)
                {
                    // place a note on this step?
                    if (Random.Range(0, 100) <= noteDensity)
                    {
                        while (rmNoteNum[melodyIndex] == 0)
                            melodyIndex = (melodyIndex + 1) % seqLength;
                        seq.addNote(i, rmNoteNum[melodyIndex] + 24, Random.Range(60, 127));
                        melodyIndex = (melodyIndex + 1) % seqLength; // advance melodyIndex, otherwise it will keep returning true and not go to the next note
                    }
                }
            }
        }

        // TEMPERATURE
        if (uiCtrl.isActiveTemp)
        {
            if (tempVal > 20)
            {
                float newVal = (tempVal - 20) / 20;
                newVal = Mathf.Lerp(-40, -10, newVal);
                setVol("tempLightVol", newVal, 1);
            }
            else
                setVol("tempLightVol", -80, 3);
        }
        else
        {
            setVol("tempLightVol", -80, 3);
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
                setVol("windLightVol", -12, 3);
                setVol("windMediumVol", -80, 3);
                setVol("windHeavyVol", -80, 3);
            }
            else if (windVal <= 13.2f)
            {
                setVol("windLightVol", -12, 3);
                setVol("windMediumVol", -6, 3);
                setVol("windHeavyVol", -80, 3);
            }
            else
            {
                setVol("windLightVol", -12, 3);
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

        // schedule the next bar to create a new sequence on
        newSeqAtBar += newSeqEveryXBars;
    }
}
