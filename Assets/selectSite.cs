using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class selectSite : MonoBehaviour
{
    // -> ACCORDATA <-
    public int siteIndex;
    private Toggle toggle;
    private seqGenerator seqGen;

    private void OnEnable()
    {
        toggle = GetComponent<Toggle>();
        if (toggle == null)
            toggle = GetComponent<Toggle>();
        if (seqGen == null)
            seqGen = GameObject.Find("AccordataController").GetComponent<seqGenerator>();
    }

    public void setSite()
    {
        seqGen.data.getData72HR(siteIndex);
    }
}
