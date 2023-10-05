using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class BattleBackground : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private Vector2 movement;

    [Header("References")]
    [SerializeField] private Canvas mainCanvas;

    private Vector2 originalPosition;
    private RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        originalPosition = rect.anchoredPosition;
        rect.sizeDelta = rect.sizeDelta + movement;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = (Input.mousePosition / mainCanvas.scaleFactor) / new Vector2(Screen.width, Screen.height) - new Vector2(0.5f, 0.5f);
        mousePosition = new Vector2(Mathf.Clamp(mousePosition.x, -0.5f, 0.5f), Mathf.Clamp(mousePosition.y, -0.5f, 0.5f));

        rect.anchoredPosition = originalPosition + (mousePosition * movement);
    }
}
