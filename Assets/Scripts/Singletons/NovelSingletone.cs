using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NovelEditor;
using System.Linq;

public class NovelSingletone : SingletonMonoBehaviour<NovelSingletone>
{
    NovelPlayer novelplayer;

    bool isNovelCreated = false;

    List<GraphicRaycaster> disabledCanvas = new List<GraphicRaycaster>();

    public void CreateNovelPlayer()
    {
        if (isNovelCreated) return;

        GameObject origin = Resources.Load<GameObject>("Prefabs/Canvas");

        if (ReferenceEquals(origin, null))
        {
            Debug.LogWarning("Failed to load NovelPlayer Prefabs!");
        }

        GameObject spawnedObj = Instantiate(origin);
        spawnedObj.name = "NovelPlayerCanvas";
        spawnedObj.transform.SetParent(this.transform);

        novelplayer = spawnedObj.GetComponentInChildren<NovelPlayer>();

        if (ReferenceEquals(novelplayer, null))
        {
            Debug.LogWarning("NovelPlayer component not found!");
        }

        DontDestroyOnLoad(spawnedObj);

        isNovelCreated = true;
    }

    public void PlayNovel(string dataName, bool hideAfterPlay)
    {
        if (!isNovelCreated) CreateNovelPlayer();
        NovelData data = Resources.Load<NovelData>(dataName);
        this.PlayNovel(data, hideAfterPlay);
    }

    public void PlayNovel(NovelData data, bool hideAfterPlay)
    {
        if (!isNovelCreated) CreateNovelPlayer();

        novelplayer.Play(data, hideAfterPlay);

        var raycasters = FindObjectsOfType<GraphicRaycaster>();
        disabledCanvas = raycasters.ToList();

        for (int i = 0; i < disabledCanvas.Count; i++)
        {
            if (disabledCanvas[i].gameObject.name != "NovelPlayerCanvas" && disabledCanvas[i].enabled)
            {
                disabledCanvas[i].enabled = false;
            }
            else
            {
                disabledCanvas.RemoveAt(i);
                i--;
            }
        }
    }

    public bool IsPlaying()
    {
        if (!isNovelCreated) CreateNovelPlayer();

        return novelplayer.IsPlaying;
    }
    public bool IsStop()
    {
        if (!isNovelCreated) CreateNovelPlayer();

        return novelplayer.IsStop;
    }

    public void SetBGMVolume(float value)
    {
        if (!isNovelCreated) CreateNovelPlayer();

        novelplayer.BGMVolume = value;
    }
    public void SetSEVolume(float value)
    {
        if (!isNovelCreated) CreateNovelPlayer();

        novelplayer.SEVolume = value;
    }
    public void SetTextSpeed(int value)
    {
        if (!isNovelCreated) CreateNovelPlayer();

        novelplayer.textSpeed = Mathf.Clamp(value, 1, 10);
    }
    public void SetAutoSpeed(float value)
    {
        if (!isNovelCreated) CreateNovelPlayer();

        novelplayer.autoSpeed = value;
    }
    public int GetTextSpeed()
    {
        if (!isNovelCreated) CreateNovelPlayer();

        return novelplayer.textSpeed;
    }
    public float GetAutoSpeed()
    {
        if (!isNovelCreated) CreateNovelPlayer();

        return novelplayer.autoSpeed;
    }

    private void Update()
    {
        if (IsStop())
        {
            foreach (GraphicRaycaster canvas in disabledCanvas)
            {
                canvas.enabled = true;
            }
            disabledCanvas.Clear();
            enabled = false;
        }
    }
}
