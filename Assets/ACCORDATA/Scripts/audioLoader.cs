using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioLoader : MonoBehaviour
{
    [Header("-> ACCORDATA <- Expects 28 audio clips in the clips array")]
    public List<AudioClip[]> ensembles;
    public AudioClip[] piano;
    public AudioClip cleanerClip;

    private Hv_AccoPlayer_AudioLib pd;

    private void OnEnable()
    {
        pd = GetComponent<Hv_AccoPlayer_AudioLib>();
    }

    void Start()
    {
        ensembles = new List<AudioClip[]>();
        ensembles.Add(piano);
        loadEnsemble(0);
        Debug.Log("Audio loaded!");
    }

    private void loadEnsemble(int ens)
    {
        for (int i = 0; i < 28; i++)
        {

            float[] buffer = new float[ensembles[ens][i].samples];

            // fill the table
            ensembles[ens][i].GetData(buffer, 0);

            int tabNoteNum = 27 + (i * 3);
            string tableName = "tab";
            if (tabNoteNum < 100)
                tableName += "0";
            tableName += tabNoteNum.ToString();
            //Debug.Log(tableName);
            pd.FillTableWithFloatBuffer(tableName, buffer);
        }
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
