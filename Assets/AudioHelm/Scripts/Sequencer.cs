// Copyright 2017 Matt Tytel

using UnityEngine;
using UnityEngine.Events;
using System;
using System.IO;
using System.Collections.Generic;

namespace AudioHelm
{
    /// <summary>
    /// A series of notes and velocities on a timeline that can be used to trigger synth or sampler notes.
    /// </summary>
    public abstract class Sequencer : MonoBehaviour, NoteHandler, ISerializationCallbackReceiver
    {
        /// <summary>
        /// A note event.
        /// </summary>
        /// <param name="note">The Note object that triggered the event.</param>
        public delegate void NoteAction(Note note);

        /// <summary>
        /// A beat event.
        /// </summary>
        /// <param name="index">The current beat index.</param>
        public delegate void BeatAction(int index);

        /// <summary>
        /// Event hook for a note on event.
        /// </summary>
        public event NoteAction OnNoteOn;

        /// <summary>
        /// Event hook for a note off event.
        /// </summary>
        public event NoteAction OnNoteOff;

        /// <summary>
        /// Event hook for a beat event.
        /// </summary>
        public event BeatAction OnBeat;

        [Serializable]
        public class NoteEvent : UnityEvent<Note> { }

        [Serializable]
        public class BeatEvent : UnityEvent<int> { }

        /// <summary>
        /// UnityEvent hook for a note on.
        /// </summary>
        [Tooltip("Triggered when note on happens. Passes the AudioHelm.Note object.")]
        public NoteEvent noteOnEvent;

        /// <summary>
        /// UnityEvent hook for a note off.
        /// </summary>
        [Tooltip("Triggered when note off happens. Passes the AudioHelm.Note object.")]
        public NoteEvent noteOffEvent;

        /// <summary>
        /// UnityEvent hook for a beat. Depends on the division setting of the seqeuncer.
        /// </summary>
        [Tooltip("Triggered when a beat happens. Passes the index of the division starting at 0.")]
        public BeatEvent beatEvent;

        List<Note> activeNotes = new List<Note>();

        class NoteComparer : IComparer<Note>
        {
            public int Compare(Note left, Note right)
            {
                if (left.start < right.start)
                    return -1;

                if (left.start > right.start)
                    return 1;
                return 0;
            }
        }

        class NotePositionComparer : IComparer<NotePosition>
        {
            public int Compare(NotePosition left, NotePosition right)
            {
                if (left.position_ < right.position_)
                    return -1;

                if (left.position_ > right.position_)
                    return 1;

                if (left.note_ < right.note_)
                    return -1;

                if (left.note_ > right.note_)
                    return 1;
                return 0;
            }
        }

        struct NotePosition
        {
            public float position_;
            public int note_;

            public NotePosition(float position, int note)
            {
                position_ = position;
                note_ = note;
            }
        }

        /// <summary>
        /// Possible divisions of the sequencer UI.
        /// </summary>
        public enum Division
        {
            kEighth,
            kSixteenth,
            kTriplet,
            kThirtySecond,
        }

        /// <summary>
        /// The length of the sequence measured in sixteenth notes.
        /// </summary>
        [Tooltip("The number of sixteenth notes in the sequencer.")]
        public int length = 16;

        /// <summary>
        /// The current index position measured in the division of the sequencer.
        /// </summary>
        public int currentIndex = -1;

        /// <summary>
        /// If the sequencer loops back to the beginning on finish.
        /// </summary>
        public bool loop = true;

        protected double beatTime = 0.0;
        protected bool paused = false;

        /// <summary>
        /// All notes in the seqeuncer.
        /// </summary>
        public NoteRow[] allNotes = new NoteRow[Utils.kMidiSize];

        /// <summary>
        /// The x/y scroll position of the inspector sequencer piano roll.
        /// </summary>
        public Vector2 scrollPosition = Vector2.zero;

        /// <summary>
        /// Should the inspector window scroll with playback.
        /// </summary>
        [Tooltip("If enabled, will scroll with playhead while sequencer is playing.")]
        public bool autoScroll = false;

        /// <summary>
        /// How often a bar or a division is placed in the sequencer inspector view.
        /// </summary>
        [Tooltip("How often a bar or a division is placed in the sequencer inspector view.")]
        public Division division = Division.kSixteenth;

        /// <summary>
        /// The smallest width to draw a sequencer beat in the inspector
        /// </summary>
        [Tooltip("How zoomed into the inspector sequencer we are. [0.0, 1.0]")]
        public float zoom = 0.3f;

        static NoteComparer noteComparer = new NoteComparer();
        static NotePositionComparer notePositionComparer = new NotePositionComparer();

