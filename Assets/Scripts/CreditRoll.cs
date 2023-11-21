using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class CreditRoll : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float rollTime = 5.0f;

    [Header("References")]
    [SerializeField] private Button backButton;
    [SerializeField] private RectTransform roll;

    [Header("Debug")]
    [SerializeField] private bool isShowButton = false;

    // Start is called before the first frame update
    void Start()
    {
        AlphaFadeManager.Instance.FadeIn(0);
        backButton.gameObject.SetActive(false);
        isShowButton = false;
        roll.DOAnchorPosY(1020.0f, rollTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isShowButton)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                isShowButton = true;
                backButton.gameObject.SetActive(true);
            }
        }
    }

    public void BackToTitleScreen()
    {
        StartCoroutine(SceneTransition("Title", 0.5f));
    }

    public IEnumerator SceneTransition(string sceneName, float animationTime)
    {
        // ÉVÅ[ÉìëJà⁄
        AlphaFadeManager.Instance.FadeOut(animationTime);
        yield return new WaitForSeconds(animationTime);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
