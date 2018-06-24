/**
 * Copyright (c) 2018 Enzien Audio, Ltd.
 * 
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice,
 *    this list of conditions, and the following disclaimer.
 * 
 * 2. Redistributions in binary form must reproduce the phrase "powered by heavy",
 *    the heavy logo, and a hyperlink to https://enzienaudio.com, all in a visible
 *    form.
 * 
 *   2.1 If the Application is distributed in a store system (for example,
 *       the Apple "App Store" or "Google Play"), the phrase "powered by heavy"
 *       shall be included in the app description or the copyright text as well as
 *       the in the app itself. The heavy logo will shall be visible in the app
 *       itself as well.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using AOT;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Hv_AccordataSynth_AudioLib))]
public class Hv_AccordataSynth_Editor : Editor {

  [MenuItem("Heavy/AccordataSynth")]
  static void CreateHv_AccordataSynth() {
    GameObject target = Selection.activeGameObject;
    if (target != null) {
      target.AddComponent<Hv_AccordataSynth_AudioLib>();
    }
  }
  
  private Hv_AccordataSynth_AudioLib _dsp;

  private void OnEnable() {
    _dsp = target as Hv_AccordataSynth_AudioLib;
  }

  public override void OnInspectorGUI() {
    bool isEnabled = _dsp.IsInstantiated();
    if (!isEnabled) {
      EditorGUILayout.LabelField("Press Play!",  EditorStyles.centeredGreyMiniLabel);
    }
    // events
    GUI.enabled = isEnabled;
    EditorGUILayout.Space();
    // lfo1_reset
    if (GUILayout.Button("lfo1_reset")) {
      _dsp.SendEvent(Hv_AccordataSynth_AudioLib.Event.Lfo1_reset);
    }
    
    // lfo2_reset
    if (GUILayout.Button("lfo2_reset")) {
      _dsp.SendEvent(Hv_AccordataSynth_AudioLib.Event.Lfo2_reset);
    }
    
    // start
    if (GUILayout.Button("start")) {
      _dsp.SendEvent(Hv_AccordataSynth_AudioLib.Event.Start);
    }
    
    // stop
    if (GUILayout.Button("stop")) {
      _dsp.SendEvent(Hv_AccordataSynth_AudioLib.Event.Stop);
    }
    
    // testrigger
    if (GUILayout.Button("testrigger")) {
      _dsp.SendEvent(Hv_AccordataSynth_AudioLib.Event.Testrigger);
    }
    
    GUILayout.EndVertical();

    // parameters
    GUI.enabled = true;
    GUILayout.BeginVertical();
    EditorGUILayout.Space();
    EditorGUI.indentLevel++;
    
    // adsr_attack
    GUILayout.BeginHorizontal();
    float adsr_attack = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Adsr_attack);
    float newAdsr_attack = EditorGUILayout.Slider("adsr_attack", adsr_attack, 10.0f, 10010.0f);
    if (adsr_attack != newAdsr_attack) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Adsr_attack, newAdsr_attack);
    }
    GUILayout.EndHorizontal();
    
    // adsr_decay
    GUILayout.BeginHorizontal();
    float adsr_decay = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Adsr_decay);
    float newAdsr_decay = EditorGUILayout.Slider("adsr_decay", adsr_decay, 10.0f, 10030.0f);
    if (adsr_decay != newAdsr_decay) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Adsr_decay, newAdsr_decay);
    }
    GUILayout.EndHorizontal();
    
    // adsr_release
    GUILayout.BeginHorizontal();
    float adsr_release = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Adsr_release);
    float newAdsr_release = EditorGUILayout.Slider("adsr_release", adsr_release, 10.0f, 10010.0f);
    if (adsr_release != newAdsr_release) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Adsr_release, newAdsr_release);
    }
    GUILayout.EndHorizontal();
    
    // adsr_sustain
    GUILayout.BeginHorizontal();
    float adsr_sustain = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Adsr_sustain);
    float newAdsr_sustain = EditorGUILayout.Slider("adsr_sustain", adsr_sustain, 0.1f, 1.1f);
    if (adsr_sustain != newAdsr_sustain) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Adsr_sustain, newAdsr_sustain);
    }
    GUILayout.EndHorizontal();
    
    // amp_lfo1_depth
    GUILayout.BeginHorizontal();
    float amp_lfo1_depth = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Amp_lfo1_depth);
    float newAmp_lfo1_depth = EditorGUILayout.Slider("amp_lfo1_depth", amp_lfo1_depth, 0.0f, 0.5f);
    if (amp_lfo1_depth != newAmp_lfo1_depth) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Amp_lfo1_depth, newAmp_lfo1_depth);
    }
    GUILayout.EndHorizontal();
    
    // amp_lfo2_depth
    GUILayout.BeginHorizontal();
    float amp_lfo2_depth = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Amp_lfo2_depth);
    float newAmp_lfo2_depth = EditorGUILayout.Slider("amp_lfo2_depth", amp_lfo2_depth, 0.0f, 0.5f);
    if (amp_lfo2_depth != newAmp_lfo2_depth) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Amp_lfo2_depth, newAmp_lfo2_depth);
    }
    GUILayout.EndHorizontal();
    
    // bpm
    GUILayout.BeginHorizontal();
    float bpm = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Bpm);
    float newBpm = EditorGUILayout.Slider("bpm", bpm, 40.0f, 200.0f);
    if (bpm != newBpm) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Bpm, newBpm);
    }
    GUILayout.EndHorizontal();
    
    // delayFeedback
    GUILayout.BeginHorizontal();
    float delayFeedback = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Delayfeedback);
    float newDelayfeedback = EditorGUILayout.Slider("delayFeedback", delayFeedback, 0.0f, 0.6f);
    if (delayFeedback != newDelayfeedback) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Delayfeedback, newDelayfeedback);
    }
    GUILayout.EndHorizontal();
    
    // delayLevel
    GUILayout.BeginHorizontal();
    float delayLevel = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Delaylevel);
    float newDelaylevel = EditorGUILayout.Slider("delayLevel", delayLevel, 0.0f, 1.0f);
    if (delayLevel != newDelaylevel) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Delaylevel, newDelaylevel);
    }
    GUILayout.EndHorizontal();
    
    // delayTimeMS
    GUILayout.BeginHorizontal();
    float delayTimeMS = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Delaytimems);
    float newDelaytimems = EditorGUILayout.Slider("delayTimeMS", delayTimeMS, 10.0f, 2000.0f);
    if (delayTimeMS != newDelaytimems) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Delaytimems, newDelaytimems);
    }
    GUILayout.EndHorizontal();
    
    // flangerDepth
    GUILayout.BeginHorizontal();
    float flangerDepth = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Flangerdepth);
    float newFlangerdepth = EditorGUILayout.Slider("flangerDepth", flangerDepth, 0.0f, 1.0f);
    if (flangerDepth != newFlangerdepth) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Flangerdepth, newFlangerdepth);
    }
    GUILayout.EndHorizontal();
    
    // flangerFeedback
    GUILayout.BeginHorizontal();
    float flangerFeedback = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Flangerfeedback);
    float newFlangerfeedback = EditorGUILayout.Slider("flangerFeedback", flangerFeedback, 0.5f, 0.99f);
    if (flangerFeedback != newFlangerfeedback) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Flangerfeedback, newFlangerfeedback);
    }
    GUILayout.EndHorizontal();
    
    // flangerMix
    GUILayout.BeginHorizontal();
    float flangerMix = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Flangermix);
    float newFlangermix = EditorGUILayout.Slider("flangerMix", flangerMix, 0.0f, 1.0f);
    if (flangerMix != newFlangermix) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Flangermix, newFlangermix);
    }
    GUILayout.EndHorizontal();
    
    // flangerSpeed
    GUILayout.BeginHorizontal();
    float flangerSpeed = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Flangerspeed);
    float newFlangerspeed = EditorGUILayout.Slider("flangerSpeed", flangerSpeed, 0.5f, 20.0f);
    if (flangerSpeed != newFlangerspeed) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Flangerspeed, newFlangerspeed);
    }
    GUILayout.EndHorizontal();
    
    // lfo1Retrig
    GUILayout.BeginHorizontal();
    float lfo1Retrig = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lfo1retrig);
    float newLfo1retrig = EditorGUILayout.Slider("lfo1Retrig", lfo1Retrig, 0.0f, 1.0f);
    if (lfo1Retrig != newLfo1retrig) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lfo1retrig, newLfo1retrig);
    }
    GUILayout.EndHorizontal();
    
    // lfo1_rate
    GUILayout.BeginHorizontal();
    float lfo1_rate = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lfo1_rate);
    float newLfo1_rate = EditorGUILayout.Slider("lfo1_rate", lfo1_rate, 0.1f, 20.0f);
    if (lfo1_rate != newLfo1_rate) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lfo1_rate, newLfo1_rate);
    }
    GUILayout.EndHorizontal();
    
    // lfo1_wave
    GUILayout.BeginHorizontal();
    float lfo1_wave = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lfo1_wave);
    float newLfo1_wave = EditorGUILayout.Slider("lfo1_wave", lfo1_wave, 1.0f, 8.0f);
    if (lfo1_wave != newLfo1_wave) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lfo1_wave, newLfo1_wave);
    }
    GUILayout.EndHorizontal();
    
    // lfo2Retrig
    GUILayout.BeginHorizontal();
    float lfo2Retrig = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lfo2retrig);
    float newLfo2retrig = EditorGUILayout.Slider("lfo2Retrig", lfo2Retrig, 0.0f, 1.0f);
    if (lfo2Retrig != newLfo2retrig) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lfo2retrig, newLfo2retrig);
    }
    GUILayout.EndHorizontal();
    
    // lfo2_rate
    GUILayout.BeginHorizontal();
    float lfo2_rate = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lfo2_rate);
    float newLfo2_rate = EditorGUILayout.Slider("lfo2_rate", lfo2_rate, 0.1f, 20.0f);
    if (lfo2_rate != newLfo2_rate) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lfo2_rate, newLfo2_rate);
    }
    GUILayout.EndHorizontal();
    
    // lfo2_wave
    GUILayout.BeginHorizontal();
    float lfo2_wave = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lfo2_wave);
    float newLfo2_wave = EditorGUILayout.Slider("lfo2_wave", lfo2_wave, 1.0f, 8.0f);
    if (lfo2_wave != newLfo2_wave) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lfo2_wave, newLfo2_wave);
    }
    GUILayout.EndHorizontal();
    
    // lpf_freq
    GUILayout.BeginHorizontal();
    float lpf_freq = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lpf_freq);
    float newLpf_freq = EditorGUILayout.Slider("lpf_freq", lpf_freq, 0.0f, 8000.0f);
    if (lpf_freq != newLpf_freq) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lpf_freq, newLpf_freq);
    }
    GUILayout.EndHorizontal();
    
    // lpf_lfo1_depth
    GUILayout.BeginHorizontal();
    float lpf_lfo1_depth = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lpf_lfo1_depth);
    float newLpf_lfo1_depth = EditorGUILayout.Slider("lpf_lfo1_depth", lpf_lfo1_depth, 0.0f, 0.5f);
    if (lpf_lfo1_depth != newLpf_lfo1_depth) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lpf_lfo1_depth, newLpf_lfo1_depth);
    }
    GUILayout.EndHorizontal();
    
    // lpf_lfo2_depth
    GUILayout.BeginHorizontal();
    float lpf_lfo2_depth = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lpf_lfo2_depth);
    float newLpf_lfo2_depth = EditorGUILayout.Slider("lpf_lfo2_depth", lpf_lfo2_depth, 0.0f, 0.5f);
    if (lpf_lfo2_depth != newLpf_lfo2_depth) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lpf_lfo2_depth, newLpf_lfo2_depth);
    }
    GUILayout.EndHorizontal();
    
    // lpf_q
    GUILayout.BeginHorizontal();
    float lpf_q = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lpf_q);
    float newLpf_q = EditorGUILayout.Slider("lpf_q", lpf_q, 0.0f, 10.0f);
    if (lpf_q != newLpf_q) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Lpf_q, newLpf_q);
    }
    GUILayout.EndHorizontal();
    
    // noteNum
    GUILayout.BeginHorizontal();
    float noteNum = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Notenum);
    float newNotenum = EditorGUILayout.Slider("noteNum", noteNum, 0.0f, 127.0f);
    if (noteNum != newNotenum) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Notenum, newNotenum);
    }
    GUILayout.EndHorizontal();
    
    // noteVelo
    GUILayout.BeginHorizontal();
    float noteVelo = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Notevelo);
    float newNotevelo = EditorGUILayout.Slider("noteVelo", noteVelo, 0.0f, 127.0f);
    if (noteVelo != newNotevelo) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Notevelo, newNotevelo);
    }
    GUILayout.EndHorizontal();
    
    // notenumber
    GUILayout.BeginHorizontal();
    float notenumber = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Notenumber);
    float newNotenumber = EditorGUILayout.Slider("notenumber", notenumber, 0.0f, 127.0f);
    if (notenumber != newNotenumber) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Notenumber, newNotenumber);
    }
    GUILayout.EndHorizontal();
    
    // osc1_pitchmod_lfo1_depth
    GUILayout.BeginHorizontal();
    float osc1_pitchmod_lfo1_depth = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Osc1_pitchmod_lfo1_depth);
    float newOsc1_pitchmod_lfo1_depth = EditorGUILayout.Slider("osc1_pitchmod_lfo1_depth", osc1_pitchmod_lfo1_depth, 0.0f, 0.5f);
    if (osc1_pitchmod_lfo1_depth != newOsc1_pitchmod_lfo1_depth) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Osc1_pitchmod_lfo1_depth, newOsc1_pitchmod_lfo1_depth);
    }
    GUILayout.EndHorizontal();
    
    // osc1_pitchmod_lfo2_depth
    GUILayout.BeginHorizontal();
    float osc1_pitchmod_lfo2_depth = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Osc1_pitchmod_lfo2_depth);
    float newOsc1_pitchmod_lfo2_depth = EditorGUILayout.Slider("osc1_pitchmod_lfo2_depth", osc1_pitchmod_lfo2_depth, 0.0f, 0.5f);
    if (osc1_pitchmod_lfo2_depth != newOsc1_pitchmod_lfo2_depth) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Osc1_pitchmod_lfo2_depth, newOsc1_pitchmod_lfo2_depth);
    }
    GUILayout.EndHorizontal();
    
    // osc1_wave
    GUILayout.BeginHorizontal();
    float osc1_wave = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Osc1_wave);
    float newOsc1_wave = EditorGUILayout.Slider("osc1_wave", osc1_wave, 1.0f, 8.0f);
    if (osc1_wave != newOsc1_wave) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Osc1_wave, newOsc1_wave);
    }
    GUILayout.EndHorizontal();
    
    // osc2_offset
    GUILayout.BeginHorizontal();
    float osc2_offset = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Osc2_offset);
    float newOsc2_offset = EditorGUILayout.Slider("osc2_offset", osc2_offset, -24.0f, 24.0f);
    if (osc2_offset != newOsc2_offset) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Osc2_offset, newOsc2_offset);
    }
    GUILayout.EndHorizontal();
    
    // osc2_pitchmod_lfo1_depth
    GUILayout.BeginHorizontal();
    float osc2_pitchmod_lfo1_depth = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Osc2_pitchmod_lfo1_depth);
    float newOsc2_pitchmod_lfo1_depth = EditorGUILayout.Slider("osc2_pitchmod_lfo1_depth", osc2_pitchmod_lfo1_depth, 0.0f, 0.5f);
    if (osc2_pitchmod_lfo1_depth != newOsc2_pitchmod_lfo1_depth) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Osc2_pitchmod_lfo1_depth, newOsc2_pitchmod_lfo1_depth);
    }
    GUILayout.EndHorizontal();
    
    // osc2_pitchmod_lfo2_depth
    GUILayout.BeginHorizontal();
    float osc2_pitchmod_lfo2_depth = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Osc2_pitchmod_lfo2_depth);
    float newOsc2_pitchmod_lfo2_depth = EditorGUILayout.Slider("osc2_pitchmod_lfo2_depth", osc2_pitchmod_lfo2_depth, 0.0f, 0.5f);
    if (osc2_pitchmod_lfo2_depth != newOsc2_pitchmod_lfo2_depth) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Osc2_pitchmod_lfo2_depth, newOsc2_pitchmod_lfo2_depth);
    }
    GUILayout.EndHorizontal();
    
    // osc2_wave
    GUILayout.BeginHorizontal();
    float osc2_wave = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Osc2_wave);
    float newOsc2_wave = EditorGUILayout.Slider("osc2_wave", osc2_wave, 1.0f, 8.0f);
    if (osc2_wave != newOsc2_wave) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Osc2_wave, newOsc2_wave);
    }
    GUILayout.EndHorizontal();
    
    // oscMix
    GUILayout.BeginHorizontal();
    float oscMix = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Oscmix);
    float newOscmix = EditorGUILayout.Slider("oscMix", oscMix, 0.0f, 1.0f);
    if (oscMix != newOscmix) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Oscmix, newOscmix);
    }
    GUILayout.EndHorizontal();
    
    // oscMix_Lfo1_Depth
    GUILayout.BeginHorizontal();
    float oscMix_Lfo1_Depth = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Oscmix_lfo1_depth);
    float newOscmix_lfo1_depth = EditorGUILayout.Slider("oscMix_Lfo1_Depth", oscMix_Lfo1_Depth, 0.0f, 1.0f);
    if (oscMix_Lfo1_Depth != newOscmix_lfo1_depth) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Oscmix_lfo1_depth, newOscmix_lfo1_depth);
    }
    GUILayout.EndHorizontal();
    
    // reverbLvl_depth
    GUILayout.BeginHorizontal();
    float reverbLvl_depth = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Reverblvl_depth);
    float newReverblvl_depth = EditorGUILayout.Slider("reverbLvl_depth", reverbLvl_depth, 40.0f, 100.0f);
    if (reverbLvl_depth != newReverblvl_depth) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Reverblvl_depth, newReverblvl_depth);
    }
    GUILayout.EndHorizontal();
    
    // ringmod_depth
    GUILayout.BeginHorizontal();
    float ringmod_depth = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Ringmod_depth);
    float newRingmod_depth = EditorGUILayout.Slider("ringmod_depth", ringmod_depth, 0.0f, 1.0f);
    if (ringmod_depth != newRingmod_depth) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Ringmod_depth, newRingmod_depth);
    }
    GUILayout.EndHorizontal();
    
    // ringmod_lfo1_depth
    GUILayout.BeginHorizontal();
    float ringmod_lfo1_depth = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Ringmod_lfo1_depth);
    float newRingmod_lfo1_depth = EditorGUILayout.Slider("ringmod_lfo1_depth", ringmod_lfo1_depth, 0.0f, 0.5f);
    if (ringmod_lfo1_depth != newRingmod_lfo1_depth) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Ringmod_lfo1_depth, newRingmod_lfo1_depth);
    }
    GUILayout.EndHorizontal();
    
    // ringmod_lfo2_depth
    GUILayout.BeginHorizontal();
    float ringmod_lfo2_depth = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Ringmod_lfo2_depth);
    float newRingmod_lfo2_depth = EditorGUILayout.Slider("ringmod_lfo2_depth", ringmod_lfo2_depth, 0.0f, 0.5f);
    if (ringmod_lfo2_depth != newRingmod_lfo2_depth) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Ringmod_lfo2_depth, newRingmod_lfo2_depth);
    }
    GUILayout.EndHorizontal();
    
    // veloToFiltFreq
    GUILayout.BeginHorizontal();
    float veloToFiltFreq = _dsp.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Velotofiltfreq);
    float newVelotofiltfreq = EditorGUILayout.Slider("veloToFiltFreq", veloToFiltFreq, 0.0f, 1.0f);
    if (veloToFiltFreq != newVelotofiltfreq) {
      _dsp.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Velotofiltfreq, newVelotofiltfreq);
    }
    GUILayout.EndHorizontal();
    EditorGUI.indentLevel--;
  }
}
#endif // UNITY_EDITOR

[RequireComponent (typeof (AudioSource))]
public class Hv_AccordataSynth_AudioLib : MonoBehaviour {
  
  // Events are used to trigger bangs in the patch context (thread-safe).
  // Example usage:
  /*
    void Start () {
        Hv_AccordataSynth_AudioLib script = GetComponent<Hv_AccordataSynth_AudioLib>();
        script.SendEvent(Hv_AccordataSynth_AudioLib.Event.Lfo1_reset);
    }
  */
  public enum Event : uint {
    Lfo1_reset = 0x5759691C,
    Lfo2_reset = 0x5C692769,
    Start = 0x6FF57CB4,
    Stop = 0x7A5B032D,
    Testrigger = 0xB237580D,
  }
  
  // Parameters are used to send float messages into the patch context (thread-safe).
  // Example usage:
  /*
    void Start () {
        Hv_AccordataSynth_AudioLib script = GetComponent<Hv_AccordataSynth_AudioLib>();
        // Get and set a parameter
        float adsr_attack = script.GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Adsr_attack);
        script.SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter.Adsr_attack, adsr_attack + 0.1f);
    }
  */
  public enum Parameter : uint {
    Adsr_attack = 0x4507E807,
    Adsr_decay = 0x5E80A776,
    Adsr_release = 0x59A0D907,
    Adsr_sustain = 0xFB8A34EF,
    Amp_lfo1_depth = 0x346D6B38,
    Amp_lfo2_depth = 0xFE61644D,
    Bpm = 0x7BB9803A,
    Delayfeedback = 0x2E2D372C,
    Delaylevel = 0x3D729CDA,
    Delaytimems = 0x35FEFFC3,
    Flangerdepth = 0x352D81C2,
    Flangerfeedback = 0xCB1B53B6,
    Flangermix = 0xDC2134A7,
    Flangerspeed = 0x77C7F5E8,
    Lfo1retrig = 0x541BE8F7,
    Lfo1_rate = 0x49E9BD6A,
    Lfo1_wave = 0x75CEE11B,
    Lfo2retrig = 0xC6B89399,
    Lfo2_rate = 0x5FC56424,
    Lfo2_wave = 0xB8EB54EA,
    Lpf_freq = 0x694E4877,
    Lpf_lfo1_depth = 0x98117DD,
    Lpf_lfo2_depth = 0x9BB769,
    Lpf_q = 0x5F290E5E,
    Notenum = 0x58249DD7,
    Notevelo = 0xB74316E4,
    Notenumber = 0xA742A623,
    Osc1_pitchmod_lfo1_depth = 0x697956A7,
    Osc1_pitchmod_lfo2_depth = 0xEA8FDC0,
    Osc1_wave = 0x45318D4E,
    Osc2_offset = 0x31189696,
    Osc2_pitchmod_lfo1_depth = 0x4BE5DAEF,
    Osc2_pitchmod_lfo2_depth = 0x31CD9B85,
    Osc2_wave = 0x7F0383DC,
    Oscmix = 0xE71DFFF0,
    Oscmix_lfo1_depth = 0xF0C07B9F,
    Reverblvl_depth = 0x1DA9841F,
    Ringmod_depth = 0x8F6651A1,
    Ringmod_lfo1_depth = 0xD8D35463,
    Ringmod_lfo2_depth = 0x8216FF4F,
    Velotofiltfreq = 0xCE4695D5,
  }
  
  // Delegate method for receiving float messages from the patch context (thread-safe).
  // Example usage:
  /*
    void Start () {
        Hv_AccordataSynth_AudioLib script = GetComponent<Hv_AccordataSynth_AudioLib>();
        script.RegisterSendHook();
        script.FloatReceivedCallback += OnFloatMessage;
    }

    void OnFloatMessage(Hv_AccordataSynth_AudioLib.FloatMessage message) {
        Debug.Log(message.receiverName + ": " + message.value);
    }
  */
  public class FloatMessage {
    public string receiverName;
    public float value;

    public FloatMessage(string name, float x) {
      receiverName = name;
      value = x;
    }
  }
  public delegate void FloatMessageReceived(FloatMessage message);
  public FloatMessageReceived FloatReceivedCallback;
  public float adsr_attack = 30.0f;
  public float adsr_decay = 30.0f;
  public float adsr_release = 1000.0f;
  public float adsr_sustain = 0.5f;
  public float amp_lfo1_depth = 0.0f;
  public float amp_lfo2_depth = 0.0f;
  public float bpm = 100.0f;
  public float delayFeedback = 0.4f;
  public float delayLevel = 0.5f;
  public float delayTimeMS = 250.0f;
  public float flangerDepth = 0.2f;
  public float flangerFeedback = 0.5f;
  public float flangerMix = 0.0f;
  public float flangerSpeed = 1.0f;
  public float lfo1Retrig = 0.0f;
  public float lfo1_rate = 1.0f;
  public float lfo1_wave = 2.0f;
  public float lfo2Retrig = 0.0f;
  public float lfo2_rate = 1.0f;
  public float lfo2_wave = 2.0f;
  public float lpf_freq = 0.0f;
  public float lpf_lfo1_depth = 0.0f;
  public float lpf_lfo2_depth = 0.0f;
  public float lpf_q = 0.0f;
  public float noteNum = 64.0f;
  public float noteVelo = 64.0f;
  public float notenumber = 60.0f;
  public float osc1_pitchmod_lfo1_depth = 0.0f;
  public float osc1_pitchmod_lfo2_depth = 0.0f;
  public float osc1_wave = 1.0f;
  public float osc2_offset = 0.0f;
  public float osc2_pitchmod_lfo1_depth = 0.0f;
  public float osc2_pitchmod_lfo2_depth = 0.0f;
  public float osc2_wave = 2.0f;
  public float oscMix = 0.5f;
  public float oscMix_Lfo1_Depth = 0.0f;
  public float reverbLvl_depth = 50.0f;
  public float ringmod_depth = 0.0f;
  public float ringmod_lfo1_depth = 0.0f;
  public float ringmod_lfo2_depth = 0.0f;
  public float veloToFiltFreq = 0.0f;

  // internal state
  private Hv_AccordataSynth_Context _context;

  public bool IsInstantiated() {
    return (_context != null);
  }

  public void RegisterSendHook() {
    _context.RegisterSendHook();
  }
  
  // see Hv_AccordataSynth_AudioLib.Event for definitions
  public void SendEvent(Hv_AccordataSynth_AudioLib.Event e) {
    if (IsInstantiated()) _context.SendBangToReceiver((uint) e);
  }
  
  // see Hv_AccordataSynth_AudioLib.Parameter for definitions
  public float GetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter param) {
    switch (param) {
      case Parameter.Adsr_attack: return adsr_attack;
      case Parameter.Adsr_decay: return adsr_decay;
      case Parameter.Adsr_release: return adsr_release;
      case Parameter.Adsr_sustain: return adsr_sustain;
      case Parameter.Amp_lfo1_depth: return amp_lfo1_depth;
      case Parameter.Amp_lfo2_depth: return amp_lfo2_depth;
      case Parameter.Bpm: return bpm;
      case Parameter.Delayfeedback: return delayFeedback;
      case Parameter.Delaylevel: return delayLevel;
      case Parameter.Delaytimems: return delayTimeMS;
      case Parameter.Flangerdepth: return flangerDepth;
      case Parameter.Flangerfeedback: return flangerFeedback;
      case Parameter.Flangermix: return flangerMix;
      case Parameter.Flangerspeed: return flangerSpeed;
      case Parameter.Lfo1retrig: return lfo1Retrig;
      case Parameter.Lfo1_rate: return lfo1_rate;
      case Parameter.Lfo1_wave: return lfo1_wave;
      case Parameter.Lfo2retrig: return lfo2Retrig;
      case Parameter.Lfo2_rate: return lfo2_rate;
      case Parameter.Lfo2_wave: return lfo2_wave;
      case Parameter.Lpf_freq: return lpf_freq;
      case Parameter.Lpf_lfo1_depth: return lpf_lfo1_depth;
      case Parameter.Lpf_lfo2_depth: return lpf_lfo2_depth;
      case Parameter.Lpf_q: return lpf_q;
      case Parameter.Notenum: return noteNum;
      case Parameter.Notevelo: return noteVelo;
      case Parameter.Notenumber: return notenumber;
      case Parameter.Osc1_pitchmod_lfo1_depth: return osc1_pitchmod_lfo1_depth;
      case Parameter.Osc1_pitchmod_lfo2_depth: return osc1_pitchmod_lfo2_depth;
      case Parameter.Osc1_wave: return osc1_wave;
      case Parameter.Osc2_offset: return osc2_offset;
      case Parameter.Osc2_pitchmod_lfo1_depth: return osc2_pitchmod_lfo1_depth;
      case Parameter.Osc2_pitchmod_lfo2_depth: return osc2_pitchmod_lfo2_depth;
      case Parameter.Osc2_wave: return osc2_wave;
      case Parameter.Oscmix: return oscMix;
      case Parameter.Oscmix_lfo1_depth: return oscMix_Lfo1_Depth;
      case Parameter.Reverblvl_depth: return reverbLvl_depth;
      case Parameter.Ringmod_depth: return ringmod_depth;
      case Parameter.Ringmod_lfo1_depth: return ringmod_lfo1_depth;
      case Parameter.Ringmod_lfo2_depth: return ringmod_lfo2_depth;
      case Parameter.Velotofiltfreq: return veloToFiltFreq;
      default: return 0.0f;
    }
  }

  public void SetFloatParameter(Hv_AccordataSynth_AudioLib.Parameter param, float x) {
    switch (param) {
      case Parameter.Adsr_attack: {
        x = Mathf.Clamp(x, 10.0f, 10010.0f);
        adsr_attack = x;
        break;
      }
      case Parameter.Adsr_decay: {
        x = Mathf.Clamp(x, 10.0f, 10030.0f);
        adsr_decay = x;
        break;
      }
      case Parameter.Adsr_release: {
        x = Mathf.Clamp(x, 10.0f, 10010.0f);
        adsr_release = x;
        break;
      }
      case Parameter.Adsr_sustain: {
        x = Mathf.Clamp(x, 0.1f, 1.1f);
        adsr_sustain = x;
        break;
      }
      case Parameter.Amp_lfo1_depth: {
        x = Mathf.Clamp(x, 0.0f, 0.5f);
        amp_lfo1_depth = x;
        break;
      }
      case Parameter.Amp_lfo2_depth: {
        x = Mathf.Clamp(x, 0.0f, 0.5f);
        amp_lfo2_depth = x;
        break;
      }
      case Parameter.Bpm: {
        x = Mathf.Clamp(x, 40.0f, 200.0f);
        bpm = x;
        break;
      }
      case Parameter.Delayfeedback: {
        x = Mathf.Clamp(x, 0.0f, 0.6f);
        delayFeedback = x;
        break;
      }
      case Parameter.Delaylevel: {
        x = Mathf.Clamp(x, 0.0f, 1.0f);
        delayLevel = x;
        break;
      }
      case Parameter.Delaytimems: {
        x = Mathf.Clamp(x, 10.0f, 2000.0f);
        delayTimeMS = x;
        break;
      }
      case Parameter.Flangerdepth: {
        x = Mathf.Clamp(x, 0.0f, 1.0f);
        flangerDepth = x;
        break;
      }
      case Parameter.Flangerfeedback: {
        x = Mathf.Clamp(x, 0.5f, 0.99f);
        flangerFeedback = x;
        break;
      }
      case Parameter.Flangermix: {
        x = Mathf.Clamp(x, 0.0f, 1.0f);
        flangerMix = x;
        break;
      }
      case Parameter.Flangerspeed: {
        x = Mathf.Clamp(x, 0.5f, 20.0f);
        flangerSpeed = x;
        break;
      }
      case Parameter.Lfo1retrig: {
        x = Mathf.Clamp(x, 0.0f, 1.0f);
        lfo1Retrig = x;
        break;
      }
      case Parameter.Lfo1_rate: {
        x = Mathf.Clamp(x, 0.1f, 20.0f);
        lfo1_rate = x;
        break;
      }
      case Parameter.Lfo1_wave: {
        x = Mathf.Clamp(x, 1.0f, 8.0f);
        lfo1_wave = x;
        break;
      }
      case Parameter.Lfo2retrig: {
        x = Mathf.Clamp(x, 0.0f, 1.0f);
        lfo2Retrig = x;
        break;
      }
      case Parameter.Lfo2_rate: {
        x = Mathf.Clamp(x, 0.1f, 20.0f);
        lfo2_rate = x;
        break;
      }
      case Parameter.Lfo2_wave: {
        x = Mathf.Clamp(x, 1.0f, 8.0f);
        lfo2_wave = x;
        break;
      }
      case Parameter.Lpf_freq: {
        x = Mathf.Clamp(x, 0.0f, 8000.0f);
        lpf_freq = x;
        break;
      }
      case Parameter.Lpf_lfo1_depth: {
        x = Mathf.Clamp(x, 0.0f, 0.5f);
        lpf_lfo1_depth = x;
        break;
      }
      case Parameter.Lpf_lfo2_depth: {
        x = Mathf.Clamp(x, 0.0f, 0.5f);
        lpf_lfo2_depth = x;
        break;
      }
      case Parameter.Lpf_q: {
        x = Mathf.Clamp(x, 0.0f, 10.0f);
        lpf_q = x;
        break;
      }
      case Parameter.Notenum: {
        x = Mathf.Clamp(x, 0.0f, 127.0f);
        noteNum = x;
        break;
      }
      case Parameter.Notevelo: {
        x = Mathf.Clamp(x, 0.0f, 127.0f);
        noteVelo = x;
        break;
      }
      case Parameter.Notenumber: {
        x = Mathf.Clamp(x, 0.0f, 127.0f);
        notenumber = x;
        break;
      }
      case Parameter.Osc1_pitchmod_lfo1_depth: {
        x = Mathf.Clamp(x, 0.0f, 0.5f);
        osc1_pitchmod_lfo1_depth = x;
        break;
      }
      case Parameter.Osc1_pitchmod_lfo2_depth: {
        x = Mathf.Clamp(x, 0.0f, 0.5f);
        osc1_pitchmod_lfo2_depth = x;
        break;
      }
      case Parameter.Osc1_wave: {
        x = Mathf.Clamp(x, 1.0f, 8.0f);
        osc1_wave = x;
        break;
      }
      case Parameter.Osc2_offset: {
        x = Mathf.Clamp(x, -24.0f, 24.0f);
        osc2_offset = x;
        break;
      }
      case Parameter.Osc2_pitchmod_lfo1_depth: {
        x = Mathf.Clamp(x, 0.0f, 0.5f);
        osc2_pitchmod_lfo1_depth = x;
        break;
      }
      case Parameter.Osc2_pitchmod_lfo2_depth: {
        x = Mathf.Clamp(x, 0.0f, 0.5f);
        osc2_pitchmod_lfo2_depth = x;
        break;
      }
      case Parameter.Osc2_wave: {
        x = Mathf.Clamp(x, 1.0f, 8.0f);
        osc2_wave = x;
        break;
      }
      case Parameter.Oscmix: {
        x = Mathf.Clamp(x, 0.0f, 1.0f);
        oscMix = x;
        break;
      }
      case Parameter.Oscmix_lfo1_depth: {
        x = Mathf.Clamp(x, 0.0f, 1.0f);
        oscMix_Lfo1_Depth = x;
        break;
      }
      case Parameter.Reverblvl_depth: {
        x = Mathf.Clamp(x, 40.0f, 100.0f);
        reverbLvl_depth = x;
        break;
      }
      case Parameter.Ringmod_depth: {
        x = Mathf.Clamp(x, 0.0f, 1.0f);
        ringmod_depth = x;
        break;
      }
      case Parameter.Ringmod_lfo1_depth: {
        x = Mathf.Clamp(x, 0.0f, 0.5f);
        ringmod_lfo1_depth = x;
        break;
      }
      case Parameter.Ringmod_lfo2_depth: {
        x = Mathf.Clamp(x, 0.0f, 0.5f);
        ringmod_lfo2_depth = x;
        break;
      }
      case Parameter.Velotofiltfreq: {
        x = Mathf.Clamp(x, 0.0f, 1.0f);
        veloToFiltFreq = x;
        break;
      }
      default: return;
    }
    if (IsInstantiated()) _context.SendFloatToReceiver((uint) param, x);
  }
  
  public void FillTableWithMonoAudioClip(string tableName, AudioClip clip) {
    if (clip.channels > 1) {
      Debug.LogWarning("Hv_AccordataSynth_AudioLib: Only loading first channel of '" +
          clip.name + "' into table '" +
          tableName + "'. Multi-channel files are not supported.");
    }
    float[] buffer = new float[clip.samples]; // copy only the 1st channel
    clip.GetData(buffer, 0);
    _context.FillTableWithFloatBuffer(tableName, buffer);
  }

  public void FillTableWithFloatBuffer(string tableName, float[] buffer) {
    _context.FillTableWithFloatBuffer(tableName, buffer);
  }

  private void Awake() {
    _context = new Hv_AccordataSynth_Context((double) AudioSettings.outputSampleRate);
  }
  
  private void Start() {
    _context.SendFloatToReceiver((uint) Parameter.Adsr_attack, adsr_attack);
    _context.SendFloatToReceiver((uint) Parameter.Adsr_decay, adsr_decay);
    _context.SendFloatToReceiver((uint) Parameter.Adsr_release, adsr_release);
    _context.SendFloatToReceiver((uint) Parameter.Adsr_sustain, adsr_sustain);
    _context.SendFloatToReceiver((uint) Parameter.Amp_lfo1_depth, amp_lfo1_depth);
    _context.SendFloatToReceiver((uint) Parameter.Amp_lfo2_depth, amp_lfo2_depth);
    _context.SendFloatToReceiver((uint) Parameter.Bpm, bpm);
    _context.SendFloatToReceiver((uint) Parameter.Delayfeedback, delayFeedback);
    _context.SendFloatToReceiver((uint) Parameter.Delaylevel, delayLevel);
    _context.SendFloatToReceiver((uint) Parameter.Delaytimems, delayTimeMS);
    _context.SendFloatToReceiver((uint) Parameter.Flangerdepth, flangerDepth);
    _context.SendFloatToReceiver((uint) Parameter.Flangerfeedback, flangerFeedback);
    _context.SendFloatToReceiver((uint) Parameter.Flangermix, flangerMix);
    _context.SendFloatToReceiver((uint) Parameter.Flangerspeed, flangerSpeed);
    _context.SendFloatToReceiver((uint) Parameter.Lfo1retrig, lfo1Retrig);
    _context.SendFloatToReceiver((uint) Parameter.Lfo1_rate, lfo1_rate);
    _context.SendFloatToReceiver((uint) Parameter.Lfo1_wave, lfo1_wave);
    _context.SendFloatToReceiver((uint) Parameter.Lfo2retrig, lfo2Retrig);
    _context.SendFloatToReceiver((uint) Parameter.Lfo2_rate, lfo2_rate);
    _context.SendFloatToReceiver((uint) Parameter.Lfo2_wave, lfo2_wave);
    _context.SendFloatToReceiver((uint) Parameter.Lpf_freq, lpf_freq);
    _context.SendFloatToReceiver((uint) Parameter.Lpf_lfo1_depth, lpf_lfo1_depth);
    _context.SendFloatToReceiver((uint) Parameter.Lpf_lfo2_depth, lpf_lfo2_depth);
    _context.SendFloatToReceiver((uint) Parameter.Lpf_q, lpf_q);
    _context.SendFloatToReceiver((uint) Parameter.Notenum, noteNum);
    _context.SendFloatToReceiver((uint) Parameter.Notevelo, noteVelo);
    _context.SendFloatToReceiver((uint) Parameter.Notenumber, notenumber);
    _context.SendFloatToReceiver((uint) Parameter.Osc1_pitchmod_lfo1_depth, osc1_pitchmod_lfo1_depth);
    _context.SendFloatToReceiver((uint) Parameter.Osc1_pitchmod_lfo2_depth, osc1_pitchmod_lfo2_depth);
    _context.SendFloatToReceiver((uint) Parameter.Osc1_wave, osc1_wave);
    _context.SendFloatToReceiver((uint) Parameter.Osc2_offset, osc2_offset);
    _context.SendFloatToReceiver((uint) Parameter.Osc2_pitchmod_lfo1_depth, osc2_pitchmod_lfo1_depth);
    _context.SendFloatToReceiver((uint) Parameter.Osc2_pitchmod_lfo2_depth, osc2_pitchmod_lfo2_depth);
    _context.SendFloatToReceiver((uint) Parameter.Osc2_wave, osc2_wave);
    _context.SendFloatToReceiver((uint) Parameter.Oscmix, oscMix);
    _context.SendFloatToReceiver((uint) Parameter.Oscmix_lfo1_depth, oscMix_Lfo1_Depth);
    _context.SendFloatToReceiver((uint) Parameter.Reverblvl_depth, reverbLvl_depth);
    _context.SendFloatToReceiver((uint) Parameter.Ringmod_depth, ringmod_depth);
    _context.SendFloatToReceiver((uint) Parameter.Ringmod_lfo1_depth, ringmod_lfo1_depth);
    _context.SendFloatToReceiver((uint) Parameter.Ringmod_lfo2_depth, ringmod_lfo2_depth);
    _context.SendFloatToReceiver((uint) Parameter.Velotofiltfreq, veloToFiltFreq);
  }
  
  private void Update() {
    // retreive sent messages
    if (_context.IsSendHookRegistered()) {
      Hv_AccordataSynth_AudioLib.FloatMessage tempMessage;
      while ((tempMessage = _context.msgQueue.GetNextMessage()) != null) {
        FloatReceivedCallback(tempMessage);
      }
    }
  }

  private void OnAudioFilterRead(float[] buffer, int numChannels) {
    Assert.AreEqual(numChannels, _context.GetNumOutputChannels()); // invalid channel configuration
    _context.Process(buffer, buffer.Length / numChannels); // process dsp
  }
}

