using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class FloatingText : MonoBehaviour
{
    TMP_Text text;
    float liveTime, timeElapsed;
    float fontSize;

    public void Init(float time, Vector3 originalPos, Vector3 moveDir, string displaytext, float maxFontSize, Color color)
    {
        text = GetComponent<TMP_Text>();
        text.fontSize = maxFontSize;
        text.text = displaytext;
        text.color = color;
        fontSize = maxFontSize;
        liveTime = time;
        text.rectTransform.position = originalPos;
        text.rectTransform.DOMove(text.rectTransform.position + (moveDir.normalized * 100.0f), liveTime);

        Destroy(gameObject, time);


        text.rectTransform.localScale = Vector3.zero;
        var sequence = DOTween.Sequence(); //Sequenceê∂ê¨
                                           //TweenÇÇ¬Ç»Ç∞ÇÈ
        sequence.Append(text.rectTransform.DOScale(1.0f, time * 0.5f))
                .Append(text.DOFade(0.0f, time * 0.5f));
    }

    private void Update()
    {
        return;

        timeElapsed += Time.deltaTime;

        text.fontSize = fontSize * Mathf.PingPong((timeElapsed / liveTime), 0.5f) * 2.0f;
    }
}
