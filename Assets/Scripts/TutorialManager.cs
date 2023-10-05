using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float sceneTransitionTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        NovelSingletone.Instance.PlayNovel("Tutorial1", true, LookAtMonitor);
    }

    // これは先日発生した私達の部隊の戦闘の様子です。 (Dialog.Tutorial-1-17)
    public void LookAtMonitor()
    {
        var sequence = DOTween.Sequence();
        sequence.AppendInterval(1.0f)
                .AppendCallback(() => 
                {
                    NovelSingletone.Instance.PlayNovel("Tutorial2", true, SceneTransit);
                });
    }

    public void SceneTransit()
    {
        StartCoroutine(SceneTransition("Battle", sceneTransitionTime));
    }

    IEnumerator SceneTransition(string sceneName, float animationTime)
    {
        // 敵キャラを設置
        SetupEnemy.ClearEnemy();
        SetupEnemy.AddEnemy("Akiho_Enemy");
        SetupEnemy.AddEnemy("Rikka_Enemy");

        // シーン遷移
        AlphaFadeManager.Instance.FadeOut(animationTime);
        yield return new WaitForSeconds(animationTime);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
