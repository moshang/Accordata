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

    public int[] aqi;
    private userSettings settings;
    public Transform siteMarkers;
    public GameObject markerPrefab;
    public uiController uiCtrl;

    public GameObject loadingWheel;

    [HideInInspector]
    public bool dataFinishedLoading = false;

    public struct Sites
    {
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

        public GameObject marker;
    }
    public Sites[] sites;

    WWW www = null;

    string url;
    string time;
    string ts;

    void Start()
    {
        settings = GetComponent<userSettings>();
        switch (settings.language)
        {
            case languages.eng:
                url = "https://taqm.epa.gov.tw/taqm/aqs.ashx?lang=en&act=aqi-epa";
                break;
            case languages.zhtw:
                url = "https://taqm.epa.gov.tw/taqm/aqs.ashx?lang=tw&act=aqi-epa";
                break;
        }

        // wait, do we need any of this?
        /*
        //time = "&time=" + System.DateTime.Now.ToString("HH:mm:ss");
        time = ""; // setting this to the time above returns empty data ;^(
        url += time;
        ts = "&ts=" + GetTime().ToString();
        url += ts;
        */
        //Debug.Log(url);
        StartCoroutine(loadAqiData());
    }

    IEnumerator loadAqiData()
    {

        using (www = new WWW(url))
        {
            while (!www.isDone)
            {
                //Debug.Log(www.progress);
                yield return null;
            }
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
        sites = new Sites[splitData.Length / numSplits];
        int splitdataOffset = 2; // through testing we know the first two strings in splitdata will be empty
        for (int i = 0; i < sites.Length; i++)
        {
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
            sites[i].marker.transform.parent = siteMarkers;
            sites[i].marker.transform.localPosition = markerPos;
            sites[i].marker.transform.localScale = new Vector3(2, 2, 1);
            sites[i].marker.name = sites[i].name;
            sites[i].marker.GetComponent<Image>().color = uiCtrl.getAqiColor(sites[i].aqi);
            yield return null;
        }
        loadingWheel.SetActive(false);
        dataFinishedLoading = true;
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
