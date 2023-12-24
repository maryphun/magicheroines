using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.SimpleLocalization.Scripts;
using TMPro;

[System.Serializable]
public struct BuffData
{
    public Sprite icon;
    public string name;
    public Action<Battler, int> start;
    public Action<Battler, int> update;
    public Action<Battler, int> end;
    public bool isBad;

    // êÌì¨ÉçÉOèoóÕóp
    public string battleLogStart;
    public string battleLogUpdate;
    public string battleLogEnd;
}

[System.Serializable]
public enum BuffType
{
    stun,
    hurt,
    heal,
    shield_up,
    shield_down,
    attack_up,
    attack_down,
    speed_up,
    speed_down,
    continuous_action,
}

[System.Serializable]
public class Buff
{
    public BuffType type;
    public BuffData data;

    public Battler target;
    public int remainingTurn;
    public int value;

    public GameObject graphic;
    public TMP_Text text;
}

public static class BuffManager
{
    public static Dictionary<BuffType, BuffData> BuffList = new Dictionary<BuffType, BuffData>();
    public static Battler currentBattler;
    public static GameObject floatingTextOrigin;
    public static bool isInitialized = false;

    public static void Init()
    {
        if (isInitialized) return;

        // stun
        {
            BuffData data = new BuffData();
            data.icon = Resources.Load<Sprite>("Icon/stunned");
            data.name = LocalizationManager.Localize("Buff.Stun");
            data.start = StunStart;
            data.end = StunEnd;
            data.update = StunUpdate;
            data.isBad = true;
            data.battleLogStart = LocalizationManager.Localize("BattleLog.StunStart");
            data.battleLogUpdate = LocalizationManager.Localize("BattleLog.StunUpdate");
            data.battleLogEnd = string.Empty;
            BuffList.Add(BuffType.stun, data);
        }
        // hurt
        {
            BuffData data = new BuffData();
            data.icon = Resources.Load<Sprite>("Icon/hurt");
            data.name = LocalizationManager.Localize("Buff.Hurt");
            data.start = HurtStart;
            data.end = HurtEnd;
            data.update = HurtUpdate;
            data.isBad = true;
            data.battleLogStart = LocalizationManager.Localize("BattleLog.HurtStart");
            data.battleLogUpdate = LocalizationManager.Localize("BattleLog.HurtUpdate").Replace("{1}", CustomColor.AddColor("{1}", CustomColor.damage()));
            data.battleLogEnd = LocalizationManager.Localize("BattleLog.HurtEnd");
            BuffList.Add(BuffType.hurt, data);
        }
        // heal
        {
            BuffData data = new BuffData();
            data.icon = Resources.Load<Sprite>("Icon/heal");
            data.name = LocalizationManager.Localize("Buff.Heal");
            data.start = HealStart;
            data.end = HealEnd;
            data.update = HealUpdate;
            data.isBad = false;
            data.battleLogStart = LocalizationManager.Localize("BattleLog.HealStart");
            data.battleLogUpdate = LocalizationManager.Localize("BattleLog.HealUpdate").Replace("{1}", CustomColor.AddColor("{1}", CustomColor.heal()));
            data.battleLogEnd = string.Empty;
            BuffList.Add(BuffType.heal, data);
        }
        // shield up
        {
            BuffData data = new BuffData();
            data.icon = Resources.Load<Sprite>("Icon/shield_up");
            data.name = LocalizationManager.Localize("Buff.Shield_up");
            data.start = ShieldUpStart;
            data.end = ShieldUpEnd;
            data.update = ShieldUpUpdate;
            data.isBad = false;
            data.battleLogStart = LocalizationManager.Localize("BattleLog.ShieldUpStart");
            data.battleLogUpdate = string.Empty;
            data.battleLogEnd = string.Empty;
            BuffList.Add(BuffType.shield_up, data);
        }
        // shield down
        {
            BuffData data = new BuffData();
            data.icon = Resources.Load<Sprite>("Icon/shield_down");
            data.name = LocalizationManager.Localize("Buff.Shield_down");
            data.start = ShieldDownStart;
            data.end = ShieldDownEnd;
            data.update = ShieldDownUpdate;
            data.isBad = true;
            data.battleLogStart = LocalizationManager.Localize("BattleLog.ShieldDownStart");
            data.battleLogUpdate = string.Empty;
            data.battleLogEnd = string.Empty;
            BuffList.Add(BuffType.shield_down, data);
        }
        // attack up
        {
            BuffData data = new BuffData();
            data.icon = Resources.Load<Sprite>("Icon/attack_up");
            data.name = LocalizationManager.Localize("Buff.Attack_up");
            data.start = AttackUpStart;
            data.end = AttackUpEnd;
            data.update = AttackUpUpdate;
            data.isBad = false;
            data.battleLogStart = LocalizationManager.Localize("BattleLog.AttackUpStart").Replace("{1}", CustomColor.AddColor("{1}", CustomColor.buffvalue()));
            data.battleLogUpdate = string.Empty;
            data.battleLogEnd = string.Empty;
            BuffList.Add(BuffType.attack_up, data);
        }
        // attack down
        {
            BuffData data = new BuffData();
            data.icon = Resources.Load<Sprite>("Icon/attack_down");
            data.name = LocalizationManager.Localize("Buff.Attack_down");
            data.start = AttackDownStart;
            data.end = AttackDownEnd;
            data.update = AttackDownUpdate;
            data.isBad = true;
            data.battleLogStart = LocalizationManager.Localize("BattleLog.AttackDownStart").Replace("{1}", CustomColor.AddColor("{1}", CustomColor.buffvalue()));
            data.battleLogUpdate = string.Empty;
            data.battleLogEnd = string.Empty;
            BuffList.Add(BuffType.attack_down, data);
        }
        // speed up
        {
            BuffData data = new BuffData();
            data.icon = Resources.Load<Sprite>("Icon/speed_up");
            data.name = LocalizationManager.Localize("Buff.Speed_up");
            data.start = SpeedUpStart;
            data.end = SpeedUpEnd;
            data.update = SpeedUpUpdate;
            data.isBad = false;
            data.battleLogStart = LocalizationManager.Localize("BattleLog.SpeedUpStart").Replace("{1}", CustomColor.AddColor("{1}", CustomColor.buffvalue()));
            data.battleLogUpdate = string.Empty;
            data.battleLogEnd = string.Empty;
            BuffList.Add(BuffType.speed_up, data);
        }
        // speed down
        {
            BuffData data = new BuffData();
            data.icon = Resources.Load<Sprite>("Icon/speed_down");
            data.name = LocalizationManager.Localize("Buff.Speed_down");
            data.start = SpeedDownStart;
            data.end = SpeedDownEnd;
            data.update = SpeedDownUpdate;
            data.isBad = true;
            data.battleLogStart = LocalizationManager.Localize("BattleLog.SpeedDownStart").Replace("{1}", CustomColor.AddColor("{1}", CustomColor.buffvalue()));
            data.battleLogUpdate = string.Empty;
            data.battleLogEnd = string.Empty;
            BuffList.Add(BuffType.speed_down, data);
        }
        // continuous action
        {
            BuffData data = new BuffData();
            data.icon = Resources.Load<Sprite>("Icon/continuous_action");
            data.name = LocalizationManager.Localize("Buff.ContinuousAction");
            data.start = ContinuousActionStart;
            data.end = ContinuousActionEnd;
            data.update = ContinuousActionUpdate;
            data.isBad = true;
            data.battleLogStart = string.Empty;
            data.battleLogUpdate = string.Empty;
            data.battleLogEnd = string.Empty;
            BuffList.Add(BuffType.continuous_action, data);
        }

        floatingTextOrigin = Resources.Load<GameObject>("Prefabs/FloatingNumber");
        isInitialized = true;
    }

