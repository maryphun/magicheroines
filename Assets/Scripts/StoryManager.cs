using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private bool isAfterBattle = false;

    [Header("Debug")]
    [SerializeField] private string battleBGM = "";

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
                    StartCoroutine(SceneTransition("Battle", 0));
                    AudioManager.Instance.PlayMusicWithCrossFade("BattleTutorial", 2.0f);
                }
                break;
            case 3:
                {
                    BattleSetup.AddEnemy("Akiho");
                    NovelSingletone.Instance.PlayNovel("Chapter1-2 Prebattle", true, GoToBattle);
                    battleBGM = "AkihoBattle";
                }
                break;
            default:
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
                    NovelSingletone.Instance.PlayNovel("Chapter1-1", true, GoToHomeScreen);
                }
                break;
            case 3:
                {
                    // ñæï‰îsñk
                    NovelSingletone.Instance.PlayNovel("Chapter1-2 AfterBattle", true, GoToHomeScreen);
                }
                break;
            default:
                break;
        }
        ProgressManager.Instance.StageProgress();
    }

    public void GoToBattle()
    {
        if (battleBGM != string.Empty)
        {
            AudioManager.Instance.PlayMusicWithCrossFade(battleBGM, 2.0f);
        }
        StartCoroutine(SceneTransition("Battle", 0));
    }

    public void GoToHomeScreen()
    {
        StartCoroutine(SceneTransition("Home", 0.5f));
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
