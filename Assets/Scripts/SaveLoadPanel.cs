using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Assets.SimpleLocalization.Scripts;

public class SaveLoadPanel : MonoBehaviour
{
    // スロット数を設定
    const int totalSlotNum = 10;

    [Header("Setting")]
    [SerializeField, Range(0.0f, 1.0f)] private float animationTime = 0.5f;

    [Header("References")]
    [SerializeField] private Button[] saveSlot = new Button[totalSlotNum];
    [SerializeField] private string[] slotComment = new string[totalSlotNum];
    [SerializeField] private Button saveTab;
    [SerializeField] private Button loadTab;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup comfirmationPopUp;
    [SerializeField] private GameObject saveComfirm;
    [SerializeField] private GameObject loadComfirm;
    [SerializeField] private TMP_Text saveComfirmText;
    [SerializeField] private TMP_Text loadComfirmText;
    [SerializeField] private TMP_InputField saveCommentInput;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private HomeSceneUI homeSceneUI;

    [Header("Debug")]
    [SerializeField] private bool isSaving = false;
    [SerializeField] private int currentSelectingSlotIndex = -1;

    private Color _darkenedTabColor = new Color(0.75f, 0.75f, 0.75f, 1.0f);

    private void Start()
    {
        for (int i = 0; i < totalSlotNum; i ++)
        {
            int slotIndex = i;
            saveSlot[i].onClick.AddListener(delegate { OnClickSlot(slotIndex); });
            slotComment[i] = string.Empty;
        }
    }

    public void OpenSaveLoadPanel()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemOpen");

        // デフォルトタブ
        SwapToSaveTab(false);

        canvasGroup.DOFade(1.0f, animationTime);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        comfirmationPopUp.alpha = 0.0f;
        comfirmationPopUp.interactable = false;
        comfirmationPopUp.blocksRaycasts = false;

        // データ初期化
        currentSelectingSlotIndex = -1;
        UpdateSlotInfo();
    }

    public void CloseSaveLoadPanel()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemCancel");

        canvasGroup.DOFade(0.0f, animationTime);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void SwapToSaveTab(bool playSE)
    {
        saveTab.interactable = false;
        loadTab.interactable = true;

        saveTab.image.color = Color.white;
        loadTab.image.color = _darkenedTabColor;

        saveTab.GetComponent<RectTransform>().anchoredPosition = new Vector3(saveTab.GetComponent<RectTransform>().anchoredPosition.x, -33.0f, 0.0f);
        loadTab.GetComponent<RectTransform>().anchoredPosition = new Vector3(saveTab.GetComponent<RectTransform>().anchoredPosition.x, -50.0f, 0.0f);

        if (playSE)
        {
            // SE
            AudioManager.Instance.PlaySFX("SystemTab");
        }

        // reset scroll rect
        scrollRect.verticalNormalizedPosition = 1.0f;

        // flag
        isSaving = true;
    }

    public void SwapToLoadTab(bool playSE)
    {
        saveTab.interactable = true;
        loadTab.interactable = false;

        saveTab.image.color = _darkenedTabColor;
        loadTab.image.color = Color.white;

        saveTab.GetComponent<RectTransform>().anchoredPosition = new Vector3(saveTab.GetComponent<RectTransform>().anchoredPosition.x, -50.0f, 0.0f);
        loadTab.GetComponent<RectTransform>().anchoredPosition = new Vector3(saveTab.GetComponent<RectTransform>().anchoredPosition.x, -33.0f, 0.0f);

        if (playSE)
        {
            // SE
            AudioManager.Instance.PlaySFX("SystemTab");
        }

        // reset scroll rect
        scrollRect.verticalNormalizedPosition = 1.0f;

        // flag
        isSaving = false;
    }

    public void OnClickSlot(int slotIndex)
    {
        currentSelectingSlotIndex = slotIndex;

        if (isSaving)
        {
            saveComfirm.SetActive(true);
            loadComfirm.SetActive(false);

            if (SaveDataManager.IsDataExist(slotIndex))
            {
                saveComfirmText.text = string.Format(LocalizationManager.Localize("System.ConfirmReplace"), slotIndex + 1);
                saveCommentInput.text = slotComment[slotIndex];
            }
            else
            {
                saveComfirmText.text = string.Format(LocalizationManager.Localize("System.ConfirmSave"), slotIndex + 1);
            }
        }
        else
        {
            saveComfirm.SetActive(false);
            loadComfirm.SetActive(true);
            loadComfirmText.text = string.Format(LocalizationManager.Localize("System.ConfirmLoad"), slotIndex+1);
        }

        comfirmationPopUp.DOFade(1.0f, animationTime);
        comfirmationPopUp.interactable = true;
        comfirmationPopUp.blocksRaycasts = true;
    }

    public void ClosePopUp()
    {
        comfirmationPopUp.DOFade(0.0f, animationTime).OnComplete(() => {
            comfirmationPopUp.interactable = false;
            comfirmationPopUp.blocksRaycasts = false;

            saveComfirm.SetActive(false);
            loadComfirm.SetActive(false);
        });
    }

    public void ComfirmPopUp()
    {
        if (isSaving)
        {
            SaveDataManager.SaveJsonData(currentSelectingSlotIndex, saveCommentInput.text);
            PlayerPrefs.SetInt("LastSavedSlot", currentSelectingSlotIndex);
            PlayerPrefs.Save();

            UpdateSlotInfo();
            ClosePopUp();
        }
        else
        {
            SaveDataManager.LoadJsonData(currentSelectingSlotIndex);
            StartCoroutine(homeSceneUI.SceneTransition("Home", 1.0f));
        }
    }

    private void UpdateSlotInfo()
    {
        for (int i = 0; i < totalSlotNum; i++)
        {
            string slotName = SaveDataManager.GetDataInfo(i, out string comment);
            if (PlayerPrefs.GetInt("LastSavedSlot", -1) == i) slotName = "<color=red>" + slotName;
            saveSlot[i].GetComponentInChildren<TMP_Text>().text = slotName;
            slotComment[i] = comment;
        }
    }
}
