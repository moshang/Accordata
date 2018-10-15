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
        delayMix = 0.0f;
    }

    // Style Specific Variables
    bool Bar2ofPair; // the style plays two bars per site and we need to keep track of whether it's the 1st or 2nd bar of the pair
    private int[,] bass = {
        {33, 0, 0, 0, 45, 0, 0, 0, 33, 0, 0, 0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
    };

    private int[,] keys = {
        {0, 0, 57, 0, 0, 0, 57, 0, 0, 0, 57, 0, 0, 0, 57, 0 },
        {0, 0, 60, 0, 0, 0, 60, 0, 0, 0, 60, 0, 0, 0, 60, 0 },
        {0, 0, 64, 0, 0, 0, 46, 0, 0, 0, 64, 0, 0, 0, 64, 0 },
    };


    public override void initStyle(int newSeqBar)
    {
        seq.setBPM(bpm);
        seq.loadEnsemble(3);
        seqGen.scale = scale;
        newSeqAtBar = newSeqBar;
        resetSFX();
        // style specific initializations
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

        for (int i = 0; i < seqLength; i++)
        {
            if (uiCtrl.isActiveAqi) // check if the value is active or muted in the ui
            {
                // kick
                if (i == 0 || i == 8)
                    seq.addNote(i, 27, 110);
                if (i == 14 && Bar2ofPair)
                    seq.addNote(i, 29, 90);
                // snare
                if (i == 4 || i == 12)
                    seq.addNote(i, 30, 110);
                else if (i % 3 == 0 && Random.Range(0, 100) < 5)
                    seq.addNote(i, 31, 110);
                // hats
                if (i % 8 == 0)
                    seq.addNote(i, 110, 127);
                else if (i % 8 == 0)
                    seq.addNote(i, 109, Random.Range(108, 117));
                else
                    seq.addNote(i, 108, Random.Range(90, 105));
                // bass
                seq.addNote(i, bass[0, i], Random.Range(117, 127));
            }
            /*
            if (uiCtrl.isActiveTemp) // check if the value is active or muted in the ui
            {
                seq.addNote(0, 0, 0);
            }
            if (uiCtrl.isActiveWind) // check if the value is active or muted in the ui
            {
                seq.addNote(0, 0, 0);
            }
                       */
            if (uiCtrl.isActiveHumidity) // check if the value is active or muted in the ui
            {
                /*
                for (int j = 0; j < 3; j++) // 3 notes in the triad
                    seq.addNote(i, keys[j, i], Random.Range(108, 117));
                    */
            }
            /*
            if (uiCtrl.isActiveRain) // check if the value is active or muted in the ui
            {
                seq.addNote(0, 0, 0);
            }
            */
        }

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
