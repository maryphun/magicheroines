using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Assets.SimpleLocalization.Scripts;

[RequireComponent(typeof(CanvasGroup))]
public class CharacterInfoPanel : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float fadeTime = 0.35f;
    [SerializeField] private float hideDelay = 1.0f;

    [Header("References")]
    [SerializeField] private Image characterIcon;
    [SerializeField] private TMP_Text characterLevel;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private TMP_Text characterHPValue;
    [SerializeField] private TMP_Text characterSPValue;
    [SerializeField] private Image characterHPFill;
    [SerializeField] private Image characterSPFill;
    [SerializeField] private CanvasGroup SPBar;

    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Battle battleManager;

    [Header("Debug")]
    [SerializeField] private bool isDisplaying;
    [SerializeField] private Battler currentBattler;
    [SerializeField] private float hideDelayCnt;

    private void Start()
    {
        // init
        isDisplaying = false;
        GetComponent<CanvasGroup>().alpha = 0.0f;
    }

    public void SetCharacter(Battler battler)
    {
        currentBattler = battler;

        characterIcon.sprite = currentBattler.icon;
        characterLevel.text = LocalizationManager.Localize("Battle.Level") + "<space=2em>" + currentBattler.currentLevel;
        characterName.text =  currentBattler.character_name;
        characterHPValue.text = "<size=125%>" + currentBattler.current_hp.ToString() + "</size>/" + currentBattler.max_hp.ToString();
        characterHPFill.fillAmount = (float)currentBattler.current_hp / (float)currentBattler.max_hp;

        if (currentBattler.max_mp > 0)
        {
            SPBar.alpha = 1.0f;
            characterSPValue.text = "<size=125%>" + currentBattler.current_mp.ToString() + "</size>/" + currentBattler.max_mp.ToString();
            characterSPFill.fillAmount = (float)currentBattler.current_mp / (float)currentBattler.max_mp;
        }
        else
        {
            SPBar.alpha = 0.0f;
        }

        isDisplaying = true;
        GetComponent<CanvasGroup>().DOFade(1.0f, fadeTime);
    }

    public void Hide()
    {
        isDisplaying = false;
        GetComponent<CanvasGroup>().DOFade(0.0f, fadeTime);
    }

    private void Update()
    {
        if (isDisplaying)
        {
            // HP Ç∆ SPÇèÓïÒÇèÌÇ…çXêV
            characterHPValue.text = "<size=125%>" + currentBattler.current_hp.ToString() + "</size>/" + currentBattler.max_hp.ToString();
            characterSPValue.text = "<size=125%>" + currentBattler.current_mp.ToString() + "</size>/" + currentBattler.max_mp.ToString();

            characterHPFill.fillAmount = (float)currentBattler.current_hp / (float)currentBattler.max_hp;
            if (currentBattler.max_mp > 0) characterSPFill.fillAmount = (float)currentBattler.current_mp / (float)currentBattler.max_mp;
        }
        

        {
            Vector3 mousePosition = Input.mousePosition / mainCanvas.scaleFactor;
            var obj = (battleManager.GetBattlerByPosition(mousePosition, false, true));
            if (obj != null)
            {
                SetCharacter(obj);
                return;
            }
            else
            {
                hideDelayCnt += Time.deltaTime;
                if (hideDelayCnt > hideDelay)
                {
                    hideDelayCnt = 0.0f;
                    Hide();
                }
            }
        }
    }
}
