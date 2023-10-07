using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class FormationPanel : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField, Range(0.0f, 1.0f)] private float animationTime = 0.5f;
    [SerializeField, Range(0.0f, 1.0f)] private float formationSelectionPanelAnimationTime = 0.15f;
    [SerializeField] private int[] moneyCostForSlot = new int[5];
    [SerializeField] private float iconGap = 200.0f;

    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup FormationSelectionPanel;
    [SerializeField] private FormationSlot[] slots = new FormationSlot[5];
    [SerializeField] private Button[] formationSelectIcon = new Button[8];

    [Header("Debug")]
    [SerializeField] private int formationSelectionPanelIndex = 0;  // 編集中のキャラ位置番号
    [SerializeField] public bool isFormationSelecting;  // キャラ編成中

    public void OpenFormationPanel()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemOpen");

        canvasGroup.DOFade(1.0f, animationTime);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        FormationSelectionPanel.interactable = false;
        FormationSelectionPanel.blocksRaycasts = false;
        FormationSelectionPanel.alpha = 0.0f;
        isFormationSelecting = false;

        formationSelectionPanelIndex = -1;

        InitializeFormation();
    }

    public void QuitFormationPanel()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemCancel");

        canvasGroup.DOFade(0.0f, animationTime);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        FormationSelectionPanel.interactable = false;
        FormationSelectionPanel.blocksRaycasts = false;
        FormationSelectionPanel.alpha = 0.0f;
        isFormationSelecting = false;

        formationSelectionPanelIndex = -1;

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ResetData(animationTime);
        }
    }

    public void InitializeFormation()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            bool isLocked = ProgressManager.Instance.GetUnlockedFormationCount() <= i;
            slots[i].Initialize(isLocked, moneyCostForSlot[i], ProgressManager.Instance.GetUnlockedFormationCount() == i, i);

            var party = ProgressManager.Instance.GetFormationParty(false);
            if (party[i].isFilled)
            {
                slots[i].SetBattler(party[i].characterData);
            }
        }
    }

    public void UpdateFormation(int slot, Character character, bool isNull, bool UpdateUI)
    {
        var originalData = ProgressManager.Instance.GetFormationParty(true);
        originalData[slot].characterData = character;
        originalData[slot].isFilled = !isNull;

        if (UpdateUI)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].ResetData();
            }
            InitializeFormation();
        }
    }

    public void UnlockSlot()
    {
        // 解放するスロット
        int slotIndex = ProgressManager.Instance.GetUnlockedFormationCount();

        // 資金コスト
        ProgressManager.Instance.SetMoney(ProgressManager.Instance.GetCurrentMoney() - moneyCostForSlot[slotIndex]);

        // 解放
        ProgressManager.Instance.UnlockedFormationCount();

        // UI更新
        slots[slotIndex].Initialize(false, moneyCostForSlot[slotIndex], false, slotIndex);

        if (slotIndex < slots.Length-1)
        {
            slots[slotIndex + 1].Initialize(true, moneyCostForSlot[slotIndex + 1], true, slotIndex + 1);
        }

        // SE再生
        AudioManager.Instance.PlaySFX("SystemUnlock", 1.5f);
    }

    public void OpenFormationSelectionPanel(int targetSlotIndex)
    {
        isFormationSelecting = true;
        FormationSelectionPanel.interactable = true;
        FormationSelectionPanel.blocksRaycasts = true;
        FormationSelectionPanel.DOFade(1.0f, formationSelectionPanelAnimationTime);

        var allCharacters = ProgressManager.Instance.GetAllCharacter();
        int characterCount = allCharacters.Count; // キャラクター所持数
        float totalGap = iconGap * (characterCount-1);
        float firstPosition = -totalGap * 0.5f;
        for (int i = 0; i < formationSelectIcon.Length; i++)
        {
            if (i >= characterCount)
            {
                // このキャラはまだ持っていない
                formationSelectIcon[i].gameObject.SetActive(false);
            }
            else
            {
                // 持っている
                formationSelectIcon[i].gameObject.SetActive(true);

                // ボタン配置
                float buttonPosition = firstPosition + (iconGap * i);
                formationSelectIcon[i].GetComponent<RectTransform>().localPosition = new Vector3(buttonPosition, 0.0f, 0.0f);

                // すでに配置されているか
                bool isAlreadyInFormation = IsCharacterInFormation(allCharacters[i]);
                formationSelectIcon[i].interactable = !isAlreadyInFormation;

                // キャラ名を表示
                formationSelectIcon[i].GetComponentInChildren<TMP_Text>().text = allCharacters[i].localizedName;
            }
        }

        formationSelectionPanelIndex = targetSlotIndex;
    }

    public void CloseFormationSelectionPanel(bool isPlaySE)
    {
        isFormationSelecting = false;
        FormationSelectionPanel.interactable = false;
        FormationSelectionPanel.blocksRaycasts = false;
        FormationSelectionPanel.DOFade(0.0f, formationSelectionPanelAnimationTime);

        formationSelectionPanelIndex = -1;

        // SE再生
        if (isPlaySE) AudioManager.Instance.PlaySFX("SystemCancel", 0.5f);
    }

    public void SelectFormationCharacter(int characterID)
    {
        UpdateFormation(formationSelectionPanelIndex, ProgressManager.Instance.GetAllCharacter()[characterID], false, true);

        // SE再生
        AudioManager.Instance.PlaySFX("SystemEquip", 1.5f);

        CloseFormationSelectionPanel(false);
    }

    private bool IsCharacterInFormation(Character character)
    {
        var party = ProgressManager.Instance.GetFormationParty(false);
        foreach (FormationSlotData data in party)
        {
            if (data.isFilled && data.characterData.characterData.characterID == character.characterData.characterID)
            {
                return true;
            }
        }
        return false;
    }
}
