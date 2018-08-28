using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum languages { eng, zhTw };
public class userSettings : MonoBehaviour
{
    [Header("--> ACCORDATA <--")]
    public static languages language = languages.eng; // default

    public delegate void LanguageChange();
    public static event LanguageChange OnLanguageChanged;

    private Settings settings;

    private void OnEnable()
    {
        // get saved user preferences
        loadSettings();
    }

    private void loadSettings()
    {
        settings = new Settings();

        if (PlayerPrefs.HasKey("language"))
        {
            settings.language = PlayerPrefs.GetInt("language");
            setLanguage((languages)settings.language, false);
        }
        else
            saveSettings();
    }

    private void saveSettings()
    {
        PlayerPrefs.SetInt("language", (int)language);
    }

    public void setLanguage(languages lang, bool saveSets = true)
    {
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

