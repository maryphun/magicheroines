using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using Assets.SimpleLocalization.Scripts;

public class AbilityPanel : MonoBehaviour
{
    [Header("Header")]
    [SerializeField, Range(0.0f, 1.0f)] private float animationTime = 0.25f;

    [Header("References")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Button abilityButton;
    [SerializeField] private TMP_Text text;
    [SerializeField] private RectTransform closeButton;
    [SerializeField] private CanvasGroup canvasGrp;
    [SerializeField] private CanvasGroup descriptionPanel;
    [SerializeField] private Canvas canvas;

    [Header("Debug")]
    [SerializeField] private bool isOpen = false;
    [SerializeField] private Action onCloseCallback;
    [SerializeField] private Tuple<Ability, Button>[] buttonList;
    [SerializeField] private bool isDescriptionShowing = false;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, -(rectTransform.sizeDelta.y+closeButton.sizeDelta.y), 0.0f);
        isOpen = false;
        isDescriptionShowing = false;
        abilityButton.gameObject.SetActive(false);

        canvasGrp.alpha = 0.0f;
        canvasGrp.interactable = false;
        canvasGrp.blocksRaycasts = false;
    }

    public void SetupAbilityData(List<Ability> abilities)
    {
        if (abilities.Count == 0) // 特殊技一つも持っていない場合
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 100.0f);
            text.gameObject.SetActive(true);
            text.rectTransform.anchoredPosition = Vector3.zero;
            
            // setup close button
            closeButton.anchoredPosition = new Vector3(closeButton.anchoredPosition.x, (rectTransform.sizeDelta.y * 0.5f) + (closeButton.sizeDelta.y * 0.5f), 0.0f);

            return;
        }
        text.gameObject.SetActive(false);

        const float spacePerAbility = 100.0f;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, abilities.Count * spacePerAbility);
        
        buttonList = new Tuple<Ability, Button>[abilities.Count];
        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i] = new Tuple<Ability, Button>(abilities[i], GameObject.Instantiate(abilityButton.gameObject, abilityButton.transform.parent).GetComponent<Button>());

            string abilityName = LocalizationManager.Localize(abilities[i].abilityNameID);
            buttonList[i].Item2.gameObject.name = abilityName;
            buttonList[i].Item2.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, 50.0f + (i * spacePerAbility), 0.0f);
            buttonList[i].Item2.transform.GetChild(0).GetComponent<TMP_Text>().text = abilityName;
            buttonList[i].Item2.gameObject.SetActive(true);
        }

        // setup close button
        closeButton.anchoredPosition = new Vector3(closeButton.anchoredPosition.x, (rectTransform.sizeDelta.y * 0.5f) + (closeButton.sizeDelta.y * 0.5f), 0.0f);
    }

    public void OpenPanel(Action callbackWhenClose)
    {
        if (isOpen) return;

        onCloseCallback = callbackWhenClose;
        rectTransform.DOAnchorPosY(0.0f, animationTime);
        isOpen = true;
        
        canvasGrp.DOFade(1.0f, animationTime);
        canvasGrp.interactable = true;
        canvasGrp.blocksRaycasts = true;
    }

    public void ClosePanel()
    {
        if (!isOpen) return;

        rectTransform.DOAnchorPosY(-(rectTransform.sizeDelta.y+closeButton.sizeDelta.y), animationTime);
        isOpen = false;

        if (!ReferenceEquals(buttonList, null))
        {
            // ボタンを削除
            foreach (var btn in buttonList)
            {
                Destroy(btn.Item2.gameObject, animationTime);
            }

            // For the garbage collector to truly find it
            buttonList = null;
        }
        
        canvasGrp.DOFade(0.0f, animationTime);
        canvasGrp.interactable = false;
        canvasGrp.blocksRaycasts = false;

        onCloseCallback?.Invoke();
    }

    private void Update()
    {
        if (!isOpen) return;

        if (Input.GetMouseButtonDown(1))
        {
            ClosePanel();
        }

        if (ReferenceEquals(buttonList, null)) return;

        // マウスクリックを検知
        Vector3 mousePosition = Input.mousePosition / canvas.scaleFactor;
        bool isMouseOnItem = false;
        foreach (var ability in buttonList)
        {
            RectTransform buttonRect = ability.Item2.GetComponent<RectTransform>();
            if (   mousePosition.x > (buttonRect.position.x - buttonRect.sizeDelta.x * 0.5f)
                && mousePosition.x < (buttonRect.position.x + buttonRect.sizeDelta.x * 0.5f)
                && mousePosition.y > (buttonRect.position.y - buttonRect.sizeDelta.y * 0.5f)
                && mousePosition.y < (buttonRect.position.y + buttonRect.sizeDelta.y * 0.5f))
            {
                // フラグ
                isMouseOnItem = true;

                // 資料表示
                ShowDescription(ability);

                // 残りのループを省略
                return;
            }
        }

        if (!isMouseOnItem && isDescriptionShowing)
        {
            HideDescription();
        }
    }

    private void ShowDescription(Tuple<Ability, Button> ability)
    {
        // レファレンス所得
        RectTransform buttonRect = ability.Item2.GetComponent<RectTransform>();

        //フラグ
        isDescriptionShowing = true;

        // UI
        //itemDescription.GetComponent<CanvasGroup>().DOFade(1.0f, 0.1f);
        //itemDescription.position = new Vector2(buttonRect.anchoredPosition.x, buttonRect.anchoredPosition.y + (buttonRect.sizeDelta.y * 0.5f) + 10.0f);

        //// データ読み込み
        //itemDescription_Name.text = LocalizationManager.Localize(item.Item2.itemNameID);
        //string effectTargetText = string.Empty;
        //switch (item.Item2.itemType)
        //{
        //    case CastType.SelfCast:
        //        effectTargetText = LocalizationManager.Localize("System.EffectSelf");
        //        break;
        //    case CastType.Teammate:
        //        effectTargetText = LocalizationManager.Localize("System.EffectTeam");
        //        break;
        //    case CastType.Enemy:
        //        effectTargetText = LocalizationManager.Localize("System.EffectEnemy");
        //        break;
        //    default:
        //        break;
        //}
        //itemDescription_Target.text = LocalizationManager.Localize("System.EffectTarget") + effectTargetText;
        //itemDescription_Effect.text = item.Item2.effectText + "\n\n" + LocalizationManager.Localize(item.Item2.descriptionID);

        //// 強制更新
        //itemDescription_Name.ForceMeshUpdate();
        //itemDescription_Target.ForceMeshUpdate();
        //itemDescription_Effect.ForceMeshUpdate();

        //// Resize UI
        //itemDescription.sizeDelta = new Vector2(itemDescription.sizeDelta.x,
        //    (itemDescription_Name.rectTransform.rect.height + itemDescription_Target.rectTransform.rect.height + (itemDescription_Effect.GetRenderedValues(false).y) + itemDescription_Effect.fontSize));
    }

    private void HideDescription()
    {
        isDescriptionShowing = false;
        descriptionPanel.DOFade(0.0f, 0.1f);
    }

    public float GetAnimTime()
    {
        return animationTime;
    }
}
