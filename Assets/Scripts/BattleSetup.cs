using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleSetup
{
    static List<EnemyDefine> enemies;
    public static bool isStoryMode = false;

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
}