class Hv_AccordataSynth_Context {

#if UNITY_IOS && !UNITY_EDITOR
  private const string _dllName = "__Internal";
#else
  private const string _dllName = "Hv_AccordataSynth_AudioLib";
#endif

  // Thread-safe message queue
  public class SendMessageQueue {
    private readonly object _msgQueueSync = new object();
    private readonly Queue<Hv_AccordataSynth_AudioLib.FloatMessage> _msgQueue = new Queue<Hv_AccordataSynth_AudioLib.FloatMessage>();

    public Hv_AccordataSynth_AudioLib.FloatMessage GetNextMessage() {
      lock (_msgQueueSync) {
        return (_msgQueue.Count != 0) ? _msgQueue.Dequeue() : null;
      }
    }

    public void AddMessage(string receiverName, float value) {
      Hv_AccordataSynth_AudioLib.FloatMessage msg = new Hv_AccordataSynth_AudioLib.FloatMessage(receiverName, value);
      lock (_msgQueueSync) {
        _msgQueue.Enqueue(msg);
      }
    }
  }

  public readonly SendMessageQueue msgQueue = new SendMessageQueue();
  private readonly GCHandle gch;
  private readonly IntPtr _context; // handle into unmanaged memory
  private SendHook _sendHook = null;

  [DllImport (_dllName)]
  private static extern IntPtr hv_AccordataSynth_new_with_options(double sampleRate, int poolKb, int inQueueKb, int outQueueKb);

