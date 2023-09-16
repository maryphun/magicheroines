using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HomeSceneUI : MonoBehaviour
{
    private void Start()
    {
#if DEBUG_MODE
        ProgressManager.Instance.InitializeProgress();
        ProgressManager.Instance.SetMoney(Random.Range(200, 9999));
        ProgressManager.Instance.SetResearchPoint(Random.Range(200, 9999));
#endif
        AlphaFadeManager.Instance.FadeIn(1.0f);
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
