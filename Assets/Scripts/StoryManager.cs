using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private bool isAfterBattle = false;

    [Header("References")]
    [SerializeField] private GameObject underdevelopment;

    // Start is called before the first frame update
    void Start()
    {
        // 画面遷移
        AlphaFadeManager.Instance.FadeIn(1.0f);

        if (isAfterBattle)
        {
            AfterStory();
        }
        else
        {
            Story();
        }
    }

    /// <summary>
    /// 戦闘前用
    /// </summary>
    void Story()
    {
        BattleSetup.Reset(true);
        switch (ProgressManager.Instance.GetCurrentStageProgress())
        {
            case 2:
                {
                    BattleSetup.AddEnemy("Drone");
                    BattleSetup.AddEnemy("Android");
                    BattleSetup.SetBattleBGM("BattleTutorial");
                    BattleSetup.SetReward(500, 25);
                    StartCoroutine(SceneTransition("Battle", 0));
                }
                break;
            case 3:
                {
                    BattleSetup.AddEnemy("Akiho_Enemy");
                    BattleSetup.SetBattleBGM("AkihoBattle");
                    BattleSetup.SetReward(1500, 150);
                    NovelSingletone.Instance.PlayNovel("Chapter1-2 Prebattle", true, GoToBattle);
                }
                break;
            default:
                // 未開発地帯
                underdevelopment.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 戦闘後用
    /// </summary>
    void AfterStory()
    {
        switch (ProgressManager.Instance.GetCurrentStageProgress())
        {
            case 2:
                {
                    NovelSingletone.Instance.PlayNovel("Chapter1-1", true, GoToRewardScreen);
                }
                break;
            case 3:
                {
                    // 明穂敗北
                    NovelSingletone.Instance.PlayNovel("Chapter1-2 AfterBattle", true, GoToRewardScreen);

                    PlayerCharacterDefine Akiho = Resources.Load<PlayerCharacterDefine>("PlayerCharacterList/4.Akiho");
                    ProgressManager.Instance.AddPlayerCharacter(Akiho);
                    Resources.UnloadAsset(Akiho);
                }
                break;
            default:
                // 未開発地帯
                underdevelopment.SetActive(true);
                break;
        }
        ProgressManager.Instance.StageProgress();
    }

    public void GoToBattle()
    {
        StartCoroutine(SceneTransition("Battle", 0));
    }

    public void GoToHomeScreen()
    {
        StartCoroutine(SceneTransition("Home", 0.5f));
    }

    public void GoToRewardScreen()
    {
        StartCoroutine(SceneTransition("Reward", 0.5f));
    }

    IEnumerator SceneTransition(string sceneName, float animationTime)
    {
        // シーン遷移
        AlphaFadeManager.Instance.FadeOut(animationTime);
        if (animationTime > 0)
        {
            yield return new WaitForSeconds(animationTime);
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