        SortedList<NotePosition, Note> sortedNoteOns =
            new SortedList<NotePosition, Note>(notePositionComparer);
        SortedList<NotePosition, Note> sortedNoteOffs =
            new SortedList<NotePosition, Note>(notePositionComparer);

        float lastSequencerPosition = -1.0f;

        /// <summary>
        /// Triggers note off events for all notes currently on in the instrument.
        /// </summary>
        public virtual void AllNotesOff()
        {
            foreach (Note note in activeNotes)
            {
                if (OnNoteOff != null)
                    OnNoteOff(note);
                if (noteOffEvent != null)
                    noteOffEvent.Invoke(note);
                note.TriggerNoteOffEvent();
            }
            activeNotes.Clear();
        }

        /// <summary>
        /// Triggers a note on event for the instrument.
        /// </summary>
        /// <param name="note">The MIDI keyboard note to play. [0, 127]</param>
        /// <param name="velocity">How hard you hit the key. [0.0, 1.0]</param>
        public abstract void NoteOn(int note, float velocity = 1.0f);

        /// <summary>
        /// Triggers a note off event for the instrument.
        /// </summary>
        /// <param name="note">The MIDI keyboard note to turn off. [0, 127]</param>
        public abstract void NoteOff(int note);

        [Obsolete("StartScheduled is deprecated. Use AudioHelmClock.StartScheduled instead.")]
        public void StartScheduled(double dspTime) { }

        /// <summary>
        /// Starts the sequencer on the start next cycle.
        /// This is useful if you have multiple synced sequencers and you want to start this one on the next go around.
        /// </summary>
        public abstract void StartOnNextCycle();

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            InitNoteRows();
        }

        protected virtual void OnEnable()
        {
            AudioHelmClock clock = AudioHelmClock.GetInstance();
            if (clock)
                clock.OnReset += AllNotesOff;

            UpdatePosition();
            AllNotesOff();
            activeNotes.Clear();
        }

        protected virtual void OnDisable()
        {
            AudioHelmClock clock = AudioHelmClock.GetInstance();
            if (clock)
                clock.OnReset -= AllNotesOff;
        }

        NotePosition NoteOnPosition(Note note)
        {
            return new NotePosition(note.start, note.note);
        }

        NotePosition NoteOffPosition(Note note)
        {
            return new NotePosition(note.end, note.note);
        }

        protected void RemoveSortedNoteEvents(Note note)
        {
            sortedNoteOns.Remove(NoteOnPosition(note));
            sortedNoteOffs.Remove(NoteOffPosition(note));
        }

        protected void AddSortedNoteEvents(Note note)
        {
            if (sortedNoteOns.ContainsKey(NoteOnPosition(note)))
                return;
            sortedNoteOns.Add(NoteOnPosition(note), note);
            NotePosition offPosition = NoteOffPosition(note);
            if (!sortedNoteOffs.ContainsKey(offPosition))
                sortedNoteOffs.Add(offPosition, note);
        }

        /// <summary>
        /// Reference to the native sequencer instance memory (if any).
        /// </summary>
        /// <returns>The reference the native sequencer. IntPtr.Zero if it doesn't exist.</returns>
        public virtual IntPtr Reference()
        {
            return IntPtr.Zero;
        }

        protected void InitNoteRows()
        {
            sortedNoteOns.Clear();
            sortedNoteOffs.Clear();

            for (int i = 0; i < allNotes.Length; ++i)
            {
                if (allNotes[i] == null)
                    allNotes[i] = new NoteRow();

                foreach (Note note in allNotes[i].notes)
                    AddSortedNoteEvents(note);
            }
        }

        /// <summary>
        /// Gets the length of the division measured in sixteenth notes.
        /// </summary>
        /// <returns>The division length measured in sixteenth notes.</returns>
        public float GetDivisionLength()
        {
            if (division == Division.kEighth)
                return 2.0f;
            if (division == Division.kSixteenth)
                return 1.0f;
            if (division == Division.kTriplet)
                return 4.0f / 3.0f;
            if (division == Division.kThirtySecond)
                return 0.5f;
            return 1.0f;
        }

        /// <summary>
        /// Notifies the sequencer of a change to the key of one of the notes.
        /// </summary>
        /// <param name="note">The MIDI note that was changed.</param>
        /// <param name="oldKey">The key the note used to be.</param>
        public void NotifyNoteKeyChanged(Note note, int oldKey)
        {
            allNotes[oldKey].notes.Remove(note);
            allNotes[note.note].notes.Add(note);

            sortedNoteOns.Remove(new NotePosition(note.start, oldKey));
            sortedNoteOffs.Remove(new NotePosition(note.end, oldKey));
            AddSortedNoteEvents(note);
        }

