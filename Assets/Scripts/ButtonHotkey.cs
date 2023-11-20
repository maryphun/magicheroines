using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// ボタンをショートカットキーに紐つける
/// </summary>
public class ButtonHotkey : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private KeyCode key;

    [Header("References")]
    [SerializeField] private TMPro.TMP_Text text;
    [SerializeField] private UnityEngine.UI.Button button;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GraphicRaycaster raycaster;

    private void Start()
    {
        if (!ReferenceEquals(text, null)) text.text = key.ToString();

        if (ReferenceEquals(eventSystem, null))
        {
            eventSystem = FindObjectOfType<EventSystem>();
        }
    }

    private void Update()
    {
        if (!button.IsInteractable()) return;
        if (!button.enabled) return;
        if (!button.gameObject.activeInHierarchy) return;

        if (Input.GetKeyDown(key))
        {
            if (CheckIsButtonReachable())
            {
                button.onClick.Invoke();
            }
        }
    }

    private bool CheckIsButtonReachable()
    {
        //Set up the new Pointer Event
        PointerEventData m_PointerEventData = new PointerEventData(eventSystem);
        //Set the Pointer Event Position to that of the mouse position
        m_PointerEventData.position = button.GetComponent<RectTransform>().position;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        raycaster.Raycast(m_PointerEventData, results);

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        if (results[0].gameObject == button.gameObject)
        {
            return true;
        }

        return false;
    }
}
