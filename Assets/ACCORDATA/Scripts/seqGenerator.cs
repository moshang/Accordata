using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AccordataStyle;

public enum Scale { major, minor, dimished, pentatonic };
public class seqGenerator : MonoBehaviour
{
    [Header("--> ACCORDATA <--")]
    public readonly int[] triad = { 0, 2, 4, 6, 7 };
    public readonly int[,] scales = {   /*major*/       { 0, 2, 4, 5, 7, 9, 11, 12 }, 
                                        /*minor*/       { 0, 2, 3, 5, 7, 9, 11, 12 }, 
                                        /*diminished*/  { 0, 2, 3, 5, 6, 8, 9, 11 }, 
                                        /*pentatonic*/  { 0, 2, 4, 7, 9, 12, 14, 16 } };
    public Scale scale = 0;
    public int numScales;
    public int originalRootNote = 48;
    private int rootNote;
    private int newValuesAtBar = 0;
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
    public Style[] styles;
    public int currentStyleIndex;

    // current site values
    private int aqiVal;
    private float tempVal;
    private float windVal;
    private float humidityVal;
    private float rainVal;

    void Start()
    {
        data = GetComponent<dataLoader>();
        clock.OnBar += everyBar;
        //clock.OnBeat += everyBeat;
        //clock.OnPulse += everyPulse;
        rootNote = originalRootNote;

        numScales = Scale.GetNames(typeof(Scale)).Length;

        styles = transform.Find("Styles").GetComponents<Style>();

        // set the first style as default
        currentStyleIndex = 0;
        switchStyle(currentStyleIndex);
    }

    public void switchStyle(int styleIndex)
    {
        seq.setBPM(styles[currentStyleIndex].bpm);

    }

    /*
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
            */
    void everyBeat(int beat)
    {

    }

    void everyBar(int bar)
    {
        //if (genStyle == style.arp && (bar % 2) == 0)
        //    scale = (scale)((int)(scale + 1) % numScales);

        if (newValuesAtBar == bar)
        {
            if (bar != 0)
            {
                thisSiteIndex = nextSiteIndex;
                uiCtrl.currentSiteIndex = thisSiteIndex;
                nextSiteIndex = (thisSiteIndex + 1) % numSites;
            }
            aqiVal = data.sites[thisHour, thisSiteIndex].aqi;
            tempVal = data.sites[thisHour, thisSiteIndex].temperature;
            windVal = data.sites[thisHour, thisSiteIndex].windspeed;
            humidityVal = data.sites[thisHour, thisSiteIndex].humidity;
            rainVal = data.sites[thisHour, thisSiteIndex].rainfall;
            uiCtrl.updateCard(thisSiteIndex);
            newValuesAtBar = bar + styles[currentStyleIndex].newValuesEveryXBars;
        }
        styles[currentStyleIndex].makeSeq(bar, aqiVal, tempVal, windVal, humidityVal, rainVal);
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
    /*
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
*/
}
