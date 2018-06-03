// Copyright 2017 Matt Tytel

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AudioHelm
{
    [AddComponentMenu("")]
    public class GroundKeyboard : MonoBehaviour
    {
        public GroundKey keyModel;
        public HelmController synth;
        public Vector3 keyOffset;
        public float verticalOffset = 0.02f;

        public int numKeys = 60;
        public int startingKey = 24;

        public int[] scale = { 0, 2, 4, 5, 7, 9, 11 };
        public int octaveSize = 12;

        GroundKey[] keys;
        HashSet<int> currentIndices = new HashSet<int>();
        HashSet<int> newIndices = new HashSet<int>();

        void Start()
        {
            keys = new GroundKey[numKeys];
            for (int i = 0; i < numKeys; ++i)
                keys[i] = CreateKey(i);
        }

        GroundKey CreateKey(int key)
        {
            GroundKey groundKey = Instantiate(keyModel, null) as GroundKey;
            groundKey.transform.parent = transform;
            Vector3 position = key * keyOffset;
            position.y = transform.position.y + verticalOffset;
            groundKey.transform.position = position;
            return groundKey;
        }

        int GetKeyFromIndex(int index)
        {
            int octave = index / scale.Length;
            int noteInScale = index % scale.Length;
            return startingKey + octave * octaveSize + scale[noteInScale];
        }

        void NoteOn(int index)
        {
            if (synth)
                synth.NoteOn(GetKeyFromIndex(index));
            keys[index].SetOn(true);
        }

        void NoteOff(int index)
        {
            if (synth)
                synth.NoteOff(GetKeyFromIndex(index));
            keys[index].SetOn(false);
        }

        void TryNoteOn(int index, Vector3 contactPoint)
        {
            if (index >= 0 && index < numKeys && keys[index].IsInside(contactPoint))
            {
                if (!keys[index].IsOn())
                    NoteOn(index);
                newIndices.Add(index);
            }
        }

        void TryNoteOff(int index)
        {
            if (keys[index].IsOn())
                NoteOff(index);
        }

        void Impulse(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                float dot = Vector3.Dot(contact.point, keyOffset);
                int closestIndex = (int)Mathf.Round(dot / keyOffset.sqrMagnitude);
                TryNoteOn(closestIndex, contact.point);
            }
        }

        IEnumerator OnCollisionStay(Collision collision)
        {
            yield return new WaitForFixedUpdate();
            Impulse(collision);
        }

        IEnumerator OnCollisionEnter(Collision collision)
        {
            yield return new WaitForFixedUpdate();
            Impulse(collision);
        }

        void FixedUpdate()
        {
            foreach (int index in currentIndices)
            {
                if (!newIndices.Contains(index))
                    TryNoteOff(index);
            }

            currentIndices = newIndices;
            newIndices = new HashSet<int>();
        }
    }
}
