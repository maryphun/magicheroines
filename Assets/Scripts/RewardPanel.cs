using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Assets.SimpleLocalization.Scripts;
using UnityEngine.SceneManagement;

public class RewardPanel : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] private float displayPositionY = -150.0f;
    [SerializeField] private float animTime = 0.25f;
    [SerializeField] private float removeAnimTime = 1f;

    [Header("References")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private CanvasGroup panelCanvas;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private Image itemSprite;

    [SerializeField] private RectTransform moneyRewardPanel;
    [SerializeField] private TMP_Text researchPointValue;
    [SerializeField] private TMP_Text moneyValue;

    [SerializeField] private Button nextButton;

    [Header("Debug")]
    [SerializeField] private bool isFirst = true;

    private void Start()
    {
        DisplayMoneyAndResearchPointReward();
        AlphaFadeManager.Instance.FadeIn(0.5f);

        // 警戒度を更新
        ProgressManager.Instance.SetSideQuestData(ProgressManager.Instance.GetSideQuestData().food + BattleSetup.sideQuestIncrement.food,
                                                  ProgressManager.Instance.GetSideQuestData().bank + BattleSetup.sideQuestIncrement.bank,
                                                  ProgressManager.Instance.GetSideQuestData().research + BattleSetup.sideQuestIncrement.research);
    }

    private void PlayAnimation()
    {
        panel.DOComplete();
        panelCanvas.DOComplete();
        
        if (isFirst)
        {
            HidePanel(moneyRewardPanel.GetComponent<CanvasGroup>(), moneyRewardPanel);
        }
        else
        {
            var copy = Instantiate(panel.gameObject, panel.transform.parent);
            copy.transform.SetSiblingIndex(0);
            HidePanel(copy.GetComponent<CanvasGroup>(), copy.GetComponent<RectTransform>());
            Destroy(copy, animTime);
        }

        panelCanvas.alpha = 0.0f;
        panel.anchoredPosition = new Vector2(0.0f, 0.0f);
        panel.DOAnchorPosY(displayPositionY, animTime);
        panelCanvas.DOFade(1.0f, animTime);

        isFirst = false;

        // SE
        AudioManager.Instance.PlaySFX("BattleTransition");
    }

    private void UpdateInformation()
    {
        // determine item or equipment to display, item first
        if (BattleSetup.itemReward != null && BattleSetup.itemReward.Count > 0)
        {
            var item = BattleSetup.itemReward[BattleSetup.itemReward.Count - 1];
            itemName.text = LocalizationManager.Localize(item.itemNameID);
            itemSprite.sprite = item.Icon;

            ProgressManager.Instance.AddItemToInventory(item);
            BattleSetup.itemReward.Remove(item);
        }
        else if (BattleSetup.equipmentReward != null && BattleSetup.equipmentReward.Count > 0)
        {
            var equipment = BattleSetup.equipmentReward[BattleSetup.equipmentReward.Count - 1];
            itemName.text = LocalizationManager.Localize(equipment.equipNameID);
            itemSprite.sprite = equipment.Icon;

            ProgressManager.Instance.AddNewEquipment(equipment);
            BattleSetup.equipmentReward.Remove(equipment);
        }
        else　// reward end
        {
            panelCanvas.DOKill(false);
            panelCanvas.alpha = 0.0f;

            nextButton.interactable = false;

            // SE
            AudioManager.Instance.PlaySFX("RewardEnd");

            // scene transition
            AlphaFadeManager.Instance.FadeOut(1.0f);
            string nextScene = BattleSetup.isStoryMode ? "Home" : "WorldMap"; 
            DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() => { SceneManager.LoadScene(nextScene, LoadSceneMode.Single); });
        }
    }

    public void Next()
    {
        // SE
        AudioManager.Instance.PlaySFX("SystemSwitch");

        PlayAnimation();
        UpdateInformation();
    }

    private void DisplayMoneyAndResearchPointReward()
    {
        // SE
        AudioManager.Instance.PlaySFX("Reward");

        // UI
        researchPointValue.DOCounter(0, BattleSetup.researchPointReward, 2.0f);
        moneyValue.DOCounter(0, BattleSetup.moneyReward, 2.0f);

        // Add resources
        ProgressManager.Instance.SetResearchPoint(ProgressManager.Instance.GetCurrentResearchPoint() + BattleSetup.researchPointReward);
        ProgressManager.Instance.SetMoney(ProgressManager.Instance.GetCurrentMoney() + BattleSetup.moneyReward);

        // Reset
        BattleSetup.SetReward(0, 0);
    }

    private void HidePanel(CanvasGroup fade, RectTransform rect)
    {
        rect.DOScale(0.5f, removeAnimTime);
        fade.DOFade(0.0f, removeAnimTime);
    }
}
