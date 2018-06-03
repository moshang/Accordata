// Copyright 2017 Matt Tytel

using UnityEditor;
using UnityEngine;

namespace AudioHelm
{
    [CustomEditor(typeof(Sampler))]
    class SamplerUI : Editor
    {
        const int scrollWidth = 15;

        KeyboardUI keyboard = new KeyboardUI();
        KeyzoneEditorUI keyzonesUI = new KeyzoneEditorUI(scrollWidth);
        SerializedProperty numVoices;
        SerializedProperty velocityTracking;
        SerializedProperty useNoteOff;
        SerializedProperty keyzones;

        const int keyzoneHeight = 120;
        const float minWidth = 200.0f;
        const float keyboardHeight = 60.0f;

        void OnEnable()
        {
            numVoices = serializedObject.FindProperty("numVoices");
            velocityTracking = serializedObject.FindProperty("velocityTracking");
            useNoteOff = serializedObject.FindProperty("useNoteOff_");
            keyzones = serializedObject.FindProperty("keyzones");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Color prev_color = GUI.backgroundColor;
            GUILayout.Space(5f);
            Sampler sampler = target as Sampler;
            int height = Mathf.Max(keyzoneHeight, keyzonesUI.GetHeight(sampler));

            Rect keyboardRect = GUILayoutUtility.GetRect(minWidth, keyboardHeight, GUILayout.ExpandWidth(true));
            GUILayout.Space(10.0f);
            Rect keyzonesRect = GUILayoutUtility.GetRect(minWidth, height, GUILayout.ExpandWidth(true));

            if (keyboard.DoKeyboardEvents(keyboardRect, sampler))
                Repaint();

            if (keyzonesUI.DoKeyzoneEvents(keyzonesRect, sampler, keyzones))
                Repaint();

            if (keyzonesRect.height == height)
                keyzonesUI.DrawKeyzones(keyzonesRect, sampler, keyzones);

            keyboard.DrawKeyboard(keyboardRect);

            GUILayout.Space(5f);
            GUI.backgroundColor = prev_color;

            EditorGUILayout.IntSlider(numVoices, 2, 20);
            EditorGUILayout.Slider(velocityTracking, 0.0f, 1.0f);
            EditorGUI.BeginChangeCheck();
            useNoteOff.boolValue = EditorGUILayout.Toggle("Use Note Off", useNoteOff.boolValue);
            if (EditorGUI.EndChangeCheck() && useNoteOff.boolValue)
                sampler.AllNotesOff();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
