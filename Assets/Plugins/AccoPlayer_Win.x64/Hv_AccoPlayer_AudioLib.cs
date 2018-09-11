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

[CustomEditor(typeof(Hv_AccoPlayer_AudioLib))]
public class Hv_AccoPlayer_Editor : Editor {

  [MenuItem("Heavy/AccoPlayer")]
  static void CreateHv_AccoPlayer() {
    GameObject target = Selection.activeGameObject;
    if (target != null) {
      target.AddComponent<Hv_AccoPlayer_AudioLib>();
    }
  }

  private Hv_AccoPlayer_AudioLib _dsp;

  private void OnEnable() {
    _dsp = target as Hv_AccoPlayer_AudioLib;
  }

  public override void OnInspectorGUI() {
    bool isEnabled = _dsp.IsInstantiated();
    if (!isEnabled) {
      EditorGUILayout.LabelField("Press Play!",  EditorStyles.centeredGreyMiniLabel);
    }
    GUILayout.BeginVertical();
    // EVENTS
    GUI.enabled = isEnabled;
    EditorGUILayout.Space();

    // clearSeq
    if (GUILayout.Button("clearSeq")) {
      _dsp.SendEvent(Hv_AccoPlayer_AudioLib.Event.Clearseq);
    }

    // start
    if (GUILayout.Button("start")) {
      _dsp.SendEvent(Hv_AccoPlayer_AudioLib.Event.Start);
    }

    // stop
    if (GUILayout.Button("stop")) {
      _dsp.SendEvent(Hv_AccoPlayer_AudioLib.Event.Stop);
    }
    // PARAMETERS
    GUI.enabled = true;
    EditorGUILayout.Space();
    EditorGUI.indentLevel++;

    // bpm
    GUILayout.BeginHorizontal();
    float bpm = _dsp.GetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Bpm);
    float newBpm = EditorGUILayout.Slider("bpm", bpm, 40.0f, 200.0f);
    if (bpm != newBpm) {
      _dsp.SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Bpm, newBpm);
    }
    GUILayout.EndHorizontal();

    // seqNoteNum
    GUILayout.BeginHorizontal();
    float seqNoteNum = _dsp.GetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Seqnotenum);
    float newSeqnotenum = EditorGUILayout.Slider("seqNoteNum", seqNoteNum, 0.0f, 127.0f);
    if (seqNoteNum != newSeqnotenum) {
      _dsp.SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Seqnotenum, newSeqnotenum);
    }
    GUILayout.EndHorizontal();

    // seqNoteVelo
    GUILayout.BeginHorizontal();
    float seqNoteVelo = _dsp.GetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Seqnotevelo);
    float newSeqnotevelo = EditorGUILayout.Slider("seqNoteVelo", seqNoteVelo, 0.0f, 22000.0f);
    if (seqNoteVelo != newSeqnotevelo) {
      _dsp.SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Seqnotevelo, newSeqnotevelo);
    }
    GUILayout.EndHorizontal();

    // seqStepNum
    GUILayout.BeginHorizontal();
    float seqStepNum = _dsp.GetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Seqstepnum);
    float newSeqstepnum = EditorGUILayout.Slider("seqStepNum", seqStepNum, 0.0f, 15.0f);
    if (seqStepNum != newSeqstepnum) {
      _dsp.SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Seqstepnum, newSeqstepnum);
    }
    GUILayout.EndHorizontal();

    // seqVoiceNum
    GUILayout.BeginHorizontal();
    float seqVoiceNum = _dsp.GetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Seqvoicenum);
    float newSeqvoicenum = EditorGUILayout.Slider("seqVoiceNum", seqVoiceNum, 0.0f, 127.0f);
    if (seqVoiceNum != newSeqvoicenum) {
      _dsp.SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Seqvoicenum, newSeqvoicenum);
    }
    GUILayout.EndHorizontal();

    EditorGUI.indentLevel--;

    // TABLES
    GUI.enabled = true;
    EditorGUILayout.Space();
    EditorGUI.indentLevel++;

    // seqNoteTab01
    EditorGUI.BeginChangeCheck();
    AudioClip seqNoteTab01Clip = EditorGUILayout.ObjectField("seqNoteTab01", _dsp.seqNoteTab01Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.seqNoteTab01Clip = seqNoteTab01Clip;
    }

    // seqNoteTab02
    EditorGUI.BeginChangeCheck();
    AudioClip seqNoteTab02Clip = EditorGUILayout.ObjectField("seqNoteTab02", _dsp.seqNoteTab02Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.seqNoteTab02Clip = seqNoteTab02Clip;
    }

    // seqNoteTab03
    EditorGUI.BeginChangeCheck();
    AudioClip seqNoteTab03Clip = EditorGUILayout.ObjectField("seqNoteTab03", _dsp.seqNoteTab03Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.seqNoteTab03Clip = seqNoteTab03Clip;
    }

    // seqNoteTab04
    EditorGUI.BeginChangeCheck();
    AudioClip seqNoteTab04Clip = EditorGUILayout.ObjectField("seqNoteTab04", _dsp.seqNoteTab04Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.seqNoteTab04Clip = seqNoteTab04Clip;
    }

    // seqNoteTab05
    EditorGUI.BeginChangeCheck();
    AudioClip seqNoteTab05Clip = EditorGUILayout.ObjectField("seqNoteTab05", _dsp.seqNoteTab05Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.seqNoteTab05Clip = seqNoteTab05Clip;
    }

    // seqNoteTab06
    EditorGUI.BeginChangeCheck();
    AudioClip seqNoteTab06Clip = EditorGUILayout.ObjectField("seqNoteTab06", _dsp.seqNoteTab06Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.seqNoteTab06Clip = seqNoteTab06Clip;
    }

    // seqNoteTab07
    EditorGUI.BeginChangeCheck();
    AudioClip seqNoteTab07Clip = EditorGUILayout.ObjectField("seqNoteTab07", _dsp.seqNoteTab07Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.seqNoteTab07Clip = seqNoteTab07Clip;
    }

    // seqNoteTab08
    EditorGUI.BeginChangeCheck();
    AudioClip seqNoteTab08Clip = EditorGUILayout.ObjectField("seqNoteTab08", _dsp.seqNoteTab08Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.seqNoteTab08Clip = seqNoteTab08Clip;
    }

    // seqVeloTab01
    EditorGUI.BeginChangeCheck();
    AudioClip seqVeloTab01Clip = EditorGUILayout.ObjectField("seqVeloTab01", _dsp.seqVeloTab01Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.seqVeloTab01Clip = seqVeloTab01Clip;
    }

    // seqVeloTab02
    EditorGUI.BeginChangeCheck();
    AudioClip seqVeloTab02Clip = EditorGUILayout.ObjectField("seqVeloTab02", _dsp.seqVeloTab02Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.seqVeloTab02Clip = seqVeloTab02Clip;
    }

    // seqVeloTab03
    EditorGUI.BeginChangeCheck();
    AudioClip seqVeloTab03Clip = EditorGUILayout.ObjectField("seqVeloTab03", _dsp.seqVeloTab03Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.seqVeloTab03Clip = seqVeloTab03Clip;
    }

    // seqVeloTab04
    EditorGUI.BeginChangeCheck();
    AudioClip seqVeloTab04Clip = EditorGUILayout.ObjectField("seqVeloTab04", _dsp.seqVeloTab04Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.seqVeloTab04Clip = seqVeloTab04Clip;
    }

    // seqVeloTab05
    EditorGUI.BeginChangeCheck();
    AudioClip seqVeloTab05Clip = EditorGUILayout.ObjectField("seqVeloTab05", _dsp.seqVeloTab05Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.seqVeloTab05Clip = seqVeloTab05Clip;
    }

    // seqVeloTab06
    EditorGUI.BeginChangeCheck();
    AudioClip seqVeloTab06Clip = EditorGUILayout.ObjectField("seqVeloTab06", _dsp.seqVeloTab06Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.seqVeloTab06Clip = seqVeloTab06Clip;
    }

    // seqVeloTab07
    EditorGUI.BeginChangeCheck();
    AudioClip seqVeloTab07Clip = EditorGUILayout.ObjectField("seqVeloTab07", _dsp.seqVeloTab07Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.seqVeloTab07Clip = seqVeloTab07Clip;
    }

    // seqVeloTab08
    EditorGUI.BeginChangeCheck();
    AudioClip seqVeloTab08Clip = EditorGUILayout.ObjectField("seqVeloTab08", _dsp.seqVeloTab08Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.seqVeloTab08Clip = seqVeloTab08Clip;
    }

    // tab027
    EditorGUI.BeginChangeCheck();
    AudioClip tab027Clip = EditorGUILayout.ObjectField("tab027", _dsp.tab027Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab027Clip = tab027Clip;
    }

    // tab030
    EditorGUI.BeginChangeCheck();
    AudioClip tab030Clip = EditorGUILayout.ObjectField("tab030", _dsp.tab030Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab030Clip = tab030Clip;
    }

    // tab033
    EditorGUI.BeginChangeCheck();
    AudioClip tab033Clip = EditorGUILayout.ObjectField("tab033", _dsp.tab033Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab033Clip = tab033Clip;
    }

    // tab036
    EditorGUI.BeginChangeCheck();
    AudioClip tab036Clip = EditorGUILayout.ObjectField("tab036", _dsp.tab036Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab036Clip = tab036Clip;
    }

    // tab039
    EditorGUI.BeginChangeCheck();
    AudioClip tab039Clip = EditorGUILayout.ObjectField("tab039", _dsp.tab039Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab039Clip = tab039Clip;
    }

    // tab042
    EditorGUI.BeginChangeCheck();
    AudioClip tab042Clip = EditorGUILayout.ObjectField("tab042", _dsp.tab042Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab042Clip = tab042Clip;
    }

    // tab045
    EditorGUI.BeginChangeCheck();
    AudioClip tab045Clip = EditorGUILayout.ObjectField("tab045", _dsp.tab045Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab045Clip = tab045Clip;
    }

    // tab048
    EditorGUI.BeginChangeCheck();
    AudioClip tab048Clip = EditorGUILayout.ObjectField("tab048", _dsp.tab048Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab048Clip = tab048Clip;
    }

    // tab051
    EditorGUI.BeginChangeCheck();
    AudioClip tab051Clip = EditorGUILayout.ObjectField("tab051", _dsp.tab051Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab051Clip = tab051Clip;
    }

    // tab054
    EditorGUI.BeginChangeCheck();
    AudioClip tab054Clip = EditorGUILayout.ObjectField("tab054", _dsp.tab054Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab054Clip = tab054Clip;
    }

    // tab057
    EditorGUI.BeginChangeCheck();
    AudioClip tab057Clip = EditorGUILayout.ObjectField("tab057", _dsp.tab057Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab057Clip = tab057Clip;
    }

    // tab060
    EditorGUI.BeginChangeCheck();
    AudioClip tab060Clip = EditorGUILayout.ObjectField("tab060", _dsp.tab060Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab060Clip = tab060Clip;
    }

    // tab063
    EditorGUI.BeginChangeCheck();
    AudioClip tab063Clip = EditorGUILayout.ObjectField("tab063", _dsp.tab063Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab063Clip = tab063Clip;
    }

    // tab066
    EditorGUI.BeginChangeCheck();
    AudioClip tab066Clip = EditorGUILayout.ObjectField("tab066", _dsp.tab066Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab066Clip = tab066Clip;
    }

    // tab069
    EditorGUI.BeginChangeCheck();
    AudioClip tab069Clip = EditorGUILayout.ObjectField("tab069", _dsp.tab069Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab069Clip = tab069Clip;
    }

    // tab072
    EditorGUI.BeginChangeCheck();
    AudioClip tab072Clip = EditorGUILayout.ObjectField("tab072", _dsp.tab072Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab072Clip = tab072Clip;
    }

    // tab075
    EditorGUI.BeginChangeCheck();
    AudioClip tab075Clip = EditorGUILayout.ObjectField("tab075", _dsp.tab075Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab075Clip = tab075Clip;
    }

    // tab078
    EditorGUI.BeginChangeCheck();
    AudioClip tab078Clip = EditorGUILayout.ObjectField("tab078", _dsp.tab078Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab078Clip = tab078Clip;
    }

    // tab081
    EditorGUI.BeginChangeCheck();
    AudioClip tab081Clip = EditorGUILayout.ObjectField("tab081", _dsp.tab081Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab081Clip = tab081Clip;
    }

    // tab084
    EditorGUI.BeginChangeCheck();
    AudioClip tab084Clip = EditorGUILayout.ObjectField("tab084", _dsp.tab084Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab084Clip = tab084Clip;
    }

    // tab087
    EditorGUI.BeginChangeCheck();
    AudioClip tab087Clip = EditorGUILayout.ObjectField("tab087", _dsp.tab087Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab087Clip = tab087Clip;
    }

    // tab090
    EditorGUI.BeginChangeCheck();
    AudioClip tab090Clip = EditorGUILayout.ObjectField("tab090", _dsp.tab090Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab090Clip = tab090Clip;
    }

    // tab093
    EditorGUI.BeginChangeCheck();
    AudioClip tab093Clip = EditorGUILayout.ObjectField("tab093", _dsp.tab093Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab093Clip = tab093Clip;
    }

    // tab096
    EditorGUI.BeginChangeCheck();
    AudioClip tab096Clip = EditorGUILayout.ObjectField("tab096", _dsp.tab096Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab096Clip = tab096Clip;
    }

    // tab099
    EditorGUI.BeginChangeCheck();
    AudioClip tab099Clip = EditorGUILayout.ObjectField("tab099", _dsp.tab099Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab099Clip = tab099Clip;
    }

    // tab102
    EditorGUI.BeginChangeCheck();
    AudioClip tab102Clip = EditorGUILayout.ObjectField("tab102", _dsp.tab102Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab102Clip = tab102Clip;
    }

    // tab105
    EditorGUI.BeginChangeCheck();
    AudioClip tab105Clip = EditorGUILayout.ObjectField("tab105", _dsp.tab105Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab105Clip = tab105Clip;
    }

    // tab108
    EditorGUI.BeginChangeCheck();
    AudioClip tab108Clip = EditorGUILayout.ObjectField("tab108", _dsp.tab108Clip, typeof(AudioClip), false) as AudioClip;
    if (EditorGUI.EndChangeCheck()) {
      _dsp.tab108Clip = tab108Clip;
    }

    EditorGUI.indentLevel--;

    GUILayout.EndVertical();
  }
}
#endif // UNITY_EDITOR

