using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioHelm;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;
using UnityEngine.Networking;

public class dataLoader : MonoBehaviour
{
    [Header("--> ACCORDATA <--")]
    // WWW
    string aqiData;
    string[] splitData;
    string[] stringSeparators = new string[] { "{Result:ok, Data:[{", "SiteId:", ",SiteName:", ",SiteKey:", ",AreaKey:", ",MonobjName:", ",Address:", ",lat:", ",lng:", ",AQI:", ",MainPollutant:" }; //, ",MainPollutantKey:", ",CityCode:", ",PM10:", ",PM10_AVG:", ",PM25:", ",PM25_AVG:" }; //  "{Result:ok, Data:[{", 
    //public readonly string[] siteID = { "FugueiCape", "Yangming", "Wanli", "Tamsui", "Keelung", "Shilin", "Linkou", "Sanchong", "Cailiao", "Xizhi", "Datong", "Zhongshan", "Dayuan", "Songshan", "Wanhua", "EPA", "Xinzhuang", "Guanyin", "Guting", "Yonghe", "Banqiao", "Taoyuan", "Tucheng", "Xindian", "Pingzhen", "Zhongli", "Longtan", "Hukou", "Hsinchu", "Zhudong", "Toufen", "Miaoli", "Sanyi", "Fengyuan", "Shalu", "Xitun", "Zhongming", "Xianxi", "Dali", "Changhua", "Puli", "Erlin", "Nantou", "Zhushan", "Lunbei", "Mailiao", "Taixi", "Douliu", "Xingang", "Alishan", "Lulin", "Puzi", "Chiayi", "Xinying", "Shanhua", "Annan", "Tainan", "Meinong", "Qiaotou", "Nanzi", "Renwu", "Zuoying", "Pingtung", "Qianjin", "Fengshan", "Fuxing", "Qianzhen", "Xiaogang", "Daliao", "Chaozhou", "Linyuan", "Hengchun", "Yilan", "Dongshan", "Hualien", "Guanshan", "Taitung", "Matsu", "Kinmen", "Magong", "Sanmin", "Chonglun" };
    public int[] aqi;

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
    }
    public Sites[] sites;

    WWW www = null;

    string url;
    string time;
    string ts;

    void Start()
    {
        switch (userSettings.language)
        {
            case languages.eng:
                url = "https://taqm.epa.gov.tw/taqm/aqs.ashx?lang=en&act=aqi-epa";
                break;
            case languages.zhtw:
                url = "https://taqm.epa.gov.tw/taqm/aqs.ashx?lang=tw&act=aqi-epa";
                break;
        }

        //time = "&time=" + System.DateTime.Now.ToString("HH:mm:ss");
        time = ""; // setting this to the time above returns empty data ;^(
        url += time;
        ts = "&ts=" + GetTime().ToString();
        url += ts;
        Debug.Log(url);
        StartCoroutine("GetText");
    }

    IEnumerator GetText() // https://docs.unity3d.com/Manual/UnityWebRequest-RetrievingTextBinaryData.html
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            aqiData = www.downloadHandler.text;
            Debug.Log(aqiData);
            string aqiDataNoQuotes = aqiData.Replace("\"", ""); // remove quotation marks
            splitData = aqiDataNoQuotes.Split(stringSeparators, StringSplitOptions.None); //, StringSplitOptions.RemoveEmptyEntries);
                                                                                          //Debug.Log("Number of sites: " + siteID.Length);
            int numSplits = 10; // 10 here for 10 split strings (and ignoring the initial split string that will return empties)
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

                Debug.Log(sites[i].name + ": " + sites[i].aqi);
            }

        }
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
