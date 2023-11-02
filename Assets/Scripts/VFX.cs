using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class VFX : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] bool fadeOut = false; // ìßñæÇ…Ç»Ç¡ÇƒÇ©ÇÁè¡Ç¶ÇÈ
    [SerializeField] bool fadeIn = false; // ìßñæÇ©ÇÁì¸ÇÈ

    private void Start()
    {
        var animator = GetComponent<Animator>();
        if (ReferenceEquals(animator.runtimeAnimatorController, null))
        {
            Debug.LogError("Animator controller of " + gameObject.name + " doesn't exist!");
            return;
        }

        if (fadeIn)
        {
            GetComponent<Image>().color = new Color(1, 1, 1, 0);
            GetComponent<Image>().DOFade(1.0f, animator.GetCurrentAnimatorClipInfo(0)[0].clip.length * 0.25f).SetEase(Ease.Linear);
        }

        if (fadeOut)
        {
            GetComponent<Image>().DOFade(0.0f, animator.GetCurrentAnimatorClipInfo(0)[0].clip.length * 0.5f)
                .SetDelay(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length)
                .SetEase(Ease.Linear);

            Object.Destroy(gameObject, animator.GetCurrentAnimatorClipInfo(0)[0].clip.length * 1.5f);
            return;
        }
        Object.Destroy(gameObject, animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
    }
}
