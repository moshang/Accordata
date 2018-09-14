﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Mode { mapAll, mapCounty, site72Hr }

public class uiController : MonoBehaviour
{
    [Header("-> ACCORDATA <-")]
    public seqGenerator seqGen;
    public dataLoader data;
    private int thisHour = 0;

    [Header("Site Card")]
    public RectTransform siteCard;
    public RectTransform siteCardAqi;
    public RectTransform siteCardDetails;
    Image siteCardBG;
    Image siteCardAqiBG;
    Image siteCardDetailsBG;
    public Text siteNameTxt;
    public Text siteAqiTxt;
    public Text pm10;
    public Text pm25;
    public Text o3;
    public Text so2;
    public Text co;
    public Text no2;
    public Text temperature;
    public Text windspeed;
    public Text humidity;
    public Text rainfall;

    [Header("Target Highlight")]
    public SpriteRenderer highlight;
    private Vector3 highlightOriginalScale = new Vector3(10, 10, 1);

    // COUNTY NAMES
    public readonly string[] countiesEn = { "Keelung", "New Taipei", "Taipei", "Taoyuan", "Yilan", "Hsinchu County", "Hsinchu", "Miaoli", "Taichung", "Hualien", "Nantou", "Changhua", "Yunlin", "Chiayi County", "Chiayi", "Tainan", "Kaohsiung", "Taitung", "Pingtung", "Penghu", "Kinmen", "Lienchiang" };
    public readonly string[] countiesTwn = { "基隆", "新台北", "台北", "桃園", "宜蘭", "新竹縣", "新竹", "苗栗", "台中", "花蓮", "南投", "彰化", "雲林", "嘉義縣", "嘉義", "台南", "高雄", "台東", "屏東", "澎湖", "金門", "連江" };

    [Header("Mode Selection")]
    public GameObject mapGO;
    public GameObject GO72HR;
    [HideInInspector]
    public Mode lastMapMode;
    public static Mode currentMode;

    public Toggle site72HrToggle;
    public Toggle countyToggle;

    // TOGGLE INTERACTABILITY
    [HideInInspector]
    public toggleImage countyTi;
    public SpriteRenderer mapGlobeIcon;
    public SpriteRenderer mapSiteIcon;
    toggleImage site72Ti;
    public SpriteRenderer chartIcon;

    public GameObject topTextAll;
    public GameObject topTextCounty;
    public GameObject topTextSite;

    // 72 HOUR
    public Text aqiValText;
    public Text temperatureValText;
    public Text windValText;
    public Text humidityValText;
    public Text rainValText;

    public GameObject playhead;

    [Header("DEBUG - don't populate with values manually")]
    public int currentSiteIndex = 0; // received from seqGenerator
    public int currentCountyIndex = 0;
    public int currentHour = 71; // received from seqGenerator

    private userSettings settings;

    // value mutes
    public bool isActiveAqi = true;
    public bool isActiveTemp = true;
    public bool isActiveWind = true;
    public bool isActiveHumidity = true;
    public bool isActiveRain = true;

    public setMute[] muteScripts;

    private void Start()
    {
        siteCardAqiBG = siteCardAqi.GetComponent<Image>();
        clock.OnBeat += everyBeat;
        settings = GetComponent<userSettings>();
        countyTi = countyToggle.GetComponent<toggleImage>();
        site72Ti = site72HrToggle.GetComponent<toggleImage>();
    }

    public void setMuteToggleRemote(valueType valType, bool val)
    {
        if (val)
        {
            switch (valType)
            {
                case valueType.aqi:
                    isActiveAqi = true;
                    break;
                case valueType.temperature:
                    isActiveTemp = true;
                    break;
                case valueType.windspeed:
                    isActiveWind = true;
                    break;
                case valueType.humidity:
                    isActiveHumidity = true;
                    break;
                case valueType.rainfall:
                    isActiveRain = true;
                    break;
            }
        }
        else
        {
            switch (valType)
            {
                case valueType.aqi:
                    isActiveAqi = false;
                    break;
                case valueType.temperature:
                    isActiveTemp = false;
                    break;
                case valueType.windspeed:
                    isActiveWind = false;
                    break;
                case valueType.humidity:
                    isActiveHumidity = false;
                    break;
                case valueType.rainfall:
                    isActiveRain = false;
                    break;
            }
        }

        foreach (setMute sm in muteScripts)
            sm.updateToggle();
    }

    // FUNCTIONS
    public void nextStyle() // TEMP
    {
        //seqGen.genStyle = (style)((int)(seqGen.genStyle + 1) % seqGen.numStyles);
        //styleButtonTxt.text = utils.UppercaseFirst(System.Enum.GetName(typeof(style), seqGen.genStyle));
    }

    public void showSelectedSite(GameObject selectedSite)
    {
        for (int i = 0; i < data.sites.Length; i++)
        {
            if (selectedSite == data.sites[thisHour, i].marker)
            {
                updateCard(i);
                currentSiteIndex = i;
                break;
            }
        }
    }

