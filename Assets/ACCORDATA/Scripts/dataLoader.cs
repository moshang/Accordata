using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using System.IO;

public class dataLoader : MonoBehaviour
{
    [Header("--> ACCORDATA <--")]
    // WWW
    private userSettings settings;
    public Transform siteMarkers;
    public GameObject markerPrefab;
    public uiController uiCtrl;

    public GameObject loadingWheel;
    public Text loadingProg;

    [HideInInspector]
    public bool dataFinishedLoading = false;

    readonly string[] EnglishNames = new string[] { "FugueiCape", "Yangming", "Wanli", "Tamsui", "Keelung", "Shilin", "Linkou", "Sanchong", "Cailiao", "Xizhi", "Datong", "Zhongshan", "Dayuan", "Songshan", "Wanhua", "Xinzhuang", "Guanyin", "Guting", "Yonghe", "Banqiao", "Taoyuan", "Tucheng", "Xindian", "Pingzhen", "Zhongli", "Longtan", "Hukou", "Hsinchu", "Zhudong", "Toufen", "Miaoli", "Sanyi", "Fengyuan", "Shalu", "Xitun", "Zhongming", "Xianxi", "Dali", "Changhua", "Puli", "Erlin", "Nantou", "Zhushan", "Mailiao", "Taixi", "Douliu", "Xingang", "Puzi", "Chiayi", "Xinying", "Shanhua", "Annan", "Tainan", "Meinong", "Qiaotou", "Nanzi", "Renwu", "Zuoying", "Pingtung", "Qianjin", "Fengshan", "Fuxing", "Qianzhen", "Xiaogang", "Daliao", "Chaozhou", "Linyuan", "Hengchun", "Yilan", "Dongshan", "Hualien", "Guanshan", "Taitung", "Matsu", "Kinmen", "Magong", "Dacheng", "Lunbei" };
    readonly string[] ChineseNames = new string[] { "富貴角", "陽明", "萬里", "淡水", "基隆", "士林", "林口", "三重", "菜寮", "汐止", "大同", "中山", "大園", "松山", "萬華", "新莊", "觀音", "古亭", "永和", "板橋", "桃園", "土城", "新店", "平鎮", "中壢", "龍潭", "湖口", "新竹", "竹東", "頭份", "苗栗", "三義", "豐原", "沙鹿", "西屯", "忠明", "線西", "大里", "彰化", "埔里", "二林", "南投", "竹山", "麥寮", "臺西", "斗六", "新港", "朴子", "嘉義", "新營", "善化", "安南", "臺南", "美濃", "橋頭", "楠梓", "仁武", "左營", "屏東", "前金", "鳳山", "復興", "前鎮", "小港", "大寮", "潮州", "林園", "恆春", "宜蘭", "冬山", "花蓮", "關山", "臺東", "馬祖", "金門", "馬公", "彰化(大城)", "崙背" };

    public struct Sites
    {
        public string EnglishName;
        public string ChineseName;
        public float lat;
        public float lng;
        public int aqi;
        public int PM10;
        public int PM25;
        public int O3;
        public float SO2;
        public float CO;
        public float NO2;
        public float temperature;
        public float windspeed;
        public float humidity;
        public float rainfall;

        public GameObject marker;
    }
    public Sites[,] sites;

    WWW www = null;

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep; // don't allow the screen to turn off as this unfortunately also kills audio
        Application.targetFrameRate = 30;
        StartCoroutine(fetchData());
    }

    IEnumerator fetchData()
    {

        using (www = new WWW("http://accordata.org/scraper/weather.csv"))
        {
            //Debug.Log("Starting!");
            yield return www;
            //Debug.Log("Done!");
            if (!string.IsNullOrEmpty(www.error))
                Debug.Log(www.error);
            else
            {
                string data = www.text;
                string[] splitData = data.Split('\n');
                foreach (string s in splitData)
                    Debug.Log(s);
                //Debug.Log("Received data for the last " + (splitData.Length / 78).ToString() + " hours.");
                StartCoroutine(populateData(splitData));
            }
        }
    }

    IEnumerator populateData(string[] data)
    {
        //Debug.Log(data.Length);
        //int tmpCounter = 0;
        int numHours = data.Length / 78;
        const int numSites = 78;
        sites = new Sites[numHours, numSites];
        for (int i = 0; i < numHours; i++)
        {
            for (int j = 0; j < numSites; j++)
            {
                string[] siteData = data[(i * 78) + j].Split(',');
                int progress = (int)(((float)((i * 78) + j) / data.Length) * 100);
                loadingProg.text = progress.ToString() + "%";
                yield return null;

                //Debug.Log(tmpCounter);
                //tmpCounter++;
                sites[i, j].ChineseName = ChineseNames[j];
                sites[i, j].EnglishName = siteData[1];
                float.TryParse(siteData[2], out sites[i, j].lat);
                float.TryParse(siteData[3], out sites[i, j].lng);
                int.TryParse(siteData[4], out sites[i, j].aqi);
                int.TryParse(siteData[5], out sites[i, j].PM10);
                int.TryParse(siteData[6], out sites[i, j].PM25);
                int.TryParse(siteData[7], out sites[i, j].O3);
                float.TryParse(siteData[8], out sites[i, j].SO2);
                float.TryParse(siteData[9], out sites[i, j].CO);
                float.TryParse(siteData[10], out sites[i, j].NO2);
                float.TryParse(siteData[11], out sites[i, j].temperature);
                float.TryParse(siteData[12], out sites[i, j].windspeed);
                float.TryParse(siteData[13], out sites[i, j].humidity);
                float.TryParse(siteData[14], out sites[i, j].rainfall);

                // add marker data for each site - just for the latest hour for now
                if (i == 0)
                {
                    Vector3 markerPos = Vector3.zero;
                    markerPos.x = utils.map(sites[i, j].lng, 120, 122, -193.6f, 384);
                    markerPos.y = utils.map(sites[i, j].lat, 21.9f, 25.3f, -542, 542);
                    sites[i, j].marker = Instantiate(markerPrefab);
                    //sites[i].marker.transform.parent = siteMarkers;
                    sites[i, j].marker.transform.SetParent(siteMarkers);
                    sites[i, j].marker.transform.localPosition = markerPos;
                    sites[i, j].marker.transform.localScale = new Vector3(2, 2, 1);
                    sites[i, j].marker.name = sites[i, j].EnglishName;
                    sites[i, j].marker.GetComponent<Image>().color = uiCtrl.getAqiColor(sites[i, j].aqi);
                }
                else if (!dataFinishedLoading)
                    dataFinishedLoading = true;
            }
        }
        loadingWheel.SetActive(false);
    }
}
