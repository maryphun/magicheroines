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

        AudioManager.Instance.PlayMusicWithFade("HomeScene", 2.0f);
    }

    public void ToWorldMapScene()
    {
        const float animationTime = 1.0f;
        StartCoroutine(SceneTransition("WorldMap", animationTime));
    }

    IEnumerator SceneTransition(string sceneName, float animationTime)
    {
        // ÉVÅ[ÉìëJà⁄
        AlphaFadeManager.Instance.FadeOut(animationTime);
        yield return new WaitForSeconds(animationTime);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
