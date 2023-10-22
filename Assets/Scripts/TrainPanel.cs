using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Assets.SimpleLocalization.Scripts;

[RequireComponent(typeof(CanvasGroup))]
public class TrainPanel : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField, Range(0.0f, 1.0f)] private float animationTime = 0.5f;

    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image characterImg;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private TMP_Text currentMood;
    [SerializeField] private Image darkGaugeFill, holyCoreGaugeFill;
    [SerializeField] private RectTransform previousCharacterBtn, nextCharacterBtn;
    [SerializeField] private Button hornyActionBtn, corruptActionBtn, researchBtn;
    [SerializeField] private GameObject unavailablePanel;

    [Header("Debug")]
    [SerializeField] private Vector2 previousCharacterBtnPos, nextCharacterBtnPos;
    [SerializeField] private List<Character> characters;
    [SerializeField] private int currentIndex;

    public void OpenTrainPanel()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemOpen");

        // UI フェイド
        canvasGroup.DOFade(1.0f, animationTime);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // 初期化
        previousCharacterBtnPos = previousCharacterBtn.localPosition;
        nextCharacterBtnPos = nextCharacterBtn.localPosition;

        // データをロード
        characters = ProgressManager.Instance.GetAllCharacter(false);

        // ヒロインじゃないキャラを排除
        currentIndex = 0;
        characters.RemoveAll(s => !s.characterData.is_heroin);

        if (characters.Count <= 0)
        {
            // 捕獲したヒロインがいない
            unavailablePanel.SetActive(true);
            return;
        }
        else
        {
            characterImg.gameObject.SetActive(true);
        }
        UpdateCharacterData();
    }

    public void QuitTrainPanel()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemCancel");

        canvasGroup.DOFade(0.0f, animationTime);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // データをクリア
        characters.Clear();
        characters = null;
    }

    public void NextCharacter()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemSwitch");

        // UI Animation
        const float animTime = 0.2f;
        DOTween.Sequence()
            .AppendCallback(() =>
            {
                nextCharacterBtn.DOLocalMoveX(nextCharacterBtnPos.x + 10.0f, animTime * 0.5f);
            })
            .AppendInterval(animTime * 0.5f)
            .AppendCallback(() =>
            {
                nextCharacterBtn.DOLocalMoveX(nextCharacterBtnPos.x, animTime * 0.5f);
            });

        // Calculate Index
        currentIndex++;
        if (currentIndex >= characters.Count)
        {
            currentIndex = 0;
        }

        // Change character
        UpdateCharacterData();
    }

    public void PreviousCharacter()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemSwitch");

        // UI Animation
        const float animTime = 0.2f;
        DOTween.Sequence()
            .AppendCallback(() => 
            { 
                previousCharacterBtn.DOLocalMoveX(previousCharacterBtnPos.x - 10.0f, animTime * 0.5f); 
            })
            .AppendInterval(animTime * 0.5f)
            .AppendCallback(() => 
            {
                previousCharacterBtn.DOLocalMoveX(previousCharacterBtnPos.x, animTime * 0.5f);
            });

        // Calculate Index
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = characters.Count-1;
        }

        // Change character
        UpdateCharacterData();
    }

    public void UpdateCharacterData()
    {
        // 名前
        characterName.text = characters[currentIndex].localizedName;

        // 条件を満たしている「心情」
        for (int i = 0; i < characters[currentIndex].characterData.characterStatus.Count; i++)
        {
            if (characters[currentIndex].dark_gauge >= characters[currentIndex].characterData.characterStatus[i].requiredDarkGauge
                && characters[currentIndex].horny_gauge >= characters[currentIndex].characterData.characterStatus[i].requiredHornyGauge)
            {
                currentMood.text = LocalizationManager.Localize(characters[currentIndex].characterData.characterStatus[i].moodNameID);
                characterImg.sprite = characters[currentIndex].characterData.characterStatus[i].character;
            }
        }

        darkGaugeFill.fillAmount = ((float)characters[currentIndex].dark_gauge) / 100.0f;
        holyCoreGaugeFill.fillAmount = ((float)characters[currentIndex].holyCore_ResearchRate) / 100.0f;

        // ボタンを更新
        hornyActionBtn.interactable = characters[currentIndex].horny_gauge < 100.0f;
        corruptActionBtn.interactable = characters[currentIndex].dark_gauge < 100.0f;
        researchBtn.interactable = characters[currentIndex].holyCore_ResearchRate < 100.0f;
    }

    /// <summary>
    /// 淫乱調教
    /// </summary>
    public void HornyTraining()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemTrainPanel");

        characters[currentIndex].horny_gauge += 25;
        UpdateCharacterData();
    }

    /// <summary>
    /// 洗脳調教
    /// </summary>
    public void BrainwashTraining()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemTrainPanel");

        characters[currentIndex].dark_gauge += 25;
        UpdateCharacterData();
    }

    /// <summary>
    /// 聖核研究
    /// </summary>
    public void HolyCoreResearch()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemTrainPanel");

        characters[currentIndex].holyCore_ResearchRate += 25;
        UpdateCharacterData();
    }
}