  [DllImport (_dllName)]
  private static extern int hv_processInlineInterleaved(IntPtr ctx,
      [In] float[] inBuffer, [Out] float[] outBuffer, int numSamples);

  [DllImport (_dllName)]
  private static extern void hv_delete(IntPtr ctx);

  [DllImport (_dllName)]
  private static extern double hv_getSampleRate(IntPtr ctx);

  [DllImport (_dllName)]
  private static extern int hv_getNumInputChannels(IntPtr ctx);

  [DllImport (_dllName)]
  private static extern int hv_getNumOutputChannels(IntPtr ctx);

  [DllImport (_dllName)]
  private static extern void hv_setSendHook(IntPtr ctx, SendHook sendHook);

  [DllImport (_dllName)]
  private static extern void hv_setPrintHook(IntPtr ctx, PrintHook printHook);

  [DllImport (_dllName)]
  private static extern int hv_setUserData(IntPtr ctx, IntPtr userData);

  [DllImport (_dllName)]
  private static extern IntPtr hv_getUserData(IntPtr ctx);

  [DllImport (_dllName)]
  private static extern void hv_sendBangToReceiver(IntPtr ctx, uint receiverHash);

  [DllImport (_dllName)]
  private static extern void hv_sendFloatToReceiver(IntPtr ctx, uint receiverHash, float x);

