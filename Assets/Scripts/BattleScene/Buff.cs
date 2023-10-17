using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.SimpleLocalization.Scripts;

[System.Serializable]
public struct BuffData
{
    public Sprite icon;
    public string name;
    public Action start;
    public Action end;
    public bool isBad;
}

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
}

public static class BuffManager
{
    public static Dictionary<BuffType, BuffData> BuffList = new Dictionary<BuffType, BuffData>();
    
    public static void Init()
    {
        // stun
        {
            BuffData data = new BuffData();
            data.icon = Resources.Load<Sprite>("Icon/stunned");
            data.name = LocalizationManager.Localize("Buff.Stun");
            data.start = StunStart;
            data.start = StunEnd;
            data.isBad = true;
            BuffList.Add(BuffType.stun, data);
        }
        // hurt
        {
            BuffData data = new BuffData();
            data.icon = Resources.Load<Sprite>("Icon/hurt");
            data.name = LocalizationManager.Localize("Buff.Hurt");
            data.start = HurtStart;
            data.start = HurtEnd;
            data.isBad = true;
            BuffList.Add(BuffType.hurt, data);
        }
        // heal
        {
            BuffData data = new BuffData();
            data.icon = Resources.Load<Sprite>("Icon/heal");
            data.name = LocalizationManager.Localize("Buff.Heal");
            data.start = HealStart;
            data.start = HealEnd;
            data.isBad = false;
            BuffList.Add(BuffType.heal, data);
        }
        // shield up
        {
            BuffData data = new BuffData();
            data.icon = Resources.Load<Sprite>("Icon/shield_up");
            data.name = LocalizationManager.Localize("Buff.Shield_up");
            data.start = ShieldUpStart;
            data.start = ShieldUpEnd;
            data.isBad = false;
            BuffList.Add(BuffType.shield_up, data);
        }
        // shield down
        {
            BuffData data = new BuffData();
            data.icon = Resources.Load<Sprite>("Icon/shield_down");
            data.name = LocalizationManager.Localize("Buff.Shield_down");
            data.start = ShieldDownStart;
            data.start = ShieldDownEnd;
            data.isBad = true;
            BuffList.Add(BuffType.shield_down, data);
        }
        // attack up
        {
            BuffData data = new BuffData();
            data.icon = Resources.Load<Sprite>("Icon/attack_up");
            data.name = LocalizationManager.Localize("Buff.Attack_up");
            data.start = AttackUpStart;
            data.start = AttackUpEnd;
            data.isBad = false;
            BuffList.Add(BuffType.shield_down, data);
        }
        // attack down
        {
            BuffData data = new BuffData();
            data.icon = Resources.Load<Sprite>("Icon/attack_down");
            data.name = LocalizationManager.Localize("Buff.Attack_down");
            data.start = AttackDownStart;
            data.start = AttackDownEnd;
            data.isBad = true;
            BuffList.Add(BuffType.attack_down, data);
        }
        // speed up
        {
            BuffData data = new BuffData();
            data.icon = Resources.Load<Sprite>("Icon/speed_up");
            data.name = LocalizationManager.Localize("Buff.Speed_up");
            data.start = SpeedUpStart;
            data.start = SpeedUpEnd;
            data.isBad = false;
            BuffList.Add(BuffType.speed_up, data);
        }
        // speed down
        {
            BuffData data = new BuffData();
            data.icon = Resources.Load<Sprite>("Icon/speed_down");
            data.name = LocalizationManager.Localize("Buff.Speed_down");
            data.start = SpeedDownStart;
            data.start = SpeedDownEnd;
            data.isBad = true;
            BuffList.Add(BuffType.speed_down, data);
        }
    }

    public static void StunStart()
    {

    }
    public static void StunEnd()
    {

    }
    public static void HurtStart()
    {

    }
    public static void HurtEnd()
    {

    }
    public static void HealStart()
    {

    }
    public static void HealEnd()
    {

    }
    public static void ShieldUpStart()
    {

    }
    public static void ShieldUpEnd()
    {

    }
    public static void ShieldDownStart()
    {

    }
    public static void ShieldDownEnd()
    {

    }
    public static void AttackUpStart()
    {

    }
    public static void AttackUpEnd()
    {

    }
    public static void AttackDownStart()
    {

    }
    public static void AttackDownEnd()
    {

    }
    public static void SpeedUpStart()
    {

    }
    public static void SpeedUpEnd()
    {

    }
    public static void SpeedDownStart()
    {

    }
    public static void SpeedDownEnd()
    {

    }
}
