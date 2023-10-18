using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class CharacterBuildingPanel : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField, Range(0.0f, 1.0f)] private float animationTime = 0.5f;

    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CharacterDataPanel characterDataPanel;
    [SerializeField] private CharacterUpgradePanel characterUpgradePanel;
    [SerializeField] private GameObject characterDataButton;
    [SerializeField] private GameObject characterUpgradeButton;
    [SerializeField] private Image[] characterIconSlots;
    [SerializeField] private RectTransform pinkPanel;

    [Header("Debug")]
    [SerializeField] List<Character> characters;
    [SerializeField, HideInInspector] private float tabLocalPosY;
    [SerializeField] private int currentCheckingSlot = 0;

    private Color _darkenedTabColor = new Color(0.75f, 0.75f, 0.75f, 1.0f);
    const float _pinkPanelShakeTime = 0.1f;
    const float _pinkPanelShakeMagnitude = 2.5f;
    public int CurrentCheckingSlot { get { return currentCheckingSlot; } }

    private void Start()
    {
        tabLocalPosY = characterDataButton.GetComponent<RectTransform>().localPosition.y;
    }

    public void OpenCharacterBuildingPanel()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemOpen");

        canvasGroup.DOFade(1.0f, animationTime);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // 初期化
        currentCheckingSlot = 0;

        // キャラクター資料を取得して表示する
        characters = ProgressManager.Instance.GetAllCharacter();
        for (int i = 0; i < characters.Count; i++)
        {
            int index = characters[i].characterData.characterID;

            if (characters[i].is_corrupted || !characters[i].characterData.is_heroin) // ヒロインキャラは闇落ちした後のみ使用御覧できる
            {
                // キャラクターが存在しているならアイコンを白くする
                characterIconSlots[index].transform.Find("Character").GetComponent<Image>().color = Color.white;
            }
        }

        for (int i = 0; i < characterIconSlots.Length; i++)
        {
            Color tmp = (i == currentCheckingSlot) ? Color.white : new Color(1, 1, 1, 0);
            characterIconSlots[i].transform.Find("Selection Highlight").GetComponent<Image>().color = tmp;
        }
        SwitchToCharacterDataTab(false);
    }

    /// <summary>
    /// 
    /// </summary>
    public void QuitCharacterBuildingPanel()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemCancel");

        canvasGroup.DOFade(0.0f, animationTime);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // COPYしたものを削除
        this.characters.Clear();
        this.characters = null;
    }

    /// <summary>
    /// 資料タブ
    /// </summary>
    public void SwitchToCharacterDataTab(bool playSE = true)
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

        characterDataPanel.gameObject.SetActive(true);
        characterUpgradePanel.gameObject.SetActive(false);

        if (playSE)
        {
            // SE
            AudioManager.Instance.PlaySFX("SystemTab");
        }

        // 資料更新
        characterDataPanel.InitializeCharacterData(characters[currentCheckingSlot]);
    }

    /// <summary>
    /// 育成タブ
    /// </summary>
    public void SwitchToCharacterUpgradeTab(bool playSE = true)
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

        characterUpgradePanel.gameObject.SetActive(true);
        characterDataPanel.gameObject.SetActive(false);

        if (playSE)
        {
            // SE
            AudioManager.Instance.PlaySFX("SystemTab");
        }

        // 資料更新
        characterUpgradePanel.InitializeUpgradePanel(characters[currentCheckingSlot]);
        characterUpgradePanel.ResetAnimation();
    }

    /// <summary>
    /// SE再生
    /// </summary>
    public void OnSwitchPanel()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemTab");
    }

    /// <summary>
    /// キャラ変更
    /// </summary>
    public void ChangeCharacterSlot(int slot)
    {
        if (characters.Count <= slot) return;

        if (characters[slot].characterData.is_heroin && !characters[slot].is_corrupted) return; // まだ闇落ちされていないヒロインキャラは確認できない

        // SE 再生
        AudioManager.Instance.PlaySFX("SystemSelect");

        characterIconSlots[currentCheckingSlot].transform.Find("Selection Highlight").GetComponent<Image>().DOFade(0.0f, 0.1f);
        currentCheckingSlot = slot;
        characterIconSlots[currentCheckingSlot].transform.Find("Selection Highlight").GetComponent<Image>().DOFade(1.0f, 0.1f);
        characterDataPanel.InitializeCharacterData(characters[currentCheckingSlot]);
        characterUpgradePanel.InitializeUpgradePanel(characters[currentCheckingSlot]);
        characterUpgradePanel.ResetAnimation();
    }
}
