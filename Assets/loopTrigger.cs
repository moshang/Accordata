using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loopTrigger : MonoBehaviour
{
    AudioSource audio;
    public AudioClip[] clips;
    int currentClip;
    // Use this for initialization
    void Start()
    {
        clock.OnBar += everyBar;
        audio = GetComponent<AudioSource>();
    }

    void everyBar(int barNum)
    {
        if (barNum % 4 == 0)
        {
            double time = AudioSettings.dspTime;
            double triggerTime = time + 8; //audio.clip.length;
            audio.PlayScheduled(triggerTime);
            //audio.PlayOneShot(clips[0], 0.75f);
            //currentClip++;
        }
    }
}
