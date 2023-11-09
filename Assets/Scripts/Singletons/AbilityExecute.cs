using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Assets.SimpleLocalization.Scripts;

public class AbilityExecute : SingletonMonoBehaviour<AbilityExecute>
{
    [Header("References")]
    [SerializeField] private Battle battleManager;
    [SerializeField] private List<Battler> targetBattlers;

    /// <summary>
    /// Send references
    /// </summary>
    /// <param name="battleManagerScript"></param>
    public void Initialize(Battle battleManagerScript)
    {
        battleManager = battleManagerScript;
    }

    #region common methods
    public void SetTargetBattlers(List<Battler> targets)
    {
        targetBattlers = targets;
    }

    public void SetTargetBattler(Battler target)
    {
        targetBattlers = new List<Battler>();
        targetBattlers.Add(target);
    }

    private FloatingText CreateFloatingText(Transform parent)
    {
        GameObject origin = Resources.Load<GameObject>("Prefabs/FloatingNumber");
        var obj = Instantiate(origin);
        obj.transform.SetParent(parent);
        var floatingTextComponent = obj.GetComponent<FloatingText>();

        return floatingTextComponent;
    }

    private void CreateFadingImage(Sprite sprite, float fadeTime)
    {
        Vector2 position = battleManager.GetCurrentBattler().GetMiddleGlobalPosition();

        // Create Canvas
        Image img = new GameObject("Using Item Graphic").AddComponent<Image>();
        img.sprite = sprite;
        img.raycastTarget = false;
        img.rectTransform.position = position;

        img.transform.SetParent(battleManager.GetCurrentBattler().transform.parent.parent);

        img.rectTransform.DOMoveY(position.y + 150.0f, fadeTime);
        img.DOFade(0.0f, fadeTime);

        Destroy(img.gameObject, fadeTime);
    }

    private void CreateMovingImage(Sprite sprite, Vector3 targetPosition, float animTime)
    {
        Vector2 position = battleManager.GetCurrentBattler().GetMiddleGlobalPosition();

        // Create Canvas
        Image img = new GameObject("Using Item Graphic").AddComponent<Image>();
        img.sprite = sprite;
        img.raycastTarget = false;
        img.rectTransform.position = position;

        img.transform.SetParent(battleManager.GetCurrentBattler().transform.parent.parent);

        img.rectTransform.DOMove(targetPosition, animTime, true).SetEase(Ease.Linear);

        Destroy(img.gameObject, animTime);
    }
    #endregion common methods

