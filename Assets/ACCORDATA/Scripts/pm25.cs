using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Audio;

public class pm25 : MonoBehaviour
{
    string pmData;
    string[] splitData;

    public readonly string[] stations = { "菜寮", "富貴角", "林口", "永和", "淡水", "汐止", "新莊", "新店", "松山", "大同", "士林", "萬華", "古亭", "忠明", "沙鹿", "大里", "豐原", "西屯", "安南", "善化", "新營", "臺南", "橋頭", "楠梓", "美濃", "仁武", "左營", "鳳山", "前金", "林園", "大寮", "前鎮", "復興", "小港", "冬山", "宜蘭", "桃園", "龍潭", "平鎮", "觀音", "竹東", "三義", "頭份", "苗栗", "彰化", "二林", "南投", "埔里", "竹山", "麥寮", "臺西", "崙背", "斗六", "新港", "朴子", "屏東", "潮州", "恆春", "關山", "臺東", "花蓮", "馬公", "中山", "新竹", "嘉義", "金門", "馬祖" };
    public int[] pm25values;

    WWW www = null;

    string url = "https://env.healthinfo.tw/air/getpm.php";

    public Text stationText;
    public Text valueText;

    uint stationIndex = 0;

    public Transform info;

    public AudioMixerSnapshot[] mSnapshot = new AudioMixerSnapshot[10];
    public AudioMixer mixer;
    float[] weights;
    int lastSnapshotIndex = 0;

    public transitionClips transition;

    IEnumerator Start()
    {
        using (www = new WWW(url))
        {
            yield return www;
            //Renderer renderer = GetComponent<Renderer>();
            //renderer.material.mainTexture = www.texture;
            //Debug.Log(www.text);
            pmData = www.text;
            pm25values = new int[stations.Length];
            readStations();
            StartCoroutine(StationChanger());
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }

    void readStations()
    {
        splitData = pmData.Split(new char[] { ';', ':', ' ', ',' });
        for (int i = 0; i < stations.Length; i++)
        {
            for (int j = 0; j < splitData.Length; j++)
            {
                if (string.Compare(splitData[j], stations[i]) == 0)
                {
                    int.TryParse(splitData[j - 1], out pm25values[i]);//pm25values[i] = splitData[j - 1];
                    break;
                }
            }
            //Debug.Log(stations.Length);
            //Debug.Log(stations[i] + pm25values[i]);
        }
    }

    IEnumerator StationChanger()
    {
        if (weights == null)
            weights = new float[mSnapshot.Length];
        AudioSource tAudio = transition.GetComponent<AudioSource>();
        while (true)
        {
            stationText.text = stations[stationIndex % stations.Length];
            int value = pm25values[stationIndex % stations.Length];
            valueText.text = value.ToString();
            valueText.color = getColor(value);
            //float lastWaveHeight = ocean.waveHeight;
            float newWaveHeight = pm25values[stationIndex % stations.Length] / 2;
            // animate in 1s
            info.localScale = new Vector3(0, 0, 0);
            //info.DOScale(new Vector3(1, 1, 1), 2);
            // mixer snapshots
            int snapshotIndex = (int)map(Mathf.Clamp(value, 0, 80), 0, 80, 0, mSnapshot.Length);
            /*
            if (snapshotIndex != lastSnapshotIndex)
            {
                // set new weigths
                for (int i = 0; i < mSnapshot.Length; i++)
                    weights[i] = 0;
                weights[lastSnapshotIndex] = .15f; // 15% of the last snpshot
                weights[snapshotIndex] = .85f; // 85% of the current snapshot
                mixer.TransitionToSnapshots(mSnapshot, weights, 2);
            }
            else
            {
                // set new weigths
                for (int i = 0; i < mSnapshot.Length; i++)
                    weights[i] = 0;
                weights[snapshotIndex] = 1f; // 100% of the current snapshot
                mixer.TransitionToSnapshots(mSnapshot, weights, 2);
            }
            */
            if (snapshotIndex > lastSnapshotIndex)
            {
                tAudio.clip = transition.up[Random.Range(0, transition.up.Length)];
                tAudio.PlayOneShot(tAudio.clip, Mathf.Clamp(lastSnapshotIndex * 0.5f, 0, 1.5f));
            }
            else if (snapshotIndex < lastSnapshotIndex)
            {
                tAudio.clip = transition.down[Random.Range(0, transition.down.Length)];
                tAudio.PlayOneShot(tAudio.clip, Mathf.Clamp(lastSnapshotIndex * 0.2f, 0, 1.5f));
            }

            mSnapshot[snapshotIndex].TransitionTo(2);

            // morph the wave height
            //StartCoroutine(morphWaveHeight(lastWaveHeight, newWaveHeight, 1.0f));

            yield return new WaitForSeconds(4);
            stationIndex++;
            lastSnapshotIndex = snapshotIndex;
        }
    }

    IEnumerator morphWaveHeight(float startVal, float endVal, float duration)
    {
        float t = 0.0f;
        while (t < 1)
        {
            //ocean.waveHeight = Mathf.Lerp(startVal, endVal, t);

            // .. and increate the t interpolater
            t += (Time.deltaTime / duration);
            yield return null;
        }
    }

    Color32 getColor(int n)
    {
        double r;
        double g;
        double b;

        if (n < 10)
        {
            r = 0;
        }
        else if (n < 18)
        {
            r = 31.875 * n - 318.75;
        }
        else if (n < 55)
        {
            r = 255;
        }
        else if (n < 70)
        {
            r = (-6.8) * n + 629;
        }
        else if (n < 100)
        {
            r = 3.4 * n - 85;
        }
        else
        {
            r = 255;
        }

        if (n < 10)
        {
            g = 25.5 * n;
        }
        else if (n < 20)
        {
            g = 255;
        }
        else if (n < 54)
        {
            g = (-7.5) * n + 405;
        }
        else if (n < 100)
        {
            g = 0;
        }
        else if (n < 200)
        {
            g = n - 100;
        }
        else
        {
            g = 100;
        }

        if (n < 5)
        {
            b = 255;
        }
        else if (n < 10)
        {
            b = (-2) * n + 265;
        }
        else if (n < 18)
        {
            b = (-30) * n + 545;
        }
        else if (n < 70)
        {
            b = 0;
        }
        else if (n < 80)
        {
            b = (15.3) * n - 1071;
        }
        else if (n < 100)
        {
            b = (5.1) * n - 255;
        }
        else
        {
            b = 255;
        }
        return new Color32((byte)r, (byte)g, (byte)b, 255);
    }

    long map(long x, long in_min, long in_max, long out_min, long out_max) { return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min; }
}