    public void updateCard(int siteIndex)
    {
        //fill the card with data
        switch (userSettings.language)
        {
            case languages.eng:
                siteNameTxt.text = data.sites[thisHour, siteIndex].EnglishName;
                break;
            case languages.zhTw:
                siteNameTxt.text = data.sites[thisHour, siteIndex].ChineseName;
                break;
        }
        siteAqiTxt.text = "AQI: " + data.sites[thisHour, siteIndex].aqi;

        //mainPollutant.text = "Main Pollutant: " + data.sites[thisHour, siteIndex].mainPollutant;
        pm25.text = data.sites[thisHour, siteIndex].PM25.ToString();
        //pm25_avg.text = "PM2.5 Avg: " + data.sites[thisHour, siteIndex].PM25_AVG;
        pm10.text = data.sites[thisHour, siteIndex].PM10.ToString();
        //pm10_avg.text = "PM10 Avg: " + data.sites[thisHour, siteIndex].PM10_AVG;
        o3.text = data.sites[thisHour, siteIndex].O3.ToString();
        //o3_8h.text = "O3 8h: " + data.sites[thisHour, siteIndex].O3_8;
        so2.text = data.sites[thisHour, siteIndex].SO2.ToString();
        co.text = data.sites[thisHour, siteIndex].CO.ToString();
        //co_8h.text = "CO 8h: " + data.sites[thisHour, siteIndex].CO_8;
        no2.text = data.sites[thisHour, siteIndex].NO2.ToString();

        // weather data
        temperature.text = data.sites[thisHour, siteIndex].temperature.ToString() + "°C";
        windspeed.text = data.sites[thisHour, siteIndex].windspeed.ToString() + "m/s";
        humidity.text = data.sites[thisHour, siteIndex].humidity.ToString() + "%";
        rainfall.text = data.sites[thisHour, siteIndex].rainfall.ToString() + "mm";

        // change the colors
        updateBGColors(data.sites[thisHour, siteIndex].aqi);

        // move the target highlight
        highlight.transform.position = data.sites[thisHour, siteIndex].marker.transform.position;
    }

    void updateBGColors(int aqiVal)
    {
        siteCardAqiBG.color = getAqiColor(aqiVal);
    }

    public Color32 getAqiColor(int aqiVal)
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

    public void setMode(Mode mode)
    {
        switch (mode)
        {
            case Mode.mapAll:
                mapGO.SetActive(true);
                GO72HR.SetActive(false);
                lastMapMode = Mode.mapAll;
                topTextAll.SetActive(true);
                topTextCounty.SetActive(false);
                topTextSite.SetActive(false);
                countyToggle.interactable = true;
                mapGlobeIcon.color = countyTi.interactableColor;
                mapSiteIcon.color = countyTi.interactableColor;
                break;
            case Mode.mapCounty:
                mapGO.SetActive(true);
                GO72HR.SetActive(false);
                lastMapMode = Mode.mapCounty;
                topTextAll.SetActive(false);
                topTextCounty.SetActive(true);
                topTextSite.SetActive(false);
                countyToggle.interactable = true;
                mapGlobeIcon.color = countyTi.interactableColor;
                mapSiteIcon.color = countyTi.interactableColor;
                break;
            case Mode.site72Hr:
                mapGO.SetActive(false);
                GO72HR.SetActive(true);
                data.getData72HR(currentSiteIndex);
                topTextAll.SetActive(false);
                topTextCounty.SetActive(false);
                topTextSite.SetActive(true);
                countyToggle.interactable = false;
                mapGlobeIcon.color = countyTi.nonInteractableColor;
                mapSiteIcon.color = countyTi.nonInteractableColor;
                update72Hr();
                break;
        }
        currentMode = mode;
    }

    public void updateCountyName()
    {
        if (currentMode == Mode.mapCounty)
            settings.setLanguage(userSettings.language); // reset the current language to force the county name to update
    }

    public void update72Hr()
    {
        aqiValText.text = (data.sites[currentHour, currentSiteIndex].aqi >= 0) ? data.sites[currentHour, currentSiteIndex].aqi.ToString() : "-";
        temperatureValText.text = (data.sites[currentHour, currentSiteIndex].temperature >= 0) ? data.sites[currentHour, currentSiteIndex].temperature.ToString() : "-";
        windValText.text = (data.sites[currentHour, currentSiteIndex].windspeed >= 0) ? data.sites[currentHour, currentSiteIndex].windspeed.ToString() : "-";
        humidityValText.text = (data.sites[currentHour, currentSiteIndex].humidity >= 0) ? data.sites[currentHour, currentSiteIndex].humidity.ToString() : "-";
        rainValText.text = (data.sites[currentHour, currentSiteIndex].rainfall >= 0) ? data.sites[currentHour, currentSiteIndex].rainfall.ToString() : "-";

        setPlayheadPos();
    }

    private void setPlayheadPos()
    {
        Vector3 playheadPos = playhead.transform.position;
        playheadPos.x = data.aqiSlider[71 - currentHour].transform.position.x;
        playhead.transform.position = playheadPos;
    }
}
