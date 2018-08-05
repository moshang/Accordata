using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

public class dataLoader : MonoBehaviour
{
    [Header("--> ACCORDATA <--")]
    // WWW
    string aqiData;
    string[] splitData;
    string[] stringSeparators = new string[] { "{Result:ok, Data:[{", "SiteId:", ",SiteName:", ",SiteKey:", ",AreaKey:", ",MonobjName:", ",Address:", ",lat:", ",lng:", ",AQI:", ",MainPollutant:", ",MainPollutantKey:", ",CityCode:", ",PM10:", ",PM10_AVG:", ",PM25:", ",PM25_AVG:", ",O3:", ",O3_8:", ",SO2:", ",CO:", ",CO_8:", ",NO2:", ",SO2_VFLAG:" }; //  "{Result:ok, Data:[{", 

    string weatherData;
    string[] splitDataWeather;
    string[] stringSeparatorsWeather = new string[] { "id=MapID", "><a href=", "class=temp1>", "</td><td class=temp2", "</td><td>", "</td></tr>" };

    public int[] aqi;
    private userSettings settings;
    public Transform siteMarkers;
    public GameObject markerPrefab;
    public uiController uiCtrl;

    public GameObject loadingWheel;

    [HideInInspector]
    public bool dataFinishedLoading = false;

    string[] localNames = new string[] { "FugueiCape", "Yangming", "Wanli", "Tamsui", "Keelung", "Shilin", "Linkou", "Sanchong", "Cailiao", "Xizhi", "Datong", "Zhongshan", "Dayuan", "Songshan", "Wanhua", "Xinzhuang", "Guanyin", "Guting", "Yonghe", "Banqiao", "Taoyuan", "Tucheng", "Xindian", "Pingzhen", "Zhongli", "Longtan", "Hukou", "Hsinchu", "Zhudong", "Toufen", "Miaoli", "Sanyi", "Fengyuan", "Shalu", "Xitun", "Zhongming", "Xianxi", "Dali", "Changhua", "Puli", "Erlin", "Nantou", "Zhushan", "Mailiao", "Taixi", "Douliu", "Xingang", "Puzi", "Chiayi", "Xinying", "Shanhua", "Annan", "Tainan", "Meinong", "Qiaotou", "Nanzi", "Renwu", "Zuoying", "Pingtung", "Qianjin", "Fengshan", "Fuxing", "Qianzhen", "Xiaogang", "Daliao", "Chaozhou", "Linyuan", "Hengchun", "Yilan", "Dongshan", "Hualien", "Guanshan", "Taitung", "Matsu", "Kinmen", "Magong", "Dacheng", "Lunbei" };

    int counter;

    public struct Sites
    {
        public string localName;
        public int id;
        public string name;
        public string key;
        public string areakey;
        public string monobjName;
        public string address;
        public float lat;
        public float lng;
        public int aqi;
        public string mainPollutant;
        public int PM10;
        public int PM10_AVG;
        public int PM25;
        public int PM25_AVG;
        public int O3;
        public int O3_8;
        public float SO2;
        public float CO;
        public float CO_8;
        public float NO2;
        public float temperature;
        public float windspeed;
        public float humidity;
        public float rainfall;

        public GameObject marker;
    }
    public Sites[] sites;

    WWW www = null;

    string url;
    string time;
    string ts;


    string[] weatherUrls = new string[] { "https://www.cwb.gov.tw/V7/observe/real/ObsN.htm", "https://www.cwb.gov.tw/V7/observe/real/ObsC.htm", "https://www.cwb.gov.tw/V7/observe/real/ObsS.htm", "https://www.cwb.gov.tw/V7/observe/real/ObsE.htm", "https://www.cwb.gov.tw/V7/observe/real/ObsI.htm" };

