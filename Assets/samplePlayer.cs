using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum instruments { piano };

[RequireComponent(typeof(AudioSource))]
public class samplePlayer : MonoBehaviour
{
    AudioSource audio;
    piano pianoSamples;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        pianoSamples = GetComponentInChildren<piano>();
    }
    public void playNote(int midiNoteNumber, float velocity, instruments instrument)
    {
        AudioClip clipToPlay = audio.clip; // temporarily setting the (empty) clip on the attached AudioSource - we'll never play that clip, but we need to assign something here or it will complain later
        int offset;
        int clipIndex;
        switch (instrument)
        {
            case instruments.piano:
                offset = -24;
                clipIndex = midiNoteNumber + offset;
                if (clipIndex < 0 || clipIndex >= pianoSamples.sample.Length)
                    return;
                clipToPlay = pianoSamples.sample[clipIndex];
                break;
        }
        audio.PlayOneShot(clipToPlay, velocity);
    }
}
