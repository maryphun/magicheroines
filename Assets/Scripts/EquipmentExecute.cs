using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentExecute : SingletonMonoBehaviour<EquipmentExecute>
{
    public IEnumerator CorruptedGloveStart(Battler battler)
    {
        battler.max_hp = Mathf.Clamp(battler.max_hp - 30, 1, battler.max_hp);
        battler.current_hp = Mathf.Clamp(battler.current_hp, 1, battler.max_hp);
        battler.defense = Mathf.Max(battler.defense -3, 0);
        battler.attack += 25;

        yield return null;
    }
    public IEnumerator CorruptedGloveEnd(Battler battler)
    {
        yield return null;
    }


    public IEnumerator NiceTshirtStart(Battler battler)
    {
        battler.speed += 5;

        yield return null;
    }
    public IEnumerator NiceTshirtEnd(Battler battler)
    {
        yield return null;
    }


    public IEnumerator HelmetStart(Battler battler)
    {
        battler.max_hp += 10;
        battler.current_hp += 10;
        battler.defense += 6;
        
        yield return null;
    }
    public IEnumerator HelmetEnd(Battler battler)
    {
        yield return null;
    }
}
