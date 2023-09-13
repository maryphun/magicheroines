using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TMP_AutoMimic : MonoBehaviour
{
    [SerializeField] TMP_Text mimicTarget;
    [HideInInspector] TMP_Text self;

    private void Start()
    {
        self = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (!mimicTarget.havePropertiesChanged) return;
        self.text = mimicTarget.text;
    }
}
