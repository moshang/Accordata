// Copyright 2017 Matt Tytel

using UnityEngine;

namespace AudioHelm
{
    [AddComponentMenu("")]
    public class EnableAfterTime : MonoBehaviour
    {
        public float time = 1.0f;
        public Sequencer sequencer;

        void Start()
        {
            Invoke("Enable", time);
        }

        void Enable()
        {
            sequencer.StartOnNextCycle();
        }
    }
}
