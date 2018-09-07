﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class localization : MonoBehaviour
{
    [Header("-> ACCORDATA <-")]
    public string termEnglish;
    public string termChinese;
    public bool insertCountyName = false;
    public bool insertSiteName = false;
    public uiController uiControl;
    public dataLoader data;
    Text txt;

    void OnEnable()
    {
        if (txt == null)
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
                if (insertCountyName)
                {
                    string newTerm = termEnglish.Replace("[county]", uiControl.countiesEn[uiControl.currentCountyIndex]);
                    txt.text = newTerm;
                }
                else if (insertSiteName)
                {
                    string newTerm = termEnglish.Replace("[site]", data.EnglishNames[uiControl.currentSiteIndex]);
                    txt.text = newTerm;
                }
                else
                    txt.text = termEnglish;
                break;
            case languages.zhTw:
                if (insertCountyName)
                {
                    string newTerm = termChinese.Replace("[county]", uiControl.countiesTwn[uiControl.currentCountyIndex]);
                    txt.text = newTerm;
                }
                else if (insertSiteName)
                {
                    string newTerm = termChinese.Replace("[site]", data.ChineseNames[uiControl.currentSiteIndex]);
                    txt.text = newTerm;
                }
                else
                    txt.text = termChinese;
                break;
        }
    }
}