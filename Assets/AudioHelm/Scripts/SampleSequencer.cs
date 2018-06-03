// Copyright 2017 Matt Tytel

using UnityEngine;
using System.Collections.Generic;

namespace AudioHelm
{
    /// <summary>
    /// ## [Switch to Manual](../manual/class_audio_helm_1_1_sample_sequencer.html)<br>
    /// A sequencer of notes over time that will send its note on/off events to
    /// a Sampler instance that is attatched to the same object.
    /// </summary>
    [RequireComponent(typeof(Sampler))]
    [AddComponentMenu("Audio Helm/Sample Sequencer")]
    [HelpURL("http://tytel.org/audiohelm/manual/class_audio_helm_1_1_sample_sequencer.html")]
    public class SampleSequencer : Sequencer
    {
        double lastWindowTime = -0.01;
        int numCycles = 0;
        bool waitTillNextCycle = false;

        const float lookaheadTime = 0.1f;

        void Awake()
        {
            numCycles = -1;
            InitNoteRows();
            AllNotesOff();
        }

        void Start()
        {
            UpdateBeatTime();
            double position = GetSequencerTime();
            lastWindowTime = position + lookaheadTime / GetSixteenthTime();
        }

        void OnDestroy()
        {
            AllNotesOff();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            double position = GetSequencerTime();
            lastWindowTime = position + lookaheadTime / GetSixteenthTime();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            AllNotesOff();
            waitTillNextCycle = false;
        }

        /// <summary>
        /// Triggers note off events for all notes currently on in the referenced Sampler.
        /// </summary>
        public override void AllNotesOff()
        {
            GetComponent<Sampler>().AllNotesOff();
            base.AllNotesOff();
        }

        /// <summary>
        /// Triggers a note on event for the referenced sampler.
        /// If the AudioSource is set to loop, you must trigger a note off event
        /// later for this note by calling NoteOff.
        /// </summary>
        /// <param name="note">The MIDI keyboard note to play. [0, 127]</param>
        /// <param name="velocity">How hard you hit the key. [0.0, 1.0]</param>
        public override void NoteOn(int note, float velocity = 1.0f)
        {
            GetComponent<Sampler>().NoteOn(note, velocity);
        }

        /// <summary>
        /// Triggers a note off event for the referenced Sampler.
        /// </summary>
        /// <param name="note">The MIDI keyboard note to turn off. [0, 127]</param>
        public override void NoteOff(int note)
        {
            GetComponent<Sampler>().NoteOff(note);
        }

        void EnableComponent()
        {
            enabled = true;
        }

        public override void StartOnNextCycle()
        {
            enabled = true;
            waitTillNextCycle = true;
            numCycles = (int)(GetSequencerTime() / length);
        }

        void FixedUpdate()
        {
            DoUpdate();
        }

        void DoUpdate()
        {
            double updateStartTime = AudioSettings.dspTime;
            UpdatePosition();

            if (AudioHelmClock.GetGlobalPause())
                return;
            
            double position = GetSequencerTime();
            float sixteenthTime = GetSixteenthTime();
            double currentTime = GetSequencerPosition() * sixteenthTime;
            double sequencerTime = length * sixteenthTime;

            double windowMax = position + lookaheadTime / sixteenthTime;
            if (windowMax <= lastWindowTime)
            {
                lastWindowTime = windowMax;
                return;
            }

            int cycles = (int)(windowMax / length);

            if (cycles > numCycles)
            {
                numCycles = cycles;
                waitTillNextCycle = false;
            }

            if (waitTillNextCycle)
            {
                lastWindowTime = windowMax;
                return;
            }

            double internalLastWindowMax = Mathf.Repeat((float)lastWindowTime, length);
            double internalWindowMax = Mathf.Repeat((float)windowMax, length);

            float startSearch = (float)internalLastWindowMax;
            float endSearch = (float)internalWindowMax;
            List<Note> notes = GetAllNoteOnsInRange(startSearch, endSearch);

            foreach (Note note in notes)
            {
                double startTime = sixteenthTime * note.start;

                // Check if we wrapped around.
                if (startTime < currentTime)
                    startTime += sequencerTime;

                double endTime = startTime + sixteenthTime * (note.end - note.start);

                double timeToStart = startTime - currentTime + updateStartTime;
                double timeToEnd = endTime - currentTime + updateStartTime;
                GetComponent<Sampler>().NoteOnScheduled(note.note, note.velocity, timeToStart, timeToEnd);
            }

            lastWindowTime = windowMax;
        }
    }
}
