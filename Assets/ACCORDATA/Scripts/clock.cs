using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class clock : MonoBehaviour
{
    [Header("--> ACCORDATA <--")]
    public float bpm = 98;
    private float currentBpm;
    double dur16th; // duration of a 16th note
    [Header("Time")]
    public string time;
    int pulses;
    int beats;
    int bars;
    public AudioSource metro;

    public delegate void PulseAction(int pulseNum); // a pulse event is sent every 16th note
    public static event PulseAction OnPulse;
    public delegate void BeatAction(int beatNum); // a beat event is sent on every beat (4 x 16ths)
    public static event BeatAction OnBeat;
    public delegate void BarAction(int barNum); // a bar event is sent on every bar/measure (4 beats)
    public static event BarAction OnBar;

    // PD HEAVY
    Hv_Accordata_AudioLib pd;

    void Start()
    {
        pulses = 0;
        dur16th = (60000 / bpm) / 1000 / 4;

        pd = GetComponent<Hv_Accordata_AudioLib>();
        pd.RegisterSendHook();
        pd.FloatReceivedCallback += OnPdPulse;
    }

    void Update()
    {

    }

    void OnPdPulse(Hv_Accordata_AudioLib.FloatMessage message)
    {
        if (OnPulse != null)
            OnPulse(pulses);
        if (pulses % 4 == 0)
        {
            beats = (pulses / 4) % 4;
            if (OnBeat != null)
                OnBeat(beats);
        }
        if (pulses % 16 == 0)
        {
            bars = (pulses / 16);
            if (OnBar != null)
                OnBar(bars);
        }
        time = bars.ToString() + ":0" + beats + ":" + pulses;
        pulses++;
        if (metro != null)
            metro.PlayOneShot(metro.clip);
    }

    public void startPlayback()
    {
        pd.SendEvent(Hv_Accordata_AudioLib.Event.Start);
    }
}
