using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleSetup
{
    static List<EnemyDefine> enemies;
    public static bool isStoryMode = false;

    public static string BattleBGM { get { return battleBGM; } }
    private static string battleBGM = string.Empty;

    public static List<EquipmentDefine> equipmentReward;
    public static List<ItemDefine> itemReward;
    public static int moneyReward;
    public static int researchPointReward;
    public static SideQuestData sideQuestIncrement;

    public static List<EnemyDefine> GetEnemyList(bool clear)
    {
        var rtn = new List<EnemyDefine>(enemies);
        if (clear)
        {
            enemies.Clear();
        }

        return rtn;
    }

    public static void SetEnemy(List<EnemyDefine> enemyList)
    {
        enemies = enemyList;
    }

    /// <summary>
    /// Žg‚¤‘O‚É•K‚¸ŒÄ‚ñ‚Å‚¨‚­‚±‚Æ
    /// </summary>
    public static void Reset(bool isStory)
    {
        enemies = new List<EnemyDefine>();
        isStoryMode = isStory;
        battleBGM = string.Empty;
        equipmentReward = new List<EquipmentDefine>();
        itemReward = new List<ItemDefine>();
        sideQuestIncrement = new SideQuestData(0,0,0);
        moneyReward = 0;
        researchPointReward = 0;
    }

    /// <summary>
    /// Enemy prefab name in Resources/EnemyList/
    /// </summary>
    public static void AddEnemy(string enemyPrefabName)
    {
        if (ReferenceEquals(enemies, null))
        {
            enemies = new List<EnemyDefine>();
        }
        var enemy = Resources.Load<EnemyDefine>("EnemyList/" + enemyPrefabName);

        if (enemy != null)
        {
            enemies.Add(enemy);
        }
        else
        {
            Debug.LogWarning("EnemyList/" + enemyPrefabName + " doesn't exist!");
        }
    }

    public static void SetBattleBGM(string clipName)
    {
        battleBGM = clipName;
    }

    public static void SetReward(int money, int researchPoint)
    {
        moneyReward = money;
        researchPointReward = researchPoint;
    }


    public static void AddItemReward(string itemName)
    {
        ItemDefine itemData = Resources.Load<ItemDefine>("ItemList/" + itemName);
        AddItemReward(itemData);
        Resources.UnloadAsset(itemData);
    }
    public static void AddEquipmentReward(string equipmentName)
    {
        EquipmentDefine equipmentData = Resources.Load<EquipmentDefine>("EquipmentList/" + equipmentName);

        if (!ProgressManager.Instance.PlayerHasEquipment(equipmentData)) // ‚·‚Å‚É‚ ‚é‘•”õ‚ÍŒJ‚è•Ô‚µ‚Ä–á‚í‚È‚¢
        {
            AddEquipmentReward(equipmentData);
        }

        Resources.UnloadAsset(equipmentData);
    }
    public static void AddItemReward(ItemDefine item)
    {
        itemReward.Add(item);
    }
    public static void AddEquipmentReward(EquipmentDefine equipment)
    {
        equipmentReward.Add(equipment);
    }

    public static void SetSideQuestIncrement(int food, int bank, int research)
    {
        sideQuestIncrement = new SideQuestData(food, bank, research);
    }
}
