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

    public void Initialize(Battle battleManagerScript)
    {
        battleManager = battleManagerScript;
    }

    public void SetTargetBattlers(List<Battler> targets)
    {
        targetBattlers = targets;
    }
    public void SetItemIcon(Sprite icon)
    {
        itemSprite = icon;
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

    // items
    public void OnUseCroissant()
    {
        StartCoroutine(Croissant());
    }

    IEnumerator Croissant()
    {
        // mp+50
        //battleManager.GetCurrentBattler();
        CreateFadingImage(itemSprite, 1.0f);

        yield return new WaitForSeconds(0.5f);

        battleManager.NextTurn(false);
    }

    public void OnUseBread()
    {
        StartCoroutine(Bread());
    }
    IEnumerator Bread()
    {
        // hp+40
        //battleManager.GetCurrentBattler();
        CreateFadingImage(itemSprite, 1.0f);

        yield return new WaitForSeconds(0.5f);

        battleManager.NextTurn(false);
    }
}
