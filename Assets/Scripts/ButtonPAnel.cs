using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class ButtonPAnel : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float originalPosition; 
    [SerializeField] private float displayDestination; 
    [SerializeField,Range(0.0f, 1.0f)] private float animationTime = 0.25f;

    [Header("References")]
    [SerializeField] private Image arrowIcon; 

    private RectTransform rect;
    private bool isDisplaying;
    private bool isEnabled;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        isDisplaying = false;
        isEnabled = true;
    }

    public void StartDisplay()
    {
        if (isDisplaying) return;

        arrowIcon.DOFade(0.0f, animationTime);
        rect.DOLocalMoveX(displayDestination, animationTime);
        isDisplaying = true;
    }

    public void EndDisplay()
    {
        if (!isDisplaying) return;

        arrowIcon.DOFade(1.0f, animationTime);
        rect.DOLocalMoveX(originalPosition, animationTime);
        isDisplaying = false;
    }

    private void Update()
    {
        if (!isEnabled) return;

        // カーソル位置を取得
        Vector3 mousePosition = Input.mousePosition;
        // カーソル位置のz座標を10に
        mousePosition.z = 10;
        // カーソル位置をワールド座標に変換
        Vector3 target = Camera.main.ScreenToWorldPoint(mousePosition);

        if (mousePosition.x > Screen.currentResolution.width * 0.75f)
        {
            StartDisplay();
        }
        else
        {
            EndDisplay();
        }
    }

    public void SetEnabled(bool enable)
    {
        isEnabled = enable;

        if (!isEnabled) 
        {
            EndDisplay();
        }
    }
}
