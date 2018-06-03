// Copyright 2017 Matt Tytel

using UnityEngine;
using UnityEngine.UI;

namespace AudioHelm
{
    [AddComponentMenu("")]
    public class PulseImageOnNote : MonoBehaviour
    {
        public MaterialPulse[] images;
        public int startingNote = 20;
        public int[] scale = { 0, 2, 4, 5, 7, 9, 11 };

        int GetNoteIndex(int note)
        {
            int noteAdjusted = (note - startingNote + Utils.kMidiSize) % Utils.kMidiSize;
            int octave = noteAdjusted / Utils.kNotesPerOctave;
            int noteInOctave = noteAdjusted - Utils.kNotesPerOctave * octave;

            for (int scaleNote = 0; scaleNote < scale.Length; ++scaleNote)
            {
                if (scale[scaleNote] >= noteInOctave)
                    return octave * scale.Length + scaleNote;
            }
            return octave * scale.Length;
        }

        public void NoteOn(Note note)
        {
            int index = GetNoteIndex(note.note) % images.Length;
            images[index].Pulse(1.0f);
        }
    }
}
