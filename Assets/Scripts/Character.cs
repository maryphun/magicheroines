using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public struct BattlerAnimation
{
    public Sprite idle, attack, magic, item, attacked, retire;
}

[System.Serializable]
public struct PlayerCharacter
{
    // system
    [Header("システム")]
    public int characterID;

    // graphic
    [Header("画像")]
    public Sprite icon;
    public Sprite sprite;
    public BattlerAnimation animations;
    public Color color;

    [Header("戦闘関連")]
    // battle related
    public string nameID;
    public int starting_level;
    public int base_hp;
    public int base_mp;
    public int base_attack;
    public int base_defense;
    public int base_speed;
    public int hp_growth;
    public int mp_growth;
    public int attack_growth;
    public int defense_growth;
    public int speed_growth;

    [Header("育成")]
    public int levelUp_base_cost;
    public int levelUp_cost_increment;

    [Header("ステータス")]
    public bool is_heroin;      //　ヒロイン
    public int max_dark_gauge; //闇落ちゲージ
    public int max_horny_gauge; //淫乱ゲージ
}

public class Character
{
    public PlayerCharacter characterData; // 基本資料
    public GameObject battler; // 戦闘像
    public int dark_gauge;
    public int horny_gauge;

    public string localizedName;
    public int current_level;
    public int current_maxHp;
    public int current_maxMp;
    public int current_hp;
    public int current_mp;
    public int current_attack;
    public int current_defense;
    public int current_speed;
}

/// <summary>
/// パーティー管理用
/// </summary>
public class FormationSlotData
{
    public Character characterData;
    public bool isFilled;

    public FormationSlotData(Character data, bool flag)
    {
        characterData = data;
        isFilled = flag;
    }
}
