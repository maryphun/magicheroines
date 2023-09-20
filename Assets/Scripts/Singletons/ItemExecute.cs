using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemExecute : SingletonMonoBehaviour<ItemExecute>
{
    [Header("References")]
    [SerializeField] private Battle battleManager;
    [SerializeField] private List<Battler> targetBattlers;

    public void Initialize(Battle battleManagerScript)
    {
        battleManager = battleManagerScript;
    }

    public void SetTargetBattler(List<Battler> targets)
    {

    }


    // items
    public void OnUseCroissant()
    {
        Debug.Log("use item croissant!");

        battleManager.NextTurn(false);
    }

    public void OnUseBread()
    {
        Debug.Log("use item bread!");

        battleManager.NextTurn(false);
    }
}
