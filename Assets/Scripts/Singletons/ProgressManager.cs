using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        playerData.currentMoney = 0;
        playerData.currentResourcesPoint = 0;
        playerData.characters = new List<Character>();

        // 初期キャラ 
        PlayerCharacterDefine tentacle = Resources.Load<PlayerCharacterDefine>("PlayerCharacterList/TentacleMan");
        AddPlayerCharacter(tentacle);
        Resources.UnloadAsset(tentacle);
    }

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
    public List<Character> GetAllCharacter()
    {
        return playerData.characters;
    }

    /// <summary>
    /// 新しい仲間追加
    /// </summary>
    public void AddPlayerCharacter(PlayerCharacterDefine newCharacter)
    {
        var obj = new Character();

        obj.characterData = newCharacter.detail;
        obj.battler = newCharacter.battler;
        obj.level = 1;
    }
}
