/* Copyright (c) Jean Marais / MoShang 2018. Licensed under GPLv3.
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class audioLoader : MonoBehaviour
{
    [Header("-> ACCORDATA <- Expects 28 audio clips in the clips array")]
    public List<AudioClip[]> ensembles;
    public AudioClip[] piano;
    public AudioClip[] pianoDrops;
    public AudioClip[] pianoMarimba;
    public AudioClip cleanerClip;

    private Hv_AccoPlayer_AudioLib pd;
    private clock clk;

    // to allow for a delay when switching ensembles
    private int ensToLoad;
    private bool clockWasRunning;
    private int currentEns;
    float ensChangeDelay = 0.3f;

    public Toggle playToggle;

    private void OnEnable()
    {
        pd = GetComponent<Hv_AccoPlayer_AudioLib>();
        clk = GetComponent<clock>();
    }

    void Start()
    {
        ensembles = new List<AudioClip[]>();
        ensembles.Add(piano); // ens 0
        ensembles.Add(pianoDrops); // ens 1
        ensembles.Add(pianoMarimba); // ens 2
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            ensChangeDelay = 0.3f;
        else
            ensChangeDelay = 1f;
        Debug.Log("Audio loaded!");
    }

    public void loadEns(int ens)
    {
        if (ens == currentEns) // don't bother loading the ensemble if we already have the correct one loaded
            return;
        else
            currentEns = ens;
        CancelInvoke(); // stop any inocations that may already be running
        ensToLoad = ens;
        clockWasRunning = clock.isRunning;
        if (clockWasRunning)
            playToggle.isOn = false; //clk.stopPlayback();

        Invoke("ldEns", ensChangeDelay);
    }

    private void ldEns()
    {
        for (int i = 0; i < 28; i++)
        {

            float[] buffer = new float[ensembles[ensToLoad][i].samples];

            // fill the table
            ensembles[ensToLoad][i].GetData(buffer, 0);

            int tabNoteNum = 27 + (i * 3);
            string tableName = "tab";
            if (tabNoteNum < 100)
                tableName += "0";
            tableName += tabNoteNum.ToString();
            //Debug.Log(tableName);
            pd.FillTableWithFloatBuffer(tableName, buffer);
        }
        if (clockWasRunning)
            Invoke("restartClock", ensChangeDelay);
    }

    private void restartClock()
    {
        //if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        playToggle.isOn = true; //clk.startPlayback();
    }

    public void clearSeqTables()
    {
        for (int i = 0; i < 16; i++)
        {
            float[] buffer = new float[cleanerClip.samples];
            cleanerClip.GetData(buffer, 0);

            if (i < 8)
            {
                string tableName = "seqNoteTab0" + (i + 1).ToString();
                pd.FillTableWithFloatBuffer(tableName, buffer);
            }
            else
            {
                string tableName = "seqVeloTab0" + (i - 7).ToString();
                pd.FillTableWithFloatBuffer(tableName, buffer);
            }
        }
    }
}
