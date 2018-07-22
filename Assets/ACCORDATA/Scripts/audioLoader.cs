using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioLoader : MonoBehaviour
{
    [Header("-> ACCORDATA <- Expects 28 audio clips in the clips array")]
    public AudioClip[] clips;
    private Hv_AccoPlayer_AudioLib pd;

    void Start()
    {
        pd = GetComponent<Hv_AccoPlayer_AudioLib>();

        for (int i = 0; i < 28; i++)
        {
            // expecting a stereo file
            float[] buffer = new float[clips[i].samples];

            // fill the table
            clips[i].GetData(buffer, 0);

            int tabNoteNum = 27 + (i * 3);
            string tableName = "tab";
            if (tabNoteNum < 100)
                tableName += "0";
            tableName += tabNoteNum.ToString();
            //Debug.Log(tableName);
            pd.FillTableWithFloatBuffer(tableName, buffer);
        }

        Debug.Log("Audio loaded!");
    }
}
