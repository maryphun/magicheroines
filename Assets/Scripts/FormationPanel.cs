using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class FormationPanel : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField, Range(0.0f, 1.0f)] private float animationTime = 0.5f;
    [SerializeField] private int[] moneyCostForSlot = new int[5];

    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private FormationSlot[] slots = new FormationSlot[5];

    public void OpenFormationPanel()
    {
        canvasGroup.DOFade(1.0f, animationTime);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        InitializeFormation();
    }

    public void QuitFormationPanel()
    {
        canvasGroup.DOFade(0.0f, animationTime);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ResetData(animationTime);
        }
    }

    public void InitializeFormation()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Initialize(ProgressManager.Instance.GetUnlockedFormationCount() <= i, moneyCostForSlot[i]);

            var character = ProgressManager.Instance.GetFormationParty()[i];
            if (!ReferenceEquals(character, null))
            {
                slots[i].SetBattler(character);
            }
        }
    }

    public void UpdateFormation(int slot, Character character)
    {
        var originalData = ProgressManager.Instance.GetFormationParty(true);
        originalData[slot] = character;
    }
}
