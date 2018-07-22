using System.Collections;
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
    public bool playLoop = true;
    public AudioSource loop;
    private int nextSiteAtBar = 0;
    private int thisSiteIndex = 0;
    private int nextSiteIndex = 1;
    private dataLoader data;
    public uiController uiCtrl;
    private int seqLength = 16;

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
        rootNote = originalRootNote;

        numScales = scale.GetNames(typeof(scale)).Length;
        numStyles = style.GetNames(typeof(style)).Length;
    }

    void makeSeq(int aqiValue = 0)
    {
        seq.clear();
        int startNote = Random.Range(50, 70);
        for (int i = 0; i < 16; i++)
            seq.addNote(i, startNote + i, 127);
        /*
        switch (genStyle)
        {
            case style.arp:

                arp(aqi);
                break;

            case style.minimalMelody:
                minimalMelody(aqi);
                break;
        }
        */
    }

    void everyBeat(int beat)
    {

    }

    void everyBar(int bar)
    {
        if (genStyle == style.arp && (bar % 2) == 0)
            scale = (scale)((int)(scale + 1) % numScales);

        if (bar % 4 == 0)
            loop.PlayOneShot(loop.clip, 0.5f);

        if (nextSiteAtBar == bar)
        {
            thisSiteIndex = nextSiteIndex;
            nextSiteIndex = (thisSiteIndex + 1) % data.sites.Length;
            if (!aqiDebug)
                aqi = data.sites[thisSiteIndex].aqi;
            uiCtrl.updateCard(thisSiteIndex);

            nextSiteAtBar = bar + getSiteBarDuration(aqi);
            regenMinimalMelody();
        }
        makeSeq(aqi);
    }
    // --> STYLES <--
    void arp(int aqiVal)
    {
        int notesInArp = Mathf.Clamp((int)utils.map(aqiVal % 100, 0, 50, 2, 6), 2, 5);
        // ARP
        int scaleIndex = 0;
        for (int i = 0; i < 16; i++)
        {
            seq.addNote(i, rootNote + scales[(int)scale, triad[scaleIndex % notesInArp]], 128);
            scaleIndex++;
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
            seq.addNote(scaleIndex, rootNote + 12 + scales[(int)scale, newNoteIndex], 127);
            lastNoteIndex = newNoteIndex;
            scaleIndex += (UnityEngine.Random.Range(2, 5) * 2);
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
                        seq.addNote(i, noteNum, 127);

                    if (aqiVal > 200)
                        seq.addNote(i, noteNum - 24, 127); // add bass 2 octave below
                    else if (aqiVal > 150)
                        seq.addNote(i, noteNum - 12, 127); // add bass 1 octave below
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
                        seq.addNote(i, rmNoteNum[i], 127); // add it but silent
                    else if (aqiVal <= 150)
                        seq.addNote(i, rmNoteNum[i], 127);

                    if (aqiVal > 200)
                        seq.addNote(i, rmNoteNum[i] - 24, 127); // add bass 2 octave below
                    else if (aqiVal > 150)
                        seq.addNote(i, rmNoteNum[i] - 12, 127); // add bass 1 octave below
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
                seq.addNote(i, rmNoteNum[melodyIndex] + 12, 127);
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
                    seq.addNote(i, rmNoteNum[melodyIndex] + 17, 127);
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
                    seq.addNote(i, rmNoteNum[melodyIndex] + 24, 127);
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
