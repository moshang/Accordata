// Copyright 2017 Matt Tytel

using UnityEngine;
using System;

namespace AudioHelm
{
    /// <summary>
    /// Part of Helm native synthesizer patch loading.
    /// A single modulation connection is loaded into this class.
    /// </summary>
    [Serializable]
    public class HelmModulationSetting
    {
        public string source;
        public string destination;
        public float amount;
    }

    /// <summary>
    /// Part of Helm native synthesizer patch loading.
    /// The parameters that control how the synth sounds are loaded into this class.
    /// </summary>
    [Serializable]
    public class HelmPatchSettings
    {
        public const int kMaxModulations = 16;
        public const int kMaxCharacters = 15;

        static readonly string[] kModulationSources =
        {
            "aftertouch",
            "amp_envelope",
            "amp_envelope_amp",
            "amp_envelope_phase",
            "fil_envelope",
            "fil_envelope_amp",
            "fil_envelope_phase",
            "mod_envelope",
            "mod_envelope_amp",
            "mod_envelope_phase",
            "mod_wheel",
            "mono_lfo_1",
            "mono_lfo_1_phase",
            "mono_lfo_2",
            "mono_lfo_2_phase",
            "note",
            "peak_meter",
            "pitch_wheel",
            "poly_lfo",
            "poly_lfo_amp",
            "poly_lfo_phase",
            "random",
            "step_sequencer",
            "step_sequencer_step",
            "velocity",
        };

        static readonly string[] kModulationDestinations =
        {
            "amp_attack",
            "amp_decay",
            "amp_release",
            "amp_sustain",
            "arp_frequency",
            "arp_gate",
            "arp_octaves",
            "arp_pattern",
            "arp_tempo",
            "beats_per_minute",
            "cross_modulation",
            "cutoff",
            "delay_dry_wet",
            "delay_feedback",
            "delay_frequency",
            "delay_tempo",
            "distortion_drive",
            "distortion_mix",
            "fil_attack",
            "fil_decay",
            "fil_env_depth",
            "fil_release",
            "fil_sustain",
            "filter_blend",
            "filter_drive",
            "formant_x",
            "formant_y",
            "keytrack",
            "mod_attack",
            "mod_decay",
            "mod_release",
            "mod_sustain",
            "mono_lfo_1_amplitude",
            "mono_lfo_1_frequency",
            "mono_lfo_1_tempo",
            "mono_lfo_1_waveform",
            "mono_lfo_2_amplitude",
            "mono_lfo_2_frequency",
            "mono_lfo_2_tempo",
            "mono_lfo_2_waveform",
            "noise_volume",
            "num_steps",
            "osc_1_transpose",
            "osc_1_tune",
            "osc_1_unison_detune",
            "osc_1_unison_voices",
            "osc_1_volume",
            "osc_1_waveform",
            "osc_2_transpose",
            "osc_2_tune",
            "osc_2_unison_detune",
            "osc_2_unison_voices",
            "osc_2_volume",
            "osc_2_waveform",
            "osc_feedback_amount",
            "osc_feedback_transpose",
            "osc_feedback_tune",
            "pitch_bend_range",
            "poly_lfo_amplitude",
            "poly_lfo_frequency",
            "poly_lfo_tempo",
            "poly_lfo_waveform",
            "polyphony",
            "portamento",
            "resonance",
            "reverb_damping",
            "reverb_dry_wet",
            "reverb_feedback",
            "step_frequency",
            "step_sequencer_tempo",
            "step_smoothing",
            "stutter_frequency",
            "stutter_resample_frequency",
            "stutter_resample_tempo",
            "stutter_softness",
            "stutter_tempo",
            "sub_shuffle",
            "sub_volume",
            "sub_waveform",
            "velocity_track",
            "volume",

            "amp_attack",
            "amp_decay",
            "amp_release",
            "amp_sustain",
            "cross_modulation",
            "cutoff",
            "fil_attack",
            "fil_decay",
            "fil_env_depth",
            "fil_release",
            "fil_sustain",
            "filter_blend",
            "filter_drive",
            "formant_x",
            "formant_y",
            "keytrack",
            "mod_attack",
            "mod_decay",
            "mod_release",
            "mod_sustain",
            "noise_volume",
            "osc_1_transpose",
            "osc_1_tune",
            "osc_1_unison_detune",
            "osc_1_unison_voices",
            "osc_1_volume",
            "osc_1_waveform",
            "osc_2_transpose",
            "osc_2_tune",
            "osc_2_unison_detune",
            "osc_2_unison_voices",
            "osc_2_volume",
            "osc_2_waveform",
            "osc_feedback_amount",
            "osc_feedback_transpose",
            "osc_feedback_tune",
            "pitch_bend_range",
            "poly_lfo_amplitude",
            "poly_lfo_frequency",
            "poly_lfo_tempo",
            "poly_lfo_waveform",
            "portamento",
            "resonance",
            "stutter_frequency",
            "stutter_resample_frequency",
            "stutter_resample_tempo",
            "stutter_softness",
            "stutter_tempo",
            "sub_shuffle",
            "sub_volume",
            "sub_waveform",
            "velocity_track",
        };

        public static readonly string[] kShortenNames = 
        {
            "stutter_resample", "stutter_resamp"
        };