    #region abilities
    public void DeepBreath()
    {
        var target = battleManager.GetCurrentBattler();
        int healAmount = (int)((float)target.max_hp * 0.20f);

        // 技名を表示
        var floatingText = CreateFloatingText(target.transform);
        string abilityName = LocalizationManager.Localize("Ability.DeepBreath");
        floatingText.Init(2.0f, target.GetMiddleGlobalPosition() + new Vector2(0.0f, target.GetCharacterSize().y * 0.25f), new Vector2(0.0f, 100.0f), abilityName, 40, target.character_color);

        var sequence = DOTween.Sequence();
        sequence.AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    // text
                    floatingText = CreateFloatingText(target.transform);
                    floatingText.Init(2.0f, target.GetMiddleGlobalPosition(), new Vector2(0.0f, 100.0f), "+" + healAmount.ToString(), 64, CustomColor.heal());

                    // effect
                    target.Heal(healAmount);

                    // play SE
                    AudioManager.Instance.PlaySFX("Hard-Breathing");

                    // VFX
                    var vfx = VFXSpawner.SpawnVFX("Worm", target.transform, target.GetGraphicRectTransform().position);
                    vfx.GetComponent<Image>().color = CustomColor.invisible();
                    vfx.GetComponent<Image>().DOFade(1.0f, 0.2f);
                    vfx.GetComponent<Image>().DOFade(0.0f, 0.2f).SetDelay(0.5f);
                })
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    battleManager.NextTurn(false);
                });
    }
    public void SuckingTentacle()
    {
        var self = battleManager.GetCurrentBattler();
        var target = targetBattlers[0];

        int suckAmount = (int)((float)target.current_mp * 0.50f);

        // 技名を表示
        var floatingText = CreateFloatingText(self.transform);
        string abilityName = LocalizationManager.Localize("Ability.SuckingTentacle");
        floatingText.Init(2.0f, self.GetMiddleGlobalPosition() + new Vector2(0.0f, self.GetCharacterSize().y * 0.25f), new Vector2(0.0f, 100.0f), abilityName, 40, self.character_color);

        // キャラ移動の準備
        Transform originalParent = self.transform.parent;
        int originalChildIndex = self.transform.GetSiblingIndex();

        var targetPos = target.GetComponent<RectTransform>().position;
        targetPos = target.isEnemy ? new Vector2(targetPos.x - target.GetCharacterSize().x * 0.15f, targetPos.y) : new Vector2(targetPos.x + target.GetCharacterSize().x * 0.15f, targetPos.y);
        var originalPos = self.GetComponent<RectTransform>().position;
        const float characterMoveTime = 0.35f;
        const float animationTime = 1f;

        AudioSource audio = AudioManager.Instance.GetSFXSource();

        var sequence = DOTween.Sequence();
        sequence.AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    // play SE
                    AudioManager.Instance.PlaySFX("CharacterMove", 0.1f);

                    // move
                    self.GetComponent<RectTransform>().DOMove(targetPos, characterMoveTime);
                })
                .AppendInterval(characterMoveTime * 0.5f)
                .AppendCallback(() =>
                {
                    // change character hirachy temporary
                    self.transform.SetParent(target.transform);
                })
                .AppendInterval(characterMoveTime * 0.5f)
                .AppendCallback(() =>
                {
                    self.PlayAnimation(BattlerAnimationType.magic);
                    target.PlayAnimation(BattlerAnimationType.attacked);

                    self.Shake(animationTime);
                    target.Shake(animationTime);

                    // suck sp
                    self.AddSP(suckAmount);
                    target.DeductSP(suckAmount);

                    // SE
                    audio = AudioManager.Instance.PlaySFX("Tentacle");

                    // text
                    floatingText = CreateFloatingText(target.transform);
                    floatingText.Init(2.0f, self.GetMiddleGlobalPosition(), new Vector2(0.0f, 100.0f), "+" + suckAmount.ToString(), 64, CustomColor.SP());
                })
                .AppendInterval(animationTime)
                .AppendCallback(() =>
                {
                    self.PlayAnimation(BattlerAnimationType.idle);
                    target.PlayAnimation(BattlerAnimationType.idle);

                    self.GetComponent<RectTransform>().DOMove(originalPos, characterMoveTime);

                    if (audio) audio.Stop();
                })
                .AppendInterval(characterMoveTime * 0.5f)
                .AppendCallback(() =>
                {
                    // return to original parent
                    self.transform.SetParent(originalParent);
                    self.transform.SetSiblingIndex(originalChildIndex);
                })
                .AppendInterval(characterMoveTime * 0.5f)
                .AppendCallback(() =>
                {
                    battleManager.NextTurn(false);
                });
    }

    public void SelfRepair()
    {
        var target = battleManager.GetCurrentBattler();
        int healAmount = (int)((float)target.max_hp * 0.25f);

        // 技名を表示
        var floatingText = CreateFloatingText(target.transform);
        string abilityName = LocalizationManager.Localize("Ability.SelfRepair");
        floatingText.Init(2.0f, target.GetMiddleGlobalPosition() + new Vector2(0.0f, target.GetCharacterSize().y * 0.25f), new Vector2(0.0f, 100.0f), abilityName, 40, target.character_color);

        var sequence = DOTween.Sequence();
        sequence.AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    // text
                    floatingText = CreateFloatingText(target.transform);
                    floatingText.Init(2.0f, target.GetMiddleGlobalPosition(), new Vector2(0.0f, 100.0f), "+" + healAmount.ToString(), 64, CustomColor.heal());

                    // effect
                    target.Heal(healAmount);

                    // play SE
                    AudioManager.Instance.PlaySFX("SelfRepair");

                    // VFX
                    VFXSpawner.SpawnVFX("SelfRepair", target.transform, target.GetGraphicRectTransform().position);
                })
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    battleManager.NextTurn(false);
                });
    }

    /// <summary>
    /// 明穂戦特殊技
    /// </summary>
    public void HealAttack()
    {
        const int damage = 30;

        var self = battleManager.GetCurrentBattler();
        var target = targetBattlers[0];

        // 技名を表示
        var floatingText = CreateFloatingText(target.transform);
        string abilityName = LocalizationManager.Localize("Ability.HealAttack");
        floatingText.Init(2.0f, target.GetMiddleGlobalPosition() + new Vector2(0.0f, target.GetCharacterSize().y * 0.25f), new Vector2(0.0f, 100.0f), abilityName, 40, target.character_color);

        // エフェクト (Holy)
        VFXSpawner.SpawnVFX("Holy", self.transform, self.GetGraphicRectTransform().position + new Vector3(0.0f, self.GetCharacterSize().y, 0.0f));
        self.PlayAnimation(BattlerAnimationType.magic);

        var sequence = DOTween.Sequence();
        sequence.AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    // エフェクト (Light)
                    VFXSpawner.SpawnVFX("Light", target.transform, target.GetGraphicRectTransform().position + new Vector3(0.0f, target.GetCharacterSize().y, 0.0f));
                    
                    // text
                    floatingText = CreateFloatingText(self.transform);
                    floatingText.Init(2.0f, self.GetMiddleGlobalPosition(), new Vector2(0.0f, 100.0f), "+" + damage.ToString(), 64, CustomColor.heal());
                    floatingText = CreateFloatingText(target.transform);
                    floatingText.Init(2.0f, target.GetMiddleGlobalPosition(), (target.GetMiddleGlobalPosition() - self.GetMiddleGlobalPosition()) + new Vector2(0.0f, 100.0f), damage.ToString(), 64, CustomColor.damage());
                    
                    // effect
                    target.DeductHP(damage);
                    self.Heal(damage);
                    
                    // animation
                    target.Shake(0.75f);
                    self.PlayAnimation(BattlerAnimationType.idle);
                    target.PlayAnimation(BattlerAnimationType.attacked);
                })
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    target.PlayAnimation(BattlerAnimationType.idle);
                    battleManager.NextTurn(false);
                });
    }

    /// <summary>
    /// 明穂戦特殊技
    /// </summary>
    public void GreatRegen()
    {
        var self = battleManager.GetCurrentBattler();
        var target = targetBattlers[0];

        float percentage = Random.Range(0.05f, 0.15f);
        int hpHealAmount = Mathf.RoundToInt((float)target.max_hp * percentage);
        int spHealAmount = Mathf.RoundToInt((float)target.max_mp * percentage);
        
        // 技名を表示
        var floatingText = CreateFloatingText(target.transform);
        string abilityName = LocalizationManager.Localize("Ability.GreatRegen");
        floatingText.Init(2.0f, target.GetMiddleGlobalPosition() + new Vector2(0.0f, target.GetCharacterSize().y * 0.25f), new Vector2(0.0f, 100.0f), abilityName, 40, target.character_color);

        // エフェクト (Holy)
        VFXSpawner.SpawnVFX("Holy", self.transform, self.GetGraphicRectTransform().position + new Vector3(0.0f, self.GetCharacterSize().y, 0.0f));
        self.PlayAnimation(BattlerAnimationType.magic);

        var sequence = DOTween.Sequence();
        sequence.AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    // animation
                    self.PlayAnimation(BattlerAnimationType.idle);

                    // エフェクト (Light)
                    VFXSpawner.SpawnVFX("Light", target.transform, target.GetGraphicRectTransform().position + new Vector3(0.0f, target.GetCharacterSize().y, 0.0f));

                    // text
                    floatingText = CreateFloatingText(self.transform);
                    floatingText.Init(2.0f, self.GetMiddleGlobalPosition() + new Vector2(-50.0f, 0.0f), new Vector2(0.0f, 100.0f), "+" + hpHealAmount.ToString(), 64, CustomColor.heal());
                    floatingText = CreateFloatingText(target.transform);
                    floatingText.Init(2.0f, target.GetMiddleGlobalPosition() + new Vector2(50.0f, 0.0f), new Vector2(0.0f, 100.0f), "+" + spHealAmount.ToString(), 64, CustomColor.SP());

                    // effect
                    target.Heal(hpHealAmount);
                    target.AddSP(spHealAmount);
                    battleManager.AddBuffToBattler(target, BuffType.heal, 3, hpHealAmount);
                })
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    battleManager.NextTurn(false);
                });
    }
    #endregion abilities
}
