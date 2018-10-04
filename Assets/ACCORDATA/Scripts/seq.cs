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

public class seq : MonoBehaviour
{
    // -> ACCORDATA <-
    // Helper functions to act as a bridge between Unity and the AccoPlayer PD plugin
    public static Hv_AccoPlayer_AudioLib pd;
    public static seq instance;
    static int[] voiceToUse;
    static audioLoader loader;
    // Use this for initialization
    private void OnEnable()
    {
        instance = this;
        pd = GetComponent<Hv_AccoPlayer_AudioLib>();
        voiceToUse = new int[16]; // store a seq track/voice value for each step
        loader = GetComponent<audioLoader>();
    }

    public static void addNote(int stepnum, int notenum, int velo)
    {
        float veloF = (float)velo / 127;
        float easedVelo = Mathf.Pow(veloF, 5);
        float cutoffFreq = easedVelo * 22000;
        //Debug.Log("Cutoff Freq.: " + cutoffFreq);
        pd.SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Seqstepnum, stepnum);
        pd.SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Seqnotenum, notenum);
        pd.SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Seqnotevelo, cutoffFreq);
        pd.SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Seqvoicenum, voiceToUse[stepnum]); // setting the voice number / sequencer track is what triggers/bangs setting the pd table values
        //Debug.Log("Added notenumber " + notenum + " to seq track " + voiceToUse[stepnum] + " on step " + stepnum);
        voiceToUse[stepnum] = (voiceToUse[stepnum] + 1) % 8;
    }

    public static void clear()
    {
        loader.clearSeqTables();
    }

    public static void setBPM(int bpm)
    {
        pd.SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Bpm, bpm);
    }

    public static void loadEnsemble(int ensIndex)
    {
        loader.loadEns(ensIndex);
    }
}
