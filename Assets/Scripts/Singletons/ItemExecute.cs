using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ItemExecute : SingletonMonoBehaviour<ItemExecute>
{
    [Header("References")]
    [SerializeField] private Battle battleManager;
    [SerializeField] private Sprite itemSprite;
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

    public void SetItemIcon(Sprite icon)
    {
        itemSprite = icon;
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

    #region items
    public void OnUseCroissant()
    {
        const int SPAmount = 50;

        var sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            // create image
            CreateFadingImage(itemSprite, 1.0f);
        })
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    // hp+40
                    var target = battleManager.GetCurrentBattler();

                    // text
                    var floatingText = CreateFloatingText(target.transform);
                    floatingText.Init(2.0f, target.GetMiddleGlobalPosition(), new Vector2(0.0f, 100.0f), "+" + SPAmount.ToString(), 64, new Color(0.33f, 1f, 0.5f));

                    // effect
                    target.Heal(SPAmount);

                    // play SE
                    AudioManager.Instance.PlaySFX("Heal");
                })
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    battleManager.NextTurn(false);
                });
    }

    public void OnUseBread()
    {
        const int healAmount = 40;

        var sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
                {
                    // create image
                    CreateFadingImage(itemSprite, 1.0f);
                })
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    // hp+40
                    var target = battleManager.GetCurrentBattler();

                    // text
                    var floatingText = CreateFloatingText(target.transform);
                    floatingText.Init(2.0f, target.GetMiddleGlobalPosition(), new Vector2(0.0f, 100.0f), "+"+healAmount.ToString(), 64, new Color(0.33f, 1f, 0.5f));

                    // effect
                    target.Heal(healAmount);

                    // play SE
                    AudioManager.Instance.PlaySFX("Heal");
                })
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    battleManager.NextTurn(false);
                });
    }

    public void OnUseThrowingKnife()
    {
        const int damage = 50;
        var target = targetBattlers[0];

        var sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            // play SE
            AudioManager.Instance.PlaySFX("Air", 0.5f);

            // create image
            CreateMovingImage(itemSprite, targetBattlers[0].GetMiddleGlobalPosition(), 0.8f);
        })
                .AppendInterval(0.8f)
                .AppendCallback(() =>
                {
                    // text
                    var floatingText = CreateFloatingText(target.transform);
                    floatingText.Init(2.0f, target.GetMiddleGlobalPosition(), (target.GetMiddleGlobalPosition() - battleManager.GetCurrentBattler().GetMiddleGlobalPosition()) + new Vector2(0.0f, 100.0f), damage.ToString(), 64, new Color(1f, 0.75f, 0.33f));

                    // effect
                    target.DeductHP(damage, true);

                    // animation
                    target.PlayAnimation(BattlerAnimationType.attacked);

                    // shake enemy
                    target.Shake(0.4f);

                    // play SE
                    AudioManager.Instance.PlaySFX("KnifeAttack");

                    // VFX
                    VFXSpawner.SpawnVFX("BloodImpact", target.transform, target.GetMiddleGlobalPosition());
                })
                .AppendInterval(0.2f)
                .AppendCallback(() =>
                {
                    // animation
                    target.PlayAnimation(BattlerAnimationType.idle);
                })
                .AppendInterval(0.3f)
                .AppendCallback(() =>
                {
                    battleManager.NextTurn(false);
                });
    }

    public void OnUseFirstAid()
    {
        // HP 10~100
        int healAmount = SeriouslyRandom.Next(10, 100);
        var target = targetBattlers[0];

        float animTime = 0.8f;

        // Ž©g‚ÉŽg‚Á‚½
        if (target == battleManager.GetCurrentBattler())
        {
            animTime = 0.5f;
        }

        var sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            // play SE
            AudioManager.Instance.PlaySFX("Air", 0.5f);

            // create image
            if (target != battleManager.GetCurrentBattler())
            {
                CreateMovingImage(itemSprite, targetBattlers[0].GetMiddleGlobalPosition(), animTime);
            }
            else
            {
                CreateFadingImage(itemSprite, 1.0f);
            }
        })
                .AppendInterval(animTime)
                .AppendCallback(() =>
                {
                    // text
                    var floatingText = CreateFloatingText(target.transform);
                    floatingText.Init(2.0f, target.GetMiddleGlobalPosition(), new Vector2(0.0f, 100.0f), "+" + healAmount.ToString(), 64, new Color(0.33f, 1f, 0.5f));

                    // effect
                    target.Heal(healAmount);

                    // play SE
                    AudioManager.Instance.PlaySFX("Heal");
                })
                .AppendInterval(0.2f)
                .AppendCallback(() =>
                {
                    // animation
                    target.PlayAnimation(BattlerAnimationType.idle);
                })
                .AppendInterval(0.3f)
                .AppendCallback(() =>
                {
                    battleManager.NextTurn(false);
                });
    }
    #endregion items
}
