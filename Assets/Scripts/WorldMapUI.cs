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
        // BGMÄ¶
        AudioManager.Instance.PlayMusicWithFade("WorldMap", 6.0f);

        // ‰æ–Ê‘JˆÚ
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
        // BGM’âŽ~
        AudioManager.Instance.StopMusicWithFade(1.0f);

        // ƒV[ƒ“‘JˆÚ
        AlphaFadeManager.Instance.FadeOut(animationTime);
        yield return new WaitForSeconds(animationTime);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void DebugNextStage()
    {
        ProgressManager.Instance.StageProgress();
        stagehandler.Init();
    }

    public void ResourceGatheringQuest()
    {
        // SE
        AudioManager.Instance.PlaySFX("SystemOpen");

        // “GƒLƒƒƒ‰‚ðÝ’u
        BattleSetup.Reset(false);
        BattleSetup.AddEnemy("Android");
        BattleSetup.AddEnemy("Drone");
        BattleSetup.SetBattleBGM("BattleTutorial");
        BattleSetup.SetReward(Random.Range(300, 600), Random.Range(100, 300));
        BattleSetup.AddEquipmentReward("Stick");
        BattleSetup.AddItemReward("‹~‹}” ");
        BattleSetup.AddItemReward("‹~‹}” ");
        BattleSetup.AddItemReward("Hƒpƒ“");
        BattleSetup.AddItemReward("Hƒpƒ“");

        const float animationTime = 1.0f;

        // ƒV[ƒ“‘JˆÚ
        AlphaFadeManager.Instance.FadeOut(animationTime);

        DOTween.Sequence()
            .AppendInterval(animationTime)
            .AppendCallback(() => { SceneManager.LoadScene("Battle", LoadSceneMode.Single); });
    }

    public void NextStory()
    {
        const float animationTime = 1.0f;

        // ƒV[ƒ“‘JˆÚ
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
}
