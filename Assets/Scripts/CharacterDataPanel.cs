using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] private Image equipment;
    [SerializeField] private TMP_Text skillList;

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
    }
}
