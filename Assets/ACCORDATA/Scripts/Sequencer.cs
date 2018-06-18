using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer : MonoBehaviour
{
    [Header("--> ACCORDATA <--")]
    public samplePlayer sampler;
    private int[,] _note;
    private float[,] _velocity;
    private instruments _instrument;
    private int _length = 16;
    private int seqIndex;

    public instruments instrument
    {
        get { return _instrument; }
        set { _instrument = value; }
    }

    public int length
    {
        get { return _length; }
        set
        {
            if (value > 0 && value < 65)
                _length = value;
            else
                Debug.LogWarning("Set a sequencer value in the range of 1 - 64 16ths.");
        }
    }

    // subscribe to pulse events
    private void OnEnable()
    {

        clock.OnPulse += onPulse;

        _note = new int[10, 64];
        _velocity = new float[10, 64];
        _instrument = instruments.piano;
    }

    // unsubscribe from pulse events
    private void OnDisable()
    {
        clock.OnPulse -= onPulse;
    }

    void onPulse(int pulse)
    {
        for (int i = 0; i < 10; i++)
        {
            if (_note[i, seqIndex] != 0)
            {
                sampler.playNote(_note[i, seqIndex], _velocity[i, seqIndex], _instrument);
            }
        }

        seqIndex = (seqIndex + 1) % _length;
    }

    public void addNote(int midiNoteNumber, float velocity, int stepPosition)
    {
        if (stepPosition >= 64)
        {
            Debug.LogWarning("The maximum step position is 63 - 64 steps, zero indexed.");
            return;
        }
        int noteLane = 0;
        while (_note[noteLane, stepPosition] != 0)
        {
            noteLane++;
            if (noteLane >= 10)
            {
                Debug.LogWarning("A sequencer step can't have more than 10 notes assigned.");
                return;
            }
        }
        _note[noteLane, stepPosition] = midiNoteNumber;
        _velocity[noteLane, stepPosition] = velocity;
    }

    public void clearStep(int stepPosition)
    {
        for (int i = 0; i < 10; i++)
            _note[i, stepPosition] = 0;
    }

    public void clearAll()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 64; j++)
                _note[i, j] = 0;
        }
    }
}
