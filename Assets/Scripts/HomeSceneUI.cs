using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HomeSceneUI : MonoBehaviour
{
    private void Start()
    {
        //ProgressManager.Instance.DebugModeInitialize();
        AlphaFadeManager.Instance.FadeIn(1.0f);

        // チュートリアルに入る?
        if (ProgressManager.Instance.GetCurrentStageProgress() == 0)
        {
            NovelSingletone.Instance.PlayNovel("Tutorial", true);
        }
    }

    public void ToWorldMapScene()
    {
        const float animationTime = 1.0f;
        StartCoroutine(SceneTransition("WorldMap", animationTime));
    }

    IEnumerator SceneTransition(string sceneName, float animationTime)
    {
        // シーン遷移
        AlphaFadeManager.Instance.FadeOut(animationTime);
        yield return new WaitForSeconds(animationTime);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
