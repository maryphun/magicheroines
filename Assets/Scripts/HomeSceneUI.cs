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

        AudioManager.Instance.PlayMusicWithFade("HomeScene", 2.0f);
    }

    public void ToWorldMapScene()
    {
        // SEçƒê∂
        AudioManager.Instance.PlaySFX("SystemPrebattle");

        AudioManager.Instance.PauseMusic();

        const float animationTime = 1.0f;
        StartCoroutine(SceneTransition("WorldMap", animationTime));
    }

    public IEnumerator SceneTransition(string sceneName, float animationTime)
    {
        // ÉVÅ[ÉìëJà⁄
        AlphaFadeManager.Instance.FadeOut(animationTime);
        yield return new WaitForSeconds(animationTime);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
