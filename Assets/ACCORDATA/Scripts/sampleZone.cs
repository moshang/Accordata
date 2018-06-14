using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class sampleZone : MonoBehaviour
{
    public AudioClip clip;
    [Range(0, 127)]
    public int rootNoteNumber;
    [Range(0, 127)]
    public int noteMin;
    [Range(0, 127)]
    public int noteMax;
    [Range(0, 127)]
    public int veloMin;
    [Range(0, 127)]
    public int veloMax;
}
