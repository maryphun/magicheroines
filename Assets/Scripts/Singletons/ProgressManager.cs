using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.SimpleLocalization.Scripts;

/// <summary>
/// プレイヤーのゲーム進捗は全てここに記録する
/// セーブロードはこの構造体を保存したら良いという認識
/// </summary>
public struct PlayerData
{
    public int currentStage;             //< 現ステージ数
    public int currentMoney;             //< 資金
    public int currentResourcesPoint;    //< 研究ポイント
    public List<Character> characters;     //< 持っているキャラクター
}

public class ProgressManager : SingletonMonoBehaviour<ProgressManager>
{
    PlayerData playerData;

    /// <summary>
    /// ゲーム進捗を初期状態にする
    /// </summary>
    public void InitializeProgress()
    {
        playerData = new PlayerData();

        playerData.currentStage = 1; // 初期ステージ (チュートリアル)
        playerData.currentMoney = 100;
        playerData.currentResourcesPoint = 50;
        playerData.characters = new List<Character>();

        // 初期キャラ 
        PlayerCharacterDefine battler = Resources.Load<PlayerCharacterDefine>("PlayerCharacterList/1.Battler");
        AddPlayerCharacter(battler);
        Resources.UnloadAsset(battler);

        PlayerCharacterDefine tentacle = Resources.Load<PlayerCharacterDefine>("PlayerCharacterList/2.TentacleMan");
        AddPlayerCharacter(tentacle);
        Resources.UnloadAsset(tentacle);
    }

    /// <summary>
    /// 現在のゲーム進行状況を取得
    /// </summary>
    public int GetCurrentStageProgress()
    {
        return playerData.currentStage;
    }

    /// <summary>
    /// 
    /// </summary>
    public void StageProgress()
    {
        playerData.currentStage++;
    }

    /// <summary>
    /// キャラクター資料を更新
    /// </summary>
    public void UpdateCharacterData(List<Character> characters)
    {
        playerData.characters = characters;
    }

    /// <summary>
    /// 持っている仲間のリストを取得
    /// </summary>
    public List<Character> GetAllCharacter(bool originalReference = false)
    {
        if (originalReference)
        {
            return playerData.characters;
        }
        else
        {
            List<Character> characterListCopy = new List<Character>(playerData.characters);
            return characterListCopy;
        }
    }

    /// <summary>
    /// 新しい仲間追加
    /// </summary>
    public void AddPlayerCharacter(PlayerCharacterDefine newCharacter)
    {
        var obj = new Character();

        obj.localizedName = LocalizationManager.Localize(newCharacter.detail.nameID);
        obj.characterData = newCharacter.detail;
        obj.battler = newCharacter.battler;
        obj.current_level = newCharacter.detail.starting_level;
        obj.current_hp = newCharacter.detail.base_hp;
        obj.current_mp = newCharacter.detail.base_mp;
        obj.current_attack = newCharacter.detail.base_attack;
        obj.current_defense = newCharacter.detail.base_defense;
        obj.current_speed = newCharacter.detail.base_speed;

        playerData.characters.Add(obj);
    }

    /// <summary>
    /// 現在資金量を取得
    /// </summary>
    public int GetCurrentMoney()
    {
        return playerData.currentMoney;
    }

    /// <summary>
    /// 資金量を変更
    /// </summary>
    public void SetMoney(int newValue)
    {
        playerData.currentMoney = Mathf.Max(newValue, 0);
    }

    /// <summary>
    /// 現在資金量を取得
    /// </summary>
    public int GetCurrentResearchPoint()
    {
        return playerData.currentResourcesPoint;
    }

    /// <summary>
    /// 資金量を変更
    /// </summary>
    public void SetResearchPoint(int newValue)
    {
        playerData.currentResourcesPoint = Mathf.Max(newValue, 0);
    }
}
