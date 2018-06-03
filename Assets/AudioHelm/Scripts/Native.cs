// Copyright 2017 Matt Tytel

using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace AudioHelm
{
    /// <summary>
    /// The native plugin interface to synthesizer and sequencer settings.
    /// If you want to control a synthesizer, a better was is through the HelmController class.
    /// If you want to modify or setup a sequencer, take a look at HelmSequencer and SampleSequencer.
    /// </summary>
    public static class Native
    {
        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern void HelmNoteOn(int channel, int note, float velocity);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern void HelmNoteOff(int channel, int note);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern void HelmFrequencyOn(int channel, float frequency, float velocity);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern void HelmFrequencyOff(int channel, float frequency);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern void HelmAllNotesOff(int channel);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern void HelmSetPitchWheel(int channel, float value);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern void HelmSetModWheel(int channel, float value);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern void HelmSetAftertouch(int channel, int note, float value);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern void HelmClearModulations(int channel);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern void HelmAddModulation(int channel, int index, string source, string dest, float amount);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern float HelmGetParameterMinimum(int index);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern float HelmGetParameterMaximum(int index);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern float HelmGetParameterValue(int channel, int paramIndex);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern bool HelmSetParameterValue(int channel, int paramIndex, float newValue);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern float HelmGetParameterPercent(int channel, int paramIndex);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern bool HelmSetParameterPercent(int channel, int paramIndex, float newPercent);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern bool HelmSilence(int channel, bool silent);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern bool HelmGetBufferData(int channel, float[] buffer, int samples, int numAudioChannels);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
        [DllImport("AudioPluginHelm")]
        #endif
        public static extern void SetBpm(float bpm);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern IntPtr CreateSequencer();

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern void DeleteSequencer(IntPtr sequencer);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern void EnableSequencer(IntPtr sequencer, bool enable);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern void ChangeSequencerLength(IntPtr sequencer, float length);


        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern void LoopSequencer(IntPtr sequencer, bool loop);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern bool ChangeSequencerChannel(IntPtr sequencer, int channel);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern void SetSequencerStart(IntPtr sequencer, double startBeat);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern IntPtr CreateNote(IntPtr sequencer, int note, float velocity, float start, float end);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern IntPtr DeleteNote(IntPtr sequencer, IntPtr note);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern IntPtr ChangeNoteStart(IntPtr sequencer, IntPtr note, float newStart);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern IntPtr ChangeNoteEnd(IntPtr sequencer, IntPtr note, float newEnd);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern IntPtr ChangeNoteValues(IntPtr sequencer, IntPtr note,
                                                     int newMidiKey, float newStart, float newEnd, float newVelocity);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern IntPtr ChangeNoteKey(IntPtr sequencer, IntPtr note, int key);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern IntPtr ChangeNoteVelocity(IntPtr note, float velocity);

        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern void SetBeatTime(double time);


        #if UNITY_IOS
          [DllImport("__Internal")]
        #else
          [DllImport("AudioPluginHelm")]
        #endif
        public static extern void Pause(bool pause);
    }
}
