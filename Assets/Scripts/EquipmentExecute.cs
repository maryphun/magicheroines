using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentExecute : SingletonMonoBehaviour<EquipmentExecute>
{
    private Battler battler;

    public void SetBattler(Battler currentBattler)
    {
        this.battler = currentBattler;
    }

    public void CorruptedGloveStart()
    {
        battler.max_hp = Mathf.Clamp(battler.max_hp - 30, 1, battler.max_hp);
        battler.current_hp = Mathf.Clamp(battler.current_hp, 1, battler.max_hp);
        battler.defense = Mathf.Max(battler.defense -3, 0);
        battler.attack += 25;
    }
    public void CorruptedGloveEnd()
    {

    }


    public void NiceTshirtStart()
    {
        battler.speed += 5;
    }
    public void NiceTshirtEnd()
    {

    }


    public void HelmetStart()
    {
        battler.max_hp += 10;
        battler.current_hp += 10;
        battler.defense += 6;

        Debug.Log("Equipment executed");
    }
    public void HelmetEnd()
    {

    }
}
