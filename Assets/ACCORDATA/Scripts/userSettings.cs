using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum languages { eng, zhTw };
public class userSettings : MonoBehaviour
{
    [Header("--> ACCORDATA <--")]
    public static languages language = languages.eng; // default

    public delegate void LanguageChange();
    public static event LanguageChange OnLanguageChanged;

    private Settings settings;

    public Toggle EngToggle;
    public Toggle ChTwToggle;

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
    }

    private void saveSettings()
    {
        PlayerPrefs.SetInt("language", (int)language);
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
}

public class Settings
{
    public int language;
}

