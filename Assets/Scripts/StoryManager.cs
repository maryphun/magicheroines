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
        // âÊñ ëJà⁄
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
    /// êÌì¨ëOóp
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
            case 4:
                {
                    BattleSetup.AddEnemy("Drone");
                    BattleSetup.AddEnemy("Android");
                    BattleSetup.SetBattleBGM("AkihoBattle");
                    BattleSetup.SetReward(550, 50);
                    StartCoroutine(SceneTransition("Battle", 0));
                }
                break;
            case 5:
                {
                    BattleSetup.AddEnemy("Drone");
                    BattleSetup.AddEnemy("Android");
                    BattleSetup.SetBattleBGM("AkihoBattle");
                    BattleSetup.SetReward(600, 50);
                    StartCoroutine(SceneTransition("Battle", 0));
                }
                break;
            case 6:
                {
                    BattleSetup.AddEnemy("Rikka_Enemy");
                    BattleSetup.SetBattleBGM("RikkaBattle");
                    BattleSetup.SetReward(2000, 300);
                    NovelSingletone.Instance.PlayNovel("Chapter2-3 Prebattle", true, GoToBattle);
                }
                break;
            default:
                // ñ¢äJî≠ínë—
                underdevelopment.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// êÌì¨å„óp
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
                    // ñæï‰îsñk
                    NovelSingletone.Instance.PlayNovel("Chapter1-2 AfterBattle", true, GoToRewardScreen);

                    PlayerCharacterDefine Akiho = Resources.Load<PlayerCharacterDefine>("PlayerCharacterList/4.Akiho");
                    ProgressManager.Instance.AddPlayerCharacter(Akiho);
                }
                break;
            case 4:
                {
                    NovelSingletone.Instance.PlayNovel("Chapter2-1", true, GoToRewardScreen);
                }
                break;
            case 5:
                {
                    NovelSingletone.Instance.PlayNovel("Chapter2-2", true, GoToRewardScreen);
                }
                break;
            case 6:
                {
                    // óßâ‘îsñk
                    NovelSingletone.Instance.PlayNovel("Chapter2-3 AfterBattle", true, GoToRewardScreen);

                    PlayerCharacterDefine Rikka = Resources.Load<PlayerCharacterDefine>("PlayerCharacterList/5.Rikka");
                    ProgressManager.Instance.AddPlayerCharacter(Rikka);
                }
                break;
            default:
                // ñ¢äJî≠ínë—
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
        // ÉVÅ[ÉìëJà⁄
        AlphaFadeManager.Instance.FadeOut(animationTime);
        if (animationTime > 0)
        {
            yield return new WaitForSeconds(animationTime);
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
