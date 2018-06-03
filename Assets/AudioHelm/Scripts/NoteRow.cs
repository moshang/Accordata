// Copyright 2017 Matt Tytel

using UnityEngine;
using System.Collections.Generic;

namespace AudioHelm
{
    /// <summary>
    /// A single note row in the sequencer.
    /// </summary>
    [System.Serializable]
    public class NoteRow : ISerializationCallbackReceiver
    {
        /// <summary>
        /// The list of all notes in this row on the sequencer.
        /// </summary>
        public List<Note> notes = new List<Note>();
        private List<Note> oldNotes = new List<Note>();

        public void OnBeforeSerialize()
        {
            oldNotes = new List<Note>(notes);
        }

        public void OnAfterDeserialize()
        {
            if (oldNotes.Count == notes.Count)
                return;
            foreach (Note note in oldNotes)
                note.TryDelete();
            foreach (Note note in notes)
                note.TryCreate();
        }
    }
}
