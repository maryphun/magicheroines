using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.SimpleLocalization.Scripts;
using System.Linq;

/// <summary>
/// プレイヤーのゲーム進捗は全てここに記録する
/// セーブロードはこの構造体を保存したら良いという認識
/// </summary>
[System.Serializable]
public struct PlayerData
{
    public int currentStage;             //< 現ステージ数
    public int currentMoney;             //< 資金
    public int currentResourcesPoint;    //< 研究ポイント
    public List<Character> characters;     //< 持っているキャラクター
    public FormationSlotData[] formationCharacters; //< パーティー編成
    public List<ItemDefine> inventory;   //< 所持アイテム
    public List<EquipmentData> equipment;   //< 所持装備
    public List<HomeDialogue> homeDialogue;   //< ホームシーンのセリフを管理する
    public int formationSlotUnlocked;    //< 解放されたスロット
}

public class ProgressManager : SingletonMonoBehaviour<ProgressManager>
{
    public PlayerData PlayerData { get { return playerData; } }
    PlayerData playerData;

#if DEBUG_MODE
    bool isDebugModeInitialized = false;
#endif

    /// <summary>
    /// ゲーム進捗を初期状態にする
    /// </summary>
    public void InitializeProgress()
    {
        playerData = new PlayerData();

        playerData.currentStage = 1; // 初期ステージ (チュートリアル)
        playerData.currentMoney = 500;
        playerData.currentResourcesPoint = 0;
        playerData.characters = new List<Character>();
        playerData.formationCharacters = new FormationSlotData[5];
        playerData.inventory = new List<ItemDefine>();
        playerData.equipment = new List<EquipmentData>();
        playerData.homeDialogue = new List<HomeDialogue>();
        playerData.formationSlotUnlocked = 2;

        // 初期ホームシーンキャラ
        HomeDialogue no5 = Resources.Load<HomeDialogue>("HomeDialogue/No5");
        playerData.homeDialogue.Add(no5);
        Resources.UnloadAsset(no5);

        // 初期キャラ 
        PlayerCharacterDefine battler = Resources.Load<PlayerCharacterDefine>("PlayerCharacterList/1.Battler");
        AddPlayerCharacter(battler);
        Resources.UnloadAsset(battler);

        PlayerCharacterDefine tentacle = Resources.Load<PlayerCharacterDefine>("PlayerCharacterList/2.TentacleMan");
        AddPlayerCharacter(tentacle);
        Resources.UnloadAsset(tentacle);

        PlayerCharacterDefine clone = Resources.Load<PlayerCharacterDefine>("PlayerCharacterList/3.Clone");
        AddPlayerCharacter(clone);
        Resources.UnloadAsset(clone);

        // 初期キャラを自動的にパーティーに編入する
        for (int i = 0; i < playerData.formationCharacters.Length; i++)
        {
            if (i < playerData.formationSlotUnlocked)
            {
                playerData.formationCharacters[i] = new FormationSlotData(playerData.characters[i].characterData.characterID, true);
            }
            else
            {
                playerData.formationCharacters[i] = new FormationSlotData(-1, false);
            }
        } 
    }

