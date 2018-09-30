﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AccordataStyle;

public class arper : Style
{
    // -> ACCORDATA <-
    private void Reset() // override the values in the parent class
    {
        // Info about the style to display in the app
        StyleNameEng = "Arper";
        StyleNameChTw = "Arper";
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
        seq.loadEnsemble(2);
        seq.setBPM(bpm);
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

        if (aqiVal < 50)
            currentSeqGenScale = 0;
        else if (aqiVal < 100)
            currentSeqGenScale = 1;
        else
            currentSeqGenScale = 2;

        seqGen.scale = (Scale)currentSeqGenScale;

        Debug.Log("AqiVal:" + aqiVal);

        // ARP
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
                velo = 45;
            seq.addNote(i, nnToAdd, velo);
            scaleIndex++;
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

        // MELODY
        scaleIndex = 0;
        int lastNoteIndex = UnityEngine.Random.Range(0, seqGen.scales.GetLength(1));

        while (scaleIndex < seqLength)
        {
            int newNoteIndex = UnityEngine.Random.Range(0, 5);
            seq.addNote(scaleIndex, rootNote + 12 + seqGen.scales[(int)seqGen.scale, newNoteIndex], Random.Range(30, 127));
            lastNoteIndex = newNoteIndex;
            scaleIndex += UnityEngine.Random.Range(1, 3);
        }

        // schedule the next bar to create a new sequence on
        newSeqAtBar += newSeqEveryXBars;
    }
}
