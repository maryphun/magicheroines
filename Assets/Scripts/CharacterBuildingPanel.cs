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

    [Header("Debug")]
    [SerializeField] List<Character> characters;

    public void OpenCharacterBuildingPanel()
    {
        canvasGroup.DOFade(1.0f, animationTime);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // 初期化
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

    public void SwitchToCharacterDataTab()
    {
        characterDataButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        characterDataButton.GetComponent<Button>().interactable = false;
        characterUpgradeButton.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 1.0f);
        characterDataButton.GetComponent<Button>().interactable = true;

        characterDataPanel.SetActive(true);
        characterUpgradePanel.SetActive(false);

    }
}
