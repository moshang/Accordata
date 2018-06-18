using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uiController : MonoBehaviour
{
    [Header("-> ACCRODATA <-")]
    public seqGenerator seqGen;
    // BUTTONS
    public Text scaleButtonTxt;

    // FUNCTIONS
    public void nextScale()
    {
        seqGen.scale = (scale)((int)(seqGen.scale + 1) % seqGen.numScales);
        scaleButtonTxt.text = utils.UppercaseFirst(System.Enum.GetName(typeof(scale), seqGen.scale));
    }
}
