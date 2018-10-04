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

public enum languages { eng, zhTw };
public class userSettings : MonoBehaviour
{
    [Header("--> ACCORDATA <--")]
    public static languages language = languages.zhTw; // default

    public delegate void LanguageChange();
    public static event LanguageChange OnLanguageChanged;

    private Settings settings;

    public Toggle EngToggle;
    public Toggle ChTwToggle;

    // should we show the help arrow?
    public static int firstRun = 1; // 1 = true, 0 = false;

    private bool settingsLoaded = false;

    private void OnEnable()
    {
        // get saved user preferences
        loadSettings();
        if (OnLanguageChanged != null)
            OnLanguageChanged();
    }

    private void loadSettings()
    {
        settings = new Settings();

        if (PlayerPrefs.HasKey("language"))
        {
            settings.language = PlayerPrefs.GetInt("language");
            //setLanguage((languages)settings.language, false);
            language = (languages)settings.language;

            if (language == languages.eng && !EngToggle.isOn)
            {
                EngToggle.isOn = true;
                ChTwToggle.isOn = false;
            }
            else if (language == languages.zhTw && !ChTwToggle.isOn)
            {
                ChTwToggle.isOn = true;
                EngToggle.isOn = false;
            }
            settingsLoaded = true;
        }
        else
        {
            saveSettings();
            settingsLoaded = true;
        }

        if (PlayerPrefs.HasKey("firstRun"))
        {
            firstRun = PlayerPrefs.GetInt("firstRun");
        }
    }

    public static void saveSettings()
    {
        PlayerPrefs.SetInt("language", (int)language);
        PlayerPrefs.SetInt("firstRun", firstRun);
    }

    public void setLanguage(languages lang, bool saveSets = true)
    {
        if (!settingsLoaded)
            return;

        language = lang;
        if (OnLanguageChanged != null)
            OnLanguageChanged();
        if (saveSets)
            saveSettings();
    }

    public void setEnglish()
    {
        setLanguage(languages.eng);
    }

    public void setChinese()
    {
        setLanguage(languages.zhTw);
    }

    public static void setFirstRun()
    {
        if (firstRun == 1)
        {
            firstRun = 0;
            saveSettings();
        }
    }
}

public class Settings
{
    public int language;
}

