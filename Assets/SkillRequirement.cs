using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using Assets.SimpleLocalization.Scripts;

public class SkillRequirement : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float animTime = 0.15f;

    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private RectTransform rect;

    [Header("Debug")]
    [SerializeField] private int requiredLevel;
    [SerializeField] private bool isHovering;
    [SerializeField] private Vector3 rectPosition;

    public void Initialization(int requiredLevel)
    {
        this.requiredLevel = requiredLevel;
        isHovering = false;
        rectPosition = rect.position + new Vector3(rect.sizeDelta.x * (rect.pivot.x + 0.5f), -rect.sizeDelta.y * (rect.pivot.y-0.5f), 0.0f);
    }

    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition / mainCanvas.scaleFactor;
        if (isHovering)
        {
            if (Vector2.Distance(rectPosition, mousePosition) > rect.sizeDelta.x * 0.5f)
            {
                OnUnhover();
            }
        }
        else
        {
            if (Vector2.Distance(rectPosition, mousePosition) < rect.sizeDelta.x * 0.5f)
            {
                OnHover();
            }
        }
    }

    private void OnHover()
    {
        isHovering = true;
        canvasGroup.DOFade(1, animTime);
        text.text = LocalizationManager.Localize("Syste‚.Requirement") + "\nLv" + requiredLevel;
        var canvasRect = canvasGroup.GetComponent<RectTransform>();
        canvasRect.position = new Vector3(rectPosition.x, canvasRect.position.y, canvasRect.position.z);
    }

    public void OnUnhover()
    {
        isHovering = false;
        canvasGroup.DOFade(0, animTime);
    }
}
