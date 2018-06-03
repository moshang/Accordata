// Copyright 2017 Matt Tytel

using UnityEngine;
using System;
using System.IO;
using System.Reflection;

namespace AudioHelm
{
    /// <summary>
    /// The most commonly set Helm native synthesizer parameters.
    /// Placed here for easy access.
    /// </summary>
    public enum CommonParam
    {
        kNone = 0,

        // Filter
        kFilterCutoff = Param.kFilterCutoff,
        kResonance = Param.kResonance,
        kFilterAttack = Param.kFilterAttack,
        kFilterRelease = Param.kFilterRelease,
        kFilterEnvelopeDepth = Param.kFilterEnvelopeDepth,

        // Formant
        kFormantX = Param.kFormantX,
        kFormantY = Param.kFormantY,

        // Amplitude
        kAmplitudeAttack = Param.kAmplitudeAttack,
        kAmplitudeRelease = Param.kAmplitudeRelease,

        // Oscillators
        kCrossMod = Param.kCrossMod,
        kOsc1Tune = Param.kOsc1Tune,
        kOsc2Tune = Param.kOsc2Tune,
        kOsc1Transpose = Param.kOsc1Transpose,
        kOsc2Transpose = Param.kOsc2Transpose,
        kOsc1UnisonDetune = Param.kOsc1UnisonDetune,
        kOsc2UnisonDetune = Param.kOsc2UnisonDetune,

        // Delay
        kDelayFrequency = Param.kDelayFrequency,
        kDelayTempo = Param.kDelayTempo,
    }

    /// <summary>
    /// All Helm native synthesizer parameters.
    /// </summary>
    public enum Param
    {
        kNone = 0,
        kAmplitudeAttack = 1,
        kAmplitudeDecay,
        kAmplitudeSustain,
        kAmplitudeRelease,
        kArpFrequency,
        kArpGate,
        kArpOctaves,
        kArpOn,
        kArpPattern,
        kArpSync,
        kArpTempo,
        kCrossMod = 13,
        kFilterCutoff,
        kDelayDryWet,
        kDelayFeedback,
        kDelayFrequency,
        kDelayOn,
        kDelaySync,
        kDelayTempo,
        kDistortionOn,
        kDistortionType,
        kDistortionDrive,
        kDistortionMix,
        kFilterAttack,
        kFilterDecay,
        kFilterEnvelopeDepth,
        kFilterRelease,
        kFilterSustain,
        kFilterDrive = 31,
        kFilterBlend,
        kFilterStyle = 34,
        kFilterShelf,
        kFilterOn,
        kFormantOn,
        kFormantX,
        kFormantY,
        kKeytrack,
        kLegato,
        kModAttack,
        kModDecay,
        kModRelease,
        kModSustain,
        kMonoLfo1Amplitude,
        kMonoLfo1Frequency,
        kMonoLfo1Retrigger,
        kMonoLfo1Sync,
        kMonoLfo1Tempo,
        kMonoLfo1Waveform,
        kMonoLfo2Amplitude,
        kMonoLfo2Frequency,
        kMonoLfo2Retrigger,
        kMonoLfo2Sync,
        kMonoLfo2Tempo,
        kMonoLfo2Waveform,
        kNoiseVolume,
        kNumSteps,
        kOsc1Transpose,
        kOsc1Tune,
        kOsc1UnisonDetune,
        kOsc1UnisonVoices,
        kOsc1Volume,
        kOsc1Waveform,
        kOsc2Transpose,
        kOsc2Tune,
        kOsc2UnisonDetune,
        kOsc2UnisonVoices,
        kOsc2Volume,
        kOsc2Waveform,
        kOscFeedbackAmount,
        kOscFeedbackTranspose,
        kOscFeedbackTune,
        kPitchBendRange = 76,
        kPolyLfoAmplitude,
        kPolyLfoFrequency,
        kPolyLfoSync,
        kPolyLfoTempo,
        kPolyLfoWaveform,
        kPolyphony,
        kPortamento,
        kPortamentoType,
        kResonance,
        kReverbDamping,
        kReverbDryWet,
        kReverbFeedback,
        kReverbOn,
        kStepFrequency,
        kStepSequencerRetrigger = 123,
        kStepSequencerSync,
        kStepSequencerTempo,
        kStepSmoothing,
        kStutterFrequency,
        kStutterOn,
        kStutterResampleFrequency,
        kStutterResampleSync,
        kStutterResampleTempo,
        kStutterSoftness,
        kStutterSync,
        kStutterTempo,
        kSubShuffle,
        kSubOctave,
        kSubVolume,
        kSubWaveform,
        kOsc1UnisonHarmonize,
        kOsc2UnisonHarmonize,
        kVelocityTrack,
        kVolume
    }

