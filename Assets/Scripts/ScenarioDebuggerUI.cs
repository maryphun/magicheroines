using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovelEditor;

public class ScenarioDebuggerUI : MonoBehaviour
{
    [SerializeField] NovelPlayer player;
    [SerializeField] NovelData data;

    public void PlayScript()
    {
        player.Play(data, true);
    }
}
