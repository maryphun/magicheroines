using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class BlackMarketManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // PLAY BGM
        AudioManager.Instance.PlayMusicWithFade("DarkHole", 5.0f);
        AlphaFadeManager.Instance.FadeIn(2.0f);

        SetupBlackMarket();
    }

    public void SetupBlackMarket()
    {

    }

    public void OnClickBackButton()
    {
        // SE
        AudioManager.Instance.PlaySFX("BattleTransition");

        // Scene Transit
        StartCoroutine(SceneTransition("Home", 1.0f));
    }


    public IEnumerator SceneTransition(string sceneName, float animationTime)
    {
        // ÉVÅ[ÉìëJà⁄
        AlphaFadeManager.Instance.FadeOut(animationTime);
        yield return new WaitForSeconds(animationTime);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
