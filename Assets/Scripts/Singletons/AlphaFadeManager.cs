using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using DG.Tweening;

public class AlphaFadeManager : SingletonMonoBehaviour<AlphaFadeManager>
{
    Canvas canvasObj = null;
    Image fadeAlpha = null;

    bool initialized = false;

    void Initializer()
    {
        if (initialized) return;

        // Create Canvas
        GameObject canvas = new GameObject("FadeAlphaCanvas");

        canvasObj = canvas.AddComponent<Canvas>();
        canvas.AddComponent<CanvasScaler>();

        /// setting canvas
        canvasObj.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.vertexColorAlwaysGammaSpace = true;
        canvasObj.sortingOrder = 99;

        canvas.transform.SetParent(transform);

        // Create Alpha Image
        GameObject img = new GameObject("FadeAlpha");

        /// setting Image
        fadeAlpha = img.AddComponent<Image>();
        fadeAlpha.color = Color.black;
        fadeAlpha.transform.SetParent(canvas.transform);
        fadeAlpha.rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
        fadeAlpha.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
        fadeAlpha.rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
        fadeAlpha.raycastTarget = true;

        initialized = true;
    }

    /// <summary>
    /// çïÇ¢âÊñ Ç©ÇÁñﬂÇÈ
    /// </summary>
    public void FadeIn(float time = 0.0f)
    {
        if (!initialized) Initializer();

        fadeAlpha.DOComplete();
        fadeAlpha.color = new Color(0, 0, 0, 1);
        fadeAlpha.DOFade(0.0f, time);

        fadeAlpha.raycastTarget = false;
    }

    /// <summary>
    /// âÊñ Ç™íiÅXçïÇ≠Ç»ÇÈ
    /// </summary>
    public void FadeOut(float time = 0.0f)
    {
        if (!initialized) Initializer();

        fadeAlpha.DOComplete();
        fadeAlpha.color = new Color(0, 0, 0, 0);
        fadeAlpha.DOFade(1.0f, time);

        fadeAlpha.raycastTarget = true;
    }

    public void Fade(float start, float end, float time = 0.0f)
    {
        if (!initialized) Initializer();

        fadeAlpha.DOComplete();
        fadeAlpha.DOFade(start, time);
        fadeAlpha.DOFade(end, time);
    }
}
