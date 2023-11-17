using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsManager
{
    public enum PlayerPrefsSave
    {
        IsFullScreen,
        BGM_Volume,
        SE_Volume,
        TextSpeed,
        AutoSpeed,
    }

    public static void LoadPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();

        int fullScreenMode = PlayerPrefs.GetInt(PlayerPrefsSave.IsFullScreen.ToString(), (int)(OptionPanel.defaultFullScreenToggle ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed));
        Screen.fullScreenMode = (FullScreenMode)fullScreenMode;
        Screen.SetResolution(1920, 1080, Screen.fullScreenMode);

        float musicVolume = PlayerPrefs.GetFloat(PlayerPrefsSave.BGM_Volume.ToString(), OptionPanel.defaultBGMVolume);
        AudioManager.Instance.SetMusicVolume(musicVolume);

        float seVolume = PlayerPrefs.GetFloat(PlayerPrefsSave.SE_Volume.ToString(), OptionPanel.defaultSEVolume);
        AudioManager.Instance.SetSEMasterVolume(seVolume);

        int textSpd = PlayerPrefs.GetInt(PlayerPrefsSave.TextSpeed.ToString(), OptionPanel.defaultTextSpeed);
        NovelSingletone.Instance.SetTextSpeed(textSpd);

        float autoSpd = PlayerPrefs.GetFloat(PlayerPrefsSave.AutoSpeed.ToString(), OptionPanel.defaultAutoSpeed);
        NovelSingletone.Instance.SetAutoSpeed(autoSpd);
    }

    public static void SetPlayerPrefs(PlayerPrefsSave name, int value)
    {
        PlayerPrefs.SetInt(name.ToString(), value);
    }
    public static void SetPlayerPrefs(PlayerPrefsSave name, float value)
    {
        PlayerPrefs.SetFloat(name.ToString(), value);
    }
}
