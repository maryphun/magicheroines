using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Assets.SimpleLocalization.Scripts;

[RequireComponent(typeof(CanvasGroup))]
public class TrainPanel : MonoBehaviour
{
    // シナリオ種類
    private enum ScenarioType
    {
        Horny, // 淫乱化
        Corruption, // 闇落ち
        CoreResearch, // 聖核研究
    }

    [Header("Setting")]
    [SerializeField, Range(0.0f, 1.0f)] private float animationTime = 0.5f;

    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image characterImg;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private TMP_Text currentMood;
    [SerializeField] private Image darkGaugeFill, holyCoreGaugeFill;
    [SerializeField] private RectTransform previousCharacterBtn, nextCharacterBtn;
    [SerializeField] private Button hornyActionBtn, corruptActionBtn, coreBtn;
    [SerializeField] private TMP_Text hornyActionCost, corruptActionCost, coreCost;
    [SerializeField] private GameObject unavailablePanel;
    [SerializeField] private CanvasGroup underDevelopmentPopUp;
    [SerializeField] private CanvasGroup newBattlerPopup;
    [SerializeField] private TMPro.TMP_Text newBattlerPopupText;
    [SerializeField] private CanvasGroup newCoreEquipmentPopup;
    [SerializeField] private TMPro.TMP_Text newCoreEquipmentText;
    [SerializeField] private Image newCoreEquipmentIcon;
    [SerializeField] private CanvasGroup researchPointPanel;
    [SerializeField] private HomeCharacter homeCharacterScript;

    [Header("Debug")]
    [SerializeField] private Vector2 previousCharacterBtnPos, nextCharacterBtnPos;
    [SerializeField] private List<Character> characters;
    [SerializeField] private int currentIndex;
    [SerializeField] private int[] cost = new int[3];
    [SerializeField] private ScenarioType currentScenarioType;

    [Header("調教シーン内容管理")]
    // 闇落ちシーン
    [SerializeField] public Dictionary<int, List<string>> CharacterID_To_HornyNovelNameList = new Dictionary<int, List<string>>();     // 淫乱化
    [SerializeField] public Dictionary<int, List<string>> CharacterID_To_BrainwashNovelNameList = new Dictionary<int, List<string>>(); // 洗脳
    [SerializeField] public Dictionary<int, List<string>> CharacterID_To_CoreNovelNameList = new Dictionary<int, List<string>>(); // 聖核研究

    void InitList()
    {
        // 淫乱化シナリオリスト
        CharacterID_To_HornyNovelNameList.Add(3, new List<string> { "Akiho/Horny_1", "Akiho/Horny_2", "Akiho/Horny_3" });
        CharacterID_To_HornyNovelNameList.Add(4, new List<string> { "Rikka/Horny_1", "Rikka/Horny_2", "Rikka/Horny_3" });
        CharacterID_To_HornyNovelNameList.Add(5, new List<string> { "Erena/Horny_1", "Erena/Horny_2", "Erena/Horny_3" });
        CharacterID_To_HornyNovelNameList.Add(6, new List<string> { "Kei/Horny_1", "Kei/Horny_2", "Kei/Horny_3" });
        CharacterID_To_HornyNovelNameList.Add(7, new List<string> { "Nayuta/Horny_1", "Nayuta/Horny_2", "Nayuta/Horny_3" });
        // 洗脳シナリオリスト
        CharacterID_To_BrainwashNovelNameList.Add(3, new List<string> { "Akiho/BrainWash_1", "Akiho/BrainWash_2", "Akiho/BrainWash_3" });
        CharacterID_To_BrainwashNovelNameList.Add(4, new List<string> { "Rikka/BrainWash_1", "Rikka/BrainWash_2", "Rikka/BrainWash_3" });
        CharacterID_To_BrainwashNovelNameList.Add(5, new List<string> { "Erena/BrainWash_1", "Erena/BrainWash_2", "Erena/BrainWash_3" });
        CharacterID_To_BrainwashNovelNameList.Add(6, new List<string> { "Kei/BrainWash_1", "Kei/BrainWash_2", "Kei/BrainWash_3" });
        CharacterID_To_BrainwashNovelNameList.Add(7, new List<string> { "Nayuta/BrainWash_1", "Nayuta/BrainWash_2", "Nayuta/BrainWash_3" });
        // 聖核研究シナリオリスト
        CharacterID_To_CoreNovelNameList.Add(3, new List<string> { "Akiho/Core_1", "Akiho/Core_2", "Akiho/Core_3" });
        CharacterID_To_CoreNovelNameList.Add(4, new List<string> { "Rikka/Core_1", "Rikka/Core_2", "Rikka/Core_3" });
        CharacterID_To_CoreNovelNameList.Add(5, new List<string> { "Erena/Core_1", "Erena/Core_2", "Erena/Core_3" });
        CharacterID_To_CoreNovelNameList.Add(6, new List<string> { "Kei/Core_1", "Kei/Core_2", "Kei/Core_3" });
        CharacterID_To_CoreNovelNameList.Add(7, new List<string> { "Nayuta/Core_1", "Nayuta/Core_2", "Nayuta/Core_3" });
    }

