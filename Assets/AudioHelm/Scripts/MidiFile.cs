// Copyright 2017 Matt Tytel

using UnityEngine;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using Sanford.Multimedia.Midi;
#endif

namespace AudioHelm
{
    public class MidiFile : MonoBehaviour
    {
        [System.Serializable]
        public class MidiData
        {
            public int length = 1;
            public List<Note> notes = new List<Note>();
        }

        public Object midiObject;
        public MidiData midiData;

        public void LoadMidiData(string filePath)
        {
            FileStream midiStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            midiData = LoadMidiData(midiStream);
        }

        public static MidiData LoadMidiData(Stream midiStream)
        {
            #if UNITY_EDITOR
            return ReadMidiFile(midiStream);
            #else
            return new MidiData();
            #endif
        }

        #if UNITY_EDITOR
        static MidiData ReadMidiFile(Stream midiStream)
        {
            MidiData midiData = new MidiData();
            Sequence midiSequence = new Sequence(midiStream);
            midiData.length = 4 * midiSequence.GetLength() / midiSequence.Division;

            foreach (Track midiTrack in midiSequence)
                ReadMidiTrack(midiData, midiTrack, midiSequence.Division);

            return midiData;
        }

        static void ReadMidiTrack(MidiData midiData, Track midiTrack, int sequencerDivision)
        {
            Dictionary<int, float> noteTimes = new Dictionary<int, float>();
            Dictionary<int, float> noteVelocities = new Dictionary<int, float>();
            for (int i = 0; i < midiTrack.Count; ++i)
            {
                MidiEvent midiEvent = midiTrack.GetMidiEvent(i);
                if (midiEvent.MidiMessage.GetBytes().Length < 3)
                    continue;

                byte midiType = (byte)(midiEvent.MidiMessage.GetBytes()[0] & 0xFF);
                byte note = (byte)(midiEvent.MidiMessage.GetBytes()[1] & 0xFF);
                byte velocity = (byte)(midiEvent.MidiMessage.GetBytes()[2] & 0xFF);
                float time = (4.0f * midiEvent.AbsoluteTicks) / sequencerDivision;

                if (midiType == (byte)ChannelCommand.NoteOff ||
                    (midiType == (byte)ChannelCommand.NoteOn) && velocity == 0)
                {
                    if (noteTimes.ContainsKey(note))
                    {
                        Note noteObject = new Note();
                        noteObject.note = note;
                        noteObject.start = noteTimes[note];
                        noteObject.end = time;
                        noteObject.velocity = noteVelocities[note];
                        midiData.notes.Add(noteObject);

                        noteTimes.Remove(note);
                        noteVelocities.Remove(note);
                    }
                }
                else if (midiType == (byte)ChannelCommand.NoteOn)
                {
                    noteTimes[note] = time;
                    noteVelocities[note] = Mathf.Min(1.0f, velocity / 127.0f);
                }
            }
        }
        #endif
    }
}
