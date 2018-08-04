using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uiController : MonoBehaviour
{
    [Header("-> ACCORDATA <-")]
    public seqGenerator seqGen;
    public dataLoader data;
    // BUTTONS
    public Text scaleButtonTxt;
    public Text styleButtonTxt;
    public Toggle debugOverride;
    public Slider aqiSlider;

    // SITECARD
    public RectTransform siteCard;
    public RectTransform siteCardAqi;
    public RectTransform siteCardDetails;
    Image siteCardBG;
    Image siteCardAqiBG;
    Image siteCardDetailsBG;
    public Text siteNameTxt;
    public Text siteAqiTxt;
    public Text mainPollutant;
    public Text pm10;
    public Text pm10_avg;
    public Text pm25;
    public Text pm25_avg;
    public Text o3;
    public Text o3_8h;
    public Text so2;
    public Text co;
    public Text co_8h;
    public Text no2;

    // TARGET HIGHLIGHT
    public SpriteRenderer highlight;
    private Vector3 highlightOriginalScale = new Vector3(10, 10, 1);

    private void Start()
    {
        //siteCardBG = siteCard.GetComponent<Image>();
        siteCardAqiBG = siteCardAqi.GetComponent<Image>();
        //siteCardDetailsBG = siteCardDetails.GetComponent<Image>();
        clock.OnBeat += everyBeat;
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
            siteAqiTxt.text = "AQI: " + ((int)aqiSlider.value).ToString();
            updateBGColors((int)aqiSlider.value);
        }
    }

    public void showSelectedSite(GameObject selectedSite)
    {
        for (int i = 0; i < data.sites.Length; i++)
        {
            if (selectedSite == data.sites[i].marker)
            {
                updateCard(i);
                break;
            }
        }
    }

    public void updateCard(int siteIndex)
    {
        //fill the card with data
        siteNameTxt.text = data.sites[siteIndex].name;
        siteAqiTxt.text = "AQI: " + data.sites[siteIndex].aqi;

        //mainPollutant.text = "Main Pollutant: " + data.sites[siteIndex].mainPollutant;
        pm25.text = "PM2.5: " + data.sites[siteIndex].PM25;
        //pm25_avg.text = "PM2.5 Avg: " + data.sites[siteIndex].PM25_AVG;
        pm10.text = "PM10: " + data.sites[siteIndex].PM10;
        //pm10_avg.text = "PM10 Avg: " + data.sites[siteIndex].PM10_AVG;
        o3.text = "O3: " + data.sites[siteIndex].O3;
        //o3_8h.text = "O3 8h: " + data.sites[siteIndex].O3_8;
        so2.text = "SO2: " + data.sites[siteIndex].SO2;
        co.text = "CO: " + data.sites[siteIndex].CO;
        //co_8h.text = "CO 8h: " + data.sites[siteIndex].CO_8;
        no2.text = "NO2: " + data.sites[siteIndex].NO2;

        // change the colors
        updateBGColors(data.sites[siteIndex].aqi);

        // move the target highlight
        highlight.transform.position = data.sites[siteIndex].marker.transform.position;
    }

    void updateBGColors(int aqiVal)
    {
        siteCardAqiBG.color = getAqiColor(aqiVal);
        //siteCardBG.color = getAqiColor(aqiVal);
        //siteCardDetailsBG.color = getAqiColor(aqiVal);
    }

    public Color32 getAqiColor(int aqiVal)
    {
        Color32 newColor = new Color32();
        if (aqiVal > 300)
            newColor = new Color32(136, 14, 79, 200);
        else if (aqiVal > 200)
            newColor = new Color32(173, 20, 87, 200);
        else if (aqiVal > 150)
            newColor = new Color32(197, 57, 41, 200);
        else if (aqiVal > 100)
            newColor = new Color32(245, 124, 0, 200);
        else if (aqiVal > 50)
            newColor = new Color32(251, 192, 45, 200);
        else
            newColor = new Color32(104, 159, 56, 200);
        return newColor;
    }

    public void everyBeat(int beatNum)
    {
        if (beatNum % 2 == 0)
            StartCoroutine(bounceHighlight());
    }

    IEnumerator bounceHighlight()
    {
        float startTime = Time.time;
        float targetDurMillis = 60000 / clock.bpm; // for 1 beat
        float linearProgress = 0;
        Vector3 highlightNewScale;
        while (linearProgress < 1)
        {
            highlightNewScale = Vector3.Lerp(highlightOriginalScale * 3, highlightOriginalScale, Easings.QuarticEaseIn(linearProgress));
            linearProgress = (Time.time - startTime) * 1000 / targetDurMillis;
            highlight.transform.localScale = highlightNewScale;
            yield return null;
        }
    }
}
