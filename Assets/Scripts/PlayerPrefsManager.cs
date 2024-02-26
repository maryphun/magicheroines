using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsManager
{
    private static int BoolToInt(bool val)
    {
        if (val)
            return 1;
        else
            return 0;
    }
    private static bool IntToBool(int val)
    {
        if (val == 0)
            return false;
        else
            return true;
    }

    public enum PlayerPrefsSave
    {
        IsFullScreen,
        BGM_Volume,
        SE_Volume,
        VOICE_Volume,
        TextSpeed,
        AutoSpeed,
        Language,
        LastestProgress,
    }

    public static void LoadPlayerPrefs()
    {
        int fullScreenMode = PlayerPrefs.GetInt(PlayerPrefsSave.IsFullScreen.ToString(), (int)(OptionPanel.defaultFullScreenToggle ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed));
        Screen.fullScreenMode = (FullScreenMode)fullScreenMode;

        if ((FullScreenMode)fullScreenMode == FullScreenMode.ExclusiveFullScreen)
        {
            Screen.SetResolution(OptionPanel.defaultResolutionSizeFull.x, OptionPanel.defaultResolutionSizeFull.y, Screen.fullScreenMode);
        }
        else
        {
            Screen.SetResolution(OptionPanel.defaultResolutionSizeWindowed.x, OptionPanel.defaultResolutionSizeWindowed.y, Screen.fullScreenMode);
        }
        
        float musicVolume = PlayerPrefs.GetFloat(PlayerPrefsSave.BGM_Volume.ToString(), OptionPanel.defaultBGMVolume);
        AudioManager.Instance.SetMusicVolume(musicVolume);

        float seVolume = PlayerPrefs.GetFloat(PlayerPrefsSave.SE_Volume.ToString(), OptionPanel.defaultSEVolume);
        AudioManager.Instance.SetSEMasterVolume(seVolume);

        float voiceVolume = PlayerPrefs.GetFloat(PlayerPrefsSave.VOICE_Volume.ToString(), OptionPanel.defaultVoiceVolume);
        NovelSingletone.Instance.SetVoiceVolume(voiceVolume);

        int textSpd = PlayerPrefs.GetInt(PlayerPrefsSave.TextSpeed.ToString(), OptionPanel.defaultTextSpeed);
        NovelSingletone.Instance.SetTextSpeed(textSpd);

        float autoSpd = PlayerPrefs.GetFloat(PlayerPrefsSave.AutoSpeed.ToString(), OptionPanel.defaultAutoSpeed);
        NovelSingletone.Instance.SetAutoSpeed(autoSpd);


        SystemLanguage lang = (SystemLanguage)PlayerPrefs.GetInt(PlayerPrefsSave.Language.ToString(), (int)OptionPanel.defaultLanguage);
        switch (lang)
        {
            case SystemLanguage.JP:
                Assets.SimpleLocalization.Scripts.LocalizationManager.Language = "Japanese";
                break;
            case SystemLanguage.EN:
                Assets.SimpleLocalization.Scripts.LocalizationManager.Language = "English";
                break;
            case SystemLanguage.SCN:
                Assets.SimpleLocalization.Scripts.LocalizationManager.Language = "Simplified Chinese";
                break;
            case SystemLanguage.TCN:
                Assets.SimpleLocalization.Scripts.LocalizationManager.Language = "Traditional Chinese";
                break;
            default:
                break;
        }
    }

    public static void SetPlayerPrefs(PlayerPrefsSave name, int value)
    {
        PlayerPrefs.SetInt(name.ToString(), value);
    }
    public static void SetPlayerPrefs(PlayerPrefsSave name, float value)
    {
        PlayerPrefs.SetFloat(name.ToString(), value);
    }

    public static bool GetBool(string name, bool defaultValue = false)
    {
        return IntToBool(PlayerPrefs.GetInt(name, BoolToInt(defaultValue)));
    }

    public static void SetBool(string name, bool value)
    {
        PlayerPrefs.SetInt(name, BoolToInt(value));
    }

    // progress
    public static void UpdateCurrentProgress(int progress)
    {
        PlayerPrefs.SetInt(PlayerPrefsSave.LastestProgress.ToString(), Mathf.Max(progress, GetLatestProgress()));
    }
    public static int GetLatestProgress()
    {
        return PlayerPrefs.GetInt(PlayerPrefsSave.LastestProgress.ToString(), 0);
    }
}
