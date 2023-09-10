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

    const float defaultBGMVolume = 0.5f;
    const float defaultSEVolume = 0.5f;
    const int defaultTextSpeed = 6;
    const float defaultAutoSpeed = 1.0f;


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

        // Update value texts
        ChangeBGMVolume();
        ChangeSEVolume();
        TextSpeedVolume();
        AutoSpeedVolume();

        isOpen = true;
    }

    public void QuitOptionPanel()
    {
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
    }

    public void ApplySetting()
    {
        tempBGMVolume = AudioManager.Instance.GetMusicVolume();
        tempSEVolume = AudioManager.Instance.GetSEMasterVolume();
        tempTextSpeed = NovelSingletone.Instance.GetTextSpeed();
        tempAutoSpeed = NovelSingletone.Instance.GetAutoSpeed();
    }
}
