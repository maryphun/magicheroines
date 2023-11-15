using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentExecute : SingletonMonoBehaviour<EquipmentExecute>
{
    public IEnumerator CorruptedGloveStart(Battler battler)
    {
        yield return null;
    }
    public IEnumerator CorruptedGloveEnd(Battler battler)
    {
        yield return null;
    }


    public IEnumerator NiceTshirtStart(Battler battler)
    {
        yield return null;
    }
    public IEnumerator NiceTshirtEnd(Battler battler)
    {
        yield return null;
    }


    public IEnumerator HelmetStart(Battler battler)
    {
        yield return null;
    }
    public IEnumerator HelmetEnd(Battler battler)
    {
        yield return null;
    }

    public IEnumerator StungunStart(Battler battler)
    {
        var ability = Resources.Load<Ability>("AbilityList/Stungun");

        if (!ReferenceEquals(ability, null))
        {
            battler.AddAbilityToCharacter(ability);
        }
        else
        {
            Debug.LogWarning("Ability data 'AbilityList/Stungun' not couldn't be loaded.");
        }
        yield return null;
    }
}
