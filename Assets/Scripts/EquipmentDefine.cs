using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipment", menuName = "作成/装備生成")]
public class EquipmentDefine : ScriptableObject
{
    [Header("セーブロード対応")]
    [SerializeField] public string pathName;

    [Header("基本資料")]
    [SerializeField] public string equipNameID;
    [SerializeField] public string descriptionID;
    [SerializeField] public string battleStartFunctionName;
    [SerializeField] public string battleEndFunctionName;
    [SerializeField, TextArea()] public string effectText;

    [Header("アイコン")]
    [SerializeField] public Sprite Icon;
}

public struct EquipmentData
{
    public EquipmentDefine data;
    public int equipingCharacterID;

    public EquipmentData(EquipmentDefine define)
    {
        data = define;
        equipingCharacterID = -1;
    }

    public void SetEquipCharacter(int characterID)
    {
        equipingCharacterID = characterID;
    }

    public void Unequip()
    {
        equipingCharacterID = -1;
    }
}