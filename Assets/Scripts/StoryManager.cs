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
                    BattleSetup.AddEquipmentReward("NiceTshirt");
                    StartCoroutine(SceneTransition("Battle", 0));
                }
                break;
            case 6:
                {
                    BattleSetup.AddEnemy("Rikka_Enemy");
                    BattleSetup.SetBattleBGM("Loop 37 (RikkaBattle)");
                    BattleSetup.SetReward(1500, 300);
                    BattleSetup.SetBattleBack(BattleBack.Basement);
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
                    BattleSetup.AddEquipmentReward("Glove");
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
                    BattleSetup.AddEquipmentReward("Shoes");
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
                    // 戦闘なし
                    BattleSetup.SetReward(2000, 300);
                    NovelSingletone.Instance.PlayNovel("Chapter5-1", true, GoToRewardScreen);
                    ProgressManager.Instance.StageProgress();

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
                    BattleSetup.SetBattleBGM("apoptosis");
                    BattleSetup.SetBattleBack(BattleBack.CentreTower);
                    NovelSingletone.Instance.PlayNovel("Chapter5-3 Prebattle", true, GoToBattle);
                }
                break;
            case 16:
                {
                    BattleSetup.AddEnemy("SeikakuSenki 1");
                    BattleSetup.AddEnemy("SeikakuSenki 2");
                    BattleSetup.AddEnemy("SeikakuSenki 3");
                    BattleSetup.AddEnemy("SeikakuSenki 4");
                    BattleSetup.SetBattleBGM("finalbattle");
                    BattleSetup.SetBattleBack(BattleBack.Council);
                    NovelSingletone.Instance.PlayNovel("ChapterFinal Prebattle", true, GoToBattle);
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
                    // 立花敗北
                    NovelSingletone.Instance.PlayNovel("Chapter2-3 AfterBattle", true, GoToRewardScreen);

                    PlayerCharacterDefine Rikka = Resources.Load<PlayerCharacterDefine>("PlayerCharacterList/5.Rikka");
                    ProgressManager.Instance.AddPlayerCharacter(Rikka);

                    // 侵食記録開放【爆乳怪人明穂の淫靡な日常】
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
                    // エレナ敗北
                    NovelSingletone.Instance.PlayNovel("Chapter3-3 AfterBattle", true, GoToRewardScreen);

                    PlayerCharacterDefine Erena = Resources.Load<PlayerCharacterDefine>("PlayerCharacterList/6.Erena");
                    ProgressManager.Instance.AddPlayerCharacter(Erena);

                    // 侵食記録開放【クローンの通常業務】
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
                    // 京敗北
                    NovelSingletone.Instance.PlayNovel("Chapter4-3 AfterBattle", true, GoToBattle);

                    // setup event battle
                    BattleSetup.Reset(false);
                    BattleSetup.SetAllowEscape(false);
                    BattleSetup.SetEventBattle(true);
                    BattleSetup.SetReward(0, 0);
                    BattleSetup.AddTeammate("10.No5(Event)");
                    BattleSetup.AddTeammate("11.No72(Event)");
                    BattleSetup.SetBattleBack(BattleBack.CentreTower);
                    BattleSetup.AddEnemy("Nayuta_Enemy");
                    BattleSetup.SetBattleBGM("zensen he totugekiseyo (ErenaBattle)");
                }
                break;
            case 13:
                {
                    // 5-1は戦闘がないのでここに辿らない
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
                    NovelSingletone.Instance.PlayNovel("ChapterFinal AfterBattle", true, GoToEndCredit);
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

    public void GoToEndCredit()
    {
        StartCoroutine(SceneTransition("EndCredit", 0.5f));
    }

    IEnumerator SceneTransition(string sceneName, float animationTime)
    {
        // シーン遷移
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
