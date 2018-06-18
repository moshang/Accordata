using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum style { arp, minimalMelody };
public enum scale { major, minor, dimished, pentatonic };
public class seqGenerator : MonoBehaviour
{
    [Header("--> ACCORDATA <--")]
    readonly int[,] triads = { { 0, 4, 7 }, { 0, 3, 7 }, { 0, 3, 6 } };
    readonly int[,] scales = { { 0, 2, 4, 5, 7, 9, 11, 12 }, { 0, 2, 3, 5, 7, 9, 11, 12 }, { 0, 2, 3, 5, 6, 8, 9, 11 }, { 0, 2, 4, 7, 9, 12, 14, 16 } };
    public scale scale = 0;
    public Sequencer[] seq;
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
    public Text siteNameTxt;
    public Text siteAqiTxt;
    [Header("Debug")]
    public bool aqiDebug;
    [Range(0, 500)]
    public int aqi;
    public RectTransform siteCard;
    Image siteCardBG;
    // STYLES
    [Header("Minimal Melody")]
    [Range(0, 100)]
    public int noteDensity = 25; // likelyhood of this 16th note containing a note
    int[] rmNoteNum;
    bool melodyExists = false;

    void Start()
    {
        data = GetComponent<dataLoader>();
        clock.OnBar += everyBar;
        rootNote = originalRootNote;
        siteCardBG = siteCard.GetComponent<Image>();
    }

    void makeSeq(int aqiValue = 0)
    {

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
        if (bar % 4 == 0)
            loop.PlayOneShot(loop.clip, 0.5f);
        if (nextSiteAtBar == bar)
        {
            thisSiteIndex = nextSiteIndex;
            nextSiteIndex = (thisSiteIndex + 1) % data.sites.Length;
            if (!aqiDebug)
                aqi = data.sites[thisSiteIndex].aqi;
            siteNameTxt.text = data.sites[thisSiteIndex].name;
            siteAqiTxt.text = "AQI: " + aqi;
            siteCardBG.color = getAqiColor(aqi);

            nextSiteAtBar = bar + getSiteBarDuration(aqi);
            regenMinimalMelody();
        }
        makeSeq(aqi);

        //minimalMelody();
        //arp();
    }
    // --> STYLES <--
    void arp(int aqiVal)
    {
        clearSeqs();

        // ARP
        int scaleIndex = 0;
        for (int i = 0; i < seq[0].length; i++)
        {
            seq[0].addNote(rootNote + triads[(int)scale, scaleIndex % 3], 0.5f, i);
            scaleIndex++;
        }

        // MELODY
        scaleIndex = 0;
        int lastNoteIndex = UnityEngine.Random.Range(0, scales.GetLength(1));
        //lastNoteNum = scales[scale, lastNoteIndex)];
        while (scaleIndex < seq[0].length)
        {
            //int newNoteIndex = lastNoteIndex + Random.Range(-2, 3);
            //if (newNoteIndex >= scales.GetLength(1) || newNoteIndex < 0)
            //    newNoteIndex = Random.Range(0, scales.GetLength(1));
            int newNoteIndex = UnityEngine.Random.Range(0, 5);
            seq[0].addNote(rootNote + 12 + scales[(int)scale, newNoteIndex], 0.5f, scaleIndex);
            lastNoteIndex = newNoteIndex;
            scaleIndex += (UnityEngine.Random.Range(2, 5) * 2);
        }
    }

    public void regenMinimalMelody()
    {
        melodyExists = false;
    }

