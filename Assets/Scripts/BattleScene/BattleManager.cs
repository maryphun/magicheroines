using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;

public class Battle : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool isDebug = false;

    [Header("Setting")]
    [SerializeField] private float characterSpace = 150.0f;
    [SerializeField, Range(1.1f, 2.0f)] private float turnEndDelay = 1.1f;
    [SerializeField, Range(1.1f, 2.0f)] private float stunWaitDelay = 0.55f;
    [SerializeField, Range(1.1f, 2.0f)] private float enemyAIDelay = 2.0f;
    [SerializeField, Range(0.1f, 1.0f)] private float characterMoveTime = 0.5f;  // < キャラがターゲットの前に移動する時間
    [SerializeField, Range(0.1f, 1.0f)] private float attackAnimPlayTime = 0.2f; // < 攻撃アニメーションの維持時間
    [SerializeField, Range(0.1f, 1.0f)] private float buffIconFadeTime = 0.4f; // < バフアイコンの出現・消し時間
    [SerializeField] private float formationPositionX = 600.0f;
    [SerializeField] private Sprite buffIconFrame;
    [SerializeField] private GameObject buffCounterText;

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
    [SerializeField] private CharacterInfoPanel characterInfoPanel;

    [Header("Debug")]
    [SerializeField] private List<Battler> characterList = new List<Battler>();
    [SerializeField] private List<Battler> enemyList = new List<Battler>();
    [SerializeField] private Battler arrowPointingTarget = null;
    [SerializeField] private List<Buff> buffedCharacters = new List<Buff>();

    private void Awake()
    {
        AlphaFadeManager.Instance.FadeIn(5.0f);

        if (isDebug) ProgressManager.Instance.DebugModeInitialize(true); // デバッグ用
        var playerCharacters = ProgressManager.Instance.GetFormationParty(false);
        var actors = new List<Character>();
        for (int i = 0; i < playerCharacters.Count(); i++)
        {
            if (playerCharacters[i].characterID != -1)
            {
                actors.Add(ProgressManager.Instance.GetCharacterByID(playerCharacters[i].characterID));
            }
        }

        List<EnemyDefine> enemyList = SetupEnemy.GetEnemyList(true);
        InitializeBattleScene(actors, enemyList);

        // Send references
        ItemExecute.Instance.Initialize(this);
        AbilityExecute.Instance.Initialize(this);
        BuffManager.Init();
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
            // バフを先にチェック
            UpdateBuffForCharacter(GetCurrentBattler());

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
            AudioManager.Instance.PlaySFX("FormationCharge", 0.5f);

            // チュートリアルに入る
            if (ProgressManager.Instance.GetCurrentStageProgress() == 1)
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
        // is character stunned
        if (IsCharacterInBuff(currentCharacter, BuffType.stun))
        {
            yield return new WaitForSeconds(stunWaitDelay);
            NextTurn(false);
        }
        else
        {
            Battler targetCharacter = turnBaseManager.GetRandomPlayerChaacter();
            StartCoroutine(AttackAnimation(currentCharacter, targetCharacter, NextTurn));
        }
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

        // is character stunned
        if (IsCharacterInBuff(currentCharacter, BuffType.stun))
        {
            yield return new WaitForSeconds(stunWaitDelay);
            NextTurn(false);
        }
        else
        {
            actionPanel.SetEnablePanel(true);
        }
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

        // play SE
        AudioManager.Instance.PlaySFX("CharacterMove", 0.1f);

        yield return new WaitForSeconds(characterMoveTime * 0.5f);
        // change character hirachy temporary
        attacker.transform.SetParent(target.transform);
        yield return new WaitForSeconds(characterMoveTime * 0.5f);

        attacker.SpawnAttackVFX(target);

        // attack miss?
        bool isMiss = (UnityEngine.Random.Range(0, 100) > CalculateHitChance(attacker.speed - target.speed));

        if (!isMiss)
        {
            // calculate damage
            int realDamge = Mathf.RoundToInt((float)(attacker.attack - target.defense) * Mathf.Clamp((((float)(attacker.currentLevel - target.currentLevel) * 0.075f) + 1.0f), 0.5f, 2.0f));
            target.DeductHP(realDamge, true);

            // play SE
            AudioManager.Instance.PlaySFX("Attacked", 0.4f);

            // animation
            target.Shake(attackAnimPlayTime + characterMoveTime);
            attacker.PlayAnimation(BattlerAnimationType.attack);
            target.PlayAnimation(BattlerAnimationType.attacked);

            AddBuffToBattler(target, BuffType.hurt, 5, 20);

            // create floating text
            var floatingText = Instantiate(floatingTextOrigin, target.transform);
            floatingText.Init(2.0f, target.GetMiddleGlobalPosition(), (target.GetMiddleGlobalPosition() - attacker.GetMiddleGlobalPosition()) + new Vector2(0.0f, 100.0f), realDamge.ToString(), 64, new Color(1f, 0.75f, 0.33f));
        }
        else
        {
            // play SE
            AudioManager.Instance.PlaySFX("Miss", 0.5f);

            // animation
            RectTransform targetGraphic = target.GetGraphicRectTransform();
            float enemyPos = targetGraphic.localPosition.x;
            targetGraphic.DOLocalMoveX(enemyPos + ((target.transform.position.x - attacker.transform.position.x) * 0.5f), attackAnimPlayTime).SetEase(Ease.InOutBounce)
                .OnComplete(() => { targetGraphic.DOLocalMoveX(enemyPos, characterMoveTime); });

            // move character shadow with it
            RectTransform shadow = target.GetShadowRectTransform();
            shadow.DOLocalMoveX(enemyPos + ((target.transform.position.x - attacker.transform.position.x) * 0.5f), attackAnimPlayTime).SetEase(Ease.InOutBounce)
                .OnComplete(() => { shadow.DOLocalMoveX(0.0f, characterMoveTime); });

            // create floating text
            var floatingText = Instantiate(floatingTextOrigin, target.transform);
            floatingText.Init(2.0f, target.GetMiddleGlobalPosition(), (target.GetMiddleGlobalPosition() - attacker.GetMiddleGlobalPosition()) + new Vector2(0.0f, 100.0f), "MISS", 32, Color.yellow);
        }

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

    /// <summary>
    /// 攻撃の命中率を計算
    /// </summary>
    int CalculateHitChance(int dexterityDifference)
    {
        if (dexterityDifference < 0)
        {
            const int baseHitChance = 85;
            const float chanceModifier = 1.25f;

            int calculatedHitChance = baseHitChance + Mathf.RoundToInt((float)dexterityDifference * chanceModifier);

            // Ensure the calculatedHitChance is within valid bounds (0 to 100)
            return Mathf.Clamp(calculatedHitChance, 0, 100);
        }

        // If the defender has equal or lower speed, chance of hitting is 100%
        return 100;
    }

    public Battler GetCurrentBattler()
    {
        return turnBaseManager.GetCurrentTurnBattler();
    }

    /// <summary>
    /// キャラクターにバフを追加
    /// </summary>
    public void AddBuffToBattler(Battler target, BuffType buff, int turn, int value)
    {
        if (IsCharacterInBuff(target, buff))
        {
            // 既にこのバフ持っている
            var instance = buffedCharacters.FirstOrDefault(x => x.target == target && x.type == buff);

            // 終了させる
            RemoveBuffInstance(instance);

            // 高い数値の方を上書きする
            turn = Mathf.Max(turn, instance.remainingTurn);
            value = Mathf.Max(value, instance.value);
        }

        Buff _buff = new Buff();
        _buff.type = buff;
        _buff.data = BuffManager.BuffList[buff];
        _buff.target = target;
        _buff.remainingTurn = turn;
        _buff.value = value;

        _buff.data.start.Invoke(target, value);

        // Graphic
        // create icon
        _buff.graphic = new GameObject(_buff.data.name + "[" + turn.ToString() + "]");
        var frame = _buff.graphic.AddComponent<Image>();
        frame.sprite = buffIconFrame;
        frame.raycastTarget = false;
        frame.rectTransform.SetParent(_buff.target.transform);
        frame.rectTransform.position =  GetPositionOfFirstBuff(_buff.target);
        frame.rectTransform.sizeDelta = new Vector2(37.0f, 37.0f);
        frame.color = new Color(1f, 1f, 1f, 0.0f);
        frame.DOFade(1.0f, buffIconFadeTime);
        
        Image icon = new GameObject(_buff.data.icon.name).AddComponent<Image>();
        icon.sprite = _buff.data.icon;
        icon.raycastTarget = false;
        icon.rectTransform.SetParent(frame.transform);
        icon.rectTransform.localPosition = Vector2.zero;
        icon.rectTransform.sizeDelta = new Vector2(30.0f, 30.0f);
        icon.color = new Color(1f, 1f, 1f, 0.0f);
        icon.DOFade(1.0f, buffIconFadeTime);

        var countingText = Instantiate(buffCounterText, frame.transform);
        _buff.text = countingText.GetComponent<TMP_Text>();
        _buff.text.text = turn.ToString();
        _buff.text.rectTransform.localPosition = new Vector2(0.0f, 17.0f);

        buffedCharacters.Add(_buff);

        ArrangeBuffIcon(target);
        characterInfoPanel.UpdateIcons(target);
    }

    /// <summary>
    /// キャラにかけられているバフを更新
    /// </summary>
    public void UpdateBuffForCharacter(Battler target)
    {
        var buffList = GetAllBuffForSpecificBattler(target);
        for (int i = 0; i < buffList.Count; i++)
        {
            var buff = buffList[i];
            
            buff.remainingTurn--;
            buff.data.update.Invoke(buff.target, buff.value);
            buff.text.text = buff.remainingTurn.ToString();

            if (buff.remainingTurn <= 0)
            {
                RemoveBuffInstance(buff);
            }
        }
        characterInfoPanel.UpdateIcons(target);
    }

    /// <summary>
    /// 特定のバフを消す
    /// </summary>
    /// <param name="instance"></param>
    private void RemoveBuffInstance(Buff instance)
    {
        instance.data.end.Invoke(instance.target, instance.value);
        instance.graphic.GetComponent<Image>().DOFade(0.0f, buffIconFadeTime);
        Destroy(instance.graphic, 0.5f);
        buffedCharacters.Remove(instance);
        ArrangeBuffIcon(instance.target);
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsCharacterInBuff(Battler battler, BuffType buffType)
    {
        List<Buff> list = GetAllBuffForSpecificBattler(battler);

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].type == buffType && list[i].target == battler)
            {
                return true;
            }
        }
        
        return false;
    }

    /// <summary>
    /// キャラクターが持っているバフを全部取得する
    /// </summary>
    public List<Buff> GetAllBuffForSpecificBattler(Battler battler)
    {
        // Use LINQ to get all elements that match the condition
        IEnumerable<Buff> rtn = buffedCharacters.Where(x => x.target == battler);

        return rtn.ToList();
    }

    /// <summary>
    /// キャラが複数のバフを持っている場合バフの表示位置をアレンジする
    /// </summary>
    public void ArrangeBuffIcon(Battler battler)
    {
        var buffs = GetAllBuffForSpecificBattler(battler);

        // Check if any matches are found
        if (buffs.Any())
        {
            // スタート位置
            Vector3 position = GetPositionOfFirstBuff(battler);
            Vector3 addition = BuffPositionAddition();

            foreach (Buff buff in buffs)
            {
                buff.graphic.GetComponent<RectTransform>().position = position;
                position += addition;
            }
        }
    }

    public Vector3 GetPositionOfFirstBuff(Battler battler)
    {
        return battler.GetMiddleGlobalPosition() + new Vector2(-battler.GetCharacterSize().x * 0.25f, battler.GetCharacterSize().y * 0.5f);
    }

    /// <summary>
    /// バフのアレンジ用：アイコン位置の加算値を取得
    /// </summary>
    public Vector3 BuffPositionAddition()
    {
        Vector3 addition = new Vector3(50.0f, 0.0f, 0.0f);

        return addition;
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

        // チュートリアル終了 (負けイべント)
        bool isTutorial = (ProgressManager.Instance.GetCurrentStageProgress() == 1);
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
