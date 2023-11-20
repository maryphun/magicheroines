using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MemoryPanel : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField, Range(0.0f, 1.0f)] private float animationTime = 0.5f;

    [Header("References")]
    [SerializeField] private CanvasGroup canvasGrp;

    public void OpenMemoryPanel()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemOpen");

        // UI フェイド
        canvasGrp.DOFade(1.0f, animationTime);
        canvasGrp.interactable = true;
        canvasGrp.blocksRaycasts = true;
    }

    public void CloseMemoryPanel()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemCancel");

        canvasGrp.DOFade(0.0f, animationTime);
        canvasGrp.interactable = false;
        canvasGrp.blocksRaycasts = false;
    }
}
