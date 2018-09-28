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

    [Header("72HRS")]
    public Slider[] aqiSlider;
    public Slider[] tempSlider;
    public Slider[] windSlider;
    public Slider[] humiditySlider;
    public Slider[] rainSlider;

    public const int hoursToRead = 72;
    [HideInInspector]
    public bool dataFinishedLoading = false;

    public const int numSites = 78;

    public readonly string[] EnglishNames = new string[] { "FugueiCape", "Yangming", "Wanli", "Tamsui", "Keelung", "Shilin", "Linkou", "Sanchong", "Cailiao", "Xizhi", "Datong", "Zhongshan", "Dayuan", "Songshan", "Wanhua", "Xinzhuang", "Guanyin", "Guting", "Yonghe", "Banqiao", "Taoyuan", "Tucheng", "Xindian", "Pingzhen", "Zhongli", "Longtan", "Hukou", "Hsinchu", "Zhudong", "Toufen", "Miaoli", "Sanyi", "Fengyuan", "Shalu", "Xitun", "Zhongming", "Xianxi", "Dali", "Changhua", "Puli", "Erlin", "Nantou", "Zhushan", "Mailiao", "Taixi", "Douliu", "Xingang", "Puzi", "Chiayi", "Xinying", "Shanhua", "Annan", "Tainan", "Meinong", "Qiaotou", "Nanzi", "Renwu", "Zuoying", "Pingtung", "Qianjin", "Fengshan", "Fuxing", "Qianzhen", "Xiaogang", "Daliao", "Chaozhou", "Linyuan", "Hengchun", "Yilan", "Dongshan", "Hualien", "Guanshan", "Taitung", "Matsu", "Kinmen", "Magong", "Dacheng", "Xiaoliuqiu" };
    public readonly string[] ChineseNames = new string[] { "富貴角", "陽明", "萬里", "淡水", "基隆", "士林", "林口", "三重", "菜寮", "汐止", "大同", "中山", "大園", "松山", "萬華", "新莊", "觀音", "古亭", "永和", "板橋", "桃園", "土城", "新店", "平鎮", "中壢", "龍潭", "湖口", "新竹", "竹東", "頭份", "苗栗", "三義", "豐原", "沙鹿", "西屯", "忠明", "線西", "大里", "彰化", "埔里", "二林", "南投", "竹山", "麥寮", "臺西", "斗六", "新港", "朴子", "嘉義", "新營", "善化", "安南", "臺南", "美濃", "橋頭", "楠梓", "仁武", "左營", "屏東", "前金", "鳳山", "復興", "前鎮", "小港", "大寮", "潮州", "林園", "恆春", "宜蘭", "冬山", "花蓮", "關山", "臺東", "馬祖", "金門", "馬公", "彰化(大城)", "小琉球" };

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
    string[] splitData;

    public Color lowValColor;
    public Color midValColor;
    public Color highValColor;

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep; // don't allow the screen to turn off as this unfortunately also kills audio
        //Application.targetFrameRate = 30;

        // don't allow switching to single site mode
        uiCtrl.site72HrToggle.interactable = false;
        uiCtrl.chartIcon.color = uiCtrl.countyTi.nonInteractableColor;
        uiCtrl.playToggle.interactable = false;
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
                splitData = data.Split('\n');
                //foreach (string s in splitData)
                //    Debug.Log(s);
                //Debug.Log("Received data for the last " + (splitData.Length / 78).ToString() + " hours.");
                populateData(splitData);
            }
        }
    }

    private void populateData(string[] data) // 78 sites for the most recent hour only
    {
        int numHours = data.Length / 78;

        sites = new Sites[numHours, numSites];
        // int hoursToRead = numHours; // to read all sites
        int hoursToRead = 1; // to read just the most recent hour

        for (int i = 0; i < hoursToRead; i++)
        {
            for (int j = 0; j < numSites; j++)
            {
                string[] siteData = data[(i * 78) + j].Split(',');

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

                // add marker data for each site - just for the most recent hour
                if (i == 0)
                {
                    Vector3 markerPos = Vector3.zero;
                    markerPos.x = utils.map(sites[i, j].lng, 120, 122, -193.6f, 384);
                    markerPos.y = utils.map(sites[i, j].lat, 21.9f, 25.3f, -542, 542);
                    // two special cases for outlying islands
                    if (sites[i, j].EnglishName == "Matsu")
                        markerPos.y = 529;
                    if (sites[i, j].EnglishName == "Kinmen")
                        markerPos.x = -318;
                    sites[i, j].marker = Instantiate(markerPrefab);
                    //sites[i].marker.transform.parent = siteMarkers;
                    sites[i, j].marker.transform.SetParent(siteMarkers);
                    sites[i, j].marker.transform.localPosition = markerPos;
                    sites[i, j].marker.transform.localScale = new Vector3(1, 1, 1);
                    sites[i, j].marker.name = sites[i, j].EnglishName;
                    sites[i, j].marker.GetComponent<Image>().color = uiCtrl.getAqiColor(sites[i, j].aqi);
                }
                dataFinishedLoading = true;
                uiCtrl.site72HrToggle.interactable = true;
                uiCtrl.chartIcon.color = uiCtrl.countyTi.interactableColor;
                uiCtrl.playToggle.interactable = true;
            }
        }

        loadingWheel.SetActive(false);
    }

    public void getData72HR(int siteIndex)
    {
        populateData72HR(splitData, siteIndex);
    }

    private void populateData72HR(string[] data, int siteIndex) // get 5 data points for the selected site for the last 72 hours
    {
        dataFinishedLoading = false;
        int numHours = data.Length / 78;

        for (int i = 0; i < hoursToRead; i++)
        {
            string[] siteData = data[(i * numSites) + siteIndex].Split(',');
            // AQI
            int aqiVal;
            int.TryParse(siteData[4], out aqiVal);
            sites[i, siteIndex].aqi = aqiVal;
            if (aqiVal >= 0) // invalid/missing data is marked as -255
            {
                aqiSlider[71 - i].value = (float)aqiVal / 250;
                aqiSlider[71 - i].transform.Find("Fill Area/Fill").GetComponent<Image>().color = uiCtrl.getAqiColor(aqiVal);
            }
            else
            {
                aqiSlider[71 - i].value = 1;
                aqiSlider[71 - i].transform.Find("Fill Area/Fill").GetComponent<Image>().color = new Color(0.42f, 0.50f, 0.57f, 1);
            }
            // TEMPERATURE
            float tempVal;
            float.TryParse(siteData[11], out tempVal);
            sites[i, siteIndex].temperature = tempVal;
            tempSlider[71 - i].value = (float)tempVal / 40;
            if (tempVal >= 0)
            {
                tempSlider[71 - i].value = (float)tempVal / 40;
                tempSlider[71 - i].transform.Find("Fill Area/Fill").GetComponent<Image>().color = getSliderColor(tempSlider[71 - i].value);
            }
            else
            {
                tempSlider[71 - i].value = 1;
                tempSlider[71 - i].transform.Find("Fill Area/Fill").GetComponent<Image>().color = new Color(0.42f, 0.50f, 0.57f, 1);
            }
            // WIND
            float windVal;
            float.TryParse(siteData[12], out windVal);
            sites[i, siteIndex].windspeed = windVal;
            if (windVal >= 0)
            {
                windSlider[71 - i].value = (float)windVal / 20; // 20 m/s = gale on Beaufort scale
                windSlider[71 - i].transform.Find("Fill Area/Fill").GetComponent<Image>().color = getSliderColor(windSlider[71 - i].value);
            }
            else
            {
                windSlider[71 - i].value = 1;
                windSlider[71 - i].transform.Find("Fill Area/Fill").GetComponent<Image>().color = new Color(0.42f, 0.50f, 0.57f, 1);
            }
            // HUMIDITY
            float humidityVal;
            float.TryParse(siteData[13], out humidityVal);
            sites[i, siteIndex].humidity = humidityVal;
            if (humidityVal >= 0)
            {
                // raise the floor to 30% humidity (rare for Taiwan) to give larger range on the graph
                humiditySlider[71 - i].value = (float)(humidityVal - 30) / 70;
                humiditySlider[71 - i].transform.Find("Fill Area/Fill").GetComponent<Image>().color = getSliderColor(humiditySlider[71 - i].value);
            }
            else
            {
                humiditySlider[71 - i].value = 1;
                humiditySlider[71 - i].transform.Find("Fill Area/Fill").GetComponent<Image>().color = new Color(0.42f, 0.50f, 0.57f, 1);
            }
            // RAIN
            float rainVal;
            float.TryParse(siteData[14], out rainVal);
            sites[i, siteIndex].rainfall = rainVal;
            if (rainVal >= 0)
            {
                rainSlider[71 - i].value = (float)rainVal / 60;
                rainSlider[71 - i].transform.Find("Fill Area/Fill").GetComponent<Image>().color = getSliderColor(rainSlider[71 - i].value);
            }
            else
            {
                rainSlider[71 - i].value = 1;
                rainSlider[71 - i].transform.Find("Fill Area/Fill").GetComponent<Image>().color = new Color(0.42f, 0.50f, 0.57f, 1);
            }
        }
        dataFinishedLoading = true;
    }

    private Color getSliderColor(float val)
    {
        Color sliderColor = lowValColor;
        if (val <= 0.5f)
        {
            val *= 2;
            sliderColor.r = Mathf.Lerp(lowValColor.r, midValColor.r, val);
            sliderColor.g = Mathf.Lerp(lowValColor.g, midValColor.g, val);
            sliderColor.b = Mathf.Lerp(lowValColor.b, midValColor.b, val);
            sliderColor.a = Mathf.Lerp(lowValColor.a, midValColor.a, val);
        }
        else
        {
            val -= 0.5f;
            val *= 2;
            sliderColor.r = Mathf.Lerp(midValColor.r, highValColor.r, val);
            sliderColor.g = Mathf.Lerp(midValColor.g, highValColor.g, val);
            sliderColor.b = Mathf.Lerp(midValColor.b, highValColor.b, val);
            sliderColor.a = Mathf.Lerp(midValColor.a, highValColor.a, val);
        }
        return sliderColor;
    }


}