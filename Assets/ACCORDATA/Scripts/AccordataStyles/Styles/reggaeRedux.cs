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

public class reggaeRedux : Style
{
    // -> ACCORDATA <-
    private void Reset() // override the values in the parent class
    {
        // Info about the style to display in the app - populate each field so we don't have blanks in the UI. If there's  no translation for the English field in Chinese, use the English value (and vice versa)
        StyleNameEng = "Reggae Redux";
        StyleNameChTw = "雷鬼解構";
        ComposerNameEng = "MoShang";
        ComposerNameChTw = "莫尚";
        StyleInfoEng = "";
        StyleInfoChTw = "";
        // Style settings
        bpm = 86;
        scale = Scale.pentatonic;
        seqLength = 16; // the length of a single bar of the sequence in 16ths
        newValuesEveryXBars = 2; // switch to the next set of values every x bars (either values from the next site, or the next hour for the same site)
        newSeqEveryXBars = 1;           /*  how often should we generate a new sequence?
                                            this value needs to be smaller than the newValuesEveryXBars value, or we will skip over new data
                                            set to 1 to regenerate the sequence with the current data every bar, even if the data hasn't changed
                                            set to more than 1 to repeat the previously generated bar */
        // Accoplayer  Global FX (ie. affects everything on the accoplayer audiosource, but not on other Unity mixer channels)
        useReverb = true;
        revrbPreset = AudioReverbPreset.Room;
        useDelay = true;
        delayTimeMS = 865;
        delayDecay = 0.5f;
        delayMix = 0.15f;
    }

    // Style Specific Variables
    public AudioClip tempLight;
    public AudioClip windLight;

    private bool Bar2ofPair; // the style plays two bars per site and we need to keep track of whether it's the 1st or 2nd bar of the pair
    private readonly int[,] bass = {
        // bar 1 of pair
        {33, 0, 0, 0, 45, 0, 0, 0, 33, 0, 0, 0, 0, 0, 0, 0 },
        {33, 0, 0, 0, 45, 0, 0, 0, 33, 0, 0, 0, 33, 0, 45, 0 },
        {33, 33, 33, 0, 45, 0, 0, 0, 33, 0, 0, 0, 0, 0, 33, 0 },
        {33, 0, 45, 0, 33, 0, 0, 0, 33, 0, 0, 0, 33, 0, 0, 0 },
        {33, 0, 0, 0, 45, 0, 0, 0, 33, 0, 0, 0, 33, 0, 45, 0 },
        // bar 2 of pair
        {33, 0, 0, 0, 45, 0, 33, 0, 45, 45, 45, 45, 40, 0, 40, 0 },
        {33, 0, 0, 0, 45, 0, 0, 0, 45, 0, 45, 45, 40, 0, 45, 0 },
        {33, 0, 45, 0, 33, 0, 45, 33, 38, 38, 38, 38, 40, 0, 40, 38 },
        {33, 0, 0, 0, 33, 0, 45, 33, 38, 38, 38, 38, 40, 0, 45, 0  },
        {33, 0, 0, 0, 45, 0, 0, 0, 38, 0, 38, 38, 45, 0, 40, 0  }
    };

    private readonly int[,] keys = {
        {0, 0, 57, 0, 0, 0, 57, 0, 0, 0, 57, 0, 0, 0, 57, 0 },
        {0, 0, 60, 0, 0, 0, 60, 0, 0, 0, 60, 0, 0, 0, 60, 0 },
        {0, 0, 64, 0, 0, 0, 64, 0, 0, 0, 64, 0, 0, 0, 64, 0 },
        {0, 0, 57, 0, 0, 0, 57, 0, 0, 0, 64, 0, 0, 0, 62, 0 },
        {0, 0, 60, 0, 0, 0, 60, 0, 0, 0, 67, 0, 0, 0, 65, 0 },
        {0, 0, 64, 0, 0, 0, 64, 0, 0, 0, 71, 0, 0, 0, 69, 0 },
    };


    public override void initStyle(int newSeqBar)
    {
        seq.setBPM(bpm);
        seq.loadEnsemble(3);
        seqGen.scale = scale;
        newSeqAtBar = newSeqBar;
        resetSFX();

        // style specific initializations
        seqGen.windLightAS.clip = windLight;
        seqGen.windLightAS.Play();
        seqGen.tempLightAS.clip = tempLight;
        seqGen.tempLightAS.Play();

        Bar2ofPair = false;
    }