    public void ApplyLoadedData(PlayerData data)
    {
        playerData = data;
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
    /// CharacterID からキャラクターを取得する
    /// </summary>
    /// <returns></returns>
    public Character GetCharacterByID(int characterID)
    {
        List<Character> characterListCopy = new List<Character>(playerData.characters);

        return characterListCopy.Find(item => item.characterData.characterID == characterID);
    }

    /// <summary>
    /// 使える仲間キャラのリストを取得
    /// </summary>
    public List<Character> GetAllUsableCharacter()
    {
        List<Character> usableCharacter = playerData.characters.Where(data => data.is_corrupted || !data.characterData.is_heroin).ToList();

        return usableCharacter;
    }

    /// <summary>
    /// 新しい仲間追加
    /// </summary>
    public void AddPlayerCharacter(PlayerCharacterDefine newCharacter)
    {
        var obj = new Character();

        obj.pathName = newCharacter.name;

        obj.localizedName = LocalizationManager.Localize(newCharacter.detail.nameID);
        obj.characterData = newCharacter.detail;
        obj.battler = newCharacter.battler;
        obj.current_level = newCharacter.detail.starting_level;
        obj.current_maxHp = newCharacter.detail.base_hp;
        obj.current_maxMp = newCharacter.detail.base_mp;
        obj.current_hp = newCharacter.detail.base_hp;
        obj.current_mp = newCharacter.detail.base_mp;
        obj.current_attack = newCharacter.detail.base_attack;
        obj.current_defense = newCharacter.detail.base_defense;
        obj.current_speed = newCharacter.detail.base_speed;
        obj.is_corrupted = !(newCharacter.detail.is_heroin); // ヒロインキャラはとりあえず使用できない
        
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

    /// <summary>
    /// パーティー編成最大数を取得
    /// </summary>
    public int GetUnlockedFormationCount()
    {
        return playerData.formationSlotUnlocked;
    }

    /// <summary>
    /// パーティー編成最大数を増加
    /// </summary>
    public void UnlockedFormationCount()
    {
        playerData.formationSlotUnlocked ++;
    }

    /// <summary>
    /// 出征パーティー取得
    /// </summary>
    public FormationSlotData[] GetFormationParty(bool originalReference = false)
    {
        if (originalReference)
        {
            return playerData.formationCharacters;
        }
        else
        {
            FormationSlotData[] partyListCopy = (FormationSlotData[])playerData.formationCharacters.Clone();
            return partyListCopy;
        }
    }

    /// <summary>
    /// 出征パーティー取得
    /// </summary>
    public void SetFormationParty(FormationSlotData[] characters)
    {
        playerData.formationCharacters = characters;
    }
    
    /// <summary>
    /// 持っているアイテムのリストを取得
    /// </summary>
    public List<ItemDefine> GetItemList(bool originalReference = false)
    {
        if (originalReference)
        {
            return playerData.inventory;
        }
        else
        {
            List<ItemDefine> itemListCopy = new List<ItemDefine>(playerData.inventory);
            return itemListCopy;
        }
    }

    /// <summary>
    /// インベントリを更新
    /// </summary>
    public void SetItemList(List<ItemDefine> newList)
    {
        playerData.inventory = newList;
    }

    /// <summary>
    /// アイテム獲得
    /// </summary>
    public void AddItemToInventory(ItemDefine item)
    {
        playerData.inventory.Add(item);
    }

    /// <summary>
    /// アイテム獲得
    /// </summary>
    public void RemoveItemFromInventory(ItemDefine item)
    {
        playerData.inventory.Remove(item);
    }

    /// <summary>
    /// ホーム台詞キャラクターを取得
    /// </summary>
    public List<HomeDialogue> GetHomeCharacter() 
    {
        return playerData.homeDialogue;
    }

    /// <summary>
    /// 装備を入手する
    /// </summary>
    public void AddNewEquipment(EquipmentDefine data)
    {
        EquipmentData newEquipment = new EquipmentData(data);
        playerData.equipment.Add(newEquipment);
    }

    /// <summary>
    /// 指定の装備アイテムを装備する, セーブデータの都合でCharacterIDとしてデータを残し、装備とキャラを紐つける
    /// </summary>
    public void ApplyEquipmentToCharacter(EquipmentDefine data, Character character)
    {
        for (int i = 0; i < playerData.equipment.Count; i++)
        {
            if (playerData.equipment[i].data.pathName == data.pathName)
            {
                playerData.equipment[i].SetEquipCharacter(character.characterData.characterID);
            }
        }
    }

    /// <summary>
    /// 装備を外す
    /// </summary>
    public void UnapplyEquipment(EquipmentDefine data)
    {
        for (int i = 0; i < playerData.equipment.Count; i++)
        {
            if (playerData.equipment[i].data.pathName == data.pathName)
            {
                playerData.equipment[i].Unequip();
            }
        }
    }

    public List<EquipmentData> GetEquipmentData(bool originalReference = false)
    {
        if (originalReference)
        {
            return playerData.equipment;
        }
        else
        {
            List<EquipmentData> copy = new List<EquipmentData>(playerData.equipment);
            return copy;
        }
    }

#if DEBUG_MODE
    public void DebugModeInitialize(bool addEnemy = false)
    {
        if (isDebugModeInitialized) return;
        ProgressManager.Instance.InitializeProgress();
        ProgressManager.Instance.SetMoney(Random.Range(200, 9999));
        ProgressManager.Instance.SetResearchPoint(Random.Range(200, 9999));
        isDebugModeInitialized = true;

        // 調教できるヒロインを追加
        PlayerCharacterDefine Akiho = Resources.Load<PlayerCharacterDefine>("PlayerCharacterList/4.Akiho");
        AddPlayerCharacter(Akiho);
        Resources.UnloadAsset(Akiho);

        PlayerCharacterDefine Rikka = Resources.Load<PlayerCharacterDefine>("PlayerCharacterList/5.Rikka");
        AddPlayerCharacter(Rikka);
        Resources.UnloadAsset(Rikka);

        // アイテムをいくつかついかする
        ItemDefine bread = Resources.Load<ItemDefine>("ItemList/食パン");
        for (int i = 0; i < Random.Range(2, 5); i++) playerData.inventory.Add(bread);
        Resources.UnloadAsset(bread);

        ItemDefine croissant = Resources.Load<ItemDefine>("ItemList/クロワッサン");
        for (int i = 0; i < Random.Range(2, 5); i++) playerData.inventory.Add(croissant);
        Resources.UnloadAsset(croissant);

        ItemDefine m24 = Resources.Load<ItemDefine>("ItemList/M24");
        for (int i = 0; i < Random.Range(2, 5); i++) playerData.inventory.Add(m24);
        Resources.UnloadAsset(m24);

        ItemDefine aid = Resources.Load<ItemDefine>("ItemList/救急箱");
        for (int i = 0; i < Random.Range(2, 5); i++) playerData.inventory.Add(aid);
        Resources.UnloadAsset(aid);

        // 敵キャラを設置
        if (addEnemy)
        {
            SetupEnemy.ClearEnemy();
            SetupEnemy.AddEnemy("Akiho_Enemy");
            SetupEnemy.AddEnemy("Rikka_Enemy");
        }
    }
#else
public void DebugModeInitialize() { }
#endif
}
