using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.SimpleLocalization.Scripts;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TitleSelectUI : MonoBehaviour
{
    enum TitleSelection
    {
        NewGame,
        Load,
        Gallery,
        Option,
        Credit,
        Exit,
    };

    Dictionary<TitleSelection, string> titleSelectionText = new Dictionary<TitleSelection, string>
    {
        [TitleSelection.NewGame] = "System.NewGame",
        [TitleSelection.Load] = "System.Load",
        [TitleSelection.Gallery] = "System.Gallery",
        [TitleSelection.Option] = "System.Option",
        [TitleSelection.Credit] = "System.Credit",
        [TitleSelection.Exit] = "System.Exit",
    };

    const int SelectionCount = 3; //< 表示する選択肢の数 

    [Header("Setting")]
    [SerializeField] private TitleSelection defaultSelection = TitleSelection.NewGame;
    [SerializeField] private float animationTime = 0.15f;

    [Header("References")]
    [SerializeField] private TMP_Text[] selection = new TMP_Text[SelectionCount];
    [SerializeField] private TMP_Text dummyText;
    [SerializeField] private OptionPanel optionPanel;
    [SerializeField] private SaveLoadPanel saveloadPanel;

    [Header("Debug")]
    [SerializeField] private TitleSelection currentSelection;
    [SerializeField] private bool animationPlaying;
    [SerializeField] private KeyCode keyPrepressed;
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Awake()
    {
        // ゲーム設定をロード
        PlayerPrefsManager.LoadPlayerPrefs();
    }

    private void Init()
    {
        AlphaFadeManager.Instance.FadeIn(1.0f);

        // フラグを初期化
        animationPlaying = false;
        keyPrepressed = KeyCode.None;
        // 選択肢を初期化
        currentSelection = defaultSelection;
        UpdateSelectionText();
        // テーマソング再生
        AudioManager.Instance.PlayMusic("ThemeSong");
    }

    private void UpdateSelectionText()
    {
        selection[0].text = LocalizationManager.Localize(titleSelectionText[GetPreviousSelection(currentSelection)]);
        selection[1].text = LocalizationManager.Localize(titleSelectionText[GetCurrentSelection()]);
        selection[2].text = LocalizationManager.Localize(titleSelectionText[GetNextSelection(currentSelection)]);
    }

    // Update is called once per frame
    private TitleSelection GetPreviousSelection(TitleSelection current)
    {
        if (current <= TitleSelection.NewGame) return TitleSelection.Exit;
        TitleSelection previous = current - 1;
        return previous;
    }

    private TitleSelection GetCurrentSelection()
    {
        return currentSelection;
    }

    private TitleSelection GetNextSelection(TitleSelection current)
    {
        if (current >= TitleSelection.Exit) return TitleSelection.NewGame;
        TitleSelection next = current + 1;
        return next;
    }

    private IEnumerator MoveSelectionDown()
    {
        // SE再生
        AudioManager.Instance.PlaySFX("SystemSelect");

        animationPlaying = true;
        // setup
        Vector3 originalPositionTop    = selection[0].rectTransform.position;
        Vector3 originalPositionMid = selection[1].rectTransform.position;
        Vector3 originalPositionBtm = selection[2].rectTransform.position;

        Vector3 originalSizeTop = selection[0].rectTransform.localScale;
        Vector3 originalSizeMid = selection[1].rectTransform.localScale;
        Vector3 originalSizeBtm = selection[2].rectTransform.localScale;

        Color originalColorTop = selection[0].color;
        Color originalColorMid = selection[1].color;
        Color originalColorBtm = selection[2].color;

        float distance = selection[0].rectTransform.position.y - selection[1].rectTransform.position.y;

        // ダミーを初期化設定
        dummyText.rectTransform.position = new Vector3(originalPositionBtm.x, originalPositionBtm.y - distance, originalPositionBtm.z);
        dummyText.text = LocalizationManager.Localize(titleSelectionText[GetNextSelection(GetNextSelection(currentSelection))]);

        // アニメーション
        selection[0].rectTransform.DOMoveY(originalPositionTop.y + distance, animationTime);
        selection[1].rectTransform.DOMoveY(originalPositionTop.y, animationTime);
        selection[2].rectTransform.DOMoveY(originalPositionMid.y, animationTime);
        dummyText.rectTransform.DOMoveY(originalPositionBtm.y, animationTime);

        selection[1].rectTransform.DOScale(originalSizeTop, animationTime);
        selection[2].rectTransform.DOScale(originalSizeMid, animationTime);
        dummyText.rectTransform.DOScale(originalSizeBtm, animationTime);

        selection[0].DOColor(new Color(0, 0, 0, 0), animationTime);
        selection[1].DOColor(originalColorTop, animationTime);
        selection[2].DOColor(originalColorMid, animationTime);
        dummyText.DOColor(originalColorBtm, animationTime);

        yield return new WaitForSeconds(animationTime);

        //set text
        currentSelection = GetNextSelection(currentSelection);
        UpdateSelectionText();

        // reset
        selection[0].rectTransform.position = originalPositionTop;
        selection[1].rectTransform.position = originalPositionMid;
        selection[2].rectTransform.position = originalPositionBtm;
        selection[0].rectTransform.localScale = originalSizeTop;
        selection[1].rectTransform.localScale = originalSizeMid;
        selection[2].rectTransform.localScale = originalSizeBtm;
        selection[0].color = originalColorTop;
        selection[1].color = originalColorMid;
        selection[2].color = originalColorBtm;
        dummyText.color = new Color(0, 0, 0, 0);

        animationPlaying = false;
    }

    private IEnumerator MoveSelectionUp()
    {
        // SE再生
        AudioManager.Instance.PlaySFX("SystemSelect");

        animationPlaying = true;
        // setup
        Vector3 originalPositionTop = selection[0].rectTransform.position;
        Vector3 originalPositionMid = selection[1].rectTransform.position;
        Vector3 originalPositionBtm = selection[2].rectTransform.position;

        Vector3 originalSizeTop = selection[0].rectTransform.localScale;
        Vector3 originalSizeMid = selection[1].rectTransform.localScale;
        Vector3 originalSizeBtm = selection[2].rectTransform.localScale;

        Color originalColorTop = selection[0].color;
        Color originalColorMid = selection[1].color;
        Color originalColorBtm = selection[2].color;

        float distance = selection[0].rectTransform.position.y - selection[1].rectTransform.position.y;

        // ダミーを初期化設定
        dummyText.rectTransform.position = new Vector3(originalPositionTop.x, originalPositionTop.y + distance, originalPositionTop.z);
        dummyText.text = LocalizationManager.Localize(titleSelectionText[GetPreviousSelection(GetPreviousSelection(currentSelection))]);

        // アニメーション
        selection[0].rectTransform.DOMoveY(originalPositionMid.y, animationTime);
        selection[1].rectTransform.DOMoveY(originalPositionBtm.y, animationTime);
        selection[2].rectTransform.DOMoveY(originalPositionBtm.y - distance, animationTime);
        dummyText.rectTransform.DOMoveY(originalPositionTop.y, animationTime);

        selection[0].rectTransform.DOScale(originalSizeMid, animationTime);
        selection[1].rectTransform.DOScale(originalSizeBtm, animationTime);
        dummyText.rectTransform.DOScale(originalSizeTop, animationTime);

        selection[0].DOColor(originalColorMid, animationTime);
        selection[1].DOColor(originalColorBtm, animationTime);
        selection[2].DOColor(new Color(0, 0, 0, 0), animationTime);
        dummyText.DOColor(originalColorBtm, animationTime);

        yield return new WaitForSeconds(animationTime);

        //set text
        currentSelection = GetPreviousSelection(currentSelection);
        UpdateSelectionText();

        // reset
        selection[0].rectTransform.position = originalPositionTop;
        selection[1].rectTransform.position = originalPositionMid;
        selection[2].rectTransform.position = originalPositionBtm;
        selection[0].rectTransform.localScale = originalSizeTop;
        selection[1].rectTransform.localScale = originalSizeMid;
        selection[2].rectTransform.localScale = originalSizeBtm;
        selection[0].color = originalColorTop;
        selection[1].color = originalColorMid;
        selection[2].color = originalColorBtm;
        dummyText.color = new Color(0, 0, 0, 0);

        animationPlaying = false;
    }

    private IEnumerator ConfirmSelection()
    {
        const float animationTime = 1.0f;

        // SE再生
        AudioManager.Instance.PlaySFX("SystemDecide");

        Vector3 originalSizeMid = selection[1].rectTransform.localScale;
        Color originalColorTop = selection[0].color;
        Color originalColorMid = selection[1].color;
        Color originalColorBtm = selection[2].color;

        // アニメーション
        selection[0].DOFade(0.0f, animationTime);
        selection[2].DOFade(0.0f, animationTime);

        const float selectionScale = 1.5f;
        selection[1].rectTransform.DOScale(new Vector3(selectionScale, selectionScale, selectionScale), animationTime);
        selection[1].DOFade(0.5f, animationTime);

        // シーン遷移
        string sceneToLoad = SceneTransition(currentSelection);
        if (sceneToLoad != string.Empty)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
            asyncLoad.allowSceneActivation = false;

            // BGM停止
            AudioManager.Instance.StopMusicWithFade(animationTime);
            // ニューゲームの場合の特殊処理
            AlphaFadeManager.Instance.FadeOut(animationTime);
            // ニューゲームなので　ゲーム進捗を初期状態にする
            ProgressManager.Instance.InitializeProgress();
            yield return new WaitForSeconds(animationTime);
            while (asyncLoad.progress < 0.9f) yield return null; // wait until the scene is completely loaded 
            asyncLoad.allowSceneActivation = true;
        }

        // complete
        selection[0].DOComplete();
        selection[1].DOComplete();
        selection[2].DOComplete();
        selection[0].rectTransform.DOComplete();
        selection[1].rectTransform.DOComplete();
        selection[2].rectTransform.DOComplete();

        // reset
        selection[1].rectTransform.DOScale(originalSizeMid, 0.0f);
        selection[0].color = originalColorTop;
        selection[1].color = originalColorMid;
        selection[2].color = originalColorBtm;
    }

    private void Update()
    {
        if (optionPanel.IsOpen()) return;
        if (saveloadPanel.IsOpen) return;

        // Todo: Use Input manager instead
        if (Input.GetKeyDown(KeyCode.UpArrow)) keyPrepressed = KeyCode.UpArrow;
        if (Input.GetKeyDown(KeyCode.DownArrow)) keyPrepressed = KeyCode.DownArrow;
        if (Input.GetKeyDown(KeyCode.Return)) keyPrepressed = KeyCode.Return;

        // mouse scroll
        if (Input.mouseScrollDelta.y > 0.0f) keyPrepressed = KeyCode.UpArrow;
        if (Input.mouseScrollDelta.y < 0.0f) keyPrepressed = KeyCode.DownArrow;

        if (!animationPlaying)
        {
            switch (keyPrepressed)
            {
                case KeyCode.UpArrow:
                    keyPrepressed = KeyCode.None;
                    StartCoroutine(MoveSelectionUp());
                    break;
                case KeyCode.DownArrow:
                    keyPrepressed = KeyCode.None;
                    StartCoroutine(MoveSelectionDown());
                    break;
                case KeyCode.Return:
                    keyPrepressed = KeyCode.None;
                    StartCoroutine(ConfirmSelection());
                    return;

                default:
                    return;

            }
        }
    }

    public void MouseClickSelect()
    {
        keyPrepressed = KeyCode.Return;
    }
    public void MouseClickUp()
    {
        Debug.Log("Clicked up");
        keyPrepressed = KeyCode.UpArrow;
    }
    public void MouseClickDown()
    {
        keyPrepressed = KeyCode.DownArrow;
    }

    private string SceneTransition(TitleSelection targetScene)
    {
        switch (targetScene)
        {
            case TitleSelection.Credit:
                return "Credit";
            case TitleSelection.Exit:
                Application.Quit();
                return string.Empty;
            case TitleSelection.Gallery:
                Init();
                return string.Empty;
            case TitleSelection.Load:
                saveloadPanel.OpenSaveLoadPanel(true);
                return string.Empty;
            case TitleSelection.NewGame:
                return "Tutorial";
            case TitleSelection.Option:
                optionPanel.OpenOptionPanel();
                return string.Empty;
            default:
                return string.Empty;
        }
    }
}
