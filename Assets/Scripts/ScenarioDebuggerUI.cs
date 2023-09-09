using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NovelEditor;

public class ScenarioDebuggerUI : MonoBehaviour
{
    [SerializeField] Canvas UICanvas;
    [SerializeField] NovelPlayer player;
    [SerializeField] NovelData data;

    private bool isPlaying = false;

    public void PlayScript()
    {
        UICanvas.enabled = false;
        player.Play(data, true);
        isPlaying = true;
    }

    private void Update()
    {
        if (isPlaying)
        {
            // 終了チェック
            if (player.IsStop)
            {
                isPlaying = false;
                UICanvas.enabled = true;
            }
        }
    }
}
