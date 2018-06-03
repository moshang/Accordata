// Copyright 2017 Matt Tytel

using UnityEditor;
using UnityEngine;

namespace AudioHelm
{
    [CustomEditor(typeof(AudioHelmClock))]
    class AudioHelmClockUI : Editor
    {
        SerializedObject serialized;

        const float kMinBpm = 20.0f;
        const float kMaxBpm = 400.0f;

        void OnEnable()
        {
            serialized = new SerializedObject(target);
        }

        public override void OnInspectorGUI()
        {
            AudioHelmClock clock = target as AudioHelmClock;
            serialized.Update();
            clock.bpm = EditorGUILayout.Slider("BPM", clock.bpm, kMinBpm, kMaxBpm);
            clock.pause = EditorGUILayout.Toggle("Pause", clock.pause);
            serialized.ApplyModifiedProperties();
        }
    }
}
