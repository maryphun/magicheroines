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
    }

    public int GetCurrentStageProgress()
    {
        return playerData.currentStage;
    }

    public void StageProgress()
    {
        playerData.currentStage++;
    }
}
