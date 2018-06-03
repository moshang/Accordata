// Copyright 2017 Matt Tytel

using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace AudioHelm
{
    /// <summary>
    /// Representation of a note in a sequencer.
    /// Changing the values in this object will change them in the sequencer.
    /// </summary>
    [System.Serializable]
    public class Note : ISerializationCallbackReceiver
    {
        /// <summary>
        /// A note event.
        /// </summary>
        /// <param name="note">The Note object that triggered the event.</param>
        public delegate void NoteAction(Note note);

        /// <summary>
        /// Event hook for a note on event.
        /// </summary>
        public event NoteAction OnNoteOn;

        /// <summary>
        /// Event hook for a note off event.
        /// </summary>
        public event NoteAction OnNoteOff;

        [SerializeField]
        private int note_;
        /// <summary>
        /// The MIDI note to play.
        /// </summary>
        public int note
        {
            get
            {
                return note_;
            }
            set
            {
                if (note_ == value)
                    return;
                int oldNote = note_;
                note_ = value;
                if (FullyNative())
                    Native.ChangeNoteKey(parent.Reference(), reference, note_);
                if (parent)
                    parent.NotifyNoteKeyChanged(this, oldNote);
            }
        }

        [SerializeField]
        private float start_;
        /// <summary>
        /// The note on time measured in sixteenth notes.
        /// </summary>
        public float start
        {
            get
            {
                return start_;
            }
            set
            {
                if (start_ == value)
                    return;
                float oldStart = start_;
                start_ = value;
                if (FullyNative())
                    Native.ChangeNoteStart(parent.Reference(), reference, start_);
                if (parent)
                    parent.NotifyNoteStartChanged(this, oldStart);
            }
        }

        [SerializeField]
        private float end_;
        /// <summary>
        /// The note off time measured in sixteenth notes.
        /// </summary>
        public float end
        {
            get
            {
                return end_;
            }
            set
            {
                if (end_ == value)
                    return;
                float oldEnd = end_;
                end_ = value;
                if (FullyNative())
                    Native.ChangeNoteEnd(parent.Reference(), reference, end_);
                if (parent)
                    parent.NotifyNoteEndChanged(this, oldEnd);
            }
        }

        [SerializeField]
        private float velocity_;
        /// <summary>
        /// The velocity of key press (how hard the note is hit). [0.0, 1.0]
        /// </summary>
        public float velocity
        {
            get
            {
                return velocity_;
            }
            set
            {
                if (velocity_ == value)
                    return;
                velocity_ = value;
                if (FullyNative())
                    Native.ChangeNoteVelocity(reference, velocity_);
            }
        }

        /// <summary>
        /// The sequencer this note belongs to.
        /// </summary>
        public Sequencer parent;

        private IntPtr reference;


        public void OnAfterDeserialize()
        {
            TryCreate();
        }

        public void OnBeforeSerialize()
        {
        }

        void CopySettingsToNative()
        {
            if (!HasNativeNote() || !HasNativeSequencer())
                return;

            Native.ChangeNoteValues(parent.Reference(), reference, note, start, end, velocity);
        }

        bool HasNativeNote()
        {
            return reference != IntPtr.Zero;
        }

        bool HasNativeSequencer()
        {
            return parent != null && parent.Reference() != IntPtr.Zero;
        }

        bool FullyNative()
        {
            return HasNativeNote() && HasNativeSequencer();
        }

        /// <summary>
        /// Sends out a note on event to all listeners.
        /// </summary>
        public void TriggerNoteOnEvent()
        {
            if (OnNoteOn != null)
                OnNoteOn(this);
        }

        /// <summary>
        /// Sends out a note off event to all listeners.
        /// </summary>
        public void TriggerNoteOffEvent()
        {
            if (OnNoteOff != null)
                OnNoteOff(this);
        }

        /// <summary>
        /// Tries to create a native note representation if one doesn't exist already.
        /// </summary>
        public void TryCreate()
        {
            if (HasNativeSequencer())
            {
                if (HasNativeNote())
                    CopySettingsToNative();
                else
                    reference = Native.CreateNote(parent.Reference(), note, velocity, start, end);
            }
        }

        /// <summary>
        /// Tries to delete the native note representation.
        /// </summary>
        public void TryDelete()
        {
            if (FullyNative())
                Native.DeleteNote(parent.Reference(), reference);
            reference = IntPtr.Zero;
        }

        /// <summary>
        /// Checks if this note overlaps a sequencer range.
        /// </summary>
        /// <returns><c>true</c>, if note overlaps the range, <c>false</c> otherwise.</returns>
        /// <param name="rangeStart">Start of the range.</param>
        /// <param name="rangeEnd">End of the range.</param>
        public bool OverlapsRange(float rangeStart, float rangeEnd)
        {
            return Utils.RangesOverlap(start, end, rangeStart, rangeEnd);
        }

        /// <summary>
        /// Checks if this note is contained withing a sequencer range
        /// </summary>
        /// <returns><c>true</c>, if note overlaps the range, <c>false</c> otherwise.</returns>
        /// <param name="rangeStart">Start of the range.</param>
        /// <param name="rangeEnd">End of the range.</param>
        public bool InsideRange(float rangeStart, float rangeEnd)
        {
            return start >= rangeStart && end <= rangeEnd;
        }

        /// <summary>
        /// Removes part of the note on/off range that is contained withing the specified range.
        /// </summary>
        /// <param name="rangeStart">Start of the range to remove.</param>
        /// <param name="rangeEnd">End of the range to remove.</param>
        public void RemoveRange(float rangeStart, float rangeEnd)
        {
            if (!OverlapsRange(rangeStart, rangeEnd))
                return;

            if (start > rangeStart)
                start = rangeEnd;
            else
                end = rangeStart;
        }
    }
}
