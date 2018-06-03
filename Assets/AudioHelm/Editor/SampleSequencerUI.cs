// Copyright 2017 Matt Tytel

using UnityEditor;
using UnityEngine;
using System.IO;

namespace AudioHelm
{
    [CustomEditor(typeof(SampleSequencer))]
    class SampleSequencerUI : Editor
    {
        const float keyboardWidth = 30.0f;
        const float scrollWidth = 15.0f;

        SerializedProperty length;
        SerializedProperty loop;
        SerializedProperty division;
        SerializedProperty allNotes;
        SerializedProperty zoom;
        SerializedProperty autoScroll;
        SerializedProperty noteOnEvent;
        SerializedProperty noteOffEvent;
        SerializedProperty beatEvent;

        SequencerUI sequencer = new SequencerUI(keyboardWidth, scrollWidth + 1);
        SequencerPositionUI sequencerPosition = new SequencerPositionUI(keyboardWidth, scrollWidth);

        float positionHeight = 10.0f;
        float sequencerHeight = 440.0f;
        const float minWidth = 200.0f;

        void OnEnable()
        {
            length = serializedObject.FindProperty("length");
            loop = serializedObject.FindProperty("loop");
            division = serializedObject.FindProperty("division");
            allNotes = serializedObject.FindProperty("allNotes");
            zoom = serializedObject.FindProperty("zoom");
            autoScroll = serializedObject.FindProperty("autoScroll");
            noteOnEvent = serializedObject.FindProperty("noteOnEvent");
            noteOffEvent = serializedObject.FindProperty("noteOffEvent");
            beatEvent = serializedObject.FindProperty("beatEvent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Color prev_color = GUI.backgroundColor;
            GUILayout.Space(5f);
            SampleSequencer sampleSequencer = target as SampleSequencer;
            Sampler sampler = sampleSequencer.GetComponent<Sampler>();
            if (sampler)
            {
                sequencer.minKey = sampler.GetMinKey();
                sequencer.maxKey = sampler.GetMaxKey();
            }
            else
            {
                sequencer.minKey = 0;
                sequencer.maxKey = Utils.kMidiSize - 1;
            }

            Rect sequencerPositionRect = GUILayoutUtility.GetRect(minWidth, positionHeight, GUILayout.ExpandWidth(true));
            float seqHeight = Mathf.Min(sequencerHeight, sequencer.GetMaxHeight());
            Rect rect = GUILayoutUtility.GetRect(minWidth, seqHeight, GUILayout.ExpandWidth(true));

            if (sequencer.DoSequencerEvents(rect, sampleSequencer, allNotes))
                Repaint();

            float startWindow = sequencer.GetMinVisibleTime() / length.intValue;
            float endWindow = sequencer.GetMaxVisibleTime(rect.width) / length.intValue;
            sequencerPosition.DrawSequencerPosition(sequencerPositionRect, sampleSequencer, startWindow, endWindow);

            if (rect.height == seqHeight)
                sequencer.DrawSequencer(rect, sampleSequencer, zoom.floatValue, allNotes);
            GUILayout.Space(5f);
            GUI.backgroundColor = prev_color;

            if (GUILayout.Button(new GUIContent("Clear Sequencer", "Remove all notes from the sequencer.")))
            {
                Undo.RecordObject(sampleSequencer, "Clear Sequencer");

                for (int i = 0; i < allNotes.arraySize; ++i)
                {
                    SerializedProperty noteRow = allNotes.GetArrayElementAtIndex(i);
                    SerializedProperty notes = noteRow.FindPropertyRelative("notes");
                    notes.ClearArray();
                }
                sampleSequencer.Clear();
            }

            if (GUILayout.Button(new GUIContent("Load MIDI File", "Load a MIDI sequence into this sequencer.")))
            {
                string path = EditorUtility.OpenFilePanel("Load MIDI Sequence", "", "mid");
                if (path.Length != 0)
                {
                    Undo.RecordObject(sampleSequencer, "Load MIDI File");
                    sampleSequencer.ReadMidiFile(new FileStream(path, FileMode.Open, FileAccess.Read));
                }
            }

            EditorGUILayout.PropertyField(length);
            EditorGUILayout.PropertyField(loop);
            sampleSequencer.length = Mathf.Max(sampleSequencer.length, 1);

            GUILayout.Space(5f);
            EditorGUILayout.LabelField("View Options", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(division);
            EditorGUILayout.Slider(zoom, 0.0f, 1.0f);
            EditorGUILayout.PropertyField(autoScroll);

            GUILayout.Space(5f);
            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(noteOnEvent);
            EditorGUILayout.PropertyField(noteOffEvent);
            EditorGUILayout.PropertyField(beatEvent);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