        public float amp_attack;
        public float amp_decay;
        public float amp_release;
        public float amp_sustain;
        public float arp_frequency;
        public float arp_gate;
        public float arp_octaves;
        public float arp_on;
        public float arp_pattern;
        public float arp_sync;
        public float arp_tempo;
        public float beats_per_minute;
        public float cross_modulation;
        public float cutoff;
        public float delay_dry_wet;
        public float delay_feedback;
        public float delay_frequency;
        public float delay_on;
        public float delay_sync;
        public float delay_tempo;
        public float distortion_drive;
        public float distortion_mix;
        public float distortion_on;
        public float distortion_type;
        public float fil_attack;
        public float fil_decay;
        public float fil_env_depth;
        public float fil_release;
        public float fil_sustain;
        public float filter_blend;
        public float filter_drive;
        public float filter_on;
        public float filter_saturation;
        public float filter_shelf;
        public float filter_style;
        public float filter_type;
        public float formant_on;
        public float formant_x;
        public float formant_y;
        public float keytrack;
        public float legato;
        public float mod_attack;
        public float mod_decay;
        public float mod_release;
        public float mod_sustain;
        public float mono_lfo_1_amplitude;
        public float mono_lfo_1_frequency;
        public float mono_lfo_1_retrigger;
        public float mono_lfo_1_sync;
        public float mono_lfo_1_tempo;
        public float mono_lfo_1_waveform;
        public float mono_lfo_2_amplitude;
        public float mono_lfo_2_frequency;
        public float mono_lfo_2_retrigger;
        public float mono_lfo_2_sync;
        public float mono_lfo_2_tempo;
        public float mono_lfo_2_waveform;
        public float noise_volume;
        public float num_steps;
        public float osc_1_transpose;
        public float osc_1_tune;
        public float osc_1_unison_detune;
        public float osc_1_unison_voices;
        public float osc_1_volume;
        public float osc_1_waveform;
        public float osc_2_transpose;
        public float osc_2_tune;
        public float osc_2_unison_detune;
        public float osc_2_unison_voices;
        public float osc_2_volume;
        public float osc_2_waveform;
        public float osc_feedback_amount;
        public float osc_feedback_transpose;
        public float osc_feedback_tune;
        public float osc_mix;
        public float pitch_bend_range;
        public float poly_lfo_amplitude;
        public float poly_lfo_frequency;
        public float poly_lfo_sync;
        public float poly_lfo_tempo;
        public float poly_lfo_waveform;
        public float polyphony;
        public float portamento;
        public float portamento_type;
        public float resonance;
        public float reverb_damping;
        public float reverb_dry_wet;
        public float reverb_feedback;
        public float reverb_on;
        public float step_frequency;
        public float step_seq_00;
        public float step_seq_01;
        public float step_seq_02;
        public float step_seq_03;
        public float step_seq_04;
        public float step_seq_05;
        public float step_seq_06;
        public float step_seq_07;
        public float step_seq_08;
        public float step_seq_09;
        public float step_seq_10;
        public float step_seq_11;
        public float step_seq_12;
        public float step_seq_13;
        public float step_seq_14;
        public float step_seq_15;
        public float step_seq_16;
        public float step_seq_17;
        public float step_seq_18;
        public float step_seq_19;
        public float step_seq_20;
        public float step_seq_21;
        public float step_seq_22;
        public float step_seq_23;
        public float step_seq_24;
        public float step_seq_25;
        public float step_seq_26;
        public float step_seq_27;
        public float step_seq_28;
        public float step_seq_29;
        public float step_seq_30;
        public float step_seq_31;
        public float step_sequencer_retrigger;
        public float step_sequencer_sync;
        public float step_sequencer_tempo;
        public float step_smoothing;
        public float stutter_frequency;
        public float stutter_on;
        public float stutter_resample_frequency;
        public float stutter_resample_sync;
        public float stutter_resample_tempo;
        public float stutter_softness;
        public float stutter_sync;
        public float stutter_tempo;
        public float sub_octave;
        public float sub_shuffle;
        public float sub_volume;
        public float sub_waveform;
        public float unison_1_harmonize;
        public float unison_2_harmonize;
        public float velocity_track;
        public float volume;

        public HelmModulationSetting[] modulations;

        public static string ConvertToPlugin(string name)
        {
            string converted = name;
            for (int i = 0; i < kShortenNames.Length; i += 2)
                converted = converted.Replace(kShortenNames[i], kShortenNames[i + 1]);
            converted = converted.Replace("_", "");
            int length = Mathf.Min(converted.Length, kMaxCharacters);
            return converted.Substring(0, length);
        }

        public static int GetSourceIndex(string source)
        {
            for (int i = 0; i < kModulationSources.Length; ++i)
            {
                if (source == kModulationSources[i])
                    return i;
            }
            return -1;
        }

        public static int GetDestinationIndex(string dest)
        {
            for (int i = 0; i < kModulationDestinations.Length; ++i)
            {
                if (dest == kModulationDestinations[i])
                    return i;
            }
            return -1;
        }
    }

    /// <summary>
    /// Part of Helm native synthesizer patch loading.
    /// All top level settings are loaded into this class.
    /// </summary>
    [Serializable]
    public class HelmPatchFormat
    {
        public string license;
        public string synth_version;
        public string patch_name;
        public string folder_name;
        public string author;
        public HelmPatchSettings settings;
    }
}
