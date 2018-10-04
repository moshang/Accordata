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

public class styleTemplate : Style
{
    // -> ACCORDATA <-
    private void Reset() // override the values in the parent class
    {
        // Info about the style to display in the app - populate each field so we don't have blanks in the UI. If there's  no translation for the English field in Chinese, use the English value (and vice versa)
        StyleNameEng = "";
        StyleNameChTw = "";
        ComposerNameEng = "";
        ComposerNameChTw = "";
        StyleInfoEng = "";
        StyleInfoChTw = "";
        // Style settings
        bpm = 90;
        scale = Scale.pentatonic;
        seqLength = 16; // the length of a single bar of the sequence in 16ths
        newValuesEveryXBars = 2; // switch to the next set of values every x bars (either values from the next site, or the next hour for the same site)
        newSeqEveryXBars = 1;           /*  how often should we generate a new sequence?
                                            this value needs to be smaller than the newValuesEveryXBars value, or we will skip over new data
                                            set to 1 to regenerate the sequence with the current data every bar, even if the data hasn't changed
                                            set to more than 1 to repeat the previously generated bar */
    }

    // Style Specific Variables

    public override void initStyle(int newSeqBar)
    {
        seq.setBPM(bpm);
        seqGen.scale = scale;
        newSeqAtBar = newSeqBar;

        // style specific initializations
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
                seq.addNote(0, 0, 0);
            }
            if (uiCtrl.isActiveTemp) // check if the value is active or muted in the ui
            {
                seq.addNote(0, 0, 0);
            }
            if (uiCtrl.isActiveWind) // check if the value is active or muted in the ui
            {
                seq.addNote(0, 0, 0);
            }
            if (uiCtrl.isActiveHumidity) // check if the value is active or muted in the ui
            {
                seq.addNote(0, 0, 0);
            }
            if (uiCtrl.isActiveRain) // check if the value is active or muted in the ui
            {
                seq.addNote(0, 0, 0);
            }
        }

        // schedule the next bar at which to create a new sequence
        newSeqAtBar += newSeqEveryXBars;
    }

    /// <summary>
    /// Gets called by the seqGenerator script every beat. It would be a good place to do tempo changes for ritardandos/accelerandos
    /// </summary>
    public override void doEveryBeat()
    {
    }
}