[RequireComponent (typeof (AudioSource))]
public class Hv_AccoPlayer_AudioLib : MonoBehaviour {
  
  // Events are used to trigger bangs in the patch context (thread-safe).
  // Example usage:
  /*
    void Start () {
        Hv_AccoPlayer_AudioLib script = GetComponent<Hv_AccoPlayer_AudioLib>();
        script.SendEvent(Hv_AccoPlayer_AudioLib.Event.Clearseq);
    }
  */
  public enum Event : uint {
    Clearseq = 0x8EBF3DD0,
    Start = 0x6FF57CB4,
    Stop = 0x7A5B032D,
  }
  
  // Parameters are used to send float messages into the patch context (thread-safe).
  // Example usage:
  /*
    void Start () {
        Hv_AccoPlayer_AudioLib script = GetComponent<Hv_AccoPlayer_AudioLib>();
        // Get and set a parameter
        float bpm = script.GetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Bpm);
        script.SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter.Bpm, bpm + 0.1f);
    }
  */
  public enum Parameter : uint {
    Bpm = 0x7BB9803A,
    Seqnotenum = 0x218A2D1E,
    Seqnotevelo = 0xA770BB0F,
    Seqstepnum = 0x56AAC061,
    Seqvoicenum = 0xE876F91B,
  }
  
