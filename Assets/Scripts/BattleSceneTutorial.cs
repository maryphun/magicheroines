using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.SimpleLocalization.Scripts;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class BattleSceneTutorial : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup tutorialUI;
    [SerializeField] private TMP_Text tutorialText;

    [Header("Debug")]
    [SerializeField] private Tween currentTween;

    private void Start()
    {
        tutorialUI.alpha = 0.0f;
        tutorialUI.blocksRaycasts = false;
        tutorialUI.interactable = false;
    }

    // Battle Scene用チュートリアルシーケンス
    public void StartBattleTutorial()
    {
        tutorialUI.DOFade(1.0f, 0.5f);
        tutorialUI.blocksRaycasts = true;
        tutorialUI.interactable = true;

        var sequence = DOTween.Sequence();
        sequence.AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    currentTween = tutorialText.DOText(LocalizationManager.Localize("Dialog.Tutorial-3-1"), 1.0f);
                    StartCoroutine(WaitForInput());
                });
    }

    IEnumerator WaitForInput()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        if (!currentTween.IsComplete())
        {
            currentTween.Complete();
        }
        else
        {
            // next
            currentTween = tutorialText.DOText(LocalizationManager.Localize("Dialog.Tutorial-3-2"), 1.0f);
        }

        // 再帰
        StartCoroutine(WaitForInput());
    }
}
