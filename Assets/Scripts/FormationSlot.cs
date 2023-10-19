using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.SimpleLocalization.Scripts;
using DG.Tweening;

public class FormationSlot : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float nameTextSize = 35.0f;
    [SerializeField] private float nonNameTextSize = 27.0f;
    [SerializeField] private Vector2 hoverCollisionSize = new Vector2(100.0f, 200.0f);
    [SerializeField] private float resourcesPanelAnimationTime = 0.5f;

    [Header("References")]
    [SerializeField] private Image lockIcon; 
    [SerializeField] private TMP_Text slotName;
    [SerializeField] private GameObject HPStatus;
    [SerializeField] private GameObject MPStatus;
    [SerializeField] private Image HPFill;
    [SerializeField] private Image MPFill;
    [SerializeField] private TMP_Text HPText;
    [SerializeField] private TMP_Text MPText;
    [SerializeField] private CanvasGroup resourcesPanel;
    [SerializeField] private Button unlockSlotButton;
    [SerializeField] private FormationPanel formationPanel;
    [SerializeField] private GameObject effect;

    [Header("Debug")]
    [SerializeField] private GameObject battler;
    [SerializeField] private Vector3 objectPosition;
    [SerializeField] private bool isDisplaying;
    [SerializeField] private int slotIndex;
    [SerializeField] private bool isSlotFilled = false;

    public void Initialize(bool isLocked, int moneyCost, bool displayCost, int slotIndex)
    {
        lockIcon.color = isLocked ? Color.white : new Color(1, 1, 1, 0);
        this.slotName.fontSize = nonNameTextSize;
        this.slotName.text = string.Empty;
        HPStatus.SetActive(false);
        MPStatus.SetActive(false);
        unlockSlotButton.gameObject.SetActive(false);
        isSlotFilled = false;

        if (displayCost)
        {
            this.slotName.text = LocalizationManager.Localize("System.Cost") + ": " + moneyCost.ToString();
            unlockSlotButton.gameObject.SetActive(true);

            // お金が足りるか
            bool isEnoughMoney = ProgressManager.Instance.GetCurrentMoney() >= moneyCost;
            unlockSlotButton.interactable = isEnoughMoney;
            unlockSlotButton.GetComponentInChildren<TMP_Text>().alpha = isEnoughMoney ? 1.0f: 0.25f;
        }
        else if (isLocked)
        {
            this.slotName.text = LocalizationManager.Localize("System.Locked");
        }

        // System
        objectPosition = GetComponent<RectTransform>().position;
        this.slotIndex = slotIndex;
    }

    public void SetBattler(Character unit)
    {
        isSlotFilled = true;
        battler = Instantiate(unit.battler, transform);
        battler.transform.localPosition = Vector3.zero;

        Battler battlerScript = battler.GetComponent<Battler>();
        battlerScript.InitializeCharacterData(unit);
        battlerScript.SetupFormationPanelMode();

        // 名前とレベル
        slotName.text = LocalizationManager.Localize("Battle.Level") + battlerScript.currentLevel + " " + unit.localizedName;
        slotName.fontSize = nameTextSize;
        slotName.color = Color.white;
        
        // ステータス表示
        if (battlerScript.max_hp > 0)
        {
            HPStatus.SetActive(true);
            HPFill.fillAmount = battlerScript.current_hp / battlerScript.max_hp;
        }
        HPText.text = LocalizationManager.Localize("Battle.HP") + "：" + battlerScript.current_hp.ToString() + "/" + battlerScript.max_hp.ToString();

        if (unit.current_maxMp > 0)
        {
            MPStatus.SetActive(true);
            MPFill.fillAmount = battlerScript.current_mp / battlerScript.max_hp;
            MPText.text = LocalizationManager.Localize("Battle.MP") + "：" + battlerScript.current_mp.ToString() + "/" + battlerScript.max_mp.ToString();
        }

        // 表示しないものを非表示に
        battlerScript.HideBars();
    }

    public void ResetData(float delay)
    {
        if (delay <= 0.0)
        {
            ResetData();
            return;
        }
        StartCoroutine(ResetDataDelay(delay));
    }

    IEnumerator ResetDataDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        ResetData();
    }

    public void ResetData()
    {
        if (!ReferenceEquals(battler, null) && isSlotFilled)
        {
            Object.Destroy(battler.gameObject);
            battler = null;
            isSlotFilled = false;
        }

        lockIcon.color = Color.white;
        this.slotName.text = string.Empty;
        HPStatus.SetActive(false);
        MPStatus.SetActive(false);
        unlockSlotButton.gameObject.SetActive(false);
    }

    public void OnClickSlot()
    {
        if (!ReferenceEquals(battler, null) && isSlotFilled)
        {
            // 外す
            // UpdateFormationで全スロットに対してResetDataをかけるのでObject.BattlerをDestroyする必要がない
            formationPanel.UpdateFormation(slotIndex, -1, true, true);

            // エフェクトを表示
            var vfx = Instantiate(effect, transform);
            vfx.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, 80.0f, 0.0f);
            vfx.GetComponent<Animator>().speed = 1.0f;

            // SE再生
            AudioManager.Instance.PlaySFX("Flee", 0.5f);
        }
        else if (ProgressManager.Instance.GetUnlockedFormationCount() > slotIndex) // 解放済み
        {
            formationPanel.OpenFormationSelectionPanel(slotIndex);

            // SE再生
            AudioManager.Instance.PlaySFX("SystemSelect");
        }
    }

    private void Update()
    {
        // 購入できるスロット
        if (ProgressManager.Instance.GetUnlockedFormationCount() != slotIndex && !isDisplaying)
        {
            return;
        }

        // カーソル位置を取得
        Vector3 mousePosition = Input.mousePosition;
        // カーソル位置のz座標を10に
        mousePosition.z = 10;
        // カーソル位置をワールド座標に変換
        Vector3 target = Camera.main.ScreenToWorldPoint(mousePosition);

        // collision check  
        if (   mousePosition.x > objectPosition.x - (hoverCollisionSize.x * 0.5f)
            && mousePosition.x < objectPosition.x + (hoverCollisionSize.x * 0.5f)
            && mousePosition.y > objectPosition.y - (hoverCollisionSize.y * 0.5f)
            && mousePosition.y < objectPosition.y + (hoverCollisionSize.y * 0.5f))
        {
            OnHover();
        }
        else
        {
            OnUnHover();
        }
    }

    public void OnHover()
    {
        if (isDisplaying) return;
        if (formationPanel.isFormationSelecting) return;
        isDisplaying = true;
        resourcesPanel.DOFade(1.0f, resourcesPanelAnimationTime);
    }

    public void OnUnHover()
    {
        if (!isDisplaying) return;
        isDisplaying = false;
        resourcesPanel.DOFade(0.0f, resourcesPanelAnimationTime);
    }
}
