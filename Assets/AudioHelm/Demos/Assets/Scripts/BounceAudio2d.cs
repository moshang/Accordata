// Copyright 2017 Matt Tytel

using UnityEngine;

namespace AudioHelm
{
    [AddComponentMenu("")]
    public class BounceAudio2d : MonoBehaviour
    {
        public HelmController synth;

        public int[] scale = { 0, 2, 4, 7, 9 };
        public int minNote = 24;
        public float maxSize = 10.0f;
        public float noteLength = 0.1f;
        public float maxSpeed = 1.0f;

        float GetCollisionStrength(Collision2D collision)
        {
            Vector3 normal = collision.contacts[0].normal;
            Vector3 bounceAmount = Vector3.Project(collision.relativeVelocity, normal);
            float speed = bounceAmount.magnitude;
            return Mathf.Clamp(speed / maxSpeed, 0.0f, 1.0f);
        }

        int GetNote()
        {
            float size = transform.localScale.x;
            float octaves = Mathf.Max(0.0f, Mathf.Log(maxSize / size, 2.0f));
            int playOctave = (int)octaves;
            int scaleNote = (int)(scale.Length * (octaves - playOctave));
            return minNote + playOctave * Utils.kNotesPerOctave + scale[scaleNote];
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            int note = GetNote();
            float strength = GetCollisionStrength(collision);

            if (synth)
                synth.NoteOn(note, strength, noteLength);

            MaterialPulse pulse = GetComponent<MaterialPulse>();
            if (pulse)
                pulse.Pulse(strength);
        }
    }
}
