using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.SimpleLocalization.Scripts;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class EquipmentPanel : MonoBehaviour
{
    const int totalSlotNumber = 15;

    [Header("Setting")]
    [SerializeField, Range(0.0f, 1.0f)] private float animTime = 0.5f;

    [Header("References")]
    [SerializeField] private CanvasGroup panel;
    [SerializeField] private Button[] equipmentSlot = new Button[totalSlotNumber];
    
    public void OpenEquipmentPanel()
    {
        panel.DOFade(1.0f, animTime);
        panel.interactable = true;
        panel.blocksRaycasts = true;
    }

    public void CloseEquipmentPanel()
    {
        panel.DOFade(0.0f, animTime);
        DOTween.Sequence().AppendInterval(animTime * 0.5f).AppendCallback(() => {
            panel.interactable = false;
            panel.blocksRaycasts = false;
        });
    }
}
