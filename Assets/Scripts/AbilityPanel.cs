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
    [SerializeField] private Battle battleManager;
    [SerializeField] private TMP_Text description_Name;   //<　技名
    [SerializeField] private TMP_Text description_Target; //<　効果対象
    [SerializeField] private TMP_Text description_Type;   //<　機能
    [SerializeField] private TMP_Text description_Info;   //<　技説明

    [Header("Debug")]
    [SerializeField] private bool isOpen = false;
    [SerializeField] private bool isHiding = false;
    [SerializeField] private Action onCloseCallback;
    [SerializeField] private Tuple<Ability, Button>[] buttonList;
    [SerializeField] private bool isDescriptionShowing = false;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, -(rectTransform.sizeDelta.y+closeButton.sizeDelta.y), 0.0f);
        isOpen = false;
        isHiding = false;
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
            Ability ability = buttonList[i].Item1;
            buttonList[i].Item2.onClick.AddListener(delegate { OnClickAbility(ability); });

            // 使用できるかをチェック
            buttonList[i].Item2.interactable = battleManager.GetCurrentBattler().current_mp >= buttonList[i].Item1.consumeSP;
        }

        // setup close button
        closeButton.anchoredPosition = new Vector3(closeButton.anchoredPosition.x, (rectTransform.sizeDelta.y * 0.5f) + (closeButton.sizeDelta.y * 0.5f), 0.0f);
    }

    public void OnClickAbility(Ability ability)
    {
        HideDescription();

        switch (ability.castType)
        {
            case CastType.SelfCast:
                AbilityExecute.Instance.Invoke(ability.functionName, 0);
                battleManager.GetCurrentBattler().DeductSP(ability.consumeSP);
                ClosePanel();
                break;
            case CastType.Teammate:
                StartCoroutine(SelectingTarget(ability, true, false, true));
                HidePanel();
                break;
            case CastType.Enemy:
                StartCoroutine(SelectingTarget(ability, false, true, true));
                HidePanel();
                break;
            default:
                break;
        }
    }

    public void OpenPanel(Action callbackWhenClose)
    {
        if (isOpen) return;

        onCloseCallback = callbackWhenClose;
        rectTransform.DOAnchorPosY(0.0f, animationTime);
        DOTween.Sequence().AppendInterval(animationTime * 0.75f).AppendCallback(() => { isOpen = true; }); // 完全に開いてから次のを操作受け付ける
        isHiding = false;

        canvasGrp.DOFade(1.0f, animationTime);
        canvasGrp.interactable = true;
        canvasGrp.blocksRaycasts = true;
    }

    public void ClosePanel()
    {
        if (!isOpen) return;

        rectTransform.DOAnchorPosY(-(rectTransform.sizeDelta.y+closeButton.sizeDelta.y), animationTime);
        isOpen = false;
        isHiding = false;

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


    private void HidePanel()
    {
        if (!isOpen) return;
        if (isHiding) return;

        rectTransform.DOAnchorPosY(-(rectTransform.sizeDelta.y + closeButton.sizeDelta.y), animationTime);
        isOpen = false;
        isHiding = true;

        canvasGrp.DOFade(0.0f, animationTime);
        canvasGrp.interactable = false;
        canvasGrp.blocksRaycasts = false;
    }

    private void UnhidePanel()
    {
        if (!isOpen) return;
        if (!isHiding) return;

        rectTransform.DOAnchorPosY(-(rectTransform.sizeDelta.y + closeButton.sizeDelta.y), animationTime);

        canvasGrp.DOFade(1.0f, animationTime);
        canvasGrp.interactable = true;
        canvasGrp.blocksRaycasts = true;
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
        RectTransform descriptionPanelRect = descriptionPanel.GetComponent<RectTransform>();

        //フラグ
        isDescriptionShowing = true;

        // UI
        descriptionPanel.GetComponent<CanvasGroup>().DOFade(1.0f, 0.1f);
        descriptionPanelRect.position = new Vector2(buttonRect.position.x, buttonRect.position.y + (buttonRect.sizeDelta.y * 0.5f) + 10.0f);

        // データ読み込み
        description_Name.text = LocalizationManager.Localize(ability.Item1.abilityNameID);
        string effectTargetText = string.Empty;
        switch (ability.Item1.castType)
        {
            case CastType.SelfCast:
                effectTargetText = LocalizationManager.Localize("System.EffectSelf");
                break;
            case CastType.Teammate:
                effectTargetText = LocalizationManager.Localize("System.EffectTeam");
                break;
            case CastType.Enemy:
                effectTargetText = LocalizationManager.Localize("System.EffectEnemy");
                break;
            default:
                break;
        }
        description_Target.text = LocalizationManager.Localize("System.EffectTarget") + effectTargetText;

        string abilityTypeText = string.Empty;
        switch (ability.Item1.abilityType)
        {
            case AbilityType.Attack:
                abilityTypeText = LocalizationManager.Localize("System.AbilityAttack");
                break;
            case AbilityType.Buff:
                abilityTypeText = LocalizationManager.Localize("System.AbilityBuff");
                break;
            case AbilityType.Heal:
                abilityTypeText = LocalizationManager.Localize("System.AbilityHeal");
                break;
            case AbilityType.Special:
                abilityTypeText = LocalizationManager.Localize("System.AbilitySpecial");
                break;
            default:
                break;
        }
        description_Type.text = LocalizationManager.Localize("System.AbilityType") + "：" + abilityTypeText;

        description_Info.text = LocalizationManager.Localize(ability.Item1.descriptionID);

        // 強制更新
        description_Name.ForceMeshUpdate();
        description_Target.ForceMeshUpdate();
        description_Type.ForceMeshUpdate();
        description_Info.ForceMeshUpdate();

        // Resize UI
        descriptionPanelRect.sizeDelta = new Vector2(descriptionPanelRect.sizeDelta.x,
            (description_Name.rectTransform.rect.height + 
             description_Target.rectTransform.rect.height + 
             description_Type.rectTransform.rect.height + 
            (description_Info.GetRenderedValues(false).y) + description_Info.fontSize));
    }

    private void HideDescription()
    {
        isDescriptionShowing = false;
        descriptionPanel.DOFade(0.0f, 0.1f);
    }

    private IEnumerator SelectingTarget(Ability ability, bool isTeammateAllowed, bool isEnemyAllowed, bool isAliveOnly)
    {
        // SE再生
        AudioManager.Instance.PlaySFX("SystemActionPanel");

        // カーソルを変更
        var texture = Resources.Load<Texture2D>("Icon/focus");
        Cursor.SetCursor(texture, new Vector2(texture.width * 0.5f, texture.height * 0.5f), CursorMode.Auto);
        bool isSelectingTarget = false;
        bool isFinished = false;

        // Tips
        Inventory.Instance.ShowTipsText();

        do
        {
            // arrow that follow the mouse
            Vector3 mousePosition = Input.mousePosition / canvas.scaleFactor;
            var targetBattler = battleManager.GetBattlerByPosition(mousePosition, isTeammateAllowed, isEnemyAllowed, isAliveOnly);

            if (!ReferenceEquals(targetBattler, null))
            {
                isSelectingTarget = true;
                battleManager.PointTargetWithArrow(targetBattler, 0.25f);
                if (Input.GetMouseButtonDown(0))
                {
                    isFinished = true;

                    // 選択した敵
                    AbilityExecute.Instance.SetTargetBattler(targetBattler);

                    // 技を使用
                    AbilityExecute.Instance.Invoke(ability.functionName, 0);
                    battleManager.GetCurrentBattler().DeductSP(ability.consumeSP);
                    ClosePanel();

                    // カーソルを戻す
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    battleManager.UnPointArrow(0.25f);
                }
            }
            else if (isSelectingTarget)
            {
                isSelectingTarget = false;
                battleManager.UnPointArrow(0.25f);
            }

            if (Input.GetMouseButtonDown(1))
            {
                // SE再生
                AudioManager.Instance.PlaySFX("SystemActionCancel");

                // キャンセル
                isFinished = true;
                battleManager.UnPointArrow(0.25f);
                UnhidePanel();

                // カーソルを戻す
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }

            yield return null;
        } while (!isFinished);

        // Hide Tips
        Inventory.Instance.HideTipsText();
    }

    public float GetAnimTime()
    {
        return animationTime;
    }
}
