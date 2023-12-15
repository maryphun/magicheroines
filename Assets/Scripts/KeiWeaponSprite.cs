using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeiWeaponSprite : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private Vector2 moveValue = new Vector2(0.0f, 10.0f);

    [Header("References")]
    [SerializeField] private Battler mainScript;

    [Header("Debug")]
    [SerializeField] private RectTransform graphic;
    [SerializeField] private Vector2 origin;
    [SerializeField] private bool isActive = false;

    [HideInInspector] public RectTransform Rect { get { return graphic; } }

    public void Init()
    {
        graphic = GetComponent<RectTransform>();
        origin = graphic.localPosition;
    }

    private void Update()
    {
        if (!isActive) return;

        float ease = mainScript.Ease; 
        
        Mathf.PingPong(ease, 1.0f);

        float value = EaseInOutSine(ease);

        graphic.localPosition = origin + (moveValue * value);
    }

    private float EaseInOutSine(float x)
    {
        return -(Mathf.Cos(Mathf.PI * x) - 1.0f) / 2.0f;
    }

    public void SetEnableMovement(bool boolean)
    {
        isActive = boolean;
    }
}
