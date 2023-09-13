using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class CharacterBuildingPanel : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField, Range(0.0f, 1.0f)] private float animationTime = 0.5f;

    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject characterDataPanel;
    [SerializeField] private GameObject characterUpgradePanel;
    [SerializeField] private GameObject characterDataButton;
    [SerializeField] private GameObject characterUpgradeButton;
    [SerializeField] private Image[] characterIconSlots;
    [SerializeField] private RectTransform pinkPanel;

    [Header("Debug")]
    [SerializeField] List<Character> characters;
    [SerializeField, HideInInspector] private float tabLocalPosY;

    private Color _darkenedTabColor = new Color(0.75f, 0.75f, 0.75f, 1.0f);
    const float _pinkPanelShakeTime = 0.1f;
    const float _pinkPanelShakeMagnitude = 2.5f;

    public void OpenCharacterBuildingPanel()
    {
        canvasGroup.DOFade(1.0f, animationTime);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // 初期化
        tabLocalPosY = characterDataButton.GetComponent<RectTransform>().localPosition.y;
        SwitchToCharacterDataTab();

        // キャラクター資料を取得して表示する
        characters = ProgressManager.Instance.GetAllCharacter();
        for (int i = 0; i < characters.Count; i++)
        {
            int index = characters[i].characterData.characterID;

            characterIconSlots[index].sprite = characters[i].characterData.icon;
        }
    }

    public void QuitCharacterBuildingPanel()
    {
        canvasGroup.DOFade(0.0f, animationTime);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        this.characters.Clear();
    }

    /// <summary>
    /// 資料タブ
    /// </summary>
    public void SwitchToCharacterDataTab()
    {
        characterDataButton.GetComponent<Image>().color = Color.white;
        characterDataButton.GetComponent<Button>().interactable = false;
        characterDataButton.GetComponent<RectTransform>().localPosition = new Vector3(characterDataButton.GetComponent<RectTransform>().localPosition.x, 
                                                                                      tabLocalPosY, 
                                                                                      characterDataButton.GetComponent<RectTransform>().localPosition.z);
        characterUpgradeButton.GetComponent<Image>().color = _darkenedTabColor;
        characterUpgradeButton.GetComponent<Button>().interactable = true;
        characterUpgradeButton.GetComponent<RectTransform>().localPosition = new Vector3(characterUpgradeButton.GetComponent<RectTransform>().localPosition.x,
                                                                                         tabLocalPosY - 20f,
                                                                                         characterUpgradeButton.GetComponent<RectTransform>().localPosition.z);

        characterDataPanel.SetActive(true);
        characterUpgradePanel.SetActive(false);

        ShakeManager.Instance.ShakeObject(pinkPanel, _pinkPanelShakeTime, _pinkPanelShakeMagnitude);
    }

    /// <summary>
    /// 育成タブ
    /// </summary>
    public void SwitchToCharacterUpgradeTab()
    {
        characterUpgradeButton.GetComponent<Image>().color = Color.white;
        characterUpgradeButton.GetComponent<Button>().interactable = false;
        characterUpgradeButton.GetComponent<RectTransform>().localPosition = new Vector3(characterUpgradeButton.GetComponent<RectTransform>().localPosition.x,
                                                                                        tabLocalPosY,
                                                                                        characterUpgradeButton.GetComponent<RectTransform>().localPosition.z);
        characterDataButton.GetComponent<Image>().color = _darkenedTabColor;
        characterDataButton.GetComponent<Button>().interactable = true;
        characterDataButton.GetComponent<RectTransform>().localPosition = new Vector3(characterDataButton.GetComponent<RectTransform>().localPosition.x,
                                                                                      tabLocalPosY - 20f,
                                                                                      characterDataButton.GetComponent<RectTransform>().localPosition.z);

        characterUpgradePanel.SetActive(true);
        characterDataPanel.SetActive(false);

        ShakeManager.Instance.ShakeObject(pinkPanel, _pinkPanelShakeTime, _pinkPanelShakeMagnitude);
    }
}
