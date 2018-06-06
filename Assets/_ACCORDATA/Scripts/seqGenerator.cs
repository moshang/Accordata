using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioHelm;

public class seqGenerator : MonoBehaviour
{
    [Header("--> ACCORDATA <--")]
    // MUSIC
    readonly int[,] triads = { { 0, 4, 7 }, { 0, 3, 7 }, { 0, 3, 6 } };
    readonly int[,] scales = { { 0, 2, 4, 5, 7, 9, 11, 12 }, { 0, 2, 3, 5, 7, 9, 11, 12 }, { 0, 2, 3, 5, 6, 8, 9, 11 } };
    int scale = 0;
    public Sequencer seq;
    public SampleSequencer sampleSeq;
    public HelmController ctrl;
    public int originalRootNote = 48;
    private int rootNote;
    public int barCounter;

    void Start()
    {
        seq.OnBeat += everyBeat;
        makeSeq();
        rootNote = originalRootNote;
    }

    void makeSeq()
    {

        seq.Clear();
        sampleSeq.Clear();

        // ARP
        int scaleIndex = 0;
        for (int i = 0; i < seq.length; i++)
        {
            seq.AddNote(rootNote + triads[scale, scaleIndex % 3], i, i + 1);
            scaleIndex++;
        }

        // MELODY
        scaleIndex = 0;
        int lastNoteIndex = UnityEngine.Random.Range(0, scales.GetLength(1));
        //lastNoteNum = scales[scale, lastNoteIndex)];
        while (scaleIndex < seq.length)
        {
            //int newNoteIndex = lastNoteIndex + Random.Range(-2, 3);
            //if (newNoteIndex >= scales.GetLength(1) || newNoteIndex < 0)
            //    newNoteIndex = Random.Range(0, scales.GetLength(1));
            int newNoteIndex = UnityEngine.Random.Range(0, 5);
            seq.AddNote(rootNote + 12 + scales[scale, newNoteIndex], scaleIndex, scaleIndex + 1);
            lastNoteIndex = newNoteIndex;
            scaleIndex += (UnityEngine.Random.Range(2, 5) * 2);
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
            Invoke("makeSeq", 0.1f);

            barCounter++;
        }
    }
}
