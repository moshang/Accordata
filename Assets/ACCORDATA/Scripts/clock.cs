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
using UnityEngine.UI;
[RequireComponent(typeof(AudioSource))]
public class clock : MonoBehaviour
{
    [Header("--> ACCORDATA <--")]
    public bool metronome = true;
    public dataLoader _dataLoader;
    public audioLoader _audioLoader;

    [Header("Time")]
    public string time;
    int pulses;
    int beats;
    int bars;
    public AudioSource metro;
    public static float bpm = 98;
    private float currentBpm;
    double dur16th; // duration of a 16th note

    [Header("Transport")]
    public Toggle playToggle;
    public Toggle styleToggle;

    public delegate void PulseAction(int pulseNum); // a pulse event is sent every 16th note
    public static event PulseAction OnPulse;
    public delegate void BeatAction(int beatNum); // a beat event is sent on every beat (4 x 16ths)
    public static event BeatAction OnBeat;
    public delegate void BarAction(int barNum); // a bar event is sent on every bar/measure (4 beats)
    public static event BarAction OnBar;

    public static bool isRunning = false;
    public GameObject helpArrow;
    // PD HEAVY
    Hv_AccoPlayer_AudioLib pd;

    public seqGenerator seqGen;

    void Start()
    {
        pulses = 0;
        dur16th = (60000 / bpm) / 1000 / 4;

        pd = GetComponent<Hv_AccoPlayer_AudioLib>();
        pd.RegisterSendHook();
        pd.FloatReceivedCallback += OnPdPulse;
    }

    void OnPdPulse(Hv_AccoPlayer_AudioLib.FloatMessage message)
    {
        //Debug.Log("Ping!");
        // PULSES
        if (OnPulse != null)
            OnPulse(pulses);

        // BEATS
        if (pulses % 4 == 0)
        {
            beats = (pulses / 4) % 4;
            if (OnBeat != null)
                OnBeat(beats);
            if (metronome && metro != null)
            {
                metro.pitch = 1;
                Invoke("playMetro", 0.1f); // allow for the 100ms delay in pd
            }
        }

        // BARS
        if (pulses % 16 == 0)
        {
            bars = (pulses / 16);
            if (OnBar != null)
                OnBar(bars);
            if (metronome && metro != null)
            {
                metro.pitch = 1.5f;
                Invoke("playMetro", 0.1f);
            }
        }
        time = bars.ToString() + ":0" + beats + ":" + pulses;
        pulses++;

    }

    private void playMetro()
    {
        metro.PlayOneShot(metro.clip);
    }

    public void startPlayback()
    {
        if (!_dataLoader.dataFinishedLoading)
            return;
        pd.SendEvent(Hv_AccoPlayer_AudioLib.Event.Start);
        isRunning = true;
    }
    public void stopPlayback()
    {
        pd.SendEvent(Hv_AccoPlayer_AudioLib.Event.Stop);
        isRunning = false;
    }

    public void togglePlayback()
    {
        if (playToggle.isOn)
        {
            userSettings.setFirstRun();
            helpArrow.SetActive(false);
            startPlayback();
            if (styleToggle.isOn)
                styleToggle.isOn = false;
        }
        else
        {
            stopPlayback();
            seqGen.resetSFX();
        }
    }

    public void testAudio()
    {
        //pd.SendEvent(Hv_AccordataSynth_AudioLib.Event.Test);
    }
}
