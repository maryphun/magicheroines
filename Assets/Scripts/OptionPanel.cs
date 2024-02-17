using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NovelEditor;
using TMPro;
using Assets.SimpleLocalization.Scripts;

public enum SystemLanguage
{
    JP,
    EN,
    SCN,
    TCN,
}

[RequireComponent(typeof(CanvasGroup))]
public class OptionPanel : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField, Range(0.0f, 1.0f)] private float animationTime = 0.5f;

    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider seVolumeSlider;
    [SerializeField] private Slider voiceVolumeSlider;
    [SerializeField] private Slider textSpeedSlider;
    [SerializeField] private Slider autoSpeedSlider;
    [SerializeField] private TMP_Text bgmVolumeValue;
    [SerializeField] private TMP_Text seVolumeValue;
    [SerializeField] private TMP_Text voiceVolumeValue;
    [SerializeField] private TMP_Text textSpeedValue;
    [SerializeField] private TMP_Text autoSpeedValue;
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private Toggle windowScreenToggle;
    [SerializeField] private Toggle JPToggle, ENToggle, SCNToggle, TCNToggle;
    [SerializeField] private Button backButton;

    public static float defaultBGMVolume = 0.5f;
    public static float defaultSEVolume = 0.5f;
    public static float defaultVoiceVolume = 0.5f;
    public static int defaultTextSpeed = 6;
    public static float defaultAutoSpeed = 1.0f;
    public static bool defaultFullScreenToggle = false;
    public static bool defaultWindowScreenToggle = true;
    public static Vector2Int defaultResolutionSizeWindowed = new Vector2Int(1280, 720);
    public static Vector2Int defaultResolutionSizeFull = new Vector2Int(1920, 1080);
    public static SystemLanguage defaultLanguage = SystemLanguage.JP;


    private float tempBGMVolume = 0.5f;
    private float tempSEVolume = 0.5f;
    private float tempVoiceVolume = 0.5f;
    private int tempTextSpeed = 6;
    private float tempAutoSpeed = 1.0f;

    private bool isChangingLanguge = false;
    private bool isOpen = false;
    
    public bool IsOpen()
    {
        return isOpen;
    }

    public void OpenOptionPanel()
    {
        // SE çƒê∂
        AudioManager.Instance.PlaySFX("SystemOpen");

        canvasGroup.DOFade(1.0f, animationTime);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        tempBGMVolume = AudioManager.Instance.GetMusicVolume();
        tempSEVolume = AudioManager.Instance.GetSEMasterVolume();
        tempVoiceVolume = NovelSingletone.Instance.GetVoiceVolume();
        tempTextSpeed = NovelSingletone.Instance.GetTextSpeed();
        tempAutoSpeed = NovelSingletone.Instance.GetAutoSpeed();

        bgmVolumeSlider.value = tempBGMVolume;
        seVolumeSlider.value = tempSEVolume;
        voiceVolumeSlider.value = tempVoiceVolume;
        textSpeedSlider.value = tempTextSpeed;
        autoSpeedSlider.value = tempAutoSpeed;
        fullScreenToggle.isOn = Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen;
        windowScreenToggle.isOn = Screen.fullScreenMode == FullScreenMode.Windowed;
        JPToggle.isOn = LocalizationManager.Language == "Japanese";
        ENToggle.isOn = LocalizationManager.Language == "English";
        SCNToggle.isOn = LocalizationManager.Language == "Simplified Chinese";
        TCNToggle.isOn = LocalizationManager.Language == "Traditional Chinese";

        // Update value texts
        ChangeBGMVolume();
        ChangeSEVolume();
        ChangeVoiceVolume();
        TextSpeedVolume();
        AutoSpeedVolume();

        isOpen = true;
    }

    public void QuitOptionPanel()
    {
        // SE çƒê∂
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
    public void ChangeVoiceVolume()
    {
        NovelSingletone.Instance.SetVoiceVolume(voiceVolumeSlider.value);

        string percentValue = (voiceVolumeSlider.value * 100).ToString("0") + "%";
        voiceVolumeValue.SetText(percentValue);
    }
    public void EndSEVolumeDrag()
    {
        // SE çƒê∂
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
        voiceVolumeSlider.DOValue(defaultVoiceVolume, 0.25f);
        textSpeedSlider.DOValue(defaultTextSpeed, 0.25f);
        autoSpeedSlider.DOValue(defaultAutoSpeed, 0.25f);
        fullScreenToggle.isOn = defaultFullScreenToggle;
        windowScreenToggle.isOn = defaultWindowScreenToggle;

        // SE çƒê∂
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

        string lastLanguage = LocalizationManager.Language;
        if (JPToggle.isOn && lastLanguage != "Japanese")
        {
            LocalizationManager.Language = "Japanese";
            PlayerPrefsManager.SetPlayerPrefs(PlayerPrefsManager.PlayerPrefsSave.Language, (int)SystemLanguage.JP);
        }
        else if (ENToggle.isOn && lastLanguage != "English")
        {
            LocalizationManager.Language = "English";
            PlayerPrefsManager.SetPlayerPrefs(PlayerPrefsManager.PlayerPrefsSave.Language, (int)SystemLanguage.EN);
        }
        else if (SCNToggle.isOn && lastLanguage != "Simplified Chinese")
        {
            LocalizationManager.Language = "Simplified Chinese";
            PlayerPrefsManager.SetPlayerPrefs(PlayerPrefsManager.PlayerPrefsSave.Language, (int)SystemLanguage.SCN);
        }
        else if (TCNToggle.isOn && lastLanguage != "Traditional Chinese")
        {
            LocalizationManager.Language = "Traditional Chinese";
            PlayerPrefsManager.SetPlayerPrefs(PlayerPrefsManager.PlayerPrefsSave.Language, (int)SystemLanguage.TCN);
        }

        // åæåÍïœçX
        if (lastLanguage != LocalizationManager.Language)
        {
            ProgressManager.Instance.RelocalizeCharactersName();
            AudioManager.Instance.PlaySFX("SystemButton");
        }

        // SE çƒê∂
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

    public void OnToggleLanguage(int lang)
    {
        if (isChangingLanguge) return;
        AudioManager.Instance.PlaySFX("SystemCursor");
        isChangingLanguge = true;
        switch ((SystemLanguage)lang)
        {
            case SystemLanguage.JP:
                ENToggle.isOn = false;
                SCNToggle.isOn = false;
                TCNToggle.isOn = false;
                break;
            case SystemLanguage.EN:
                JPToggle.isOn = false;
                SCNToggle.isOn = false;
                TCNToggle.isOn = false;
                break;
            case SystemLanguage.SCN:
                JPToggle.isOn = false;
                ENToggle.isOn = false;
                TCNToggle.isOn = false;
                break;
            case SystemLanguage.TCN:
                JPToggle.isOn = false;
                ENToggle.isOn = false;
                SCNToggle.isOn = false;
                break;
            default:
                break;
        }
        isChangingLanguge = false;
    }

    private void Update()
    {
        if (!isOpen) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            backButton.onClick.Invoke();
        }
    }
}
