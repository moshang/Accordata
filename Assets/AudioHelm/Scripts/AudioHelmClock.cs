// Copyright 2017 Matt Tytel

using UnityEngine;

namespace AudioHelm
{
    /// <summary>
    /// ## [Switch to Manual](../manual/class_audio_helm_1_1_audio_helm_clock.html)<br>
    /// Sets the BPM (beats per minute) of all sequencers and native Helm instances.
    /// </summary>
    [AddComponentMenu("Audio Helm/Audio Helm Clock")]
    [HelpURL("http://tytel.org/audiohelm/manual/class_audio_helm_1_1_audio_helm_clock.html")]
    public class AudioHelmClock : MonoBehaviour
    {
        static float globalBpm = 120.0f;
        static bool globalPause = false;
        static double globalBeatTime = 0.0;
        static double lastSampledTime = 0.0;
        static AudioHelmClock singleton;

        const double waitToSync = 1.0;
        const double SECONDS_PER_MIN = 60.0;

        /// <summary>
        /// A reset event that is triggered when time restarts.
        /// </summary>
        public delegate void ResetAction();

        /// <summary>
        /// Event hook for a reset event
        /// </summary>
        public event ResetAction OnReset;

        [SerializeField]
        float bpm_ = 120.0f;

        [SerializeField]
        bool pause_ = false;

        /// <summary>
        /// Gets or sets the beats per minute.
        /// </summary>
        /// <value>The new or current bpm.</value>
        public float bpm
        {
            get
            {
                return bpm_;
            }
            set
            {
                bpm_ = value;
                SetGlobalBpm();
            }
        }

        /// <summary>
        /// Gets or sets the pause state
        /// </summary>
        /// <value>The new pause state.</value>
        public bool pause
        {
            get
            {
                return pause_;
            }
            set
            {
                pause_ = value;
                SetGlobalPause();
            }
        }

        public static AudioHelmClock GetInstance()
        {
            return singleton;
        }

        void Awake()
        {
            singleton = this;
            SetGlobalBpm();
            Reset();
            Native.Pause(globalPause);
        }

        void OnDestroy()
        {
            Native.Pause(false);
        }

        void SetGlobalBpm()
        {
            if (bpm_ > 0.0f)
            {
                Native.SetBpm(bpm_);
                globalBpm = bpm_;
            }
        }

        void SetGlobalPause()
        {
            Native.SetBeatTime(globalBeatTime);
            Native.Pause(pause_);
            lastSampledTime = AudioSettings.dspTime;
            globalPause = pause_;
        }

        public void StartScheduled(double timeToStart)
        {
            lastSampledTime = AudioSettings.dspTime;
            double deltaTime = timeToStart - lastSampledTime;
            globalBeatTime = -deltaTime * globalBpm / SECONDS_PER_MIN;
            Native.SetBeatTime(globalBeatTime);
        }

        /// <summary>
        /// Resets time and all sequencers from the beginning.
        /// </summary>
        public void Reset()
        {
            globalBeatTime = -waitToSync;
            Native.SetBeatTime(globalBeatTime);
            lastSampledTime = AudioSettings.dspTime;

            if (OnReset != null)
                OnReset();
        }

        /// <summary>
        /// Gets the global beats per minute.
        /// </summary>
        public static float GetGlobalBpm()
        {
            return globalBpm;
        }

        /// <summary>
        /// Gets the global synchronize time.
        /// </summary>
        public static double GetGlobalBeatTime()
        {
            return globalBeatTime;
        }

        /// <summary>
        /// Gets the global pause state.
        /// </summary>
        public static bool GetGlobalPause()
        {
            return globalPause;
        }

        /// <summary>
        /// Get the last time the clock was updated.
        /// </summary>
        public static double GetLastSampledTime()
        {
            return lastSampledTime;
        }

        void FixedUpdate()
        {
            if (pause_)
                return;
            
            double time = AudioSettings.dspTime;
            double deltaTime = time - lastSampledTime;
            lastSampledTime = time;

            globalBeatTime += deltaTime * globalBpm / SECONDS_PER_MIN;
            Native.SetBeatTime(globalBeatTime);
        }
    }
}
