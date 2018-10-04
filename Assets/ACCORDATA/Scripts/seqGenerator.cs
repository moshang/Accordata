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
using AccordataStyle;
using UnityEngine.Audio;

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
    public int thisSiteIndex = 0;
    public int nextSiteIndex = 1;
    public int countySiteIndex = 0; // the index into the array of siteIndices for a specific county
    public int thisHour = 0;
    public int nextHour = 70;
    public dataLoader data;
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

    // reference to the mixer
    public AudioMixer mixer;

    [Header("Default SFX Clips")]
    public AudioClip tempLight;
    public AudioClip tempMedium;
    public AudioClip tempHeavy;
    public AudioClip windLight;
    public AudioClip windMedium;
    public AudioClip windHeavy;
    public AudioClip humidityLight;
    public AudioClip humidityMedium;
    public AudioClip humidityHeavy;
    public AudioClip rainLight;
    public AudioClip rainMedium;
    public AudioClip rainHeavy;

    public AudioSource tempLightAS;
    public AudioSource tempMediumAS;
    public AudioSource tempHeavyAS;
    public AudioSource windLightAS;
    public AudioSource windMediumAS;
    public AudioSource windHeavyAS;
    public AudioSource humidityLightAS;
    public AudioSource humidityMediumAS;
    public AudioSource humidityHeavyAS;
    public AudioSource rainLightAS;
    public AudioSource rainMediumAS;
    public AudioSource rainHeavyAS;

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
            if (userSettings.language == languages.eng)
                newStyleSelector.GetComponentInChildren<Text>().text = local.termEnglish;
            else
                newStyleSelector.GetComponentInChildren<Text>().text = local.termChinese;
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
        styles[currentStyleIndex].killCoroutines(); // stop any coroutines that might be running in the old style
        int newSeqAtBar = styles[currentStyleIndex].newSeqAtBar;
        currentStyleIndex = styleIndex;
        styles[styleIndex].initStyle(newSeqAtBar);
        newValuesAtBar = newSeqAtBar;
    }

    void everyBeat(int beat)
    {
        styles[currentStyleIndex].doEveryBeat();
    }

    void everyBar(int bar)
    {
        if (newValuesAtBar == bar) // grab a new set of AQI & weather values
        {
            if (bar != 0)
            {
                switch (uiController.currentMode)
                {
                    case Mode.mapAll:
                        thisSiteIndex = nextSiteIndex;
                        uiCtrl.currentSiteIndex = thisSiteIndex;
                        nextSiteIndex = (thisSiteIndex + 1) % dataLoader.numSites;
                        thisHour = 0;
                        break;
                    case Mode.mapCounty:
                        thisSiteIndex = nextSiteIndex;
                        uiCtrl.currentSiteIndex = thisSiteIndex;
                        countySiteIndex = (countySiteIndex + 1) % uiCtrl.sitesInCounty[uiCtrl.CurrentCountyIndex].Length;
                        nextSiteIndex = (uiCtrl.sitesInCounty[uiCtrl.CurrentCountyIndex][countySiteIndex]);
                        Debug.Log(uiCtrl.sitesInCounty[uiCtrl.CurrentCountyIndex].Length);
                        Debug.Log("nextSiteIndex: " + nextSiteIndex);
                        thisHour = 0;
                        break;
                    case Mode.site72Hr:
                        thisHour = nextHour;
                        uiCtrl.currentHour = thisHour;
                        uiCtrl.update72Hr();
                        nextHour = ((thisHour - 1) >= 0) ? thisHour - 1 : dataLoader.hoursToRead - 1;
                        break;
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

    public void resetSFX()
    {
        styles[currentStyleIndex].resetSFX();
    }

    public void resetSfxClips()
    {
        tempLightAS.clip = tempLight;
        tempLightAS.Play();
        tempMediumAS.clip = tempMedium;
        tempMediumAS.Play();
        tempHeavyAS.clip = tempHeavy;
        tempHeavyAS.Play();
        windLightAS.clip = windLight;
        windLightAS.Play();
        windMediumAS.clip = windMedium;
        windMediumAS.Play();
        windHeavyAS.clip = windHeavy;
        windHeavyAS.Play();
        humidityLightAS.clip = humidityLight;
        humidityLightAS.Play();
        humidityMediumAS.clip = humidityMedium;
        humidityMediumAS.Play();
        humidityHeavyAS.clip = humidityHeavy;
        humidityHeavyAS.Play();
        rainLightAS.clip = rainLight;
        rainLightAS.Play();
        rainMediumAS.clip = rainMedium;
        rainMediumAS.Play();
        rainHeavyAS.clip = rainHeavy;
        rainHeavyAS.Play();
    }
}
