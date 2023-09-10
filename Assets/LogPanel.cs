using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class LogPanel : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField, Range(0.0f, 1.0f)] private float fadeAnimationTime = 0.5f;

    [Header("References")]
    [SerializeField] private TMP_Text logText;
    [SerializeField] private CanvasGroup logPanelCanvas;

    public void OpenPanel()
    {
        string log = LoggerManager.Instance.GetLogAsSingleString();

        logText.text = log;

        logPanelCanvas.DOFade(1.0f, fadeAnimationTime);
        logPanelCanvas.interactable = true;
        logPanelCanvas.blocksRaycasts = true;
    }

    public void ClosePanel()
    {
        logPanelCanvas.DOFade(0.0f, fadeAnimationTime);
        logPanelCanvas.interactable = false;
        logPanelCanvas.blocksRaycasts = false;
    }
}