        /// <summary>
        /// Notifies the sequencer of a change to one of the note start positions.
        /// </summary>
        /// <param name="note">The MIDI note that was changed.</param>
        /// <param name="oldStart">The previous start position of the note.</param>
        public void NotifyNoteStartChanged(Note note, float oldStart)
        {
            sortedNoteOns.Remove(new NotePosition(oldStart, note.note));
            sortedNoteOns.Add(new NotePosition(note.start, note.note), note);
        }

        /// <summary>
        /// Notifies the sequencer of a change to one of the note end positions.
        /// </summary>
        /// <param name="note">The MIDI note that was changed.</param>
        /// <param name="oldEnd">The previous end position of the note.</param>
        public void NotifyNoteEndChanged(Note note, float oldEnd)
        {
            sortedNoteOffs.Remove(new NotePosition(oldEnd, note.note));
            sortedNoteOffs.Add(new NotePosition(note.end, note.note), note);
        }

        /// <summary>
        /// Removes a note from the sequencer.
        /// </summary>
        /// <param name="note">Note.</param>
        public void RemoveNote(Note note)
        {
            allNotes[note.note].notes.Remove(note);
            RemoveSortedNoteEvents(note);
            note.TryDelete();
            note.parent = null;
        }

        /// <summary>
        /// Check if a note exists within a given range in the sequencer.
        /// </summary>
        /// <returns><c>true</c>, if a note exists in the range, <c>false</c> otherwise.</returns>
        /// <param name="note">The MIDI note to check the range in.</param>
        /// <param name="start">The start of the range measured in sixteenths.</param>
        /// <param name="end">The end of the range measured in sixteenths.</param>
        public bool NoteExistsInRange(int note, float start, float end)
        {
            return GetNoteInRange(note, start, end) != null;
        }

        /// <summary>
        /// Gets the first note in a given range in the sequencer.
        /// </summary>
        /// <returns>The first found note. Returns null if no note was found.</returns>
        /// <param name="note">The MIDI note to look for.</param>
        /// <param name="start">The start of the range measured in sixteenths.</param>
        /// <param name="end">The end of the range measured in sixteenths.</param>
        /// <param name="ignore">A note to ignore if found.</param>
        public Note GetNoteInRange(int note, float start, float end, Note ignore = null)
        {
            if (note >= Utils.kMidiSize || note < 0 || allNotes == null || allNotes[note] == null)
                return null;
            foreach (Note noteObject in allNotes[note].notes)
            {
                if (noteObject.OverlapsRange(start, end) && noteObject != ignore)
                    return noteObject;
            }
            return null;
        }

        /// <summary>
        /// Get all Note objects in the sequencer.
        /// </summary>
        /// <returns>A list of all Note objects sorted by their start time.</returns>
        public List<Note> GetAllNotes()
        {
            return new List<Note>(sortedNoteOns.Values);
        }

        /// <summary>
        /// Get all Note objects that have a note on in the given range in this sequencer.
        /// </summary>
        /// <returns>A list of all Note objects with note ons in the given range.</returns>
        /// <param name="start">The search start position measured in sixteenths.</param>
        /// <param name="end">The search end position measured in sixteenths.</param>
        public List<Note> GetAllNoteOnsInRange(float start, float end)
        {
            return GetAllNoteEventsInRange(start, end, sortedNoteOns);
        }

        /// <summary>
        /// Get all Note objects that have a note off in the given range in this sequencer.
        /// </summary>
        /// <returns>A list of all Note objects with note offs in the given range.</returns>
        /// <param name="start">The search start position measured in sixteenths.</param>
        /// <param name="end">The search end position measured in sixteenths.</param>
        public List<Note> GetAllNoteOffsInRange(float start, float end)
        {
            return GetAllNoteEventsInRange(start, end, sortedNoteOffs);
        }

        /// <summary>
        /// Removes all notes that overlap a given range.
        /// </summary>
        /// <param name="note">The MIDI note to match.</param>
        /// <param name="start">The start of the range measured in sixteenths.</param>
        /// <param name="end">The end of the range measured in sixteenths.</param>
        public void RemoveNotesInRange(int note, float start, float end)
        {
            if (allNotes == null || allNotes[note] == null)
                return;

            List<Note> toRemove = new List<Note>();
            foreach (Note noteObject in allNotes[note].notes)
            {
                if (noteObject.OverlapsRange(start, end))
                    toRemove.Add(noteObject);
            }
            foreach (Note noteObject in toRemove)
                RemoveNote(noteObject);
        }

