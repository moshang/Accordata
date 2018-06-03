// Copyright 2017 Matt Tytel

namespace AudioHelm
{
    /// <summary>
    /// An interface for classes that can receive note on and off events.
    /// </summary>
    public interface NoteHandler
    {
        void AllNotesOff();
        void NoteOn(int note, float velocity = 1.0f);
        void NoteOff(int note);
    }
}
