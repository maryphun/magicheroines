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
        // BGMçƒê∂
        AudioManager.Instance.PlayMusicWithFade("WorldMap", 6.0f);

        // âÊñ ëJà⁄
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
        // BGMí‚é~
        AudioManager.Instance.StopMusicWithFade(1.0f);

        // ÉVÅ[ÉìëJà⁄
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

        // ÉVÅ[ÉìëJà⁄
        AlphaFadeManager.Instance.FadeOut(animationTime);
        DOTween.Sequence()
            .AppendInterval(animationTime)
            .AppendCallback(() => { SceneManager.LoadScene("Story", LoadSceneMode.Single); });

        // SE
        AudioManager.Instance.PlaySFX("QuestStart");

        // Switch BGM
        AudioManager.Instance.StopMusicWithFade(animationTime);
    }

    private string GetChapterName(int progress)
    {
        return "Chapter " + (((progress - 1) / 3) + 1).ToString() + "-" + (((progress - 1) % 3) + 1).ToString();
    }

    private bool CheckCondition()
    {
        // èåèÇ™ñûÇΩÇ≥ÇÍÇƒÇ¢Ç»Ç¢?
        if (ProgressManager.Instance.GetCurrentStageProgress() == 6) // óßâ‘îsñkChapter
        {
            if (!ProgressManager.Instance.HasCharacter(3))
            {
                NovelSingletone.Instance.PlayNovel("Condition Chapter2-3", true);
                return false;
            }
        }

        return true;
    }
}