    string[] northAreaIDs = new string[] { "C0A92", "46693", "C0A94", "46690", "46694", "C0A9E", "C0AH5", "C0AD3", "C0AD3", "C0AH0", "C0A9E", "C0A9E", "C0C54", "C0AH7", "C0AH1", "C0ACA", "C0C59", "C0AH1", "C0AH1", "46688", "C0C48", "C0AD4", "A0A9M", "C0C65", "C0C70", "C0C67", "C0D65", "46757", "C0D56", "C0E73", "C0E75", "C0E53" };
    string[] centralAreaIDs = new string[] { "C0F9M", "46777", "C0F9T", "C0F9T", "C0G64", "C0F9N", "C0G76", "C0H89", "C0G73", "C0I46", "C0I11", "A0K42", "C0K53", "C0K40", "C0M79", "C0M65", "46748" };
    string[] southAreaIDs = new string[] { "C0O95", "C0O90", "C0O95", "46741", "C0V31", "C0V76", "C0V67", "C0V68", "C0V81", "C0R17", "C0V69", "C0V44", "C0V21", "C0V69", "C0V72", "C0V73", "C0R22", "C0V72", "46759" };
    string[] eastAreaIDs = new string[] { "46708", "C0U91", "46699", "C0S89", "46766" };
    string[] islandIDs = new string[] { "46799", "46711", "46735" };
    string[] mobileIDs = new string[] { "C0G74", "46711" };

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep; // don't allow the screen to turn off as this unfortunately also kills audio
        Application.targetFrameRate = 45;
        settings = GetComponent<userSettings>();
        switch (settings.language)
        {
            case languages.eng:
                url = "https://taqm.epa.gov.tw/taqm/aqs.ashx?lang=en&act=aqi-epa";
                break;
            case languages.zhTw:
                url = "https://taqm.epa.gov.tw/taqm/aqs.ashx?lang=tw&act=aqi-epa";
                break;
        }