  [DllImport (_dllName)]
  private static extern uint hv_msg_getTimestamp(IntPtr message);

  [DllImport (_dllName)]
  private static extern bool hv_msg_hasFormat(IntPtr message, string format);

  [DllImport (_dllName)]
  private static extern float hv_msg_getFloat(IntPtr message, int index);

  [DllImport (_dllName)]
  private static extern bool hv_table_setLength(IntPtr ctx, uint tableHash, uint newSampleLength);

  [DllImport (_dllName)]
  private static extern IntPtr hv_table_getBuffer(IntPtr ctx, uint tableHash);

  [DllImport (_dllName)]
  private static extern float hv_samplesToMilliseconds(IntPtr ctx, uint numSamples);

  [DllImport (_dllName)]
  private static extern uint hv_stringToHash(string s);

  private delegate void PrintHook(IntPtr context, string printName, string str, IntPtr message);

  private delegate void SendHook(IntPtr context, string sendName, uint sendHash, IntPtr message);

  public Hv_AccordataSynth_Context(double sampleRate, int poolKb=10, int inQueueKb=42, int outQueueKb=2) {
    gch = GCHandle.Alloc(msgQueue);
    _context = hv_AccordataSynth_new_with_options(sampleRate, poolKb, inQueueKb, outQueueKb);
    hv_setPrintHook(_context, new PrintHook(OnPrint));
    hv_setUserData(_context, GCHandle.ToIntPtr(gch));
  }

