using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class clock : MonoBehaviour
{
    [Header("--> ACCORDATA <--")]
    public float bpm = 98;
    private float currentBpm;
    double nextTriggerTime;
    double dur16th; // duration of a 16th note
    double startTime;
    double timerStartTime;
    [Header("Time")]
    public string time;
    int pulses;
    int beats;
    int bars;
    AudioSource metro;

    public delegate void PulseAction(int pulseNum); // a pulse event is sent every 16th note
    public static event PulseAction OnPulse;
    public delegate void BeatAction(int beatNum); // a beat event is sent on every beat (4 x 16ths)
    public static event BeatAction OnBeat;
    public delegate void BarAction(int barNum); // a bar event is sent on every bar/measure (4 beats)
    public static event BarAction OnBar;


    // Use this for initialization
    void Start()
    {
        metro = GetComponent<AudioSource>();
        pulses = 0;
        dur16th = (60000 / bpm) / 1000 / 4;
        startTime = 0;
    }

    void Update()
    {
        if (startTime == 0)
        {
            if (Input.GetKeyDown("space"))
                startPlayback();
        }
        else
        {
            if (bpm != currentBpm)
            {
                dur16th = (60000 / bpm) / 1000 / 4;
                nextTriggerTime = AudioSettings.dspTime + dur16th;
                currentBpm = bpm;
            }

            if (AudioSettings.dspTime + Time.deltaTime > nextTriggerTime) // the trigger time should be in the next frame
                StartCoroutine(frameChecker());
        }
    }

    IEnumerator frameChecker()
    {
        while (AudioSettings.dspTime < nextTriggerTime)
        {
            // do nothing
        }
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
        nextTriggerTime = startTime + pulses * dur16th;
        metro.PlayOneShot(metro.clip);
        yield return null;
    }

    void startPlayback()
    {
        timerStartTime = Time.time;
        startTime = AudioSettings.dspTime;
        nextTriggerTime = startTime + dur16th;
        currentBpm = bpm;
        if (OnPulse != null)
            OnPulse(pulses); // 0
        if (OnBeat != null)
            OnBeat(pulses / 4); // 0
        if (OnBeat != null)
            OnBeat(pulses / 4); // 0
        time = bars.ToString() + ":0" + beats + ":" + pulses;
        pulses++;
    }
}