        /// <summary>
        /// Removes all notes that are fully contained in a given range.
        /// </summary>
        /// <param name="note">The MIDI note to match.</param>
        /// <param name="start">The start of the range measured in sixteenths.</param>
        /// <param name="end">The end of the range measured in sixteenths.</param>
        public void RemoveNotesContainedInRange(int note, float start, float end, Note ignore = null)
        {
            if (allNotes == null || allNotes[note] == null)
                return;

            List<Note> toRemove = new List<Note>();
            foreach (Note noteObject in allNotes[note].notes)
            {
                if (noteObject.InsideRange(start, end) && noteObject != ignore)
                    toRemove.Add(noteObject);
            }
            foreach (Note noteObject in toRemove)
                RemoveNote(noteObject);
        }

        /// <summary>
        /// Removes all notes that are fully contained and trim notes that partially overlap range by removing the time inside the range.
        /// </summary>
        /// <param name="note">The MIDI note to match.</param>
        /// <param name="start">The start of the range measured in sixteenths.</param>
        /// <param name="end">The end of the range measured in sixteenths.</param>
        public void ClampNotesInRange(int note, float start, float end, Note ignore = null)
        {
            RemoveNotesContainedInRange(note, start, end, ignore);

            Note noteInRange = GetNoteInRange(note, start, end, ignore);
            while (noteInRange != null)
            {
                noteInRange.RemoveRange(start, end);
                noteInRange = GetNoteInRange(note, start, end, ignore);
            }
        }

        /// <summary>
        /// Add a note to the sequencer.
        /// </summary>
        /// <returns>The Note object added to the seqeuncer.</returns>
        /// <param name="note">The MIDI note.</param>
        /// <param name="start">The start of the note measured in sixteenths.</param>
        /// <param name="end">The end of the note measured in sixteenths.</param>
        /// <param name="velocity">The velocity of the note (how hard the key is hit).</param>
        public Note AddNote(int note, float start, float end, float velocity = 1.0f)
        {
            ClampNotesInRange(note, start, end);
            note = Mathf.Clamp(note, 0, Utils.kMidiSize - 1);
            Note noteObject = new Note()
            {
                note = note,
                start = start,
                end = end,
                velocity = velocity,
                parent = this
            };

            noteObject.TryCreate();

            if (allNotes[note] == null)
                allNotes[note] = new NoteRow();
            allNotes[note].notes.Add(noteObject);
            allNotes[note].notes.Sort(noteComparer);

            AddSortedNoteEvents(noteObject);
            return noteObject;
        }

        /// <summary>
        /// Transposes all the notes in the sequencer up by number of semitones.
        /// Negative transpose moves notes down semitones.
        /// </summary>
        /// <param name="transpose">The number of semitones to transpose by.</param>
        public void TransposeNotes(int transpose)
        {
            List<Note> notes = GetAllNotes();
            if (transpose > 0)
                notes.Reverse();

            foreach (Note note in notes)
            {
                int new_note = note.note + transpose;
                new_note = new_note >= Utils.kMidiSize ? Utils.kMidiSize - 1 : new_note;
                new_note = new_note < 0 ? 0 : new_note;
                note.note = new_note;
            }
        }

        void ReadMidiData(MidiFile.MidiData midiData)
        {
            if (midiData == null || midiData.notes == null)
                return;

            Clear();
            length = midiData.length;

            foreach (Note note in midiData.notes)
                AddNote(note.note, note.start, note.end, note.velocity);
        }

        // TODO: Get MIDI reading out of Beta.
        /// <summary>
        /// Read a MIDI file's tracks into this sequencer.
        /// Currently in Beta. This may not work for all MIDI files or as expected.
        /// </summary>
        /// <param name="midiStream">The MIDI file stream.</param>
        public void ReadMidiFile(Stream midiStream)
        {
            ReadMidiData(MidiFile.LoadMidiData(midiStream));
        }

        /// <summary>
        /// Read a MIDI file object into this sequencer.
        /// Currently in Beta. This may not work for all MIDI files or as expected.
        /// </summary>
        /// <param name="midiFile">The MIDI file object.</param>
        public void ReadMidiFile(MidiFile midiFile)
        {
            if (midiFile != null)
                ReadMidiData(midiFile.midiData);
        }

        /// <summary>
        /// Clear the sequencer of all notes.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < allNotes.Length; ++i)
            {
                if (allNotes[i] != null)
                {
                    foreach (Note note in allNotes[i].notes)
                    {
                        note.TryDelete();
                        note.parent = null;
                    }

                    allNotes[i].notes.Clear();
                }
            }

