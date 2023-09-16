using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class ResourcesValue : MonoBehaviour
{
    [System.Serializable]
    enum ResourcesType
    {
        Money,
        ResourcesPoint,
    }

    [SerializeField] private ResourcesType type;

    private TMP_Text text;

    public void Start()
    {
        text = GetComponent<TMP_Text>();
        UpdateValue();
    }

    private void Update()
    {
        UpdateValue();
    }

    private void UpdateValue()
    {
        this.text.text = type == ResourcesType.Money ? ProgressManager.Instance.GetCurrentMoney().ToString() : ProgressManager.Instance.GetCurrentResearchPoint().ToString();
    }
}
