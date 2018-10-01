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

    public delegate void PulseAction(int pulseNum); // a pulse event is sent every 16th note
    public static event PulseAction OnPulse;
    public delegate void BeatAction(int beatNum); // a beat event is sent on every beat (4 x 16ths)
    public static event BeatAction OnBeat;
    public delegate void BarAction(int barNum); // a bar event is sent on every bar/measure (4 beats)
    public static event BarAction OnBar;

    public static bool isRunning = false;

    // PD HEAVY
    Hv_AccoPlayer_AudioLib pd;

    // TEMP
    public samplePlayer sampler;

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
            startPlayback();
            if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android)
                _audioLoader.deactivateStyleToggles();
        }
        else
        {
            stopPlayback();
            seqGen.resetSFX();
            if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android)
                _audioLoader.reactivateStyleToggles();
        }
    }

    public void testAudio()
    {
        //pd.SendEvent(Hv_AccordataSynth_AudioLib.Event.Test);
    }
}