  ~Hv_AccordataSynth_Context() {
    hv_delete(_context);
    GC.KeepAlive(_context);
    GC.KeepAlive(_sendHook);
    gch.Free();
  }

  public void RegisterSendHook() {
    // Note: send hook functionality only applies to messages containing a single float value
    if (_sendHook == null) {
      _sendHook = new SendHook(OnMessageSent);
      hv_setSendHook(_context, _sendHook);
    }
  }

  public bool IsSendHookRegistered() {
    return (_sendHook != null);
  }

  public double GetSampleRate() {
    return hv_getSampleRate(_context);
  }

  public int GetNumInputChannels() {
    return hv_getNumInputChannels(_context);
  }

  public int GetNumOutputChannels() {
    return hv_getNumOutputChannels(_context);
  }

  public void SendBangToReceiver(uint receiverHash) {
    hv_sendBangToReceiver(_context, receiverHash);
  }

  public void SendFloatToReceiver(uint receiverHash, float x) {
    hv_sendFloatToReceiver(_context, receiverHash, x);
  }

  public void FillTableWithFloatBuffer(string tableName, float[] buffer) {
    uint tableHash = hv_stringToHash(tableName);
    if (hv_table_getBuffer(_context, tableHash) != IntPtr.Zero) {
      hv_table_setLength(_context, tableHash, (uint) buffer.Length);
      Marshal.Copy(buffer, 0, hv_table_getBuffer(_context, tableHash), buffer.Length);
    } else {
      Debug.Log(string.Format("Table '{0}' doesn't exist in the patch context.", tableName));
    }
  }

  public uint StringToHash(string s) {
    return hv_stringToHash(s);
  }

  public int Process(float[] buffer, int numFrames) {
    return hv_processInlineInterleaved(_context, buffer, buffer, numFrames);
  }

  [MonoPInvokeCallback(typeof(PrintHook))]
  private static void OnPrint(IntPtr context, string printName, string str, IntPtr message) {
    float timeInSecs = hv_samplesToMilliseconds(context, hv_msg_getTimestamp(message)) / 1000.0f;
    Debug.Log(string.Format("{0} [{1:0.000}]: {2}", printName, timeInSecs, str));
  }

  [MonoPInvokeCallback(typeof(SendHook))]
  private static void OnMessageSent(IntPtr context, string sendName, uint sendHash, IntPtr message) {
    if (hv_msg_hasFormat(message, "f")) {
      SendMessageQueue msgQueue = (SendMessageQueue) GCHandle.FromIntPtr(hv_getUserData(context)).Target;
      msgQueue.AddMessage(sendName, hv_msg_getFloat(message, 0));
    }
  }
}
