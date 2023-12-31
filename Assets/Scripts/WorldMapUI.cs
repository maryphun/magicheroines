using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class WorldMapUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StageHandler stagehandler;
    [SerializeField] private TMPro.TMP_Text chapterName;

    private void Start()
    {
        // BGM再生
        AudioManager.Instance.PlayMusicWithFade("Specification", 6.0f);

        // 画面遷移
        AlphaFadeManager.Instance.FadeIn(1.0f);

        chapterName.text = GetChapterName(ProgressManager.Instance.GetCurrentStageProgress());
    }

    public void ToHomeScene()
    {
        // SE
        AudioManager.Instance.PlaySFX("SystemTrainPanel");

        const float animationTime = 1.0f;
        StartCoroutine(SceneTransition("Home", animationTime));
    }

    IEnumerator SceneTransition(string sceneName, float animationTime)
    {
        // BGM停止
        AudioManager.Instance.StopMusicWithFade(1.0f);

        // シーン遷移
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false; //Don't let the Scene activate until you allow it to

        AlphaFadeManager.Instance.FadeOut(animationTime);

        yield return new WaitForSeconds(animationTime);
        while (asyncLoad.progress < 0.9f) yield return null; // wait until the scene is completely loaded 

        asyncLoad.allowSceneActivation = true;
    }

    public void DebugNextStage()
    {
        ProgressManager.Instance.StageProgress();
        stagehandler.Init();
    }

    public void NextStory()
    {
        if (!CheckCondition()) return;

        const float animationTime = 1.0f;

        // シーン遷移
        AlphaFadeManager.Instance.FadeOut(animationTime);
        DOTween.Sequence()
            .AppendInterval(animationTime)
            .AppendCallback(() => { SceneManager.LoadScene("Story", LoadSceneMode.Single); });

        // SE
        AudioManager.Instance.PlaySFX("QuestStart");

        // Switch BGM
        AudioManager.Instance.StopMusicWithFade(animationTime);

        // オートセーブを実行する
        AutoSave.ExecuteAutoSave();
    }

    private string GetChapterName(int progress)
    {
        return "Chapter " + (((progress - 1) / 3) + 1).ToString() + "-" + (((progress - 1) % 3) + 1).ToString();
    }

    private bool CheckCondition()
    {
        // 条件が満たされていない?
        if (ProgressManager.Instance.GetCurrentStageProgress() == 6) // 2ｰ3(六花編最終話)←明穂闇堕ち最終段階が必要
        {
            if (!ProgressManager.Instance.HasCharacter(3, true))
            {
                NovelSingletone.Instance.PlayNovel("Condition Chapter2-3", true);
                return false;
            }
        }
        else if (ProgressManager.Instance.GetCurrentStageProgress() == 9) // 3-3(エレナ編最終話)←六花闇堕ち最終段階が必要
        {
            if (!ProgressManager.Instance.HasCharacter(4, true))
            {
                NovelSingletone.Instance.PlayNovel("Condition Chapter3-3", true);
                return false;
            }
        }
        else if (ProgressManager.Instance.GetCurrentStageProgress() == 15) // 5‐3(那由多編最終話)←エレナ闇堕ち最終段階が必要
        {
            if (!ProgressManager.Instance.HasCharacter(5, true))
            {
                NovelSingletone.Instance.PlayNovel("Condition Chapter5-3", true);
                return false;
            }
        }

        return true;
    }
}