    /// <summary>
    /// Gets called by the seqGenerator script at the start of every bar. Suggested use of this function is to populate note values for the sequencer to play in the current bar. 
    /// </summary>
    /// <param name="barNum"></param>
    /// <param name="aqiVal"></param>
    /// <param name="tempVal"></param>
    /// <param name="windVal"></param>
    /// <param name="humidityVal"></param>
    /// <param name="rainVal"></param>
    public override void makeSeq(int barNum, int aqiVal, float tempVal, float windVal, float humidityVal, float rainVal)
    {
        if (barNum != newSeqAtBar)
            return;
        seq.clear();

        int bassPhrase;

        if (!Bar2ofPair)
            bassPhrase = Random.Range(0, 4);
        else
            bassPhrase = Random.Range(5, 10);

        for (int i = 0; i < seqLength; i++)
        {
            // AQI
            if (uiCtrl.isActiveAqi) // check if the value is active or muted in the ui
            {
                // kick
                if (i == 0 || (i == 8 && aqiVal > 50))
                    seq.addNote(i, 27, 110);
                if ((i == 4 || i == 12) && aqiVal > 50)
                    seq.addNote(i, 27, 110);
                if (i == 14 && Bar2ofPair)
                    seq.addNote(i, 29, 90);
                // snare
                if (aqiVal > 30)
                {
                    if (i == 4 || (i == 12 && aqiVal > 50))
                        seq.addNote(i, 30, 110);
                    else if (i % 3 == 0 && Random.Range(0, 100) < (aqiVal / 12))
                        seq.addNote(i, 31, 110);
                }
                // hats
                if (i % 8 == 0)
                    seq.addNote(i, 110, 127);
                else if (i % 8 == 0)
                    seq.addNote(i, 109, Random.Range(108, 117));
                else
                    seq.addNote(i, 108, Random.Range(90, 105));
                // bass
                if (bass[bassPhrase, i] != 0)
                    seq.addNote(i, bass[bassPhrase, i], Random.Range(117, 127));
            }

            // HUMIDITY
            if (uiCtrl.isActiveHumidity) // check if the value is active or muted in the ui
            {
                if ((i == 6 || i == 14) && humidityVal < 85)
                {
                    // do nothing
                }
                else
                {
                    if (!Bar2ofPair)
                    {
                        for (int j = 0; j < 3; j++) // 3 notes in the triad
                        {
                            if (keys[j, i] != 0)
                                seq.addNote(i, keys[j, i], (int)Random.Range(90, (90 + (humidityVal - 85))));
                        }
                    }
                    else
                    {
                        for (int j = 3; j < 6; j++) // 3 notes in the triad
                        {
                            if (keys[j, i] != 0)
                                seq.addNote(i, keys[j, i], (int)Random.Range(90, (90 + (humidityVal - 85))));
                        }
                    }
                }
            }
            // RAIN
            if (uiCtrl.isActiveRain) // check if the value is active or muted in the ui
            {
                if (rainVal > 10 && i % 2 == 0)
                {
                    int nn = seqGen.scales[1, (15 - i) % 8] + 98;
                    if (nn <= 107 && nn >= 96)
                        seq.addNote(i, nn, Mathf.Clamp((int)Random.Range(80, 90 + (rainVal / 2)), 0, 127));
                }
                if (rainVal > 20 && i % 2 != 0)
                {
                    int nn = seqGen.scales[1, Random.Range(0, 8)] + 98;
                    if (nn <= 107 && nn >= 96)
                        seq.addNote(i, nn, Mathf.Clamp((int)Random.Range(80, 90 + (rainVal / 2)), 0, 127));
                }
            }
        }

        // TEMPERATURE
        if (uiCtrl.isActiveTemp)
        {
            if (tempVal > 20)
            {
                float newVal = (tempVal - 20) / 20;
                newVal = Mathf.Lerp(-20, 0, newVal);
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
                setVol("windLightVol", -24, 3);
                setVol("windMediumVol", -80, 3);
                setVol("windHeavyVol", -80, 3);
            }
            else if (windVal <= 13.2f)
            {
                setVol("windLightVol", -18, 3);
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

        // RAIN
        if (uiCtrl.isActiveRain)
            setDelayMix(Easings.QuadraticEaseOut(utils.map(Mathf.Clamp(rainVal, 0, 60), 0, 60, 0.15f, 0.75f)));
        else
            setDelayMix(0.15f);

        // schedule the next bar at which to create a new sequence
        newSeqAtBar += newSeqEveryXBars;
        Bar2ofPair = !Bar2ofPair;
    }

    /// <summary>
    /// Gets called by the seqGenerator script every beat. It would be a good place to do tempo changes for ritardandos/accelerandos
    /// </summary>
    public override void doEveryBeat()
    {
    }
}
