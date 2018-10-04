/* Copyright (c) Jean Marais / MoShang 2018. Licensed under GPLv3.
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum instruments { piano };

[RequireComponent(typeof(AudioSource))]
public class samplePlayer : MonoBehaviour
{
    AudioSource audio;
    piano pianoSamples;

    // these values were arrived at with a 440Hz sine wave and a guitar tuner
    public float[] pitchesUp = { 1f, 1.061f, 1.124f, 1.1905f, 1.26f, 1.335f, 1.414f, 1.5f, 1.59f, 1.685f, 1.78f, 1.89f, 2, 2.12f, 2.245f, 2.38f, 2.52f, 2.67f, 2.83f, 3 };
    public float[] pitchesDown = { 1f, 0.945f, 0.89f, 0.84f, 0.795f, 0.75f, 0.707f, 0.667f, 0.63f, 0.595f, 0.562f, 0.53f, 0.5f, 0.472f, 0.446f, 0.42f, 0.397f, 0.374f, 0.353f, 0.334f, 0.315f, 0.297f, 0.281f, 0.265f, 0.25f };
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
