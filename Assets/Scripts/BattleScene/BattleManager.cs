using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Battle : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float characterSpace = 150.0f;

    [Header("References")]
    [SerializeField] private Transform playerFormation;
    [SerializeField] private Transform enemyFormation;

    [Header("Debug")]
    [SerializeField] private List<Battler> charactersList = new List<Battler>();
    [SerializeField] private List<Battler> enemiesList = new List<Battler>();

    private void Awake()
    {
        ProgressManager.Instance.DebugModeInitialize(); // デバッグ用
        AlphaFadeManager.Instance.FadeIn(5.0f);

        var playerCharacters = ProgressManager.Instance.GetAllCharacter(false);

        EnemyDefine akiho = Resources.Load<EnemyDefine>("EnemyList/Akiho_Enemy");
        List<EnemyDefine> enemyList = new List<EnemyDefine>();
        enemyList.Add(akiho);

        InitializeBattleScene(playerCharacters, enemyList);
    }

    // Start is called before the first frame update
    void InitializeBattleScene(List<Character> actors, List<EnemyDefine> enemies)
    {
        const float max_positionY = 130.0f;
        const float totalGap = 330.0f;

        // プレイヤーキャラ生成
        float positionX = (actors.Count * characterSpace) * 0.5f;
        float positionY_gap = totalGap / actors.Count;
        float positionY = max_positionY - positionY_gap;
        foreach (Character actor in actors)
        {
            GameObject obj = Instantiate<GameObject>(actor.battler, playerFormation);
            obj.transform.localPosition = new Vector3(positionX, positionY, 0.0f);

            Battler component = obj.GetComponent<Battler>();
            component.InitializeCharacterData(actor);
            
            charactersList.Add(component);

            positionX -= characterSpace;
            positionY -= positionY_gap;
        }

        // 敵キャラ生成
        foreach (EnemyDefine enemy in enemies)
        {
            GameObject obj = Instantiate<GameObject>(enemy.battler, enemyFormation);
            obj.transform.localPosition = Vector3.zero;

            Battler component = obj.GetComponent<Battler>();
            component.InitializeEnemyData(enemy);

            enemiesList.Add(component);
        }
    }
}
