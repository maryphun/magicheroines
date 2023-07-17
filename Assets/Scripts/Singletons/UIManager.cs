using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DialogueDir
{
    Left,
    Right,
    None,
}

public struct Dialogue
{
    public Dialogue(DialogueDir dir, string txt)
    {
        fromDir = dir;
        text = txt;
    }

    public DialogueDir fromDir;
    public string text;
}

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    [Header("References")]
    [SerializeField] private DialogueBox prefabOrigin;

    DialogueBox dialogue;

    bool isDialogueBoxSpawned = false;

    public void Initialize()
    {
        Debug.Log("Initialize UI manager");
        prefabOrigin = Resources.Load<DialogueBox>("Prefabs/DialogueBox");
    }

    public void StartDialogueBox(Canvas canvas)
    {
        if (!isDialogueBoxSpawned)
        {
            dialogue = Instantiate(prefabOrigin, canvas.transform);
        }

        dialogue.StartDialogueBox();
    }

    public void StopDialogueBox()
    {
        dialogue.StopDialogueBox();
    }
}
