using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Assets.SimpleLocalization.Scripts;
using UnityEngine.SceneManagement;

/// <summary>
/// 警戒度
/// </summary>
public struct SideQuestData
{
    public int food;
    public int bank;
    public int research;

    public SideQuestData(int foodQuest, int bankQuest, int researchQuest)
    {
        food = foodQuest;
        bank = bankQuest;
        research = researchQuest;
    }
}

[RequireComponent(typeof(CanvasGroup))]
public class SideQuestPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGrp;
    [SerializeField] private TMP_Text rewardFood, rewardBank, rewardResearch;
    [SerializeField] private TMP_Text alertLevelFood, alertLevelBank, alertLevelResearch;

    public void OpenSideQuestPanel()
    {
        // SE
        AudioManager.Instance.PlaySFX("SystemOpen");

        canvasGrp.DOFade(1.0f, 1.0f);
        canvasGrp.interactable = true;
        canvasGrp.blocksRaycasts = true;

        // Init
        rewardFood.text = LocalizationManager.Localize("System.RewardQuest") + ": " + LocalizationManager.Localize("Battle.Item");
        rewardBank.text = LocalizationManager.Localize("System.RewardQuest") + ": " + LocalizationManager.Localize("System.Money");
        rewardResearch.text = LocalizationManager.Localize("System.RewardQuest") + ": " + LocalizationManager.Localize("System.ResearchPoint");

        alertLevelFood.text = LocalizationManager.Localize("System.AlertLevel") + ": <color=yellow>";
        alertLevelBank.text = LocalizationManager.Localize("System.AlertLevel") + ": <color=yellow>";
        alertLevelResearch.text = LocalizationManager.Localize("System.AlertLevel") + ": <color=yellow>";

        // 警戒度を表す
        string star = LocalizationManager.Localize("System.Star");
        for (int i = 0; i < ProgressManager.Instance.GetSideQuestData().food; i++) alertLevelFood.text = alertLevelFood.text + star;
        for (int i = 0; i < ProgressManager.Instance.GetSideQuestData().bank; i++) alertLevelBank.text = alertLevelBank.text + star;
        for (int i = 0; i < ProgressManager.Instance.GetSideQuestData().research; i++) alertLevelResearch.text = alertLevelResearch.text + star;
    }

    public void CloseSideQuestPanel()
    {
        // SE
        AudioManager.Instance.PlaySFX("SystemCancel");

        canvasGrp.DOFade(0.0f, 1.0f);
        canvasGrp.interactable = false;
        canvasGrp.blocksRaycasts = false;
    }

    public void OnClickSideQuestFood()
    {
        // SE
        AudioManager.Instance.PlaySFX("SystemAlert2");

        // 敵キャラを設置
        BattleSetup.Reset(false);
        BattleSetup.AddEnemy("Android");
        BattleSetup.AddEnemy("Drone");
        BattleSetup.SetBattleBGM("BattleTutorial");
        BattleSetup.SetSideQuestIncrement(1, 0, 0);
        BattleSetup.SetReward(Random.Range(100, 200), Random.Range(10, 50));
        CheckEquipmentDrop();
        BattleSetup.AddItemReward("救急箱");
        BattleSetup.AddItemReward("食パン");
        BattleSetup.AddItemReward("クロワッサン");

        const float animationTime = 1.0f;

        // シーン遷移
        AlphaFadeManager.Instance.FadeOut(animationTime);

        DOTween.Sequence()
               .AppendInterval(animationTime)
               .AppendCallback(() => { SceneManager.LoadScene("Battle", LoadSceneMode.Single); });
    }

    public void OnClickSideQuestBank()
    {
        // SE
        AudioManager.Instance.PlaySFX("SystemAlert2");

        // SE
        AudioManager.Instance.PlaySFX("SystemAlert2");

        // 敵キャラを設置
        BattleSetup.Reset(false);
        BattleSetup.AddEnemy("Android");
        BattleSetup.AddEnemy("Drone");
        BattleSetup.SetBattleBGM("BattleTutorial");
        BattleSetup.SetSideQuestIncrement(1, 0, 0);
        BattleSetup.SetReward(Random.Range(300, 900), Random.Range(10, 50));
        CheckEquipmentDrop();

        const float animationTime = 1.0f;

        // シーン遷移
        AlphaFadeManager.Instance.FadeOut(animationTime);

        DOTween.Sequence()
               .AppendInterval(animationTime)
               .AppendCallback(() => { SceneManager.LoadScene("Battle", LoadSceneMode.Single); });
    }

    public void OnClickSideQuestResearch()
    {
        // SE
        AudioManager.Instance.PlaySFX("SystemAlert2");

        // SE
        AudioManager.Instance.PlaySFX("SystemAlert2");

        // 敵キャラを設置
        BattleSetup.Reset(false);
        BattleSetup.AddEnemy("Android");
        BattleSetup.AddEnemy("Drone");
        BattleSetup.SetBattleBGM("BattleTutorial");
        BattleSetup.SetSideQuestIncrement(1, 0, 0);
        BattleSetup.SetReward(Random.Range(100, 300), Random.Range(100, 300));
        CheckEquipmentDrop();

        const float animationTime = 1.0f;

        // シーン遷移
        AlphaFadeManager.Instance.FadeOut(animationTime);

        DOTween.Sequence()
               .AppendInterval(animationTime)
               .AppendCallback(() => { SceneManager.LoadScene("Battle", LoadSceneMode.Single); });
    }

    private void CheckEquipmentDrop()
    {
        var alertPoint = ProgressManager.Instance.GetSideQuestData();
        if (alertPoint.food == 1 && alertPoint.bank == 1 && alertPoint.research == 1)
        {
            BattleSetup.AddEquipmentReward("Stick");
        }
        if (ProgressManager.Instance.GetCurrentStageProgress() >= 4) // chapter 2
        {
            BattleSetup.AddEquipmentReward("Cushion");
        }
    }
}
