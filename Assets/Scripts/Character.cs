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
    public Sprite tachie;
    public BattlerAnimation animations;

    [Header("戦闘関連")]
    // battle related
    public string name;
    public int max_hp;
    public int max_mp;
    public int attack;
    public int defense;
    public int hp_growth;
    public int mp_growth;
    public int attack_growth;
    public int defense_growth;

    [Header("ステータス")]
    public bool is_heroin;      //　ヒロイン
    public int max_dark_gauge; //闇落ちゲージ
    public int max_horny_gauge; //淫乱ゲージ
}

public class Character
{
    public PlayerCharacter characterData; // 基本資料
    public GameObject battler; // 戦闘像
    public int level;
    public int dark_gauge;
    public int horny_gauge;
}