    public static void CurrentTurn(Battler battler)
    {
        currentBattler = battler;
    }

    public static void CreateFloatingText(string text, Color color, float size, Battler target)
    {
        // create floating text
        var floatingText = UnityEngine.GameObject.Instantiate(floatingTextOrigin, target.transform);
        floatingText.GetComponent<FloatingText>().Init(2.0f, target.GetMiddleGlobalPosition() + new Vector2(0.0f, target.GetCharacterSize().y * 0.35f), new Vector2(0.0f, 100.0f), text, size, color);
    }

    public static void StunStart(Battler target, int value) { }
    public static void StunUpdate(Battler target, int value) { }
    public static void StunEnd(Battler target, int value) { }

    public static void HurtStart(Battler target, int value) { }
    public static void HurtUpdate(Battler target, int value) 
    { 
        target.DeductHP(value);
        CreateFloatingText(value.ToString(), new Color(1f, 0.75f, 0.33f), 32, target);
        AudioManager.Instance.PlaySFX("Damage");
    }
    public static void HurtEnd(Battler target, int value) { }

    public static void HealStart(Battler target, int value) { }
    public static void HealUpdate(Battler target, int value) 
    {  
        target.Heal(value);
        CreateFloatingText(value.ToString(), new Color(0.33f, 1f, 0.5f), 32, target);
        AudioManager.Instance.PlaySFX("Heal2");
    }
    public static void HealEnd(Battler target, int value) { }

    public static void ShieldUpStart(Battler target, int value) {  target.defense += value; }
    public static void ShieldUpUpdate(Battler target, int value) { }
    public static void ShieldUpEnd(Battler target, int value) { target.defense -= value; }

    public static void ShieldDownStart(Battler target, int value) {  target.defense -= value;  }
    public static void ShieldDownUpdate(Battler target, int value) { }
    public static void ShieldDownEnd(Battler target, int value) {  target.defense += value;  }

    public static void AttackUpStart(Battler target, int value) {  target.attack += value; }
    public static void AttackUpUpdate(Battler target, int value) { }
    public static void AttackUpEnd(Battler target, int value) { target.attack -= value; }

    public static void AttackDownStart(Battler target, int value) { target.attack -= value; }
    public static void AttackDownUpdate(Battler target, int value) { }
    public static void AttackDownEnd(Battler target, int value) { target.attack += value; }

    public static void SpeedUpStart(Battler target, int value) { target.speed += value; }
    public static void SpeedUpUpdate(Battler target, int value) { }
    public static void SpeedUpEnd(Battler target, int value) { target.speed -= value; }

    public static void SpeedDownStart(Battler target, int value) { target.speed -= value; }
    public static void SpeedDownUpdate(Battler target, int value) { }
    public static void SpeedDownEnd(Battler target, int value) { target.speed += value; }

    public static void ContinuousActionStart(Battler target, int value) { }
    public static void ContinuousActionUpdate(Battler target, int value) { }
    public static void ContinuousActionEnd(Battler target, int value) 
    {
        AbilityOverdrive overdrive;
        if (target.TryGetComponent<AbilityOverdrive>(out overdrive))
        {
            overdrive.Trigger();
        }
    }
}
