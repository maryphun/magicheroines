using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using DG.Tweening;
using Assets.SimpleLocalization.Scripts;

public class CharacterDataPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image characterSprite;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private TMP_Text levelValue;
    [SerializeField] private TMP_Text hpValue;
    [SerializeField] private TMP_Text mpValue;
    [SerializeField] private TMP_Text attackValue;
    [SerializeField] private TMP_Text defenseValue;
    [SerializeField] private TMP_Text speedValue;
    [SerializeField] private Button equipmentButton;
    [SerializeField] private Image equipment;
    [SerializeField] private TMP_Text skillList;
    [SerializeField] private Button[] skillButtonList = new Button[4];
    [SerializeField] private Image[] skillImageList = new Image[4];
    [SerializeField] private bool[] skillAvailable = new bool[4];
    [SerializeField] private CharacterBuildingPanel mainPanel;
    [SerializeField] private EquipmentPanel equipmentPanel;

    // pop up
    [SerializeField] private CanvasGroup popup;
    [SerializeField] private TMP_Text abilityName;
    [SerializeField] private TMP_Text abilityType;
    [SerializeField] private TMP_Text abilityCastType;
    [SerializeField] private TMP_Text abilityDescription;

    [Header("Setting")]
    [SerializeField] private Sprite lockIcon;

    /// <summary>
    /// 表示するキャラクターを更新
    /// </summary>
    public void InitializeCharacterData(Character character)
    {
        characterSprite.sprite = character.characterData.sprite;
        characterName.text = character.localizedName;
        levelValue.text = character.current_level.ToString();
        hpValue.text = character.current_hp.ToString();
        mpValue.text = character.current_mp.ToString();
        attackValue.text = character.current_attack.ToString();
        defenseValue.text = character.current_defense.ToString();
        speedValue.text = character.current_speed.ToString();

        // setup ability
        List<Ability> abilities = new List<Ability>(character.characterData.abilities);

        // 順番並べ
        if (abilities.Count > 0)
        {
            abilities.Sort((x, y) => x.requiredLevel.CompareTo(y.requiredLevel));

            for (int i = 0; i < abilities.Count; i++)
            {
                skillAvailable[i] = (character.current_level >= abilities[i].requiredLevel
                                    && character.horny_gauge >= abilities[i].requiredHornyness);
            }
        }
        else
        {
            skillAvailable = skillAvailable.Select(_ => false).ToArray();
        }

        // ボタンを初期化
        for (int i = 0; i < skillButtonList.Length; i++)
        {
            if (skillAvailable[i])
            {
                skillButtonList[i].image.color = new Color(1, 1, 1, 1);
                skillButtonList[i].interactable = true;
                skillButtonList[i].onClick.RemoveAllListeners();
                Ability abilityInfo = abilities[i];
                skillButtonList[i].onClick.AddListener(delegate { OnClickAbility(abilityInfo); });
                skillImageList[i].color = new Color(1, 1, 1, 1);
                skillImageList[i].sprite = abilities[i].icon;
            }
            else
            {
                skillButtonList[i].interactable = false;

                if (abilities.Count > i)
                {
                    // 技が存在しているが開放条件まだ満たしていない
                    skillButtonList[i].image.color = new Color(1, 1, 1, 1);
                    skillImageList[i].color = new Color(1, 1, 1, 1);

                    skillImageList[i].sprite = lockIcon;
                }
                else
                {
                    // そもそも技存在していない
                    skillButtonList[i].image.color = new Color(1, 1, 1, 0);
                    skillImageList[i].color = new Color(1, 1, 1, 0);
                }
            }
        }

        // 技説明pop upを初期化
        popup.alpha = 0.0f;
        popup.interactable = false;
        popup.blocksRaycasts = false;

        UpdateEquipmentIcon();
    }

    public void OnClickAbility(Ability ability)
    {
        // 技説明pop upを初期化
        popup.DOFade(1.0f, 0.2f);
        popup.interactable = true;
        popup.blocksRaycasts = true;

        abilityName.text = LocalizationManager.Localize(ability.abilityNameID);
        abilityType.text = LocalizationManager.Localize("System.AbilityType") + "：" + AbilityTypeToString(ability.abilityType);
        abilityCastType.text = LocalizationManager.Localize("System.EffectTarget") + CastTypeToString(ability.castType);
        abilityDescription.text = LocalizationManager.Localize(ability.descriptionID);

        // SE 再生
        AudioManager.Instance.PlaySFX("SystemOpen");
    }

    public void CloseAbilityInfoPopup()
    {
        popup.DOFade(0.0f, 0.2f).OnComplete(() => {
            popup.interactable = false;
            popup.blocksRaycasts = false;
        });

        // SE 再生
        AudioManager.Instance.PlaySFX("SystemCancel");
    }

    private string CastTypeToString(CastType castType)
    {
        switch (castType)
        {
            case CastType.SelfCast:
                return LocalizationManager.Localize("System.EffectSelf");
            case CastType.Teammate:
                return LocalizationManager.Localize("System.EffectTeam");
            case CastType.Enemy:
                return LocalizationManager.Localize("System.EffectEnemy");
            default:
                return string.Empty;
        }
    }

    private string AbilityTypeToString(AbilityType abilityType)
    {
        switch (abilityType)
        {
            case AbilityType.Attack:
                return LocalizationManager.Localize("System.AbilityAttack");
                break;
            case AbilityType.Buff:
                return LocalizationManager.Localize("System.AbilityBuff");
                break;
            case AbilityType.Heal:
                return LocalizationManager.Localize("System.AbilityHeal");
                break;
            case AbilityType.Special:
                return LocalizationManager.Localize("System.AbilitySpecial");
            default:
                return string.Empty;
        }
    }

    public void UpdateEquipmentIcon()
    {
        var EquipmentData = ProgressManager.Instance.GetEquipmentData();
        bool isEquiped = EquipmentData.Any(x => x.equipingCharacterID == mainPanel.CurrentCheckingSlot);
        if (isEquiped)
        {
            var data = ProgressManager.Instance.GetEquipmentData().FirstOrDefault(x => x.equipingCharacterID == mainPanel.CurrentCheckingSlot).data;
            equipment.sprite = data.Icon;
            equipment.color = Color.white;
            equipmentButton.image.color = equipmentPanel.GetColorByEquipmentType(data.equipmentType);
        }
        else
        {
            equipment.color = new Color(1, 1, 1, 0);
            equipmentButton.image.color = Color.white;
        }
    }
}
