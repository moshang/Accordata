using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioHelm;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;
using UnityEngine.Networking;

public class seqGenerator : MonoBehaviour
{
    [Header("--> ACCORDATA <--")]
    // WWW
    string aqiData;
    string[] splitData;
    string[] stringSeparators = new string[] { "SiteId\":" };
    public readonly string[] stations = { "菜寮", "富貴角", "林口", "永和", "淡水", "汐止", "新莊", "新店", "松山", "大同", "士林", "萬華", "古亭", "忠明", "沙鹿", "大里", "豐原", "西屯", "安南", "善化", "新營", "臺南", "橋頭", "楠梓", "美濃", "仁武", "左營", "鳳山", "前金", "林園", "大寮", "前鎮", "復興", "小港", "冬山", "宜蘭", "桃園", "龍潭", "平鎮", "觀音", "竹東", "三義", "頭份", "苗栗", "彰化", "二林", "南投", "埔里", "竹山", "麥寮", "臺西", "崙背", "斗六", "新港", "朴子", "屏東", "潮州", "恆春", "關山", "臺東", "花蓮", "馬公", "中山", "新竹", "嘉義", "金門", "馬祖" };
    //public int[] pm25values;

    WWW www = null;

    string url = "https://taqm.epa.gov.tw/taqm/aqs.ashx?lang=tw&act=aqi-epa";
    string time;
    string ts;

    //public Text stationText;
    //public Text valueText;

    // MUSIC
    readonly int[,] triads = { { 0, 4, 7 }, { 0, 3, 7 }, { 0, 3, 6 } };
    readonly int[,] scales = { { 0, 2, 4, 5, 7, 9, 11, 12 }, { 0, 2, 3, 5, 7, 9, 11, 12 }, { 0, 2, 3, 5, 6, 8, 9, 11 } };
    int scale = 0;
    public Sequencer seq;
    public SampleSequencer sampleSeq;
    public HelmController ctrl;
    public int originalRootNote = 48;
    private int rootNote;
    public int barCounter;


    void Start()
    {
        seq.OnBeat += everyBeat;
        makeSeq();
        rootNote = originalRootNote;
        //time = "&time=" + System.DateTime.Now.ToString("HH:mm:ss");
        time = "";
        url += time;
        ts = "&ts=" + GetTime().ToString();
        url += ts;
        Debug.Log(url);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
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
            splitData = aqiData.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in splitData)
                Debug.Log(str);
        }
    }

    /*
    IEnumerator Start()
    {
        using (www = new WWW(url))
        {
            //yield return new WaitForSeconds(0.5f);
            yield return www;
            //Renderer renderer = GetComponent<Renderer>();
            //renderer.material.mainTexture = www.texture;
            //Debug.Log(www.text);
            aqiData = www.text;
            Debug.Log(aqiData);
            //pm25values = new int[stations.Length];
            splitData = aqiData.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);  //(new string[] { "div"}); = new string[] { "[stop]" };
            foreach (string str in splitData)
                Debug.Log(str);
            //readStations();
            //StartCoroutine(StationChanger());
            //parseAqi(aqiData);
            //Debug.Log(parseAqi(aqiData));

        }
    }
    */
    /*
       string parseAqi(string htmlToParse)
       {
           string pattern = "<div id="thisMap"(.*?)</pre>";
           Regex regex = new Regex(pattern, RegexOptions.None);
           Match m = regex.Match(htmlToParse);
           if (m.Success)
           {
               return m.Value;
           }
           return string.Empty;
       }
    */
    void makeSeq()
    {

        seq.Clear();
        sampleSeq.Clear();

        // ARP
        int scaleIndex = 0;
        for (int i = 0; i < seq.length; i++)
        {
            seq.AddNote(rootNote + triads[scale, scaleIndex % 3], i, i + 1);
            scaleIndex++;
        }

        // MELODY
        scaleIndex = 0;
        int lastNoteIndex = UnityEngine.Random.Range(0, scales.GetLength(1));
        //lastNoteNum = scales[scale, lastNoteIndex)];
        while (scaleIndex < seq.length)
        {
            //int newNoteIndex = lastNoteIndex + Random.Range(-2, 3);
            //if (newNoteIndex >= scales.GetLength(1) || newNoteIndex < 0)
            //    newNoteIndex = Random.Range(0, scales.GetLength(1));
            int newNoteIndex = UnityEngine.Random.Range(0, 5);
            seq.AddNote(rootNote + 12 + scales[scale, newNoteIndex], scaleIndex, scaleIndex + 1);
            lastNoteIndex = newNoteIndex;
            scaleIndex += (UnityEngine.Random.Range(2, 5) * 2);
        }
    }

    void everyBeat(int beat)
    {
        //ctrl.SetParameterValue(Param.kFilterCutoff, Random.Range(28, 127));
        if (beat == 0)
        {

            if (barCounter % 4 == 0)
            {
                rootNote -= 3;
                scale = (scale + 1) % 3;
            }
            else
                rootNote = originalRootNote;
        }
        //Debug.Log("----------> " + beat);
        if (beat == 15)
        {
            Invoke("makeSeq", 0.1f);

            barCounter++;
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
