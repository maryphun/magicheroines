using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Battle : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float characterSpace = 150.0f;
    [SerializeField, Range(1.1f, 2.0f)] private float turnEndDelay = 1.1f;
    [SerializeField, Range(1.1f, 2.0f)] private float enemyAIDelay = 2.0f;

    [Header("References")]
    [SerializeField] private Transform playerFormation;
    [SerializeField] private Transform enemyFormation;
    [SerializeField] private TurnBase turnBaseManager;
    [SerializeField] private ActionPanel actionPanel;
    [SerializeField] private CharacterArrow characterArrow;
    [SerializeField] private RectTransform actionTargetArrow;

    [Header("Debug")]
    [SerializeField] private List<Battler> characterList = new List<Battler>();
    [SerializeField] private List<Battler> enemyList = new List<Battler>();

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
            
            characterList.Add(component);

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

            enemyList.Add(component);
        }

        // 行動順を決める
        turnBaseManager.Initialization(characterList, enemyList);

        // 早速最初のターンを始める
        NextTurn(true);
    }

    /// <summary>
    /// 次のターン
    /// </summary>
    public void NextTurn(bool isFirstTurn)
    {
        if (!isFirstTurn)
        {
            turnBaseManager.NextBattler();
        }

        Battler currentTurnCharacter = turnBaseManager.GetCurrentTurnBattler();

        if (currentTurnCharacter.isEnemy)
        {
            actionPanel.SetEnablePanel(false);

            // AI 行動
            StartCoroutine(EnemyAI());
        }
        else
        {
            // 行動出来るまでに遅延させる
            StartCoroutine(TurnEndDelay());
        }
    }

    // マウスが指しているところがBattlerが存在しているなら返す
    public Battler GetBattlerByPosition(Vector2 mousePosition, bool enemyOnly)
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            Vector2 size = enemyList[i].GetCharacterSize();
            Vector2 position = enemyList[i].GetGraphicRectTransform().position;
            if (   mousePosition.x > position.x - size.x * 0.5f 
                && mousePosition.x < position.x + size.x * 0.5f
                && mousePosition.y > position.y - size.y * 0.5f
                && mousePosition.y < position.y + size.y * 0.5f)
            {
                return enemyList[i];
            }
        }

        if (enemyOnly) return null;

        for (int i = 0; i < characterList.Count; i++)
        {
            Vector2 size = characterList[i].GetCharacterSize();
            Vector2 position = characterList[i].GetGraphicRectTransform().position;
            if (   mousePosition.x > position.x - size.x * 0.5f 
                && mousePosition.x < position.x + size.x * 0.5f
                && mousePosition.y > position.y - size.y * 0.5f
                && mousePosition.y < position.y + size.y * 0.5f)
            {
                return characterList[i];
            }
        }

        return null;
    }

    IEnumerator EnemyAI()
    {
        Battler currentCharacter = turnBaseManager.GetCurrentTurnBattler();
        characterArrow.SetCharacter(currentCharacter, currentCharacter.GetCharacterSize().y);
        actionTargetArrow.position = currentCharacter.GetGraphicRectTransform().position;

        yield return new WaitForSeconds(enemyAIDelay);
        
        // TODO: 敵AI作成
        NextTurn(false);
    }

    IEnumerator TurnEndDelay()
    {
        actionPanel.SetEnablePanel(false);
        yield return new WaitForSeconds(turnEndDelay);

        Battler currentCharacter = turnBaseManager.GetCurrentTurnBattler();
        characterArrow.SetCharacter(currentCharacter, currentCharacter.GetCharacterSize().y);
        actionTargetArrow.position = currentCharacter.GetGraphicRectTransform().position;
        actionPanel.SetEnablePanel(true);
    }

    public void PointTargetWithArrow(Battler target, float animTime)
    {
        var originPos = turnBaseManager.GetCurrentTurnBattler().GetGraphicRectTransform().position;
        var targetPos = target.GetGraphicRectTransform().position;
        var length = Vector2.Distance(originPos, targetPos);

        actionTargetArrow.sizeDelta = new Vector2(actionTargetArrow.rect.width, 100.0f);
        actionTargetArrow.DOSizeDelta(new Vector2(actionTargetArrow.rect.width, length), animTime);
        actionTargetArrow.GetComponent<Image>().DOFade(1.0f, animTime);

        // rotate
        // Calculate direction vector
        Vector3 direction = (targetPos - originPos).normalized;

        // Calculate angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate object A
        actionTargetArrow.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void UnPointArrow(float animTime)
    {
        actionTargetArrow.GetComponent<Image>().DOFade(0.0f, animTime);
    }
}
