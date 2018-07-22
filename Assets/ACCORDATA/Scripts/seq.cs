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
    // Use this for initialization
    private void OnEnable()
    {
        instance = this;
        pd = GetComponent<Hv_AccoPlayer_AudioLib>();
        voiceToUse = new int[16]; // store a seq track/voice value for each step
    }

    public static void addNote(int stepnum, int notenum, int velo)
    {
        float cutoffFreq = ((float)velo / 127 * 22000);
        pd.SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Seqstepnum, stepnum);
        pd.SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Seqnotenum, notenum);
        pd.SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Seqnotevelo, cutoffFreq);
        pd.SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Seqvoicenum, voiceToUse[stepnum]); // setting the voice number / sequencer track is what triggers/bangs setting the pd table values
        Debug.Log("Added notenumber " + notenum + " to seq track " + voiceToUse[stepnum] + " on step " + stepnum);
        voiceToUse[stepnum] = (voiceToUse[stepnum] + 1) % 8;
    }

    public static void clear()
    {
        for (int i = 0; i < 8; i++) // 8 sequencer tracks
        {
            for (int j = 0; j < 16; j++)  //  16 steps
            {
                pd.SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Seqstepnum, j);
                pd.SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Seqnotenum, 0); // set a note number of 0
                pd.SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Seqvoicenum, i); // setting the voice number / sequencer track is what triggers/bangs setting the pd table values
            }
        }
    }

    public static void setBPM(int bpm)
    {
        pd.SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Bpm, bpm);
    }
}
