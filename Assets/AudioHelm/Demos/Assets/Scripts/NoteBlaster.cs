// Copyright 2017 Matt Tytel

using UnityEngine;

namespace AudioHelm
{
    [AddComponentMenu("")]
    public class NoteBlaster : MonoBehaviour
    {
        public HelmController player;
        public int min = 40;
        public int max = 50;

        void Update()
        {
            int note = Random.Range(min, max);
            if (Random.Range(0.0f, 1.0f) < 0.5f)
                player.NoteOn(note);
            else
                player.NoteOff(note);
        }
    }
}