  // Tables within the patch context can be filled directly with audio content
  // Example usage:
  /*
    public AudioClip clip;

    void Start () {
        Hv_AccoPlayer_AudioLib script = GetComponent<Hv_AccoPlayer_AudioLib>();
        // copy clip contents into a temporary buffer
        float[] buffer = new float[clip.samples];
        clip.GetData(buffer, 0);
        // fill a buffer called "channelL"
        looper.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Channell, buffer);
        // notify a (non-exposed) receiver of the buffer size
        looper.SendFloatToReceiver("setTableSize-channelL", clip.samples);
    }
  */
  public enum Table : uint {
    Seqnotetab01 = 0x5091A95,
    Seqnotetab02 = 0xC5F6D7F3,
    Seqnotetab03 = 0xEAB9A3A,
    Seqnotetab04 = 0xF5A66BC8,
    Seqnotetab05 = 0xD000CF15,
    Seqnotetab06 = 0x8ECD308A,
    Seqnotetab07 = 0x9C5683D7,
    Seqnotetab08 = 0xAC959D9E,
    Seqvelotab01 = 0xF3A3B434,
    Seqvelotab02 = 0xAE55D9F3,
    Seqvelotab03 = 0x39AB615C,
    Seqvelotab04 = 0x419C0049,
    Seqvelotab05 = 0x58EE0EBD,
    Seqvelotab06 = 0xC14785A9,
    Seqvelotab07 = 0xA90FD922,
    Seqvelotab08 = 0x785BB5B6,
    Tab027 = 0xC95C2CA6,
    Tab030 = 0xCB7A06D3,
    Tab033 = 0xDF691547,
    Tab036 = 0x82C3B8A5,
    Tab039 = 0xA405A422,
    Tab042 = 0x6A5426E5,
    Tab045 = 0xF4DAFD9C,
    Tab048 = 0x75E145C8,
    Tab051 = 0xC8A2A4B7,
    Tab054 = 0x2B7049C7,
    Tab057 = 0xD90415BE,
    Tab060 = 0xFD6FD961,
    Tab063 = 0x6EA63C52,
    Tab066 = 0xA58E818,
    Tab069 = 0x1AA671B9,
    Tab072 = 0x4C156C29,
    Tab075 = 0x71378A3A,
    Tab078 = 0x23069FD5,
    Tab081 = 0x932F6A10,
    Tab084 = 0xC2EDAB1A,
    Tab087 = 0xB83DCCD9,
    Tab090 = 0x2EFCECA8,
    Tab093 = 0x761929FC,
    Tab096 = 0x2CD9D61E,
    Tab099 = 0xE05666E,
    Tab102 = 0xC223E33E,
    Tab105 = 0x71054A7E,
    Tab108 = 0x62795757,
  }
  
