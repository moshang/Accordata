// Copyright 2017 Matt Tytel

using UnityEditor;
using UnityEngine;

namespace AudioHelm
{
    [CustomEditor(typeof(HelmPatch))]
    class HelmPatchUI : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            HelmPatch patch = target as HelmPatch;
            Object oldFile = patch.patchObject;
            patch.patchObject = EditorGUILayout.ObjectField("Patch File", oldFile, typeof(Object), false);
            if (oldFile != patch.patchObject)
            {
                Undo.RecordObject(patch, "Change Patch File");
                string path = AssetDatabase.GetAssetPath(patch.patchObject);
                patch.LoadPatchData(path);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