        sites = new Sites[localNames.Length];
        Debug.Log("Num. of sites: " + sites.Length);
        int numSiteIDs = northAreaIDs.Length + centralAreaIDs.Length + southAreaIDs.Length + eastAreaIDs.Length + islandIDs.Length + mobileIDs.Length;
        Debug.Log("Num. of site IDs: " + numSiteIDs);
        StartCoroutine(loadAqiData());
    }

    IEnumerator loadAqiData()
    {

        using (www = new WWW(url))
        {
            //Debug.Log("Starting!");
            yield return www;
            //Debug.Log("Done!");
            if (!string.IsNullOrEmpty(www.error))
                Debug.Log(www.error);
            else
                StartCoroutine(parseAqiData(www));
        }
    }

    IEnumerator parseAqiData(WWW w)
    {
        aqiData = w.text;
        //Debug.Log(aqiData);
        string aqiDataNoQuotes = aqiData.Replace("\"", ""); // remove quotation marks
        splitData = aqiDataNoQuotes.Split(stringSeparators, StringSplitOptions.None); //, StringSplitOptions.RemoveEmptyEntries);
                                                                                      //Debug.Log("Number of sites: " + siteID.Length);
        int numSplits = 23; // 10 here for 10 split strings (and ignoring the initial split string that will return empties)
        //sites = new Sites[splitData.Length / numSplits];
        int splitdataOffset = 2; // through testing we know the first two strings in splitdata will be empty
        for (int i = 0; i < sites.Length; i++)
        {
            sites[i].localName = localNames[i];
            int.TryParse(splitData[(i * numSplits) + splitdataOffset], out sites[i].id);
            sites[i].name = splitData[(i * numSplits) + 1 + splitdataOffset];
            sites[i].key = splitData[(i * numSplits) + 2 + splitdataOffset];
            sites[i].areakey = splitData[(i * numSplits) + 3 + splitdataOffset];
            sites[i].monobjName = splitData[(i * numSplits) + 4 + splitdataOffset];
            sites[i].address = splitData[(i * numSplits) + 5 + splitdataOffset];
            float.TryParse(splitData[(i * numSplits) + 6 + splitdataOffset], out sites[i].lat);
            float.TryParse(splitData[(i * numSplits) + 7 + splitdataOffset], out sites[i].lng);
            int.TryParse(splitData[(i * numSplits) + 8 + splitdataOffset], out sites[i].aqi);
            sites[i].mainPollutant = splitData[(i * numSplits) + 9 + splitdataOffset];
            // skip 2
            int.TryParse(splitData[(i * numSplits) + 12 + splitdataOffset], out sites[i].PM10);
            int.TryParse(splitData[(i * numSplits) + 13 + splitdataOffset], out sites[i].PM10_AVG);
            int.TryParse(splitData[(i * numSplits) + 14 + splitdataOffset], out sites[i].PM25);
            int.TryParse(splitData[(i * numSplits) + 15 + splitdataOffset], out sites[i].PM25_AVG);
            int.TryParse(splitData[(i * numSplits) + 16 + splitdataOffset], out sites[i].O3);
            int.TryParse(splitData[(i * numSplits) + 17 + splitdataOffset], out sites[i].O3_8);
            float.TryParse(splitData[(i * numSplits) + 18 + splitdataOffset], out sites[i].SO2);
            float.TryParse(splitData[(i * numSplits) + 19 + splitdataOffset], out sites[i].CO);
            float.TryParse(splitData[(i * numSplits) + 20 + splitdataOffset], out sites[i].CO_8);
            float.TryParse(splitData[(i * numSplits) + 21 + splitdataOffset], out sites[i].NO2);

            Vector3 markerPos = Vector3.zero;
            markerPos.x = utils.map(sites[i].lng, 120, 122, -193.6f, 384);
            markerPos.y = utils.map(sites[i].lat, 21.9f, 25.3f, -542, 542);
            sites[i].marker = Instantiate(markerPrefab);
            //sites[i].marker.transform.parent = siteMarkers;
            sites[i].marker.transform.SetParent(siteMarkers);
            sites[i].marker.transform.localPosition = markerPos;
            sites[i].marker.transform.localScale = new Vector3(2, 2, 1);
            sites[i].marker.name = sites[i].name;
            sites[i].marker.GetComponent<Image>().color = uiCtrl.getAqiColor(sites[i].aqi);

            //Debug.Log(sites[i].localName + " : " + sites[i].name);

            yield return null;
        }
        StartCoroutine(loadWeatherData());
    }

    IEnumerator loadWeatherData()
    {
        for (int i = 0; i < weatherUrls.Length; i++)
        {
            using (www = new WWW(weatherUrls[i]))
            {
                //Debug.Log("Starting!");
                yield return www;
                if (!string.IsNullOrEmpty(www.error))
                    Debug.Log(www.error);
                else
                {
                    weatherData = www.text;
                    parseWeatherData(i);
                }
            }
        }
        loadingWheel.SetActive(false);
        dataFinishedLoading = true;
    }

    private void parseWeatherData(int area)
    {
        string weatherDataNoQuotes = weatherData.Replace("\"", ""); // remove quotation marks
        splitDataWeather = weatherDataNoQuotes.Split(stringSeparatorsWeather, StringSplitOptions.None);


        foreach (string str in splitDataWeather)
            Debug.Log(str);
        Debug.Log("-----------------------------------------------------");

        string[] siteIDsInArea;
        switch (area)
        {
            case 0: // north
                siteIDsInArea = northAreaIDs;
                for (int i = 0; i < siteIDsInArea.Length; i++) // the site IDs for this area
                {
                    for (int j = 0; j < splitDataWeather.Length; j++) // the weather data array for this area
                    {
                        //Debug.Log(splitDataWeather.Length + " <-----------------------------");
                        if (splitDataWeather[j] == siteIDsInArea[i])
                        {
                            populateWeather(i, j, 0);
                            break;
                        }
                    }
                }
                break;
            case 1: // central
                siteIDsInArea = centralAreaIDs;
                for (int i = 0; i < siteIDsInArea.Length; i++) // the site IDs for this area
                {
                    for (int j = 0; j < splitDataWeather.Length; j++) // the weather data array for this area
                    {
                        //Debug.Log(splitDataWeather.Length + " <-----------------------------");
                        if (splitDataWeather[j] == siteIDsInArea[i])
                        {
                            populateWeather(i, j, 32);
                            break;
                        }
                    }
                }
                break;
            case 2: // south
                siteIDsInArea = southAreaIDs;
                break;
            case 3: // east
                siteIDsInArea = eastAreaIDs;
                break;
            case 4: // islands
                siteIDsInArea = islandIDs;
                break;
        }
    }

    void populateWeather(int i, int j, int siteIndexOffset)
    {
        float.TryParse(splitDataWeather[(j + 3)], out sites[i + siteIndexOffset].temperature);
        float.TryParse(splitDataWeather[(j + 7)], out sites[i + siteIndexOffset].windspeed);
        float.TryParse(splitDataWeather[(j + 12)], out sites[i + siteIndexOffset].humidity);
        float.TryParse(splitDataWeather[(j + 15)], out sites[i + siteIndexOffset].rainfall);
    }

    private Int64 GetTime()
    {
        Int64 retval = 0;
        var st = new DateTime(1970, 1, 1);
        TimeSpan t = (DateTime.Now.ToUniversalTime() - st);
        retval = (Int64)(t.TotalMilliseconds + 0.5);
        return retval;
    }
}