    /// <summary>
    /// Utility functions that are useful for audio/MIDI/music.
    /// </summary>
    public static class Utils
    {
        public const int kMidiSize = 128;
        public const int kNotesPerOctave = 12;
        public const int kMaxChannels = 16;
        public const float kBpmToSixteenths = 4.0f / 60.0f;
        public const double kSecondsPerMinute = 60.0;
        public const double kSixteenthsPerBeat = 4.0;
        public const int kMinOctave = -2;
        public const int kMiddleC = 60;

        static bool[] blackKeys = { false, true, false, true,
                                    false, false, true, false,
                                    true, false, true, false };

        /// <summary>
        /// Checks if a given MIDI key is a black key.
        /// </summary>
        /// <returns><c>true</c>, if the key is a black key, <c>false</c> otherwise.</returns>
        /// <param name="key">Key.</param>
        public static bool IsBlackKey(int key)
        {
            return blackKeys[key % kNotesPerOctave];
        }

        /// <summary>
        /// Checks if a given MIDI key is a 'C' note.
        /// </summary>
        /// <returns><c>true</c>, if the key is a black key, <c>false</c> otherwise.</returns>
        /// <param name="key">Key.</param>
        public static bool IsC(int key)
        {
            return key % kNotesPerOctave == 0;
        }

        /// <summary>
        /// Gets the keyboard octave of a given key.
        /// </summary>
        /// <returns>The octave of the given key.</returns>
        /// <param name="key">The key to check the octave of.</param>
        public static int GetOctave(int key)
        {
            return key / kNotesPerOctave + kMinOctave;
        }

        /// <summary>
        /// Takes two notes midi semitones apart and returns the ratio of the frequencies.
        /// </summary>
        /// <returns>The ratio of the two notes.</returns>
        /// <param name="midi">Number of semitones changed.</param>
        public static float MidiChangeToRatio(int midi)
        {
            return Mathf.Pow(2, (1.0f * midi) / kNotesPerOctave);
        }

        /// <summary>
        /// Check if two float ranges overlap.
        /// </summary>
        /// <returns><c>true</c>, if the ranges overlap, <c>false</c> otherwise.</returns>
        /// <param name="start">Start of range 1.</param>
        /// <param name="end">End of Range 1.</param>
        /// <param name="rangeStart">Start of range 2.</param>
        /// <param name="rangeEnd">End of range 2.</param>
        public static bool RangesOverlap(float start, float end, float rangeStart, float rangeEnd)
        {
            return !(start < rangeStart && end <= rangeStart) &&
                   !(start >= rangeEnd && end > rangeEnd);
        }

        /// <summary>
        /// Sets up an AudioSource for playing to a Helm Native instance.
        /// </summary>
        /// <param name="audio">The AudioSource to initialize.</param>
        public static void InitAudioSource(AudioSource audio)
        {
            AudioClip one = AudioClip.Create("helm", 1, 1, AudioSettings.outputSampleRate, false);
            one.SetData(new float[] { 1 }, 0);

            audio.clip = one;
            audio.loop = true;
            if (Application.isPlaying)
                audio.Play();
        }

        /// <summary>
        /// Copies all properties and fields of one Component to another GameObject.
        /// </summary>
        /// <returns>The instantiated and copied Component.</returns>
        /// <param name="original">The original Component to be copied.</param>
        /// <param name="destination">The GameObject to put the copied Component on.</param>
        /// <typeparam name="T">The type of Component.</typeparam>
        public static T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            System.Type type = original.GetType();
            Component copy = destination.AddComponent(type);

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;
            PropertyInfo[] properties = type.GetProperties(flags);
            foreach (PropertyInfo property in properties)
            {
                if (property.CanWrite && !property.IsDefined(typeof(ObsoleteAttribute), true))
                {
                    try
                    {
                        property.SetValue(copy, property.GetValue(original, null), null);
                    }
                    catch { }
                }
            }
            FieldInfo[] fields = type.GetFields(flags);
            foreach (FieldInfo field in fields)
                field.SetValue(copy, field.GetValue(original));

            return copy as T;
        }
    }
}
