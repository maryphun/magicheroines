using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipDialogueUI : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float interval = 0.05f;

    [Header("References")]
    [SerializeField] private NovelEditor.NovelPlayer novelPlayer;

    [Header("Debug")]
    [SerializeField] private bool isSkipping = false;
    [SerializeField] private bool isFlashing = false;
    [SerializeField] private float intervalcnt;

    private void Start()
    {
        isSkipping = false;
        enabled = false;
    }

    public void StartSkipping()
    {
        intervalcnt = interval;
        isSkipping = true;
        isFlashing = false;
        enabled = true;
    }

    public void StopSkipping()
    {
        intervalcnt = interval;
        isSkipping = false;
        isFlashing = false;
        enabled = false;
    }

    private void Update()
    {
        if (isSkipping)
        {
            if (Input.anyKeyDown)
            {
                StopSkipping();
                return;
            }

            if (novelPlayer.IsImageChanging)
            {
                return;
            }

            intervalcnt -= Time.deltaTime;
            if (intervalcnt <= 0.0f)
            {
                intervalcnt = interval;

                if (isFlashing)
                {
                    isFlashing = false;
                    novelPlayer.FlashText();
                }
                else
                {
                    isFlashing = true;
                    novelPlayer.GoNext();
                }
            }
        }
        else
        {
            enabled = false;
        }
    }
}
