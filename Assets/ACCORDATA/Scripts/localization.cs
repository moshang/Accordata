using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class localization : MonoBehaviour
{
    [Header("-> ACCORDATA <-")]
    public string termEnglish;
    public string termChinese;
    Text txt;

    void Start()
    {
        txt = GetComponent<Text>();
        changeUiLanguage();
        userSettings.OnLanguageChanged += changeUiLanguage;
    }


    void OnDisable()
    {
        userSettings.OnLanguageChanged += changeUiLanguage;
    }

    void changeUiLanguage()
    {
        switch (userSettings.language)
        {
            case languages.eng:
                txt.text = termEnglish;
                break;
            case languages.zhTw:
                txt.text = termChinese;
                break;
        }
    }
}
