// Copyright 2017 Matt Tytel

using UnityEngine;
using System.IO;

namespace AudioHelm
{
    public class HelmPatch : MonoBehaviour
    {
        public Object patchObject;
        public HelmPatchFormat patchData;

        public void LoadPatchData(string filePath)
        {
            string patchText = File.ReadAllText(filePath);
            patchData = JsonUtility.FromJson<HelmPatchFormat>(patchText);
        }
    }
}
