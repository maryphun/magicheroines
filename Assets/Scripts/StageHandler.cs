using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class StageHandler : MonoBehaviour
{
    const int totalStageNum = 16;

    [Header("Setting")]
    [SerializeField] private float stageIconSpacing = 350.0f;
    [SerializeField] private float arrowOffsetY = 50.0f;

    [Header("References")]
    [SerializeField] private RectTransform rect;
    [SerializeField] private StageArrow stageArrow;
    [SerializeField] private GameObject checkedIcon;
    [SerializeField] private RectTransform[] stageIcon = new RectTransform[totalStageNum];

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        int currentStage = ProgressManager.Instance.GetCurrentStageProgress();

        rect.anchoredPosition = new Vector3((currentStage-1) * (-stageIconSpacing), rect.anchoredPosition.y, 0.0f);

        stageArrow.SetStage(stageIcon[currentStage-1], arrowOffsetY);


        for (int i = 0; i < currentStage-1; i++)
        {
            var obj = Instantiate(checkedIcon, stageIcon[i]);
            obj.GetComponent<RectTransform>().localPosition = Vector3.zero;
            obj.GetComponent<RectTransform>().sizeDelta = stageIcon[i].sizeDelta;
            obj.SetActive(true);
        }
    }
}
