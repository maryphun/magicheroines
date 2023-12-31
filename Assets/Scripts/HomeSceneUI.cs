using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HomeSceneUI : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool isDebug = false;

    private void Awake()
    {
        if (isDebug) ProgressManager.Instance.DebugModeInitialize();
    }

    private void Start()
    {
        AlphaFadeManager.Instance.FadeIn(1.0f);

        AudioManager.Instance.PlayMusicWithFade("Loop 32 (HomeScene)", 2.0f);
    }

    public void ToWorldMapScene()
    {
        // SE再生
        AudioManager.Instance.PlaySFX("SystemPrebattle");

        AudioManager.Instance.PauseMusic();

        const float animationTime = 1.0f;
        StartCoroutine(SceneTransition("WorldMap", animationTime));
    }
    public void ToBlackMarketScene()
    {
        // SE再生
        AudioManager.Instance.PlaySFX("BattleTransition");

        const float animationTime = 1.0f;
        AudioManager.Instance.StopMusicWithFade(animationTime);
        StartCoroutine(SceneTransition("BlackMarket", animationTime));
    }

    public IEnumerator SceneTransition(string sceneName, float animationTime)
    {
        // シーン遷移
        AlphaFadeManager.Instance.FadeOut(animationTime);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false; //Don't let the Scene activate until you allow it to
        yield return new WaitForSeconds(animationTime);
        while (asyncLoad.progress < 0.9f) yield return null; // wait until the scene is completely loaded 
        asyncLoad.allowSceneActivation = true;
    }
}
