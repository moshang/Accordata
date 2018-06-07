using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioHelm;

public enum style { arp, minimalMelody };
public class seqGenerator : MonoBehaviour
{
    [Header("--> ACCORDATA <--")]
    readonly int[,] triads = { { 0, 4, 7 }, { 0, 3, 7 }, { 0, 3, 6 } };
    readonly int[,] scales = { { 0, 2, 4, 5, 7, 9, 11, 12 }, { 0, 2, 3, 5, 7, 9, 11, 12 }, { 0, 2, 3, 5, 6, 8, 9, 11 } };
    int scale = 0;
    public Sequencer[] seq;
    public SampleSequencer sampleSeq;
    public HelmController ctrl;
    public int originalRootNote = 48;
    private int rootNote;
    public int barCounter;
    public style genStyle = style.minimalMelody;

    // STYLES
    [Header("Minimal Melody")]
    [Range(0, 100)]
    public int note16thLikelyhood = 25;
    int[] rmNoteNum;
    bool melodyExists = false;


    void Start()
    {
        seq[0].OnBeat += everyBeat;
        makeSeq();
        rootNote = originalRootNote;

    }

    private void Update()
    {

    }

    void makeSeq(int aqiValue = 0)
    {

        switch (genStyle)
        {
            case style.arp:
                arp();
                break;

            case style.minimalMelody:
                minimalMelody();
                break;
        }
    }

    void everyBeat(int beat)
    {
        //ctrl.SetParameterValue(Param.kFilterCutoff, Random.Range(28, 127));
        if (beat == 0)
        {

            if (barCounter % 4 == 0)
            {
                rootNote -= 3;
                scale = (scale + 1) % 3;
            }
            else
                rootNote = originalRootNote;
        }
        //Debug.Log("----------> " + beat);
        if (beat == 15)
        {
            //Invoke("makeSeq", 0.1f);
            makeSeq();
            barCounter++;
        }
    }

    // --> STYLES <--
    void arp()
    {
        clearSeqs();
        sampleSeq.Clear();

        // ARP
        int scaleIndex = 0;
        for (int i = 0; i < seq[0].length; i++)
        {
            seq[0].AddNote(rootNote + triads[scale, scaleIndex % 3], i, i + 1);
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
            seq[0].AddNote(rootNote + 12 + scales[scale, newNoteIndex], scaleIndex, scaleIndex + 1);
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
                if (Random.Range(0, 100) <= note16thLikelyhood)
                {
                    noteNum = originalRootNote + scales[1, Random.Range(0, 5)];
                    seq[0].AddNote(noteNum, i, i + 1);
                    seq[0].AddNote(noteNum - 24, i, i + 1); // add bass
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
                    seq[0].AddNote(rmNoteNum[i], i, i + 1);
                    seq[0].AddNote(rmNoteNum[i] - 12, i, i + 1);
                }
            }
        }

        // seq_1
        int melodyIndex = 0;
        for (int i = 0; i < seq[0].length; i++)
        {
            // place a note on this step?
            if (Random.Range(0, 100) <= note16thLikelyhood)
            {
                while (rmNoteNum[melodyIndex] == 0)
                    melodyIndex = (melodyIndex + 1) % seq[0].length;
                seq[1].AddNote(rmNoteNum[melodyIndex] + 7, i, i + 1);
                melodyIndex = (melodyIndex + 1) % seq[0].length; // advance melodyIndex, otherwise it will keep returning true and not go to the next note
            }
        }

        // seq_2
        melodyIndex = 0;
        for (int i = 0; i < seq[0].length; i++)
        {
            // place a note on this step?
            if (Random.Range(0, 100) <= note16thLikelyhood)
            {
                while (rmNoteNum[melodyIndex] == 0)
                    melodyIndex = (melodyIndex + 1) % seq[0].length;
                seq[2].AddNote(rmNoteNum[melodyIndex] + 12, i, i + 1);
                melodyIndex = (melodyIndex + 1) % seq[0].length; // advance melodyIndex, otherwise it will keep returning true and not go to the next note
            }
        }

        // seq_3
        melodyIndex = 0;
        for (int i = 0; i < seq[0].length; i++)
        {
            // place a note on this step?
            if (Random.Range(0, 100) <= note16thLikelyhood)
            {
                while (rmNoteNum[melodyIndex] == 0)
                    melodyIndex = (melodyIndex + 1) % seq[0].length;
                seq[3].AddNote(rmNoteNum[melodyIndex] + 17, i, i + 1);
                melodyIndex = (melodyIndex + 1) % seq[0].length; // advance melodyIndex, otherwise it will keep returning true and not go to the next note
            }
        }
    }

    private void clearSeqs()
    {
        foreach (Sequencer sequ in seq)
            sequ.Clear();
    }
}
