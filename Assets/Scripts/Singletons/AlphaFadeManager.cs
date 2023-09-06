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
        canvasObj.sortingOrder = 1;

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

        initialized = true;
    }

    /// <summary>
    /// çïÇ¢âÊñ Ç©ÇÁñﬂÇÈ
    /// </summary>
    public void FadeIn(float time = 0.0f)
    {
        if (!initialized) Initializer();

        fadeAlpha.DOComplete();
        fadeAlpha.DOFade(1.0f, time);
        fadeAlpha.DOFade(0.0f, time);
    }

    /// <summary>
    /// âÊñ Ç™íiÅXçïÇ≠Ç»ÇÈ
    /// </summary>
    public void FadeOut(float time = 0.0f)
    {
        if (!initialized) Initializer();

        fadeAlpha.DOComplete();
        fadeAlpha.DOFade(0.0f, time);
        fadeAlpha.DOFade(1.0f, time);
    }

    public void Fade(float start, float end, float time = 0.0f)
    {
        if (!initialized) Initializer();

        fadeAlpha.DOComplete();
        fadeAlpha.DOFade(start, time);
        fadeAlpha.DOFade(end, time);
    }
}
