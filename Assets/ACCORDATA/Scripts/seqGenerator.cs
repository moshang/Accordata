﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum style { arp, minimalMelody };
public enum scale { major, minor, dimished, pentatonic };
public class seqGenerator : MonoBehaviour
{
    [Header("--> ACCORDATA <--")]
    readonly int[] triad = { 0, 2, 4, 6, 7 };
    readonly int[,] scales = { { 0, 2, 4, 5, 7, 9, 11, 12 }, { 0, 2, 3, 5, 7, 9, 11, 12 }, { 0, 2, 3, 5, 6, 8, 9, 11 }, { 0, 2, 4, 7, 9, 12, 14, 16 } };
    public scale scale = 0;
    public int numScales;
    public int numStyles;
    public int originalRootNote = 48;
    private int rootNote;
    public int barCounter;
    public style genStyle = style.minimalMelody;
    private int nextSiteAtBar = 0;
    private int thisSiteIndex = 0;
    private int nextSiteIndex = 1;
    private int thisHour = 0;
    private dataLoader data;
    public uiController uiCtrl;
    private int seqLength = 16;
    readonly int numSites = 87;

    [Header("Debug")]
    public bool aqiDebug;
    [Range(0, 500)]
    public int aqi;
    // STYLES
    [Header("Minimal Melody")]
    [Range(0, 100)]
    public int noteDensity = 25; // likelyhood of this 16th note containing a note
    int[] rmNoteNum;
    private bool melodyExists = false;


    void Start()
    {
        data = GetComponent<dataLoader>();
        clock.OnBar += everyBar;
        //clock.OnPulse += everyPulse;
        rootNote = originalRootNote;

        numScales = scale.GetNames(typeof(scale)).Length;
        numStyles = style.GetNames(typeof(style)).Length;
    }

    void makeSeq(int aqiValue = 0)
    {
        seq.clear();
        int startNote = Random.Range(50, 70);

        switch (genStyle)
        {
            case style.arp:

                arp(aqi);
                break;

            case style.minimalMelody:
                minimalMelody(aqi);
                break;
        }
    }

    void everyBeat(int beat)
    {

    }

    void everyBar(int bar)
    {
        if (genStyle == style.arp && (bar % 2) == 0)
            scale = (scale)((int)(scale + 1) % numScales);

        if (nextSiteAtBar == bar)
        {
            thisSiteIndex = nextSiteIndex;
            uiCtrl.currentSiteIndex = thisSiteIndex;
            nextSiteIndex = (thisSiteIndex + 1) % numSites;
            if (!aqiDebug)
                aqi = data.sites[thisHour, thisSiteIndex].aqi;
            uiCtrl.updateCard(thisSiteIndex);

            nextSiteAtBar = bar + getSiteBarDuration(aqi);
            regenMinimalMelody();
        }
        makeSeq(aqi);
    }
    /*
    void everyPulse(int pulse)
    {
        if ((pulse + 1) % 16 == 0)
            Invoke("newSeq", 0.01f);
    }

    private void newSeq()
    {
        makeSeq(aqi);
    }
    */
    // --> STYLES <--
    void arp(int aqiVal)
    {
        int notesInArp = Mathf.Clamp((int)utils.map(aqiVal % 100, 0, 50, 2, 6), 2, 5);
        // ARP
        int scaleIndex = 0;
        for (int i = 0; i < 16; i++)
        {
            int nnToAdd = rootNote + scales[(int)scale, triad[scaleIndex % notesInArp]] - 12;
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

        // repeating 2 notes

        for (int i = 0; i < 16; i++)
        {
            if (i % 2 == 0)
            {
                seq.addNote(i, rootNote + scales[(int)scale, triad[0]] + 24, 60 - i);
                seq.addNote(i, rootNote + scales[(int)scale, triad[1]] + 24, 60 - i);
            }
        }

        // MELODY
        scaleIndex = 0;
        int lastNoteIndex = UnityEngine.Random.Range(0, scales.GetLength(1));
        //lastNoteNum = scales[scale, lastNoteIndex)];
        while (scaleIndex < 16)
        {
            //int newNoteIndex = lastNoteIndex + Random.Range(-2, 3);
            //if (newNoteIndex >= scales.GetLength(1) || newNoteIndex < 0)
            //    newNoteIndex = Random.Range(0, scales.GetLength(1));
            int newNoteIndex = UnityEngine.Random.Range(0, 5);
            seq.addNote(scaleIndex, rootNote + 12 + scales[(int)scale, newNoteIndex], Random.Range(30, 127));
            lastNoteIndex = newNoteIndex;
            scaleIndex += UnityEngine.Random.Range(1, 3);
        }
    }

    public void regenMinimalMelody()
    {
        melodyExists = false;
        if (originalRootNote == 48)
            originalRootNote = 55;
        else
            originalRootNote = 48;
    }

    void minimalMelody(int aqiVal = 0)
    {

        // seq_0
        if (!melodyExists)
        {
            rmNoteNum = new int[seqLength];
            for (int i = 0; i < seqLength; i++)
            {
                int noteNum = 0;
                if (i == 0 || Random.Range(0, 100) <= 20)  // use a fixed low value for teh bass line // noteDensity)
                {
                    noteNum = originalRootNote + scales[(int)scale, Random.Range(0, 7)];
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
        // set note likelyhood
        /*
        if (aqiVal <= 50)
            noteDensity = 35;
        else if (aqiVal <= 100)
            noteDensity = 65;
        else if (aqiVal <= 150)
            noteDensity = 85;
*/

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

    int getSiteBarDuration(int aqiVal)
    {
        int duration = 1;
        if (aqiVal < 51) // good
            duration = 2;
        else if (aqiVal < 101) // moderate
            duration = 2;
        else // unhealthy for sensitive groups and above
            duration = 4;
        return duration;
    }
}
