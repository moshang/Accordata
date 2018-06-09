using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class test : MonoBehaviour
{
    string pmData;
    string[] splitData;

    public readonly string[] stations = { "菜寮", "富貴角", "林口", "永和", "淡水", "汐止", "新莊", "新店", "松山", "大同", "士林", "萬華", "古亭", "忠明", "沙鹿", "大里", "豐原", "西屯", "安南", "善化", "新營", "臺南", "橋頭", "楠梓", "美濃", "仁武", "左營", "鳳山", "前金", "林園", "大寮", "前鎮", "復興", "小港", "冬山", "宜蘭", "桃園", "龍潭", "平鎮", "觀音", "竹東", "三義", "頭份", "苗栗", "彰化", "二林", "南投", "埔里", "竹山", "麥寮", "臺西", "崙背", "斗六", "新港", "朴子", "屏東", "潮州", "恆春", "關山", "臺東", "花蓮", "馬公", "中山", "新竹", "嘉義", "金門", "馬祖" };
    public int[] pm25values;

    string url = "https://env.healthinfo.tw/air/getpm.php";
    IEnumerator Start()
    {
        using (WWW www = new WWW(url))
        {
            yield return www;
            //Renderer renderer = GetComponent<Renderer>();
            //renderer.material.mainTexture = www.texture;
            //Debug.Log(www.text);
            pmData = www.text;
            pm25values = new int[stations.Length];
            readStations();
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
            Debug.Log(stations[i] + pm25values[i]);
        }
    }

}