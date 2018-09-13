using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AccordataStyle;

public class minimalMelody : Style
{
    // -> ACCORDATA <-
    private void Reset()
    {
        // Info about the style to display in the app
        StyleNameEng = "Minimal Melody";
        StyleNameChTw = "";
        ComposerNameEng = "MoShang";
        ComposerNameChTw = "莫尚";
        StyleInfoEng = "";
        StyleInfoChTw = "";
        // Style settings
        bpm = 85;
        scale = Scale.pentatonic;
        seqLength = 16; // the length of a single bar of the sequence in 16ths
        newValuesEveryXBars = 2; // switch to the next set of values every x bars (either values from the next site, or the next hour for the same site)
        newSeqEveryXBars = 2;           /*  how often should we generate a new sequence?
                                            this value needs to be smaller than the newValuesEveryXBars value, or we will skip over new data
                                            setting to 1 to regenerates the sequence with the current data every bar, even if the data hasn't changed
                                            setting to more than 1 repeats the previously generated bar */
    }

    // Style Specific Variables
    private int noteDensity = 25; // likelyhood of this 16th note containing a note
    private int[] rmNoteNum;
    private bool melodyExists = false;

    public override void initStyle(int newSeqBar)
    {
        seq.setBPM(bpm);
        seqGen.scale = scale;
        newSeqAtBar = newSeqBar;
    }

    public override void makeSeq(int barNum, int aqiVal, float tempVal, float windVal, float humidityVal, float rainVal)
    {
        if (barNum != newSeqAtBar)
            return;

        seq.clear();
        int startNote = Random.Range(50, 70);
        melodyExists = false;
        // seq_0
        if (!melodyExists)
        {
            rmNoteNum = new int[seqLength];
            for (int i = 0; i < seqLength; i++)
            {
                int noteNum = 0;
                if (i == 0 || Random.Range(0, 100) <= 20)  // use a fixed low value for teh bass line // noteDensity)
                {
                    noteNum = seqGen.originalRootNote + seqGen.scales[(int)seqGen.scale, Random.Range(0, 7)];
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

        // schedule the next bar to create a new sequence on
        newSeqAtBar += newSeqEveryXBars;
    }


    public void regenMinimalMelody()
    {
        /*
        melodyExists = false;
        if (originalRootNote == 48)
            originalRootNote = 55;
        else
            originalRootNote = 48;
            */
    }
}
