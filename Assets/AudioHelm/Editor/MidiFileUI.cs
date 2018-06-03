// Copyright 2017 Matt Tytel

using UnityEditor;
using UnityEngine;

namespace AudioHelm
{
    [CustomEditor(typeof(MidiFile))]
    class MidiFileUI : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MidiFile patch = target as MidiFile;
            Object oldFile = patch.midiObject;
            patch.midiObject = EditorGUILayout.ObjectField("Midi File", oldFile, typeof(Object), false);
            if (oldFile != patch.midiObject)
            {
                Undo.RecordObject(patch, "Change Patch File");
                string path = AssetDatabase.GetAssetPath(patch.midiObject);
                patch.LoadMidiData(path);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