            sortedNoteOns.Clear();
            sortedNoteOffs.Clear();
        }

        /// <summary>
        /// Gets the time in seconds of a single sixteenth note in the sequencer.
        /// </summary>
        /// <returns>The time in seconds of a sixteenth note.</returns>
        public float GetSixteenthTime()
        {
            return 1.0f / (Utils.kBpmToSixteenths * AudioHelmClock.GetGlobalBpm());
        }

        protected double GetSequencerTime()
        {
            return Utils.kSixteenthsPerBeat * beatTime;
        }

        /// <summary>
        /// Gets the current position of the sequencer measured in sixteenth notes.
        /// </summary>
        /// <returns>The current position of the sequencer measured in sixteenth notes.</returns>
        public double GetSequencerPosition()
        {
            double sequencerTime = GetSequencerTime();
            int cycles = 0;
            if (loop)
                cycles = (int)(sequencerTime / length);
            return sequencerTime - cycles * length;
        }

        List<Note> GetAllNoteEventsInRange(float start, float end,
                                           SortedList<NotePosition, Note> events)
        {
            List<Note> notesInRange = new List<Note>();
            NotePosition startSearch = new NotePosition(start, -2);
            NotePosition endSearch = new NotePosition(end, -1);

            events.Add(startSearch, null);
            events.Add(endSearch, null);
            int indexStart = events.IndexOfKey(startSearch);
            int indexEnd = events.IndexOfKey(endSearch);

            IList<Note> notes = events.Values;
            int numNotes = events.Count;

            for (int i = (indexStart + 1) % numNotes; i != indexEnd; i = (i + 1) % numNotes)
                notesInRange.Add(notes[i]);

            events.Remove(startSearch);
            events.Remove(endSearch);

            return notesInRange;
        }

        void UpdateIndex()
        {
            int nextIndex = (int)(GetSequencerPosition() / GetDivisionLength());
            if (currentIndex != nextIndex)
            {
                if (OnBeat != null)
                    OnBeat(nextIndex);
                if (beatEvent != null)
                    beatEvent.Invoke(nextIndex);
            }
            currentIndex = nextIndex;
        }

        protected void UpdateBeatTime()
        {
            double globalBeatTime = AudioHelmClock.GetGlobalBeatTime();
            double bpm = AudioHelmClock.GetGlobalBpm();
            double lastUpdate = AudioHelmClock.GetLastSampledTime();

            double time = AudioSettings.dspTime;
            beatTime = globalBeatTime + bpm * (time - lastUpdate) / Utils.kSecondsPerMinute;
        }

        void SendNoteOff(Note note)
        {
            if (OnNoteOff != null)
                OnNoteOff(note);
            if (noteOffEvent != null)
                noteOffEvent.Invoke(note);
            note.TriggerNoteOffEvent();
            activeNotes.Remove(note);
        }

        void SendNoteOn(Note note) 
        {
            if (OnNoteOn != null)
                OnNoteOn(note);
            if (noteOnEvent != null)
                noteOnEvent.Invoke(note);
            note.TriggerNoteOnEvent();
            activeNotes.Add(note);
        }

        /// <summary>
        /// Update the position of the sequencer and fire any events that have occurred.
        /// </summary>
        protected void UpdatePosition()
        {
            if (AudioHelmClock.GetGlobalPause()) {
                if (!paused)
                    AllNotesOff();
                paused = true;
                return;
            }
            paused = false;

            UpdateBeatTime();
            UpdateIndex();
            float nextPosition = (float)GetSequencerPosition();

            if (nextPosition < 0.0f || nextPosition < lastSequencerPosition)
            {
                lastSequencerPosition = nextPosition;
                return;
            }

            List<Note> noteOns = GetAllNoteOnsInRange(lastSequencerPosition, nextPosition);
            List<Note> noteOffs = GetAllNoteOffsInRange(lastSequencerPosition, nextPosition);

            int noteOnIndex = 0;
            int noteOffIndex = 0;

            while (noteOnIndex < noteOns.Count && noteOffIndex < noteOffs.Count) {
                if (noteOns[noteOnIndex].start < noteOffs[noteOffIndex].end)
                    SendNoteOn(noteOns[noteOnIndex++]);
                else
                    SendNoteOff(noteOffs[noteOffIndex++]);
            }

            for (; noteOnIndex < noteOns.Count; ++noteOnIndex)
                SendNoteOn(noteOns[noteOnIndex]);
            
            for (; noteOffIndex < noteOffs.Count; ++noteOffIndex)
                SendNoteOff(noteOffs[noteOffIndex]);

            lastSequencerPosition = nextPosition;
        }
    }
}
