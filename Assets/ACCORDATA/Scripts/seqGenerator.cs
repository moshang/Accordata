using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AccordataStyle;

public enum Scale { major, minor, dimished, pentatonic };
public class seqGenerator : MonoBehaviour
{
    [Header("--> ACCORDATA <--")]
    public readonly int[] triad = { 0, 2, 4, 6, 7 };
    public readonly int[,] scales = {   /*major*/       { 0, 2, 4, 5, 7, 9, 11, 12 }, 
                                        /*minor*/       { 0, 2, 3, 5, 7, 9, 11, 12 }, 
                                        /*diminished*/  { 0, 2, 3, 5, 6, 8, 9, 11 }, 
                                        /*pentatonic*/  { 0, 2, 4, 7, 9, 12, 14, 16 } };
    public Scale scale = 0;
    public int numScales;
    public int originalRootNote = 48;
    private int newValuesAtBar = 0;
    private int thisSiteIndex = 0;
    private int nextSiteIndex = 1;
    private int thisHour = 71;
    private int nextHour = 70;
    private dataLoader data;
    public uiController uiCtrl;
    private int seqLength = 16;

    [Header("Styles")]
    public Style[] styles;
    public int currentStyleIndex;
    public GameObject styleSelectorPref;
    public GameObject stylesListGO;

    // current site values
    private int aqiVal;
    private float tempVal;
    private float windVal;
    private float humidityVal;
    private float rainVal;

    void Start()
    {
        data = GetComponent<dataLoader>();
        clock.OnBar += everyBar;
        //clock.OnBeat += everyBeat;
        //clock.OnPulse += everyPulse;

        numScales = Scale.GetNames(typeof(Scale)).Length;

        styles = transform.Find("Styles").GetComponents<Style>();

        // add UI buttons for each style
        for (int i = 0; i < styles.Length; i++)
        {
            GameObject newStyleSelector = Instantiate(styleSelectorPref, stylesListGO.transform);
            selectStyle slctStyle = newStyleSelector.GetComponentInChildren<selectStyle>();
            slctStyle.styleIndex = i;
            slctStyle.GetComponent<Toggle>().group = stylesListGO.GetComponent<ToggleGroup>();
            localization local = newStyleSelector.GetComponentInChildren<localization>();
            if (local == null)
                Debug.Break();
            local.termEnglish = styles[i].StyleNameEng;
            local.termChinese = styles[i].StyleNameChTw;
            //if ()
            newStyleSelector.GetComponentInChildren<Text>().text = local.termEnglish;
        }

        // set the first style as default
        switchStyle(1);
    }
    /// <summary>
    /// Switch to the style selected by the user
    /// </summary>
    /// <param name="styleIndex"></param>
    public void switchStyle(int styleIndex)
    {
        int newSeqAtBar = styles[currentStyleIndex].newSeqAtBar;
        currentStyleIndex = styleIndex;
        styles[styleIndex].initStyle(newSeqAtBar);
    }

    void everyBeat(int beat)
    {
    }

    void everyBar(int bar)
    {
        if (newValuesAtBar == bar) // grab a new set of AQI & weather values
        {
            if (bar != 0)
            {
                if (uiController.currentMode == Mode.mapAll || uiController.currentMode == Mode.mapCounty)
                {
                    thisSiteIndex = nextSiteIndex;
                    uiCtrl.currentSiteIndex = thisSiteIndex;
                    nextSiteIndex = (thisSiteIndex + 1) % dataLoader.numSites;
                }
                else if (uiController.currentMode == Mode.site72Hr)
                {
                    thisHour = nextHour;
                    uiCtrl.currentHour = thisHour;
                    uiCtrl.update72Hr();
                    nextHour = ((thisHour - 1) >= 0) ? thisHour - 1 : dataLoader.hoursToRead - 1;
                }
            }
            Debug.Log("thisSiteIndex: " + thisSiteIndex);
            aqiVal = data.sites[thisHour, thisSiteIndex].aqi;
            tempVal = data.sites[thisHour, thisSiteIndex].temperature;
            windVal = data.sites[thisHour, thisSiteIndex].windspeed;
            humidityVal = data.sites[thisHour, thisSiteIndex].humidity;
            rainVal = data.sites[thisHour, thisSiteIndex].rainfall;
            uiCtrl.updateCard(thisSiteIndex);
            newValuesAtBar = bar + styles[currentStyleIndex].newValuesEveryXBars;
        }
        styles[currentStyleIndex].makeSeq(bar, aqiVal, tempVal, windVal, humidityVal, rainVal);
    }
}
