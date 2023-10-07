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
                    floatingText.Init(1.0f, target.GetMiddleGlobalPosition(), new Vector2(0.0f, 100.0f), SPAmount.ToString(), 64, new Color(0.33f, 1f, 0.5f));

                    // effect
                    target.Heal(SPAmount);
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
                    floatingText.Init(1.0f, target.GetMiddleGlobalPosition(), new Vector2(0.0f, 100.0f), healAmount.ToString(), 64, new Color(0.33f, 1f, 0.5f));

                    // effect
                    target.Heal(healAmount);
                })
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    battleManager.NextTurn(false);
                });
    }
    #endregion items
}
