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
    [Header("ƒVƒXƒeƒ€")]
    public int characterID;

    // graphic
    [Header("‰æ‘œ")]
    public Sprite icon;
    public BattlerAnimation animations;

    [Header("Ž‘—¿")]
    // battle related
    public string name;
    public int max_hp;
    public int max_mp;
    public int attackDamage;
}

public class PlayerCharacters : SingletonMonoBehaviour<PlayerCharacters>
{

}
