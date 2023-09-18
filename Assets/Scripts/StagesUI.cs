using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StagesUI : MonoBehaviour
{
    public class StageObjectParts
    {
        public Image parent;
        public Image line;
        public Image graphic;
        public Image outline;

        public StageObjectParts(Image _parent, Image _line, Image _graphic, Image _outline)
        {
            parent  = _parent;
            line    = _line;
            graphic = _graphic;
            outline = _outline;
        }
    }

    [Header("Setting")]
    [SerializeField] private Color lockedColor = new Color(0.196f, 0.196f, 0.196f, 1);
    [SerializeField] private Color unlockedColor = new Color(1,1,1,1);

    [Header("References")]
    [SerializeField] List<GameObject> stageObj = new  List<GameObject>();

    private List<StageObjectParts> stageObjParts = new List<StageObjectParts>();
    private int unlockedStage = 0;

    private void Awake()
    {
        // ステージパーツを初期化
        for (int i = 0; i < stageObj.Count; i++)
        {
            StageObjectParts dummy = new StageObjectParts(stageObj[i].GetComponent<Image>(), 
                                                          stageObj[i].transform.Find("Line").GetComponent<Image>(),
                                                          stageObj[i].transform.Find("Graphic").GetComponent<Image>(),
                                                          stageObj[i].transform.Find("Outline").GetComponent<Image>());

            stageObjParts.Add(dummy);
        }
    }

    private void Start()
    {
        // 進捗を初期化
        int currentStageProgress = ProgressManager.Instance.GetCurrentStageProgress();
        
        for (int i = 0; i < stageObjParts.Count; i++)
        {
            InitializeStage(stageObjParts[i]);
            if (currentStageProgress > i)
            {
                unlockedStage++;
                StartCoroutine(UnlockStage(stageObjParts[i]));
            }
            else
            {
                stageObjParts[i].graphic.color = lockedColor;
            }
        }
    }

    private void InitializeStage(StageObjectParts stage)
    {
        stage.line.fillAmount = 1.0f;
        stage.line.color = new Color(stage.line.color.r, stage.line.color.g, stage.line.color.b, 0.25f);
    }
    
    IEnumerator UnlockStage(StageObjectParts stage, float animationTime = 0.0f)
    {
        GameObject newLine = Instantiate(stage.line.gameObject, stage.parent.transform);
        newLine.name = "NewLine";
        var componentImage = newLine.GetComponent<Image>();
        componentImage.color = new Color(stage.line.color.r, stage.line.color.g, stage.line.color.b, 1.0f);
        componentImage.fillAmount = 0.0f;
        componentImage.DOFillAmount(1.0f, animationTime*0.5f);

        yield return new WaitForSeconds(animationTime*0.5f);

        // unattach
        stage.line.transform.SetParent(transform, true);
        newLine.transform.SetParent(transform, true);

        // shake
        stage.graphic.DOColor(unlockedColor, animationTime*0.5f);
        ShakeManager.Instance.ShakeObject(stage.parent.rectTransform, animationTime * 0.5f, 1f);

        yield return new WaitForSeconds(animationTime * 0.5f);

        stage.line.transform.SetParent(stage.parent.transform, true);
        newLine.transform.SetParent(stage.parent.transform, true);
    }

    public void PlayStageAnimation()
    {
        int currentStageProgress = ProgressManager.Instance.GetCurrentStageProgress();
        Debug.Log("Current Stage Progress = [" + currentStageProgress.ToString() + "], unlockedStage = [" + unlockedStage.ToString() + "]");
        if (currentStageProgress > unlockedStage)
        {
            // アニメーション再生
            for (int i = unlockedStage; i < currentStageProgress; i++)
            {
                StartCoroutine(UnlockStage(stageObjParts[i], 2.0f));
            }
        }
        unlockedStage = currentStageProgress;
    }
}
