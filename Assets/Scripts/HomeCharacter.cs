using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Assets.SimpleLocalization.Scripts;

public class HomeCharacter : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField, Range(0.0f, 1.0f)] private float clickScale = 0.99f;
    [SerializeField, Range(0.0f, 1.0f)] private float animationTime = 0.6f;
    [SerializeField, Range(1.0f, 5.0f)] private float extraDisplayTime = 3f;

    [Header("References")]
    [SerializeField] RectTransform characterRect;
    [SerializeField] RectTransform dialogueBack;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] Image characterSprite;

    [Header("Debug")]
    [SerializeField] private HomeDialogue[] dialogues;
    [SerializeField] private int currentCharacterIndex;
    [SerializeField] private int lastDialogueIndex;
    [SerializeField] private Sequence animSequence;

    public void Start()
    {
        characterSprite.alphaHitTestMinimumThreshold = 0.5f;
        dialogueBack.localScale = new Vector3(1, 0, 1);
        dialogueText.text = string.Empty;
        currentCharacterIndex = -1;
        lastDialogueIndex = -1;
        animSequence = DOTween.Sequence();

        Initialization();
        SetupCharacter();
        TriggerDialogue();
    }

    private void Initialization()
    {
        var characters = ProgressManager.Instance.GetHomeCharacter();
        if (characters != null)
        {
            dialogues = characters.ToArray();
        }
    }

    public void SetupCharacter()
    {
        int randomIndex;
        if (dialogues.Length <= 1) currentCharacterIndex = -1;
        do
        {
            randomIndex = Random.Range(0, dialogues.Length);
        } while (randomIndex == currentCharacterIndex);

        currentCharacterIndex = randomIndex;

        characterSprite.sprite = dialogues[currentCharacterIndex].characterSprite;
        characterSprite.color = Color.white;
        characterSprite.DOFade(1.0f, 0.5f);
    }

    public void SwapCharacter()
    {
        int randomIndex;
        if (dialogues.Length <= 1) currentCharacterIndex = -1;
        do
        {
            randomIndex = Random.Range(0, dialogues.Length);
        } while (randomIndex == currentCharacterIndex);

        currentCharacterIndex = randomIndex;
        characterSprite.color = Color.white;

        characterSprite.DOFade(0.0f, 0.25f).OnComplete(() => { characterSprite.sprite = dialogues[currentCharacterIndex].characterSprite; });
        characterSprite.DOFade(1.0f, 0.25f).SetDelay(0.3f);

        if (animSequence.IsPlaying())
        {
            animSequence.Complete();
            dialogueText.DOComplete();
            dialogueBack.DOComplete();

            dialogueText.DOFade(0.0f, 0.25f);
            dialogueBack.DOScaleY(0.0f, 0.25f).SetDelay(0.25f);
        }

        OnChangeCharacter();
    }

    public void OnClickCharacter()
    {
        if (characterSprite == null) return;

        // SE
        AudioManager.Instance.PlaySFX("SystemCuteClick");

        // Animation
        DOTween.Sequence()
            .AppendCallback(() => { characterRect.DOScale(clickScale, animationTime / 6f).SetEase(Ease.OutCubic); })
            .AppendInterval(animationTime / 6f)
            .AppendCallback(() => { characterRect.DOScale(1.00f, animationTime - (animationTime / 6f)).SetEase(Ease.OutElastic); });

        TriggerDialogue();
    }

    private void OnChangeCharacter()
    {
        // Display Dialogue
        var dialogue = GetDialogue();
        dialogueText.text = string.Empty;
        dialogueText.alpha = 0.0f;

        float clipLength = 0.0f;
        if (dialogue.clip != null)
        {
            clipLength = dialogue.clip.length;
        }

        DOTween.Sequence()
            .AppendInterval(0.35f)
            .AppendCallback(() => 
            {
                TriggerDialogue();
            });
    }

    public void TriggerDialogue()
    {
        // Display Dialogue
        var dialogue = GetDialogue();
        dialogueText.text = string.Empty;
        dialogueText.alpha = 0.0f;

        float clipLength = 0.0f;
        if (dialogue.clip != null)
        {
            clipLength = dialogue.clip.length;
        }

        if (animSequence.IsPlaying())
        {
            animSequence.Complete();
            dialogueText.DOComplete();
            dialogueBack.DOComplete();

            dialogueText.text = string.Empty;
            dialogueBack.localScale = new Vector3(1, 0, 1);
        }

        animSequence = DOTween.Sequence()
            .AppendCallback(() => { dialogueBack.DOScaleY(1.0f, 0.25f); })
            .AppendInterval(0.25f)
            .AppendCallback(() => {
                dialogueText.DOFade(1.0f, 0.25f);
                dialogueText.DOText(LocalizationManager.Localize(dialogue.dialogueID), Mathf.Clamp(clipLength, 1.0f, 3.0f)).SetEase(Ease.Linear);
                if (dialogue.clip != null)
                {
                    AudioManager.Instance.PlayClip(dialogue.clip);
                }
            })
            .AppendInterval(extraDisplayTime + clipLength)
            .AppendCallback(() => { dialogueText.DOFade(0.0f, 0.25f); })
            .AppendInterval(0.25f)
            .AppendCallback(() => {
                dialogueBack.DOScaleY(0.0f, 0.25f);
            });
    }

    private HomeSceneDialogue GetDialogue()
    {
        int currentStage = ProgressManager.Instance.GetCurrentStageProgress();

        // 表示できるセリフを取得
        int randomIndex;
        if (dialogues[currentCharacterIndex].dialogueList.Count <= 1) lastDialogueIndex = -1;
        do
        {
            randomIndex = Random.Range(0, dialogues[currentCharacterIndex].dialogueList.Count);
        } while (randomIndex == lastDialogueIndex
        || dialogues[currentCharacterIndex].dialogueList[randomIndex].startStage > currentStage
        || dialogues[currentCharacterIndex].dialogueList[randomIndex].endStage <= currentStage);

        lastDialogueIndex = randomIndex;
        return dialogues[currentCharacterIndex].dialogueList[randomIndex];
    }
    
    // 新しく追加されたキャラを表示
    public void SetToLastCharacter()
    {
        Initialization();
        currentCharacterIndex = dialogues.Length-1;
        characterSprite.sprite = dialogues[currentCharacterIndex].characterSprite;
        characterSprite.color = Color.white;
        characterSprite.DOFade(1.0f, 0.5f);
    }

}
