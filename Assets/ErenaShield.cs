using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class ErenaShield : MonoBehaviour
{
    private Battler battler;
    private RectTransform rect;
    private Image img;
    private EventType type;

    public enum EventType
    {
        DivineShield,
        StunShield,
    }

    public void Init(Battler battler, RectTransform rect, EventType type)
    {
        this.battler = battler;
        this.rect = rect;
        img = rect.GetComponent<Image>();
        this.type = type;

        if (this.type == EventType.DivineShield)
        {
            battler.onAttackedEvent.AddListener(OnAttackDivineShield);
        }
        if (this.type == EventType.StunShield)
        {
            battler.onAttackedEvent.AddListener(OnAttackStunShield);
        }
    }

    public void OnAttackDivineShield(int damage)
    {
        battler.onAttackedEvent.RemoveListener(OnAttackDivineShield);
        AudioManager.Instance.PlaySFX("NewAbility"); // shield break

        const float animTime = 0.5f;
        rect.DOScale(3.0f, animTime);
        img.DOFade(0.0f, animTime);
        Destroy(gameObject, animTime + Time.deltaTime);
    }

    public void OnAttackStunShield(int damage)
    {
        battler.onAttackedEvent.RemoveListener(OnAttackStunShield);
        AudioManager.Instance.PlaySFX("NewAbility"); // shield break

        const float animTime = 0.5f;
        rect.DOScale(3.0f, animTime);
        img.DOFade(0.0f, animTime);
        Destroy(gameObject, animTime + Time.deltaTime);
    }

    public void Destroy()
    {
        if (this.type == EventType.DivineShield)
        {
            battler.onAttackedEvent.RemoveListener(OnAttackDivineShield);
        }
        if (this.type == EventType.StunShield)
        {
            battler.onAttackedEvent.RemoveListener(OnAttackStunShield);
        }
        Destroy(gameObject);
    }
}