    private void Awake()
    {
        InitList();
    }

    public void OpenTrainPanel()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemOpen");

        // UI フェイド
        canvasGroup.DOFade(1.0f, animationTime);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // 初期化
        previousCharacterBtn.gameObject.SetActive(true);
        nextCharacterBtn.gameObject.SetActive(true);
        previousCharacterBtnPos = previousCharacterBtn.localPosition;
        nextCharacterBtnPos = nextCharacterBtn.localPosition;

        // データをロード
        characters = ProgressManager.Instance.GetAllCharacter(false);

        // ヒロインじゃないキャラを排除
        currentIndex = 0;
        characters.RemoveAll(s => !s.characterData.is_heroin);

        if (characters.Count <= 0)
        {
            // 捕獲したヒロインがいない
            unavailablePanel.SetActive(true);
            return;
        }
        else
        {
            characterImg.gameObject.SetActive(true);

            if (characters.Count == 1)
            {
                // 1人しかいない
                previousCharacterBtn.gameObject.SetActive(false);
                nextCharacterBtn.gameObject.SetActive(false);
            }
        }
        UpdateCharacterData();
    }

    public void QuitTrainPanel()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemCancel");

        canvasGroup.DOFade(0.0f, animationTime);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // データをクリア
        characters.Clear();
        characters = null;
    }

    public void NextCharacter()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemSwitch");

        // UI Animation
        const float animTime = 0.2f;
        DOTween.Sequence()
            .AppendCallback(() =>
            {
                nextCharacterBtn.DOLocalMoveX(nextCharacterBtnPos.x + 10.0f, animTime * 0.5f);
            })
            .AppendInterval(animTime * 0.5f)
            .AppendCallback(() =>
            {
                nextCharacterBtn.DOLocalMoveX(nextCharacterBtnPos.x, animTime * 0.5f);
            });

        // Calculate Index
        currentIndex++;
        if (currentIndex >= characters.Count)
        {
            currentIndex = 0;
        }

        // Change character
        UpdateCharacterData();
    }

    public void PreviousCharacter()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemSwitch");

        // UI Animation
        const float animTime = 0.2f;
        DOTween.Sequence()
            .AppendCallback(() => 
            { 
                previousCharacterBtn.DOLocalMoveX(previousCharacterBtnPos.x - 10.0f, animTime * 0.5f); 
            })
            .AppendInterval(animTime * 0.5f)
            .AppendCallback(() => 
            {
                previousCharacterBtn.DOLocalMoveX(previousCharacterBtnPos.x, animTime * 0.5f);
            });

        // Calculate Index
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = characters.Count-1;
        }

        // Change character
        UpdateCharacterData();
    }

    public void UpdateCharacterData()
    {
        // 名前
        characterName.text = characters[currentIndex].localizedName;

        // 条件を満たしている「心情」
        var characterStatus = characters[currentIndex].GetCurrentStatus();
        currentMood.text = LocalizationManager.Localize(characterStatus.moodNameID);
        characterImg.sprite = characterStatus.character;

        darkGaugeFill.fillAmount = ((float)characters[currentIndex].corruptionEpisode) / (float)CharacterID_To_BrainwashNovelNameList[characters[currentIndex].characterData.characterID].Count;
        holyCoreGaugeFill.fillAmount = ((float)characters[currentIndex].holyCoreEpisode) / (float)CharacterID_To_CoreNovelNameList[characters[currentIndex].characterData.characterID].Count;

        // ボタンを更新 (Has next scenario?)
        hornyActionBtn.interactable = characters[currentIndex].hornyEpisode < CharacterID_To_HornyNovelNameList[characters[currentIndex].characterData.characterID].Count;
        corruptActionBtn.interactable = characters[currentIndex].corruptionEpisode < CharacterID_To_BrainwashNovelNameList[characters[currentIndex].characterData.characterID].Count;
        coreBtn.interactable = characters[currentIndex].holyCoreEpisode < CharacterID_To_CoreNovelNameList[characters[currentIndex].characterData.characterID].Count;

        // 淫乱化
        if (!hornyActionBtn.interactable)
        {
            // (調教完了)
            hornyActionCost.text = "<color=#d400ff><size=25>" + LocalizationManager.Localize("System.ResearchComplete");
        }
        else
        {
            cost[0] = (characters[currentIndex].hornyEpisode * 50) + 50;
            hornyActionCost.text = LocalizationManager.Localize("System.ResearchPointCost") + "\n<size=25><color=#ed94ff>" + cost[0];

            // ポイント不足
            if (ProgressManager.Instance.GetCurrentResearchPoint() < cost[0]) hornyActionBtn.interactable = false;
        }

        // 闇落ち
        if (!corruptActionBtn.interactable)
        {
            // (調教完了)
            corruptActionCost.text = "<color=#d400ff><size=25>" + LocalizationManager.Localize("System.ResearchComplete");
        }
        else
        {
            cost[1] = (characters[currentIndex].corruptionEpisode * 50) + 50;
            corruptActionCost.text = LocalizationManager.Localize("System.ResearchPointCost") + "\n<size=25><color=#ed94ff>" + cost[1];

            // ポイント不足
            if (ProgressManager.Instance.GetCurrentResearchPoint() < cost[1]) corruptActionBtn.interactable = false;
        }

        // 聖核研究
        if (!coreBtn.interactable)
        {
            // (調教完了)
            coreCost.text = "<color=#d400ff><size=25>" + LocalizationManager.Localize("System.ResearchComplete");
        }
        else
        {
            cost[2] = (characters[currentIndex].holyCoreEpisode * 50) + 50;
            coreCost.text = LocalizationManager.Localize("System.ResearchPointCost") + "\n<size=25><color=#ed94ff>" + cost[2];

            // ポイント不足
            if (ProgressManager.Instance.GetCurrentResearchPoint() < cost[2]) coreBtn.interactable = false;
        }
    }

    /// <summary>
    /// 淫乱調教
    /// </summary>
    public void HornyTraining()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemTrainPanel");

        // BGM 停止
        AudioManager.Instance.PauseMusic();

        List<string> episodeList = CharacterID_To_HornyNovelNameList[characters[currentIndex].characterData.characterID];
        currentScenarioType = ScenarioType.Horny;

        // シナリオ再生
        canvasGroup.interactable = false;

        // ポイント消費
        ProgressManager.Instance.SetResearchPoint(ProgressManager.Instance.GetCurrentResearchPoint() - cost[0]);

        // 画面遷移
        AlphaFadeManager.Instance.FadeOut(0.5f);
        DOTween.Sequence().AppendInterval(0.6f).AppendCallback(() => 
        {
            AlphaFadeManager.Instance.FadeIn(0.5f);
            NovelSingletone.Instance.PlayNovel("TrainScene/" + episodeList[characters[currentIndex].hornyEpisode], true, ReturnFromEpisode);
            characters[currentIndex].hornyEpisode++;
        });
    }

    /// <summary>
    /// 洗脳調教
    /// </summary>
    public void BrainwashTraining()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemTrainPanel");
        // BGM 停止
        AudioManager.Instance.PauseMusic();

        List<string> episodeList = CharacterID_To_BrainwashNovelNameList[characters[currentIndex].characterData.characterID];
        currentScenarioType = ScenarioType.Corruption;

        // シナリオ再生
        AlphaFadeManager.Instance.FadeOut(1.0f);

        // ポイント消費
        ProgressManager.Instance.SetResearchPoint(ProgressManager.Instance.GetCurrentResearchPoint() - cost[1]);

        // 画面遷移
        AlphaFadeManager.Instance.FadeOut(0.5f);
        DOTween.Sequence().AppendInterval(0.6f).AppendCallback(() =>
        {
            AlphaFadeManager.Instance.FadeIn(0.5f);
            NovelSingletone.Instance.PlayNovel("TrainScene/" + episodeList[characters[currentIndex].corruptionEpisode], true, ReturnFromEpisode);
            characters[currentIndex].corruptionEpisode++;
        });

        // キャラ獲得?
        if (characters[currentIndex].corruptionEpisode >= episodeList.Count - 1)
        {
            // 闇落ち
            characters[currentIndex].is_corrupted = true;
        }
    }

    /// <summary>
    /// 聖核研究
    /// </summary>
    public void HolyCoreResearch()
    {
        // SE 再生
        AudioManager.Instance.PlaySFX("SystemTrainPanel");

        // BGM 停止
        AudioManager.Instance.PauseMusic();

        List<string> episodeList = CharacterID_To_CoreNovelNameList[characters[currentIndex].characterData.characterID];
        currentScenarioType = ScenarioType.CoreResearch;

        // シナリオ再生
        AlphaFadeManager.Instance.FadeOut(1.0f);

        // ポイント消費
        ProgressManager.Instance.SetResearchPoint(ProgressManager.Instance.GetCurrentResearchPoint() - cost[2]);

        // 画面遷移
        AlphaFadeManager.Instance.FadeOut(0.5f);
        DOTween.Sequence().AppendInterval(0.6f).AppendCallback(() =>
        {
            AlphaFadeManager.Instance.FadeIn(0.5f);
            NovelSingletone.Instance.PlayNovel("TrainScene/" + episodeList[characters[currentIndex].holyCoreEpisode], true, ReturnFromEpisode);
            characters[currentIndex].holyCoreEpisode++;
        });
    }

    private void ReturnFromEpisode()
    {
        switch (currentScenarioType)
        {
            case ScenarioType.Horny:
                // 淫乱化
                if (characters[currentIndex].hornyEpisode >= CharacterID_To_HornyNovelNameList[characters[currentIndex].characterData.characterID].Count - 1)
                {
                    // 特殊技獲得

                }
                break;
            case ScenarioType.Corruption:
                // update heroin data
                if (characters[currentIndex].is_corrupted)
                {
                    CallNewBattlerPopUp();
                    AddNewHomeCharacter(characters[currentIndex].characterData.characterID);

                    // update character name
                    characters[currentIndex].localizedName = LocalizationManager.Localize(characters[currentIndex].characterData.corruptedName);
                }
                break;
            case ScenarioType.CoreResearch:
                // 淫乱化
                if (characters[currentIndex].holyCoreEpisode >= CharacterID_To_CoreNovelNameList[characters[currentIndex].characterData.characterID].Count - 1)
                {
                    // 聖核装備獲得
                    EquipmentDefine newEquipment = characters[currentIndex].characterData.coreEquipment;
                    CallNewCoreEquipmentPopup(newEquipment);
                    ProgressManager.Instance.AddNewEquipment(newEquipment);
                }
                break;
            default:
                break;
        }

        canvasGroup.interactable = true;
        UpdateCharacterData();

        // Play home music
        AudioManager.Instance.PlayMusicWithFade("Loop 32 (HomeScene)", 4.0f);

        // オートセーブを実行する
        AutoSave.ExecuteAutoSave();
    }

    public void CloseUnderDevelopmentPopup()
    {
        underDevelopmentPopUp.DOKill(false);
        underDevelopmentPopUp.DOFade(0.0f, 0.1f).OnComplete(() =>
        { 
            underDevelopmentPopUp.interactable = false;
            underDevelopmentPopUp.blocksRaycasts = false;
        });
    }

    public void CallNewBattlerPopUp()
    {
        // SE
        AudioManager.Instance.PlaySFX("SystemNewHeroin");

        // Update Text
        newBattlerPopupText.text = CorruptedMessage(characters[currentIndex].characterData.characterID);

        // UI
        newBattlerPopup.DOKill(false);
        newBattlerPopup.DOFade(1.0f, 0.5f);
        newBattlerPopup.interactable = true;
        newBattlerPopup.blocksRaycasts = true;
    }
    public void CloseNewBattlerPopup()
    {
        newBattlerPopup.DOKill(false);
        newBattlerPopup.DOFade(0.0f, 0.1f).OnComplete(() =>
        {
            newBattlerPopup.interactable = false;
            newBattlerPopup.blocksRaycasts = false;
        });
    }

    public void CallNewCoreEquipmentPopup(EquipmentDefine newEquipment)
    {
        // SE
        AudioManager.Instance.PlaySFX("SystemEquip");

        // Update Text
        newCoreEquipmentText.text = CoreEquipmentMessage(characters[currentIndex].characterData.characterID, newEquipment);
        newCoreEquipmentIcon.sprite = newEquipment.Icon;

        // UI
        newCoreEquipmentPopup.DOKill(false);
        newCoreEquipmentPopup.DOFade(1.0f, 0.5f);
        newCoreEquipmentPopup.interactable = true;
        newCoreEquipmentPopup.blocksRaycasts = true;
    }
    public void CloseNewCoreEquipmentPopup()
    {
        newCoreEquipmentPopup.DOKill(false);
        newCoreEquipmentPopup.DOFade(0.0f, 0.1f).OnComplete(() =>
        {
            newCoreEquipmentPopup.interactable = false;
            newCoreEquipmentPopup.blocksRaycasts = false;
        });
    }

    /// <summary>
    /// 闇落ち成功のシステムメッセージ
    /// </summary>
    public string CorruptedMessage(int characterID)
    {
        string s = string.Empty;
        switch ((PlayerCharacerID)characterID)
        {
            case PlayerCharacerID.Akiho: // 明穂
                s = CustomColor.AddColor(LocalizationManager.Localize("Name.Akiho"), CustomColor.akiho());
                return LocalizationManager.Localize("System.NewBattler").Replace("{s}", s);
            case PlayerCharacerID.Rikka: // 立花
                s = CustomColor.AddColor(LocalizationManager.Localize("Name.Rikka"), CustomColor.rikka());
                return LocalizationManager.Localize("System.NewBattler").Replace("{s}", s);
            case PlayerCharacerID.Erena: // エレナ
                s = CustomColor.AddColor(LocalizationManager.Localize("Name.Erena"), CustomColor.erena());
                return LocalizationManager.Localize("System.NewBattler").Replace("{s}", s);
            case PlayerCharacerID.Kei: // 京
                s = CustomColor.AddColor(LocalizationManager.Localize("Name.Kei"), CustomColor.kei());
                return LocalizationManager.Localize("System.NewBattler").Replace("{s}", s);
            case PlayerCharacerID.Nayuta: // 那由多
                s = CustomColor.AddColor(LocalizationManager.Localize("Name.Nayuta"), CustomColor.nayuta());
                return LocalizationManager.Localize("System.NewBattler").Replace("{s}", s);
            default:
                return string.Empty;
        }
    }

    /// <summary>
    /// 聖核装備獲得システムメッセージ
    /// </summary>
    public string CoreEquipmentMessage(int characterID, EquipmentDefine equipment)
    {
        string s = string.Empty;
        switch ((PlayerCharacerID)characterID)
        {
            case PlayerCharacerID.Akiho: // 明穂
                s = CustomColor.AddColor(LocalizationManager.Localize(equipment.equipNameID), CustomColor.akiho());
                return LocalizationManager.Localize("System.NewCoreEquipment").Replace("{s}", s);
            case PlayerCharacerID.Rikka: // 立花
                s = CustomColor.AddColor(LocalizationManager.Localize(equipment.equipNameID), CustomColor.rikka());
                return LocalizationManager.Localize("System.NewCoreEquipment").Replace("{s}", s);
            case PlayerCharacerID.Erena: // エレナ
                s = CustomColor.AddColor(LocalizationManager.Localize(equipment.equipNameID), CustomColor.erena());
                return LocalizationManager.Localize("System.NewCoreEquipment").Replace("{s}", s);
            case PlayerCharacerID.Kei: // 京
                s = CustomColor.AddColor(LocalizationManager.Localize(equipment.equipNameID), CustomColor.kei());
                return LocalizationManager.Localize("System.NewCoreEquipment").Replace("{s}", s);
            case PlayerCharacerID.Nayuta: // 那由多
                s = CustomColor.AddColor(LocalizationManager.Localize(equipment.equipNameID), CustomColor.nayuta());
                return LocalizationManager.Localize("System.NewCoreEquipment").Replace("{s}", s);
            default:
                return string.Empty;
        }
    }

    // ホーム台詞キャラ追加
    public void AddNewHomeCharacter(int characterID)
    {
        switch (characterID)
        {
            case 3: // 明穂
                HomeDialogue akiho = Resources.Load<HomeDialogue>("HomeDialogue/Akiho");
                ProgressManager.Instance.AddHomeCharacter(akiho);
                homeCharacterScript.SetToLastCharacter();
                return;
            case 4: // 立花
                HomeDialogue rikka = Resources.Load<HomeDialogue>("HomeDialogue/Rikka");
                ProgressManager.Instance.AddHomeCharacter(rikka);
                ProgressManager.Instance.RemoveHomeCharacter("No5"); // 立花が闇落ちした後No5をホームキャラから排除
                homeCharacterScript.SetToLastCharacter();
                return;
            case 5: // エレナ
                HomeDialogue erena = Resources.Load<HomeDialogue>("HomeDialogue/Erena");
                ProgressManager.Instance.AddHomeCharacter(erena);
                homeCharacterScript.SetToLastCharacter();
                return;
            case 6: // 京
                HomeDialogue kei = Resources.Load<HomeDialogue>("HomeDialogue/Kei");
                ProgressManager.Instance.AddHomeCharacter(kei);
                homeCharacterScript.SetToLastCharacter();
                return;
            case 7: // 那由多
                HomeDialogue nayuta = Resources.Load<HomeDialogue>("HomeDialogue/Nayuta");
                ProgressManager.Instance.AddHomeCharacter(nayuta);
                homeCharacterScript.SetToLastCharacter();
                return;
            default:
                return;
        }
    }

    public void DisplayResearchPointPanel(bool display)
    {
        researchPointPanel.DOFade(display ? 1.0f: 0.0f, 0.5f);
    }
}
