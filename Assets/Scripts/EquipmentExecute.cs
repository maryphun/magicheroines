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
    public IEnumerator StolenUndieStart(Battler battler)
    {
        battler.onTurnBeginEvent.AddListener(EquipmentMethods.StolenUndie);
        yield return null;
    }
    public IEnumerator StolenUndieEnd(Battler battler)
    {
        battler.onTurnBeginEvent.RemoveListener(EquipmentMethods.StolenUndie);
        yield return null;
    }
}

public static class EquipmentMethods
{
    private static Battle battleManager;
    private static GameObject floatingTextOrigin;

    public static void Initialization(Battle reference)
    {
        battleManager = reference;
        floatingTextOrigin = Resources.Load<GameObject>("Prefabs/FloatingNumber");
    }

    public static void CreateFloatingText(string text, Color color, float size, Battler target)
    {
        if (floatingTextOrigin == null) return;

        // create floating text
        var floatingText = UnityEngine.GameObject.Instantiate(floatingTextOrigin, target.transform);
        floatingText.GetComponent<FloatingText>().Init(2.0f, target.GetMiddleGlobalPosition() + new Vector2(0.0f, target.GetCharacterSize().y * 0.35f), new Vector2(0.0f, 100.0f), text, size, color);
    }

    public static void StolenUndie()
    {
        if (battleManager == null) return;
        Battler battler = battleManager.GetCurrentBattler();

        // SP ‚Ì—Ê
        const float percentage = 0.12f;

        int healAmount = Mathf.FloorToInt((float)battler.max_mp * percentage);

        battler.AddSP(healAmount);
        CreateFloatingText(healAmount.ToString(), CustomColor.heal(), 32, battler); ;
        AudioManager.Instance.PlaySFX("Heal2");
    }

    public static void CushionExecute(int healAmount)
    {
        if (battleManager == null) return;
        Battler battler = battleManager.GetCurrentBattler();

        battler.Heal(healAmount);
    }
}
