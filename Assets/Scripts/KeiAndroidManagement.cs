using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Battler))]
public class KeiAndroidManagement : MonoBehaviour
{
    [Header("Debug")]
    Battler kei;
    Battler[] spawnedAndroid = new Battler[2];
    Battle battleManager;
    List<EnemyDefine> possibleSpawn; 

    // Start is called before the first frame update
    void Start()
    {
        battleManager = FindObjectOfType<Battle>();
        if (battleManager == null) return; // バトル画面ではない

        kei.onDeathEvent.AddListener(OnDead);

        // 初期化
        for (int i = 0; i < spawnedAndroid.Length; i++)
        {
            spawnedAndroid[i] = null;
        }

        // 京が召喚できるモノをロード
        possibleSpawn = new List<EnemyDefine>();
        possibleSpawn.Add(Resources.Load<EnemyDefine>("EnemyList/" + "Drone 4"));
        possibleSpawn.Add(Resources.Load<EnemyDefine>("EnemyList/" + "Android 1"));
        possibleSpawn.Add(Resources.Load<EnemyDefine>("EnemyList/" + "GoldDrone 2"));
        possibleSpawn.Add(Resources.Load<EnemyDefine>("EnemyList/" + "GoldAndroid 3"));
        possibleSpawn.Add(Resources.Load<EnemyDefine>("EnemyList/" + "DarkAndroid 2"));
        possibleSpawn.Add(Resources.Load<EnemyDefine>("EnemyList/" + "DarkAndroid 3"));
    }

    public void SpawnNewAndroid()
    {
        // どのキャラを召喚するのか決める
        EnemyDefine targetSummon = possibleSpawn[UnityEngine.Random.Range(0, possibleSpawn.Count)];

        {
            // 生成
            GameObject obj = Instantiate<GameObject>(targetSummon.battler, kei.transform.parent);

            // 位置を決める
            obj.transform.localPosition = new Vector3(positionX, positionY, 0.0f);
            obj.transform.SetSiblingIndex(siblingIndex);

            // 敵を初期化
            Battler component = obj.GetComponent<Battler>();
            component.InitializeEnemyData(targetSummon);

            // ターン順位の最後尾に置く
            battleManager.AddEnemy(component);

            // 戦闘ログ
            battleManager.AddBattleLog("abc");
        }
    }

    // 京ちゃんがリタイア
    public void OnDead()
    {
        // けいと一緒にタ倒られる
        for (int i = 0; i < spawnedAndroid.Length; i ++)
        {
            if (spawnedAndroid[i] != null)
            {
                spawnedAndroid[i].KillBattler();
            }
        }
    }
}