  // Delegate method for receiving float messages from the patch context (thread-safe).
  // Example usage:
  /*
    void Start () {
        Hv_AccoPlayer_AudioLib script = GetComponent<Hv_AccoPlayer_AudioLib>();
        script.RegisterSendHook();
        script.FloatReceivedCallback += OnFloatMessage;
    }

    void OnFloatMessage(Hv_AccoPlayer_AudioLib.FloatMessage message) {
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
  public float bpm = 100.0f;
  public float seqNoteNum = 0.0f;
  public float seqNoteVelo = 0.0f;
  public float seqStepNum = 0.0f;
  public float seqVoiceNum = 0.0f;
  public AudioClip seqNoteTab01Clip = null;
  public AudioClip seqNoteTab02Clip = null;
  public AudioClip seqNoteTab03Clip = null;
  public AudioClip seqNoteTab04Clip = null;
  public AudioClip seqNoteTab05Clip = null;
  public AudioClip seqNoteTab06Clip = null;
  public AudioClip seqNoteTab07Clip = null;
  public AudioClip seqNoteTab08Clip = null;
  public AudioClip seqVeloTab01Clip = null;
  public AudioClip seqVeloTab02Clip = null;
  public AudioClip seqVeloTab03Clip = null;
  public AudioClip seqVeloTab04Clip = null;
  public AudioClip seqVeloTab05Clip = null;
  public AudioClip seqVeloTab06Clip = null;
  public AudioClip seqVeloTab07Clip = null;
  public AudioClip seqVeloTab08Clip = null;
  public AudioClip tab027Clip = null;
  public AudioClip tab030Clip = null;
  public AudioClip tab033Clip = null;
  public AudioClip tab036Clip = null;
  public AudioClip tab039Clip = null;
  public AudioClip tab042Clip = null;
  public AudioClip tab045Clip = null;
  public AudioClip tab048Clip = null;
  public AudioClip tab051Clip = null;
  public AudioClip tab054Clip = null;
  public AudioClip tab057Clip = null;
  public AudioClip tab060Clip = null;
  public AudioClip tab063Clip = null;
  public AudioClip tab066Clip = null;
  public AudioClip tab069Clip = null;
  public AudioClip tab072Clip = null;
  public AudioClip tab075Clip = null;
  public AudioClip tab078Clip = null;
  public AudioClip tab081Clip = null;
  public AudioClip tab084Clip = null;
  public AudioClip tab087Clip = null;
  public AudioClip tab090Clip = null;
  public AudioClip tab093Clip = null;
  public AudioClip tab096Clip = null;
  public AudioClip tab099Clip = null;
  public AudioClip tab102Clip = null;
  public AudioClip tab105Clip = null;
  public AudioClip tab108Clip = null;

  // internal state
  private Hv_AccoPlayer_Context _context;

  public bool IsInstantiated() {
    return (_context != null);
  }

  public void RegisterSendHook() {
    _context.RegisterSendHook();
  }
  
  // see Hv_AccoPlayer_AudioLib.Event for definitions
  public void SendEvent(Hv_AccoPlayer_AudioLib.Event e) {
    if (IsInstantiated()) _context.SendBangToReceiver((uint) e);
  }
  
  // see Hv_AccoPlayer_AudioLib.Parameter for definitions
  public float GetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter param) {
    switch (param) {
      case Parameter.Bpm: return bpm;
      case Parameter.Seqnotenum: return seqNoteNum;
      case Parameter.Seqnotevelo: return seqNoteVelo;
      case Parameter.Seqstepnum: return seqStepNum;
      case Parameter.Seqvoicenum: return seqVoiceNum;
      default: return 0.0f;
    }
  }

  public void SetFloatParameter(Hv_AccoPlayer_AudioLib.Parameter param, float x) {
    switch (param) {
      case Parameter.Bpm: {
        x = Mathf.Clamp(x, 40.0f, 200.0f);
        bpm = x;
        break;
      }
      case Parameter.Seqnotenum: {
        x = Mathf.Clamp(x, 0.0f, 127.0f);
        seqNoteNum = x;
        break;
      }
      case Parameter.Seqnotevelo: {
        x = Mathf.Clamp(x, 0.0f, 22000.0f);
        seqNoteVelo = x;
        break;
      }
      case Parameter.Seqstepnum: {
        x = Mathf.Clamp(x, 0.0f, 15.0f);
        seqStepNum = x;
        break;
      }
      case Parameter.Seqvoicenum: {
        x = Mathf.Clamp(x, 0.0f, 127.0f);
        seqVoiceNum = x;
        break;
      }
      default: return;
    }
    if (IsInstantiated()) _context.SendFloatToReceiver((uint) param, x);
  }
  
  public void SendFloatToReceiver(string receiverName, float x) {
    _context.SendFloatToReceiver(StringToHash(receiverName), x);
  }

  public void FillTableWithMonoAudioClip(string tableName, AudioClip clip) {
    if (clip.channels > 1) {
      Debug.LogWarning("Hv_AccoPlayer_AudioLib: Only loading first channel of '" +
          clip.name + "' into table '" +
          tableName + "'. Multi-channel files are not supported.");
    }
    float[] buffer = new float[clip.samples]; // copy only the 1st channel
    clip.GetData(buffer, 0);
    _context.FillTableWithFloatBuffer(StringToHash(tableName), buffer);
  }

  public void FillTableWithMonoAudioClip(uint tableHash, AudioClip clip) {
    if (clip.channels > 1) {
      Debug.LogWarning("Hv_AccoPlayer_AudioLib: Only loading first channel of '" +
          clip.name + "' into table '" +
          tableHash + "'. Multi-channel files are not supported.");
    }
    float[] buffer = new float[clip.samples]; // copy only the 1st channel
    clip.GetData(buffer, 0);
    _context.FillTableWithFloatBuffer(tableHash, buffer);
  }

  public void FillTableWithFloatBuffer(string tableName, float[] buffer) {
    _context.FillTableWithFloatBuffer(StringToHash(tableName), buffer);
  }

  public void FillTableWithFloatBuffer(uint tableHash, float[] buffer) {
    _context.FillTableWithFloatBuffer(tableHash, buffer);
  }

  public uint StringToHash(string str) {
    return _context.StringToHash(str);
  }

  private void Awake() {
    _context = new Hv_AccoPlayer_Context((double) AudioSettings.outputSampleRate);
    // Note: only copies first channel from audio clips
    if (seqNoteTab01Clip != null) {
      // load buffer seqNoteTab01
      int length = seqNoteTab01Clip.samples;
      float[] buffer = new float[length];
      seqNoteTab01Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Seqnotetab01, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-seqNoteTab01"), length);
    }
    if (seqNoteTab02Clip != null) {
      // load buffer seqNoteTab02
      int length = seqNoteTab02Clip.samples;
      float[] buffer = new float[length];
      seqNoteTab02Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Seqnotetab02, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-seqNoteTab02"), length);
    }
    if (seqNoteTab03Clip != null) {
      // load buffer seqNoteTab03
      int length = seqNoteTab03Clip.samples;
      float[] buffer = new float[length];
      seqNoteTab03Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Seqnotetab03, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-seqNoteTab03"), length);
    }
    if (seqNoteTab04Clip != null) {
      // load buffer seqNoteTab04
      int length = seqNoteTab04Clip.samples;
      float[] buffer = new float[length];
      seqNoteTab04Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Seqnotetab04, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-seqNoteTab04"), length);
    }
    if (seqNoteTab05Clip != null) {
      // load buffer seqNoteTab05
      int length = seqNoteTab05Clip.samples;
      float[] buffer = new float[length];
      seqNoteTab05Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Seqnotetab05, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-seqNoteTab05"), length);
    }
    if (seqNoteTab06Clip != null) {
      // load buffer seqNoteTab06
      int length = seqNoteTab06Clip.samples;
      float[] buffer = new float[length];
      seqNoteTab06Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Seqnotetab06, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-seqNoteTab06"), length);
    }
    if (seqNoteTab07Clip != null) {
      // load buffer seqNoteTab07
      int length = seqNoteTab07Clip.samples;
      float[] buffer = new float[length];
      seqNoteTab07Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Seqnotetab07, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-seqNoteTab07"), length);
    }
    if (seqNoteTab08Clip != null) {
      // load buffer seqNoteTab08
      int length = seqNoteTab08Clip.samples;
      float[] buffer = new float[length];
      seqNoteTab08Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Seqnotetab08, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-seqNoteTab08"), length);
    }
    if (seqVeloTab01Clip != null) {
      // load buffer seqVeloTab01
      int length = seqVeloTab01Clip.samples;
      float[] buffer = new float[length];
      seqVeloTab01Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Seqvelotab01, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-seqVeloTab01"), length);
    }
    if (seqVeloTab02Clip != null) {
      // load buffer seqVeloTab02
      int length = seqVeloTab02Clip.samples;
      float[] buffer = new float[length];
      seqVeloTab02Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Seqvelotab02, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-seqVeloTab02"), length);
    }
    if (seqVeloTab03Clip != null) {
      // load buffer seqVeloTab03
      int length = seqVeloTab03Clip.samples;
      float[] buffer = new float[length];
      seqVeloTab03Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Seqvelotab03, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-seqVeloTab03"), length);
    }
    if (seqVeloTab04Clip != null) {
      // load buffer seqVeloTab04
      int length = seqVeloTab04Clip.samples;
      float[] buffer = new float[length];
      seqVeloTab04Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Seqvelotab04, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-seqVeloTab04"), length);
    }
    if (seqVeloTab05Clip != null) {
      // load buffer seqVeloTab05
      int length = seqVeloTab05Clip.samples;
      float[] buffer = new float[length];
      seqVeloTab05Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Seqvelotab05, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-seqVeloTab05"), length);
    }
    if (seqVeloTab06Clip != null) {
      // load buffer seqVeloTab06
      int length = seqVeloTab06Clip.samples;
      float[] buffer = new float[length];
      seqVeloTab06Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Seqvelotab06, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-seqVeloTab06"), length);
    }
    if (seqVeloTab07Clip != null) {
      // load buffer seqVeloTab07
      int length = seqVeloTab07Clip.samples;
      float[] buffer = new float[length];
      seqVeloTab07Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Seqvelotab07, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-seqVeloTab07"), length);
    }
    if (seqVeloTab08Clip != null) {
      // load buffer seqVeloTab08
      int length = seqVeloTab08Clip.samples;
      float[] buffer = new float[length];
      seqVeloTab08Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Seqvelotab08, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-seqVeloTab08"), length);
    }
    if (tab027Clip != null) {
      // load buffer tab027
      int length = tab027Clip.samples;
      float[] buffer = new float[length];
      tab027Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab027, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab027"), length);
    }
    if (tab030Clip != null) {
      // load buffer tab030
      int length = tab030Clip.samples;
      float[] buffer = new float[length];
      tab030Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab030, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab030"), length);
    }
    if (tab033Clip != null) {
      // load buffer tab033
      int length = tab033Clip.samples;
      float[] buffer = new float[length];
      tab033Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab033, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab033"), length);
    }
    if (tab036Clip != null) {
      // load buffer tab036
      int length = tab036Clip.samples;
      float[] buffer = new float[length];
      tab036Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab036, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab036"), length);
    }
    if (tab039Clip != null) {
      // load buffer tab039
      int length = tab039Clip.samples;
      float[] buffer = new float[length];
      tab039Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab039, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab039"), length);
    }
    if (tab042Clip != null) {
      // load buffer tab042
      int length = tab042Clip.samples;
      float[] buffer = new float[length];
      tab042Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab042, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab042"), length);
    }
    if (tab045Clip != null) {
      // load buffer tab045
      int length = tab045Clip.samples;
      float[] buffer = new float[length];
      tab045Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab045, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab045"), length);
    }
    if (tab048Clip != null) {
      // load buffer tab048
      int length = tab048Clip.samples;
      float[] buffer = new float[length];
      tab048Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab048, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab048"), length);
    }
    if (tab051Clip != null) {
      // load buffer tab051
      int length = tab051Clip.samples;
      float[] buffer = new float[length];
      tab051Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab051, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab051"), length);
    }
    if (tab054Clip != null) {
      // load buffer tab054
      int length = tab054Clip.samples;
      float[] buffer = new float[length];
      tab054Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab054, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab054"), length);
    }
    if (tab057Clip != null) {
      // load buffer tab057
      int length = tab057Clip.samples;
      float[] buffer = new float[length];
      tab057Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab057, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab057"), length);
    }
    if (tab060Clip != null) {
      // load buffer tab060
      int length = tab060Clip.samples;
      float[] buffer = new float[length];
      tab060Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab060, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab060"), length);
    }
    if (tab063Clip != null) {
      // load buffer tab063
      int length = tab063Clip.samples;
      float[] buffer = new float[length];
      tab063Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab063, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab063"), length);
    }
    if (tab066Clip != null) {
      // load buffer tab066
      int length = tab066Clip.samples;
      float[] buffer = new float[length];
      tab066Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab066, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab066"), length);
    }
    if (tab069Clip != null) {
      // load buffer tab069
      int length = tab069Clip.samples;
      float[] buffer = new float[length];
      tab069Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab069, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab069"), length);
    }
    if (tab072Clip != null) {
      // load buffer tab072
      int length = tab072Clip.samples;
      float[] buffer = new float[length];
      tab072Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab072, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab072"), length);
    }
    if (tab075Clip != null) {
      // load buffer tab075
      int length = tab075Clip.samples;
      float[] buffer = new float[length];
      tab075Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab075, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab075"), length);
    }
    if (tab078Clip != null) {
      // load buffer tab078
      int length = tab078Clip.samples;
      float[] buffer = new float[length];
      tab078Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab078, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab078"), length);
    }
    if (tab081Clip != null) {
      // load buffer tab081
      int length = tab081Clip.samples;
      float[] buffer = new float[length];
      tab081Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab081, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab081"), length);
    }
    if (tab084Clip != null) {
      // load buffer tab084
      int length = tab084Clip.samples;
      float[] buffer = new float[length];
      tab084Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab084, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab084"), length);
    }
    if (tab087Clip != null) {
      // load buffer tab087
      int length = tab087Clip.samples;
      float[] buffer = new float[length];
      tab087Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab087, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab087"), length);
    }
    if (tab090Clip != null) {
      // load buffer tab090
      int length = tab090Clip.samples;
      float[] buffer = new float[length];
      tab090Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab090, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab090"), length);
    }
    if (tab093Clip != null) {
      // load buffer tab093
      int length = tab093Clip.samples;
      float[] buffer = new float[length];
      tab093Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab093, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab093"), length);
    }
    if (tab096Clip != null) {
      // load buffer tab096
      int length = tab096Clip.samples;
      float[] buffer = new float[length];
      tab096Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab096, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab096"), length);
    }
    if (tab099Clip != null) {
      // load buffer tab099
      int length = tab099Clip.samples;
      float[] buffer = new float[length];
      tab099Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab099, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab099"), length);
    }
    if (tab102Clip != null) {
      // load buffer tab102
      int length = tab102Clip.samples;
      float[] buffer = new float[length];
      tab102Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab102, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab102"), length);
    }
    if (tab105Clip != null) {
      // load buffer tab105
      int length = tab105Clip.samples;
      float[] buffer = new float[length];
      tab105Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab105, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab105"), length);
    }
    if (tab108Clip != null) {
      // load buffer tab108
      int length = tab108Clip.samples;
      float[] buffer = new float[length];
      tab108Clip.GetData(buffer, 0);
      _context.FillTableWithFloatBuffer((uint) Hv_AccoPlayer_AudioLib.Table.Tab108, buffer);
      _context.SendFloatToReceiver(_context.StringToHash("setTableSize-tab108"), length);
    }
  }
  
  private void Start() {
    _context.SendFloatToReceiver((uint) Parameter.Bpm, bpm);
    _context.SendFloatToReceiver((uint) Parameter.Seqnotenum, seqNoteNum);
    _context.SendFloatToReceiver((uint) Parameter.Seqnotevelo, seqNoteVelo);
    _context.SendFloatToReceiver((uint) Parameter.Seqstepnum, seqStepNum);
    _context.SendFloatToReceiver((uint) Parameter.Seqvoicenum, seqVoiceNum);
  }
  
  private void Update() {
    // retreive sent messages
    if (_context.IsSendHookRegistered()) {
      Hv_AccoPlayer_AudioLib.FloatMessage tempMessage;
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

class Hv_AccoPlayer_Context {

#if UNITY_IOS && !UNITY_EDITOR
  private const string _dllName = "__Internal";
#else
  private const string _dllName = "Hv_AccoPlayer_AudioLib";
#endif

  // Thread-safe message queue
  public class SendMessageQueue {
    private readonly object _msgQueueSync = new object();
    private readonly Queue<Hv_AccoPlayer_AudioLib.FloatMessage> _msgQueue = new Queue<Hv_AccoPlayer_AudioLib.FloatMessage>();

    public Hv_AccoPlayer_AudioLib.FloatMessage GetNextMessage() {
      lock (_msgQueueSync) {
        return (_msgQueue.Count != 0) ? _msgQueue.Dequeue() : null;
      }
    }

    public void AddMessage(string receiverName, float value) {
      Hv_AccoPlayer_AudioLib.FloatMessage msg = new Hv_AccoPlayer_AudioLib.FloatMessage(receiverName, value);
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
  private static extern IntPtr hv_AccoPlayer_new_with_options(double sampleRate, int poolKb, int inQueueKb, int outQueueKb);

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

  public Hv_AccoPlayer_Context(double sampleRate, int poolKb=10, int inQueueKb=7, int outQueueKb=2) {
    gch = GCHandle.Alloc(msgQueue);
    _context = hv_AccoPlayer_new_with_options(sampleRate, poolKb, inQueueKb, outQueueKb);
    hv_setPrintHook(_context, new PrintHook(OnPrint));
    hv_setUserData(_context, GCHandle.ToIntPtr(gch));
  }

  ~Hv_AccoPlayer_Context() {
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

  public void FillTableWithFloatBuffer(uint tableHash, float[] buffer) {
    if (hv_table_getBuffer(_context, tableHash) != IntPtr.Zero) {
      hv_table_setLength(_context, tableHash, (uint) buffer.Length);
      Marshal.Copy(buffer, 0, hv_table_getBuffer(_context, tableHash), buffer.Length);
    } else {
      Debug.Log(string.Format("Table '{0}' doesn't exist in the patch context.", tableHash));
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
    //Debug.Log(string.Format("{0} [{1:0.000}]: {2}", printName, timeInSecs, str));
  }

  [MonoPInvokeCallback(typeof(SendHook))]
  private static void OnMessageSent(IntPtr context, string sendName, uint sendHash, IntPtr message) {
    if (hv_msg_hasFormat(message, "f")) {
      SendMessageQueue msgQueue = (SendMessageQueue) GCHandle.FromIntPtr(hv_getUserData(context)).Target;
      msgQueue.AddMessage(sendName, hv_msg_getFloat(message, 0));
    }
  }
}
