/* Copyright (c) Jean Marais / MoShang 2018. Licensed under GPLv3.
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections;
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
    public Font fontEn;
    public Font fontChTw;
    public bool simplifica;

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
                if (!simplifica)
                    txt.font = fontEn;
                if (insertCountyName)
                {
                    string newTerm = termEnglish.Replace("[county]", uiControl.countiesEn[uiControl.CurrentCountyIndex]);
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
                if (!simplifica)
                    txt.font = fontChTw;
                if (insertCountyName)
                {
                    string newTerm = termChinese.Replace("[county]", uiControl.countiesTwn[uiControl.CurrentCountyIndex]);
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
