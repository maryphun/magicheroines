using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.SimpleLocalization.Scripts;

public class FormationSlot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image lockIcon; 
    [SerializeField] private TMP_Text cost;

    [Header("Debug")]
    [SerializeField] private GameObject battler;

    public void Initialize(bool isLocked, int moneyCost)
    {
        lockIcon.color = isLocked ? Color.white : new Color(1, 1, 1, 0);
        this.cost.text = LocalizationManager.Localize("System.Cost") + ": " + moneyCost.ToString();
    }

    public void SetBattler(Character unit)
    {
        battler = Instantiate(unit.battler, transform);
        battler.transform.localPosition = Vector3.zero;
        battler.GetComponent<Battler>().InitializeCharacterData(unit);
    }

    public void ResetData(float delay)
    {
        StartCoroutine(ResetDataDelay(delay));
    }

    IEnumerator ResetDataDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Destroy(battler);
        battler = null;

        lockIcon.color = Color.white;
        this.cost.text = string.Empty;
    }
}
