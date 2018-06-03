// Copyright 2017 Matt Tytel

using UnityEngine;
using UnityEngine.UI;

namespace AudioHelm
{
    [AddComponentMenu("")]
    public class SyncAudioAndSequencers : MonoBehaviour
    {
        public AudioHelmClock clock;
        public AudioSource loop;
        public float waitTime = 3.0f;
        public Text text;

        int lastSecond = 0;

        void Start()
        {
            double time = AudioSettings.dspTime + waitTime;
            clock.StartScheduled(time);
            loop.PlayScheduled(time);
        }

        void Update()
        {
            waitTime -= Time.deltaTime;
            int second = Mathf.CeilToInt(waitTime);

            if (second != lastSecond)
            {
                lastSecond = second;

                if (lastSecond < 1)
                {
                    Destroy(this);
                    text.text = "PLAY";
                }
                else if (text)
                    text.text = "" + lastSecond;
            }
        }
    }
}
