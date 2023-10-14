using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class Battle : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool isDebug = false;

    [Header("Setting")]
    [SerializeField] private float characterSpace = 150.0f;
    [SerializeField, Range(1.1f, 2.0f)] private float turnEndDelay = 1.1f;
    [SerializeField, Range(1.1f, 2.0f)] private float enemyAIDelay = 2.0f;
    [SerializeField, Range(0.1f, 1.0f)] private float characterMoveTime = 0.5f;  // < キャラがターゲットの前に移動する時間
    [SerializeField, Range(0.1f, 1.0f)] private float attackAnimPlayTime = 0.2f; // < 攻撃アニメーションの維持時間
    [SerializeField] private float formationPositionX = 600.0f;

    [Header("References")]
    [SerializeField] private RectTransform playerFormation;
    [SerializeField] private RectTransform enemyFormation;
    [SerializeField] private TurnBase turnBaseManager;
    [SerializeField] private ActionPanel actionPanel;
    [SerializeField] private CharacterArrow characterArrow;
    [SerializeField] private RectTransform actionTargetArrow;
    [SerializeField] private BattleSceneTransition sceneTransition;
    [SerializeField] private FloatingText floatingTextOrigin;
    [SerializeField] private BattleSceneTutorial battleSceneTutorial;

    [Header("Debug")]
    [SerializeField] private List<Battler> characterList = new List<Battler>();
    [SerializeField] private List<Battler> enemyList = new List<Battler>();
    [SerializeField] private Battler arrowPointingTarget = null;

    private void Awake()
    {
        AlphaFadeManager.Instance.FadeIn(5.0f);

        if (isDebug) ProgressManager.Instance.DebugModeInitialize(true); // デバッグ用
        var playerCharacters = ProgressManager.Instance.GetFormationParty(false);
        var actors = new List<Character>();
        for (int i = 0; i < playerCharacters.Count(); i++)
        {
            if (playerCharacters[i].characterData != null)
            {
                actors.Add(playerCharacters[i].characterData);
            }
        }

        List<EnemyDefine> enemyList = SetupEnemy.GetEnemyList(true);
        InitializeBattleScene(actors, enemyList);

        ItemExecute.Instance.Initialize(this); // Send references
    }

    private void Start()
    {
        sceneTransition.StartScene(NextTurn);
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
        positionX = -(enemies.Count * characterSpace) * 0.5f;
        positionY_gap = totalGap / enemies.Count;
        positionY = max_positionY - positionY_gap;
        int siblingIndex = 0;
        foreach (EnemyDefine enemy in enemies)
        {
            GameObject obj = Instantiate<GameObject>(enemy.battler, enemyFormation);
            obj.transform.localPosition = new Vector3(positionX, positionY, 0.0f);
            obj.transform.SetSiblingIndex(siblingIndex);

            Battler component = obj.GetComponent<Battler>();
            component.InitializeEnemyData(enemy);

            enemyList.Add(component);

            positionX += characterSpace;
            positionY -= positionY_gap;
            siblingIndex++;
        }

        // 行動順を決める
        turnBaseManager.Initialization(characterList, enemyList);

        // 初期位置
        playerFormation.DOLocalMoveX(-formationPositionX * 2.1f, 0.0f, true);
        enemyFormation.DOLocalMoveX(formationPositionX * 2.1f, 0.0f, true);
        playerFormation.GetComponent<CanvasGroup>().alpha = 0.0f;
        enemyFormation.GetComponent<CanvasGroup>().alpha = 0.0f;
    }

    /// <summary>
    /// 次のターン
    /// </summary>
    public void NextTurn(bool isFirstTurn)
    {
        // ターンを始める前に戦闘が終わっているかをチェック
        if (IsVictory())
        {
            BattleEnd(true);
            return;
        }

        if (IsDefeat())
        {
            BattleEnd(false);
            return;
        }

        if (!isFirstTurn)
        {
            turnBaseManager.NextBattler();
        }
        else
        {
            // 最初のターン
            // キャラクターが位置に付く
            playerFormation.DOLocalMoveX(-formationPositionX, 0.5f).SetEase(Ease.OutQuart);
            enemyFormation.DOLocalMoveX(formationPositionX, 0.5f).SetEase(Ease.OutQuart);
            playerFormation.GetComponent<CanvasGroup>().DOFade(1.0f, 0.25f);
            enemyFormation.GetComponent<CanvasGroup>().DOFade(1.0f, 0.25f);

            // SE
            AudioManager.Instance.PlaySFX("FormationCharge");

            // チュートリアルに入る
            if (ProgressManager.Instance.GetCurrentStageProgress() == 0)
            {
                battleSceneTutorial.StartBattleTutorial();
            }
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

    /// <summary>
    /// リタイアしたキャラクターが出たらターンから外す
    /// </summary>
    public void UpdateTurnBaseManager(bool rearrange)
    {
        turnBaseManager.UpdateTurn(rearrange);
    }

    // マウスが指しているところがBattlerが存在しているなら返す
    public Battler GetBattlerByPosition(Vector2 mousePosition, bool allowTeammate, bool allowEnemy, bool aliveOnly)
    {
        if (allowEnemy)
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                Vector2 size = enemyList[i].GetCharacterSize() * new Vector2(0.5f, 1.0f);
                Vector2 position = enemyList[i].GetGraphicRectTransform().position + new Vector3(0.0f, size.y * 0.5f);
                if ((enemyList[i].isAlive || !aliveOnly)
                    && mousePosition.x > position.x - size.x * 0.5f
                    && mousePosition.x < position.x + size.x * 0.5f
                    && mousePosition.y > position.y - size.y * 0.5f
                    && mousePosition.y < position.y + size.y * 0.5f)
                {
                    return enemyList[i];
                }
            }
        }

        if (allowTeammate)
        {
            for (int i = 0; i < characterList.Count; i++)
            {
                Vector2 size = characterList[i].GetCharacterSize() * new Vector2(0.5f, 1.0f);
                Vector2 position = characterList[i].GetGraphicRectTransform().position + new Vector3(0.0f, size.y * 0.5f);
                if ((characterList[i].isAlive || !aliveOnly)
                    && mousePosition.x > position.x - size.x * 0.5f
                    && mousePosition.x < position.x + size.x * 0.5f
                    && mousePosition.y > position.y - size.y * 0.5f
                    && mousePosition.y < position.y + size.y * 0.5f)
                {
                    return characterList[i];
                }
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

        // TODO: 敵技作成
        // 攻撃目標を選択
        Battler targetCharacter = turnBaseManager.GetRandomPlayerChaacter();
        StartCoroutine(AttackAnimation(currentCharacter, targetCharacter, NextTurn));
    }

    IEnumerator TurnEndDelay()
    {
        actionPanel.SetEnablePanel(false);
        yield return new WaitForSeconds(turnEndDelay);

        Battler currentCharacter = turnBaseManager.GetCurrentTurnBattler();
        characterArrow.SetCharacter(currentCharacter, currentCharacter.GetCharacterSize().y);

        var originPos = currentCharacter.GetGraphicRectTransform().position;
        originPos = currentCharacter.isEnemy ? new Vector2(originPos.x - currentCharacter.GetCharacterSize().x * 0.25f, originPos.y + currentCharacter.GetCharacterSize().y * 0.5f) : new Vector2(originPos.x + currentCharacter.GetCharacterSize().x * 0.25f, originPos.y + currentCharacter.GetCharacterSize().y * 0.5f);
        actionTargetArrow.position = originPos;
        actionPanel.SetEnablePanel(true);
    }

    public void PointTargetWithArrow(Battler target, float animTime)
    {
        if (arrowPointingTarget == target) return;

        Battler currentBattler = turnBaseManager.GetCurrentTurnBattler();

        if (target == currentBattler) return; // 自分に指すことはないだろう

        var originPos = currentBattler.GetGraphicRectTransform().position;
        originPos = currentBattler.isEnemy ? new Vector2(originPos.x - currentBattler.GetCharacterSize().x * 0.25f, originPos.y + currentBattler.GetCharacterSize().y * 0.5f) : new Vector2(originPos.x + currentBattler.GetCharacterSize().x * 0.25f, originPos.y + currentBattler.GetCharacterSize().y * 0.5f);
        var targetPos = target.GetGraphicRectTransform().position;
        targetPos = target.isEnemy ? new Vector2(targetPos.x - target.GetCharacterSize().x * 0.25f, targetPos.y + target.GetCharacterSize().y * 0.5f) : new Vector2(targetPos.x, targetPos.y + target.GetCharacterSize().y * 0.5f);
        var length = Vector2.Distance(originPos, targetPos);

        actionTargetArrow.sizeDelta = new Vector2(actionTargetArrow.rect.width, 100.0f);
        actionTargetArrow.DOSizeDelta(new Vector2(actionTargetArrow.rect.width, length), animTime);
        actionTargetArrow.GetComponent<Image>().DOFade(0.2f, animTime);

        // rotate
        // Calculate direction vector
        Vector3 diff = targetPos - originPos;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        actionTargetArrow.rotation = Quaternion.Euler(0f, 0f, rot_z - 90.0f);

        arrowPointingTarget = target;
    }

    public void UnPointArrow(float animTime)
    {
        actionTargetArrow.GetComponent<Image>().DOFade(0.0f, animTime);
        arrowPointingTarget = null;
    }

    public void AttackCommand(Battler target)
    {
        StartCoroutine(AttackAnimation(turnBaseManager.GetCurrentTurnBattler(), target, NextTurn));
    }

    public void IdleCommand()
    {
        var battler = GetCurrentBattler();

        if (battler.max_mp == 0)
        {
            // 回復できるSPがない
            NextTurn(false);

            // SE再生
            AudioManager.Instance.PlaySFX("SystemActionPanel");
            return;
        }

        // 総SPの15%~20%を回復する
        int healAmount = Mathf.RoundToInt((float)battler.max_mp * UnityEngine.Random.Range(0.15f, 0.2f));

        var sequence = DOTween.Sequence();
                sequence
                .AppendCallback(() =>
                {
                    // text
                    var floatingText = Instantiate(floatingTextOrigin, battler.transform);
                    floatingText.Init(2f, battler.GetMiddleGlobalPosition(), new Vector2(0.0f, 150.0f), "+"+healAmount.ToString(), 64, new Color(0.75f, 0.75f, 1.00f));

                    // play SE
                    AudioManager.Instance.PlaySFX("PowerCharge");

                    // effect
                    battler.AddSP(healAmount);
                })
                .AppendInterval(0.25f)
                .AppendCallback(() =>
                {
                    NextTurn(false);
                });
    }

    IEnumerator AttackAnimation(Battler attacker, Battler target, Action<bool> callback)
    {
        Transform originalParent = attacker.transform.parent;
        int originalChildIndex = attacker.transform.GetSiblingIndex();

        var targetPos = target.GetComponent<RectTransform>().position;
        targetPos = target.isEnemy ? new Vector2(targetPos.x - target.GetCharacterSize().x * 0.5f, targetPos.y) : new Vector2(targetPos.x + target.GetCharacterSize().x * 0.5f, targetPos.y);
        var originalPos = attacker.GetComponent<RectTransform>().position;
        attacker.GetComponent<RectTransform>().DOMove(targetPos, characterMoveTime);

        yield return new WaitForSeconds(characterMoveTime * 0.5f);
        // change character hirachy temporary
        attacker.transform.SetParent(target.transform);
        yield return new WaitForSeconds(characterMoveTime * 0.5f);

        attacker.PlayAnimation(BattlerAnimationType.attack);
        target.PlayAnimation(BattlerAnimationType.attacked);
        attacker.SpawnAttackVFX(target);

        // calculate damage
        int realDamge = Mathf.RoundToInt((float)(attacker.attack - target.defense) * Mathf.Clamp((((float)(attacker.currentLevel - target.currentLevel) * 0.075f) + 1.0f), 0.5f, 2.0f));
        target.DeductHP(realDamge, true);

        // play SE
        AudioManager.Instance.PlaySFX("Attacked", 0.4f);
        
        target.Shake(attackAnimPlayTime + characterMoveTime);

        // check turns
        UpdateTurnBaseManager(false);

        // create floating text
        var floatingText = Instantiate(floatingTextOrigin, target.transform);
        floatingText.Init(2.0f, target.GetMiddleGlobalPosition(), (target.GetMiddleGlobalPosition() - attacker.GetMiddleGlobalPosition()) + new Vector2(0.0f, 100.0f), realDamge.ToString(), 64, new Color(1f, 0.75f, 0.33f));

        yield return new WaitForSeconds(attackAnimPlayTime);

        attacker.PlayAnimation(BattlerAnimationType.idle);
        target.PlayAnimation(BattlerAnimationType.idle);

        attacker.GetComponent<RectTransform>().DOMove(originalPos, characterMoveTime);

        yield return new WaitForSeconds(characterMoveTime * 0.5f);
        // return to original parent
        attacker.transform.SetParent(originalParent);
        attacker.transform.SetSiblingIndex(originalChildIndex);
        yield return new WaitForSeconds(characterMoveTime * 0.5f);

        callback?.Invoke(false);
    }

    public Battler GetCurrentBattler()
    {
        return turnBaseManager.GetCurrentTurnBattler();
    }

    /// <summary>
    ///  敵を全員倒せた
    /// </summary>
    private bool IsVictory()
    {
        // 敵全滅か
        Battler result = enemyList.Find(s => s.isAlive);
        if (result == null)
        {
            // 生存者いない
            return true;
        }

        // 戦闘が続く
        return false;
    }

    /// <summary>
    /// 味方全員リタイアした
    /// </summary>
    private bool IsDefeat()
    {
        // 味方全滅か
        Battler result = characterList.Find(s => s.isAlive);
        if (result == null)
        {
            // 生存者いない
            return true;
        }

        // 戦闘が続く
        return false;
    }

    private void BattleEnd(bool isVictory)
    {
        actionTargetArrow.gameObject.SetActive(false);
        characterArrow.gameObject.SetActive(false);
        actionPanel.SetEnablePanel(false);

        bool isTutorial = (ProgressManager.Instance.GetCurrentStageProgress() == 0);
        if (!isTutorial)
        {
            sceneTransition.EndScene(isVictory, ChangeScene);
        }
        else
        {
            // 敗北イベント(0.5秒待ってから)
            DOTween.Sequence().AppendInterval(0.5f).AppendCallback(() => { NovelSingletone.Instance.PlayNovel("Tutorial3", true, sceneTransition.EndTutorial); });
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