    void minimalMelody(int aqiVal = 0)
    {
        clearSeqs();
        // seq_0
        if (!melodyExists)
        {
            rmNoteNum = new int[seq[0].length];
            for (int i = 0; i < seq[0].length; i++)
            {
                int noteNum = 0;
                if (i == 0 || Random.Range(0, 100) <= 20)  // use a fixed low value for teh bass line // noteDensity)
                {
                    noteNum = originalRootNote + scales[(int)scale, Random.Range(0, 7)];
                    if (aqiVal <= 100)
                        seq[0].addNote(noteNum, 0f, i); // add it but silent
                    else if (aqiVal <= 150)
                        seq[0].addNote(noteNum, 0.5f, i);

                    if (aqiVal > 200)
                        seq[0].addNote(noteNum - 24, 0.5f, i); // add bass 2 octave below
                    else if (aqiVal > 150)
                        seq[0].addNote(noteNum - 12, 0.5f, i); // add bass 1 octave below
                }
                rmNoteNum[i] = noteNum;
            }
            melodyExists = true;
        }

        else
        {
            for (int i = 0; i < seq[0].length; i++)
            {
                if (rmNoteNum[i] != 0)
                {
                    if (aqiVal <= 100)
                        seq[0].addNote(rmNoteNum[i], 0f, i); // add it but silent
                    else if (aqiVal <= 150)
                        seq[0].addNote(rmNoteNum[i], 0.5f, i);

                    if (aqiVal > 200)
                        seq[0].addNote(rmNoteNum[i] - 24, 0.5f, i); // add bass 2 octave below
                    else if (aqiVal > 150)
                        seq[0].addNote(rmNoteNum[i] - 12, 0.5f, i); // add bass 1 octave below
                }
            }
        }

        // set note likelyhood
        if (aqiVal <= 50)
            noteDensity = 35;
        else if (aqiVal <= 100)
            noteDensity = 65;
        else if (aqiVal <= 150)
            noteDensity = 85;

        // seq_1
        int melodyIndex = 0;
        for (int i = 0; i < seq[0].length; i++)
        {
            // place a note on this step?
            if (Random.Range(0, 100) <= noteDensity)
            {
                while (rmNoteNum[melodyIndex] == 0)
                    melodyIndex = (melodyIndex + 1) % seq[0].length;
                seq[1].addNote(rmNoteNum[melodyIndex] + 12, 0.5f, i);
                melodyIndex = (melodyIndex + 1) % seq[0].length; // advance melodyIndex, otherwise it will keep returning true and not go to the next note
            }
        }

        if (aqiVal > 50)
        {
            // seq_2
            melodyIndex = 0;
            for (int i = 0; i < seq[0].length; i++)
            {
                // place a note on this step?
                if (Random.Range(0, 100) <= noteDensity)
                {
                    while (rmNoteNum[melodyIndex] == 0)
                        melodyIndex = (melodyIndex + 1) % seq[0].length;
                    seq[2].addNote(rmNoteNum[melodyIndex] + 17, 0.5f, i);
                    melodyIndex = (melodyIndex + 1) % seq[0].length; // advance melodyIndex, otherwise it will keep returning true and not go to the next note
                }
            }
        }

        if (aqiVal > 100)
        {
            // seq_3
            melodyIndex = 0;
            for (int i = 0; i < seq[0].length; i++)
            {
                // place a note on this step?
                if (Random.Range(0, 100) <= noteDensity)
                {
                    while (rmNoteNum[melodyIndex] == 0)
                        melodyIndex = (melodyIndex + 1) % seq[0].length;
                    seq[3].addNote(rmNoteNum[melodyIndex] + 24, 0.5f, i);
                    melodyIndex = (melodyIndex + 1) % seq[0].length; // advance melodyIndex, otherwise it will keep returning true and not go to the next note
                }
            }
        }
    }

    private void clearSeqs()
    {
        foreach (Sequencer sequ in seq)
            sequ.clearAll();
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

    Color32 getAqiColor(int aqiVal)
    {
        Color32 newColor = new Color32();
        if (aqi > 300)
            newColor = new Color32(136, 14, 79, 255);
        else if (aqi > 200)
            newColor = new Color32(173, 20, 87, 255);
        else if (aqi > 150)
            newColor = new Color32(197, 57, 41, 255);
        else if (aqi > 100)
            newColor = new Color32(245, 124, 0, 255);
        else if (aqi > 50)
            newColor = new Color32(251, 192, 45, 255);
        else
            newColor = new Color32(104, 159, 56, 255);
        return newColor;
    }
}
