// Copyright 2017 Matt Tytel

using UnityEngine;
using UnityEngine.UI;

namespace AudioHelm
{
    [AddComponentMenu("")]
    public class TriggerVisibleNotes : MonoBehaviour
    {
        public MaterialOnOff[] images;
        public int startingNote = 20;

        int GetIndex(Note note)
        {
            return (note.note - startingNote) % images.Length;
        }

        public void NoteOn(Note note)
        {
            int index = GetIndex(note);
            if (index >= 0 && index < images.Length)
                images[index].On();
        }

        public void NoteOff(Note note)
        {
            int index = GetIndex(note);
            if (index >= 0 && index < images.Length)
                images[index].Off();
        }

        public void Clear()
        {
            foreach (MaterialOnOff onOff in images)
                onOff.Off();
        }
    }
}
