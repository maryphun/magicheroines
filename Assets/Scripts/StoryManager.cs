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
        BattleSetup.SetAllowEscape(true);
        switch (ProgressManager.Instance.GetCurrentStageProgress())
        {
            case 2:
                {
                    BattleSetup.AddEnemy("Drone");
                    BattleSetup.AddEnemy("Android");
                    BattleSetup.SetBattleBGM("BattleTutorial");
                    BattleSetup.SetReward(350, 25);
                    StartCoroutine(SceneTransition("Battle", 0));
                }
                break;
            case 3:
                {
                    BattleSetup.AddEnemy("Akiho_Enemy");
                    BattleSetup.SetBattleBGM("AkihoBattle");
                    BattleSetup.SetReward(1250, 150);
                    BattleSetup.SetBattleBack(BattleBack.Basement);
                    NovelSingletone.Instance.PlayNovel("Chapter1-2 Prebattle", true, GoToBattle);
                }
                break;
            case 4:
                {
                    BattleSetup.AddEnemy("Drone");
                    BattleSetup.AddEnemy("GoldAndroid");
                    BattleSetup.SetBattleBGM("AkihoBattle");
                    BattleSetup.SetReward(400, 50);
                    StartCoroutine(SceneTransition("Battle", 0));
                }
                break;
            case 5:
                {
                    BattleSetup.AddEnemy("GoldAndroid");
                    BattleSetup.AddEnemy("GoldAndroid 2");
                    BattleSetup.SetBattleBGM("AkihoBattle");
                    BattleSetup.SetReward(450, 50);
                    BattleSetup.SetBattleBack(BattleBack.Basement);
                    StartCoroutine(SceneTransition("Battle", 0));
                }
                break;
            case 6:
                {
                    BattleSetup.AddEnemy("Rikka_Enemy");
                    BattleSetup.SetBattleBGM("Loop 37 (RikkaBattle)");
                    BattleSetup.SetReward(1500, 300);
                    NovelSingletone.Instance.PlayNovel("Chapter2-3 Prebattle", true, GoToBattle);
                }
                break;
            case 7:
                {
                    BattleSetup.AddEnemy("Tank");
                    BattleSetup.SetBattleBGM("Loop 12 (Battle2)");
                    BattleSetup.SetReward(500, 75);
                    StartCoroutine(SceneTransition("Battle", 0));
                }
                break;
            case 8:
                {
                    BattleSetup.AddEnemy("GoldDrone");
                    BattleSetup.AddEnemy("DarkAndroid");
                    BattleSetup.AddEnemy("GoldDrone");
                    BattleSetup.SetBattleBGM("Loop 12 (Battle2)");
                    BattleSetup.SetReward(550, 75);
                    StartCoroutine(SceneTransition("Battle", 0));
                }
                break;
            case 9:
                {
                    BattleSetup.AddEnemy("Erena_Enemy");
                    BattleSetup.SetBattleBGM("zensen he totugekiseyo (ErenaBattle)");
                    BattleSetup.SetReward(1750, 300);
                    BattleSetup.SetBattleBack(BattleBack.Council);
                    NovelSingletone.Instance.PlayNovel("Chapter3-3 Prebattle", true, GoToBattle);
                }
                break;
            case 10:
                {
                    BattleSetup.AddEnemy("DarkTank");
                    BattleSetup.SetBattleBGM("Loop (Battle3)");
                    BattleSetup.SetReward(600, 100);
                    StartCoroutine(SceneTransition("Battle", 0));
                }
                break;
            case 11:
                {
                    BattleSetup.AddEnemy("Drone 4");
                    BattleSetup.AddEnemy("DarkAndroid");
                    BattleSetup.AddEnemy("DarkAndroid 2");
                    BattleSetup.AddEnemy("Drone 4");
                    BattleSetup.SetBattleBGM("Loop (Battle3)");
                    BattleSetup.SetReward(600, 100);
                    StartCoroutine(SceneTransition("Battle", 0));
                }
                break;
            case 12:
                {
                    BattleSetup.AddEnemy("Kei_Enemy");
                    BattleSetup.SetBattleBGM("Mystic Edge (KeiBattle)");
                    BattleSetup.SetBattleBack(BattleBack.CentreTower);
                    NovelSingletone.Instance.PlayNovel("Chapter4-3 Prebattle", true, GoToBattle);
                }
                break;
            case 13:
                {
                    // êÌì¨Ç»Çµ
                    BattleSetup.SetReward(2000, 300);
                    NovelSingletone.Instance.PlayNovel("Chapter5-1", true, GoToRewardScreen);

                    PlayerCharacterDefine Kei = Resources.Load<PlayerCharacterDefine>("PlayerCharacterList/7.Kei");
                    ProgressManager.Instance.AddPlayerCharacter(Kei);
                }
                break;
            case 14:
                {
                    BattleSetup.AddEnemy("GoldDrone");
                    BattleSetup.AddEnemy("DarkAndroid");
                    BattleSetup.AddEnemy("GoldDrone");
                    BattleSetup.SetBattleBGM("Loop 12 (Battle2)");
                    BattleSetup.SetReward(650, 115);
                    StartCoroutine(SceneTransition("Battle", 0));
                }
                break;
            case 15:
                {
                    BattleSetup.AddEnemy("Nayuta_Enemy");
                    BattleSetup.SetBattleBGM("Mystic Edge (KeiBattle)");
                    BattleSetup.SetBattleBack(BattleBack.CentreTower);
                    NovelSingletone.Instance.PlayNovel("Chapter5-3 Prebattle", true, GoToBattle);
                }
                break;
            case 16:
                {
                    BattleSetup.AddEnemy("Nayuta_Enemy");
                    BattleSetup.SetBattleBGM("finalbattle");
                    BattleSetup.SetBattleBack(BattleBack.Council);
                    NovelSingletone.Instance.PlayNovel("ChapterFinal Prebattle", true, GoToBattle);
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

                    // êNêHãLò^äJï˙Åyîöì˚âˆêlñæï‰ÇÃà˙ÍrÇ»ì˙èÌÅz
                    ProgressManager.Instance.AddNewRecord("Record.AkihoPaizuri", "AkihoPaizuri");
                }
                break;
            case 7:
                {
                    NovelSingletone.Instance.PlayNovel("Chapter3-1", true, GoToRewardScreen);
                }
                break;
            case 8:
                {
                    NovelSingletone.Instance.PlayNovel("Chapter3-2", true, GoToRewardScreen);
                }
                break;
            case 9:
                {
                    // ÉGÉåÉiîsñk
                    NovelSingletone.Instance.PlayNovel("Chapter3-3 AfterBattle", true, GoToRewardScreen);

                    PlayerCharacterDefine Erena = Resources.Load<PlayerCharacterDefine>("PlayerCharacterList/6.Erena");
                    ProgressManager.Instance.AddPlayerCharacter(Erena);

                    // êNêHãLò^äJï˙ÅyÉNÉçÅ[ÉìÇÃí èÌã∆ñ±Åz
                    ProgressManager.Instance.AddNewRecord("Record.CloneTask", "CloneTask");
                }
                break;
            case 10:
                {
                    NovelSingletone.Instance.PlayNovel("Chapter4-1", true, GoToRewardScreen);
                }
                break;
            case 11:
                {
                    NovelSingletone.Instance.PlayNovel("Chapter4-2", true, GoToRewardScreen);
                }
                break;
            case 12:
                {
                    // ãûîsñk
                    NovelSingletone.Instance.PlayNovel("Chapter4-3 AfterBattle", true, GoToBattle);

                    // setup event battle
                    BattleSetup.Reset(false);
                    BattleSetup.SetAllowEscape(false);
                    BattleSetup.SetEventBattle(true);
                    BattleSetup.SetReward(0, 0);
                    BattleSetup.AddTeammate("10.No5(Event)");
                    BattleSetup.AddTeammate("11.No72(Event)");
                    BattleSetup.AddEnemy("Nayuta_Enemy");
                    BattleSetup.SetBattleBGM("zensen he totugekiseyo (ErenaBattle)");
                }
                break;
            case 13:
                {
                    // 5-1ÇÕêÌì¨Ç™Ç»Ç¢ÇÃÇ≈Ç±Ç±Ç…íHÇÁÇ»Ç¢
                }
                break;
            case 14:
                {
                    NovelSingletone.Instance.PlayNovel("Chapter5-2", true, GoToRewardScreen);
                }
                break;
            case 15:
                {
                    NovelSingletone.Instance.PlayNovel("Chapter5-3 AfterBattle", true, GoToRewardScreen);
                }
                break;
            case 16:
                {
                    NovelSingletone.Instance.PlayNovel("ChapterFinal AfterBattle", true, GoToRewardScreen);
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
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false; //Don't let the Scene activate until you allow it to
        if (animationTime > 0)
        {
            yield return new WaitForSeconds(animationTime);
        }
        while (asyncLoad.progress < 0.9f) yield return null; // wait until the scene is completely loaded 
        asyncLoad.allowSceneActivation = true;
    }
}
