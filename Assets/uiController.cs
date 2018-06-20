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
    public Text styleButtonTxt;
    public Toggle debugOverride;
    public Slider aqiSlider;

    // SITECARD
    public RectTransform siteCard;
    Image siteCardBG;
    public Text siteNameTxt;
    public Text siteAqiTxt;

    private void Start()
    {
        siteCardBG = siteCard.GetComponent<Image>();
    }

    // FUNCTIONS
    public void nextScale()
    {
        seqGen.scale = (scale)((int)(seqGen.scale + 1) % seqGen.numScales);
        scaleButtonTxt.text = utils.UppercaseFirst(System.Enum.GetName(typeof(scale), seqGen.scale));
    }

    public void nextStyle()
    {
        seqGen.genStyle = (style)((int)(seqGen.genStyle + 1) % seqGen.numStyles);
        styleButtonTxt.text = utils.UppercaseFirst(System.Enum.GetName(typeof(style), seqGen.genStyle));
    }

    public void handleToggle()
    {
        if (debugOverride.isOn)
            seqGen.aqiDebug = true;
        else
            seqGen.aqiDebug = false;
    }

    public void handleSlider()
    {
        if (seqGen.aqiDebug)
        {
            seqGen.aqi = (int)aqiSlider.value;
            updateCard(siteNameTxt.text, seqGen.aqi);
        }
    }

    public void updateCard(string nameTxt, int aqiVal)
    {
        siteNameTxt.text = nameTxt;
        siteAqiTxt.text = "AQI: " + aqiVal;
        siteCardBG.color = getAqiColor(aqiVal);
    }

    Color32 getAqiColor(int aqiVal)
    {
        Color32 newColor = new Color32();
        if (aqiVal > 300)
            newColor = new Color32(136, 14, 79, 255);
        else if (aqiVal > 200)
            newColor = new Color32(173, 20, 87, 255);
        else if (aqiVal > 150)
            newColor = new Color32(197, 57, 41, 255);
        else if (aqiVal > 100)
            newColor = new Color32(245, 124, 0, 255);
        else if (aqiVal > 50)
            newColor = new Color32(251, 192, 45, 255);
        else
            newColor = new Color32(104, 159, 56, 255);
        return newColor;
    }
}
