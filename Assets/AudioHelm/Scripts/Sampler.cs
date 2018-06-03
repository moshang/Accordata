// Copyright 2017 Matt Tytel

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AudioHelm
{
    /// <summary>
    /// ## [Switch to Manual](../manual/class_audio_helm_1_1_sampler.html)<br>
    /// The Sampler is a type of instrument that has a collection of audio samples to play
    /// and will play them at different rates to change the pitch for different notes.
    /// A list of keyzones define what samples play when what notes are hit.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Audio Helm/Sampler")]
    [HelpURL("http://tytel.org/audiohelm/manual/class_audio_helm_1_1_sampler.html")]
    public class Sampler : MonoBehaviour, NoteHandler
    {
        class ActiveNote
        {
            public int note = 0;
            public List<AudioSource> audioSources;
            public double startTime = 0.0;

            public ActiveNote(int n, List<AudioSource> sources, double start)
            {
                note = n;
                audioSources = sources;
                startTime = start;
            }
        }

        /// <summary>
        /// List of all the keyzones in the sampler.
        /// </summary>
        public List<Keyzone> keyzones = new List<Keyzone>() { new Keyzone() };

        /// <summary>
        /// How much the velocity of a note on event affects the volume of the samples.
        /// 0.0 for no effect and 1.0 for full effect.
        /// </summary>
        [Tooltip("How much the velocity of a note on event affects the volume of the samples. " +
                 "0.0 for no effect and 1.0 for full effect")]
        public float velocityTracking = 1.0f;

        /// <summary>
        /// Total number of concurrently playing sounds from this Sampler (polyphony).
        /// Increase this if your voices are cutting out from new voices coming in.
        /// </summary>
        [Tooltip("Total number of concurrently playing sounds from this Sampler (polyphony). " +
                 "Increase this if your voices are cutting out unexpectedly.")]
        public int numVoices = 8;

        [Tooltip("Does a voice silence when it gets a note off event?")]
        [SerializeField]
        private bool useNoteOff_;
        /// <summary>
        /// Does a voice silence when it gets a note off event?
        /// </summary>
        public bool useNoteOff
        {
            get
            {
                return useNoteOff_;
            }
            set
            {
                useNoteOff_ = value;
                if (useNoteOff_)
                    AllNotesOff();
            }
        }

        int audioIndex = 0;
        readonly List<ActiveNote> activeNotes = new List<ActiveNote>();

        // We end sample early to prevent click at end of sample caused by Unity pitch change.
        const double endEarlyTime = 0.01;

        void Awake()
        {
            AllNotesOff();

            AudioSource[] audios = GetComponents<AudioSource>();
            int voicesToAdd = numVoices - audios.Length;
            int originalIndex = 0;
            for (int i = 0; i < voicesToAdd; ++i)
            {
                Utils.CopyComponent(audios[originalIndex], gameObject);
                originalIndex = (originalIndex + 1) % audios.Length;
            }
        }

        void OnDestroy()
        {
            AllNotesOff();
        }

        void OnDisable()
        {
            AllNotesOff();
        }

        /// <summary>
        /// Adds an empty keyzone to the Sampler.
        /// </summary>
        /// <returns>The keyzone created.</returns>
        public Keyzone AddKeyzone()
        {
            Keyzone keyzone = new Keyzone();
            keyzones.Add(keyzone);
            return keyzone;
        }

        /// <summary>
        /// Removes a keyzone from the Sampler.
        /// </summary>
        /// <returns>The removed keyzones index. -1 if it doesnt exist.</returns>
        /// <param name="keyzone">The keyzone to remove.</param>
        public int RemoveKeyzone(Keyzone keyzone)
        {
            int index = keyzones.IndexOf(keyzone);
            keyzones.Remove(keyzone);
            return index;
        }

        AudioSource GetNextAudioSource()
        {
            AudioSource[] audios = GetComponents<AudioSource>();
            foreach (AudioSource audioSource in audios)
            {
                if (!audioSource.isPlaying)
                    return audioSource;
            }
            audioIndex = (audioIndex + 1) % audios.Length;
            return audios[audioIndex];
        }

        void PrepNote(AudioSource audioSource, int note, float velocity)
        {
            audioSource.pitch = Utils.MidiChangeToRatio(note - Utils.kMiddleC);
            audioSource.volume = Mathf.Lerp(1.0f - velocityTracking, 1.0f, velocity);
        }

        void PrepNote(AudioSource audioSource, Keyzone keyzone, int note, float velocity)
        {
            audioSource.pitch = Utils.MidiChangeToRatio(note - keyzone.rootKey);
            audioSource.clip = keyzone.audioClip;
            audioSource.outputAudioMixerGroup = keyzone.mixer;
            audioSource.volume = Mathf.Lerp(1.0f - velocityTracking, 1.0f, velocity);
        }

        List<Keyzone> GetValidKeyzones(int note, float velocity = 1.0f)
        {
            List<Keyzone> validKeyzones = new List<Keyzone>();
            foreach (Keyzone keyzone in keyzones)
            {
                if (keyzone.ValidForNote(note, velocity))
                    validKeyzones.Add(keyzone);
            }
            return validKeyzones;
        }

        List<AudioSource> GetPreppedAudioSources(int note, float velocity)
        {
            List<AudioSource> audioSources = new List<AudioSource>();
            List<Keyzone> validKeyzones = GetValidKeyzones(note, velocity);
            foreach (Keyzone keyzone in validKeyzones)
            {
                AudioSource audioSource = GetNextAudioSource();
                PrepNote(audioSource, keyzone, note, velocity);
                audioSources.Add(audioSource);
            }
            return audioSources;
        }

        /// <summary>
        /// Gets the lowest midi key that the sampler responds to.
        /// </summary>
        /// <returns>The lowest valid midi key.</returns>
        public int GetMinKey()
        {
            if (keyzones.Count == 0)
                return 0;

            int min = Utils.kMidiSize;
            foreach (Keyzone keyzone in keyzones)
                min = Mathf.Min(keyzone.minKey, min);

            return min;
        }

        /// <summary>
        /// Gets the highest midi key that the sampler responds to.
        /// </summary>
        /// <returns>The highest valid midi key.</returns>
        public int GetMaxKey()
        {
            if (keyzones.Count == 0)
                return Utils.kMidiSize - 1;

            int max = 0;
            foreach (Keyzone keyzone in keyzones)
                max = Mathf.Max(keyzone.maxKey, max);

            return max;
        }

        /// <summary>
        /// Triggers note off events for all notes currently on in the sampler.
        /// </summary>
        public void AllNotesOff()
        {
            AudioSource[] audios = GetComponents<AudioSource>();
            foreach (AudioSource audioSource in audios)
                audioSource.Stop();

            activeNotes.Clear();
        }

        IEnumerator TurnVoiceOffInSeconds(int note, float seconds)
        {
            yield return new WaitForSeconds(seconds);

            DoNoteOff(note);
        }

        /// <summary>
        /// Triggers a note on event for the Sampler.
        /// If the AudioSource is set to loop, you must trigger a note off event
        /// later for this note by calling NoteOff.
        /// </summary>
        /// <param name="note">The MIDI keyboard note to play. [0, 127]</param>
        /// <param name="velocity">How hard you hit the key. [0.0, 1.0]</param>
        public void NoteOn(int note, float velocity = 1.0f)
        {
            List<AudioSource> audioSources = GetPreppedAudioSources(note, velocity);
            activeNotes.Add(new ActiveNote(note, audioSources, AudioSettings.dspTime));
            foreach (AudioSource audioSource in audioSources)
            {
                if (audioSource.isActiveAndEnabled)
                {
                    audioSource.Play();
                    if (!audioSource.loop)
                    {
                        double length = (audioSource.clip.length - endEarlyTime) / audioSource.pitch;
                        audioSource.SetScheduledEndTime(AudioSettings.dspTime + length);
                    }
                }
            }
        }

        /// <summary>
        /// Triggers a note on event for the Sampler at the givent time and turns it off at the given time.
        /// </summary>
        /// <param name="note">The MIDI keyboard note to play. [0, 127]</param>
        /// <param name="velocity">How hard you hit the key. [0.0, 1.0]</param>
        /// <param name="timeToStart">DSP time to start the note.</param>
        /// <param name="timeToEnd">DSP time to end the note.</param>
        public void NoteOnScheduled(int note, float velocity, double timeToStart, double timeToEnd)
        {
            List<AudioSource> audioSources = GetPreppedAudioSources(note, velocity);
            activeNotes.Add(new ActiveNote(note, audioSources, timeToStart));

            double length = timeToEnd - timeToStart;
            if (!useNoteOff)
                length = Mathf.Infinity;

            foreach (AudioSource audioSource in audioSources)
            {
                if (!audioSource.loop)
                    length = Math.Min(length, (audioSource.clip.length - endEarlyTime) / audioSource.pitch);

                audioSource.PlayScheduled(timeToStart);
                if (!useNoteOff)
                    audioSource.SetScheduledEndTime(timeToStart + length);
            }

            if (useNoteOff)
                StartCoroutine(TurnVoiceOffInSeconds(note, (float)(timeToStart + length)));
        }

        ActiveNote FindActiveNote(int note)
        {
            foreach (ActiveNote activeNote in activeNotes)
            {
                if (note == activeNote.note)
                    return activeNote;
            }
            return null;
        }

        /// <summary>
        /// Triggers a note off event for the Sampler.
        /// </summary>
        /// <param name="note">The MIDI keyboard note to turn off. [0, 127]</param>
        public void NoteOff(int note)
        {
            if (!useNoteOff)
                return;

            DoNoteOff(note);
        }

        void DoNoteOff(int note)
        {
            ActiveNote activeNote = FindActiveNote(note);

            if (activeNote == null || AudioSettings.dspTime < activeNote.startTime)
                return;

            activeNotes.Remove(activeNote);
            foreach (AudioSource audioSource in activeNote.audioSources)
                audioSource.volume = 0.0f;
        }
    }
}
