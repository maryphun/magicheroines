using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NovelEditor;
using TMPro;
using Assets.SimpleLocalization.Scripts;

[RequireComponent(typeof(CanvasGroup))]
public class OptionPanel : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField, Range(0.0f, 1.0f)] private float animationTime = 0.5f;

    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider seVolumeSlider;
    [SerializeField] private Slider textSpeedSlider;
    [SerializeField] private Slider autoSpeedSlider;
    [SerializeField] private TMP_Text bgmVolumeValue;
    [SerializeField] private TMP_Text seVolumeValue;
    [SerializeField] private TMP_Text textSpeedValue;
    [SerializeField] private TMP_Text autoSpeedValue;
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private Toggle windowScreenToggle;

    public static float defaultBGMVolume = 0.5f;
    public static float defaultSEVolume = 0.5f;
    public static int defaultTextSpeed = 6;
    public static float defaultAutoSpeed = 1.0f;
    public static bool defaultFullScreenToggle = true;
    public static bool defaultWindowScreenToggle = false;
    public static Vector2Int defaultResolutionSizeWindowed = new Vector2Int(1280, 720);
    public static Vector2Int defaultResolutionSizeFull = new Vector2Int(1920, 1080);


    private float tempBGMVolume = 0.5f;
    private float tempSEVolume = 0.5f;
    private int tempTextSpeed = 6;
    private float tempAutoSpeed = 1.0f;

    private bool isOpen = false;
    
    public bool IsOpen()
    {
        return isOpen;
    }

    public void OpenOptionPanel()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemOpen");

        canvasGroup.DOFade(1.0f, animationTime);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        tempBGMVolume = AudioManager.Instance.GetMusicVolume();
        tempSEVolume = AudioManager.Instance.GetSEMasterVolume();
        tempTextSpeed = NovelSingletone.Instance.GetTextSpeed();
        tempAutoSpeed = NovelSingletone.Instance.GetAutoSpeed();

        bgmVolumeSlider.value = tempBGMVolume;
        seVolumeSlider.value = tempSEVolume;
        textSpeedSlider.value = tempTextSpeed;
        autoSpeedSlider.value = tempAutoSpeed;
        fullScreenToggle.isOn = Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen;
        windowScreenToggle.isOn = Screen.fullScreenMode == FullScreenMode.Windowed;
        
        // Update value texts
        ChangeBGMVolume();
        ChangeSEVolume();
        TextSpeedVolume();
        AutoSpeedVolume();

        isOpen = true;
    }

    public void QuitOptionPanel()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemCancel");

        canvasGroup.DOFade(0.0f, animationTime);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        AudioManager.Instance.SetMusicVolume(tempBGMVolume);
        AudioManager.Instance.SetSEMasterVolume(tempSEVolume);
        NovelSingletone.Instance.SetTextSpeed(tempTextSpeed);
        NovelSingletone.Instance.SetAutoSpeed(tempAutoSpeed);

        isOpen = false;
    }

    public void ChangeBGMVolume()
    {
        AudioManager.Instance.SetMusicVolume(bgmVolumeSlider.value);
        NovelSingletone.Instance.SetBGMVolume(bgmVolumeSlider.value);

        string percentValue = (bgmVolumeSlider.value * 100).ToString("0") + "%";
        bgmVolumeValue.SetText(percentValue);
    }
    public void ChangeSEVolume()
    {
        AudioManager.Instance.SetSEMasterVolume(seVolumeSlider.value);
        NovelSingletone.Instance.SetSEVolume(seVolumeSlider.value);

        string percentValue = (seVolumeSlider.value * 100).ToString("0") + "%";
        seVolumeValue.SetText(percentValue);
    }
    public void EndSEVolumeDrag()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemAlert");
    }
    public void TextSpeedVolume()
    {
        NovelSingletone.Instance.SetTextSpeed((int)(textSpeedSlider.value));

        textSpeedValue.SetText(((int)textSpeedSlider.value).ToString());
    }
    public void AutoSpeedVolume()
    {
        NovelSingletone.Instance.SetAutoSpeed(autoSpeedSlider.value);

        string percentValue = (autoSpeedSlider.value).ToString("0.00") + LocalizationManager.Localize("Option.Second");
        autoSpeedValue.SetText(percentValue);
    }

    public void SetToDefault()
    {
        bgmVolumeSlider.DOValue(defaultBGMVolume, 0.25f);
        seVolumeSlider.DOValue(defaultSEVolume, 0.25f);
        textSpeedSlider.DOValue(defaultTextSpeed, 0.25f);
        autoSpeedSlider.DOValue(defaultAutoSpeed, 0.25f);
        fullScreenToggle.isOn = defaultFullScreenToggle;
        windowScreenToggle.isOn = defaultWindowScreenToggle;

        // SE 再生
        AudioManager.Instance.PlaySFX("SystemSelect");
    }

    public void ApplySetting()
    {
        tempBGMVolume = AudioManager.Instance.GetMusicVolume();
        PlayerPrefsManager.SetPlayerPrefs(PlayerPrefsManager.PlayerPrefsSave.BGM_Volume, tempBGMVolume);
        tempSEVolume = AudioManager.Instance.GetSEMasterVolume();
        PlayerPrefsManager.SetPlayerPrefs(PlayerPrefsManager.PlayerPrefsSave.SE_Volume, tempSEVolume);
        tempTextSpeed = NovelSingletone.Instance.GetTextSpeed();
        PlayerPrefsManager.SetPlayerPrefs(PlayerPrefsManager.PlayerPrefsSave.TextSpeed, tempTextSpeed);
        tempAutoSpeed = NovelSingletone.Instance.GetAutoSpeed();
        PlayerPrefsManager.SetPlayerPrefs(PlayerPrefsManager.PlayerPrefsSave.AutoSpeed, tempAutoSpeed);

        // apply to change full screen mode
        if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen)
        {
            if (windowScreenToggle.isOn)
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.SetResolution(defaultResolutionSizeWindowed.x, defaultResolutionSizeWindowed.y, FullScreenMode.Windowed);
                PlayerPrefsManager.SetPlayerPrefs(PlayerPrefsManager.PlayerPrefsSave.IsFullScreen, (int)FullScreenMode.Windowed);
            }
        }
        else
        {
            if (fullScreenToggle.isOn)
            {
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                Screen.SetResolution(defaultResolutionSizeFull.x, defaultResolutionSizeFull.y, FullScreenMode.ExclusiveFullScreen);
                PlayerPrefsManager.SetPlayerPrefs(PlayerPrefsManager.PlayerPrefsSave.IsFullScreen, (int)FullScreenMode.ExclusiveFullScreen);
            }
        }

        // SE 再生
        AudioManager.Instance.PlaySFX("SystemSelect");
    }

    public void OnToggleFullScreen()
    {
        AudioManager.Instance.PlaySFX("SystemCursor");
        windowScreenToggle.isOn = !fullScreenToggle.isOn;
    }
    public void OnToggleWindoedScreen()
    {
        AudioManager.Instance.PlaySFX("SystemCursor");
        fullScreenToggle.isOn = !windowScreenToggle.isOn;
    }

    private void Update()
    {
        if (!isOpen) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitOptionPanel();
        }
    }
}
