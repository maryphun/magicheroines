using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;



public class DialogueBox : MonoBehaviour
{
    [SerializeField] private RectTransform characterLeft;
    [SerializeField] private RectTransform characterRight;
    [SerializeField] private RectTransform outline;
    [SerializeField] private RectTransform gray;
    [SerializeField] private RectTransform boxHolder;
    [SerializeField] private TMP_Text dialogueText;

    List<Dialogue> registeredDialogue = new List<Dialogue>();
    int dialogueCnt = 0;

    public void Awake()
    {
        dialogueText.text = ""; // clear
        characterLeft.localPosition = new Vector3(-1137f, 333.64f, 0f);
        characterRight.localPosition = new Vector3(1137f, 333.64f, 0f);
        boxHolder.localPosition = new Vector3(0.0f, -330.0f, 0.0f);
    }

    public void StartDialogueBox()
    {
        registeredDialogue.Add(new Dialogue(DialogueDir.Left, "こんにちは！"));
        registeredDialogue.Add(new Dialogue(DialogueDir.Right, "テストテストテスト！　テスト！　てすと"));
        registeredDialogue.Add(new Dialogue(DialogueDir.Left, "漢字ひらがなカタナカROMAJIromaji。；："));
        dialogueCnt = 0;

        StartCoroutine(StartDialogueBoxAnim());
    }

    public void StopDialogueBox()
    {
        characterLeft.DOLocalMoveX(-1137f, 0.8f, false);
        characterRight.DOLocalMoveX(1137f, 0.8f, false);
        boxHolder.DOLocalMoveY(-330.0f, 0.8f, false);
    }

    IEnumerator StartDialogueBoxAnim()
    {
        boxHolder.DOLocalMoveY(0.0f, 0.8f, false);
        characterLeft.DOLocalMoveX(-888.0f, 0.8f, false);
        characterLeft.GetComponent<Image>().DOColor(new Color(0.75f, 0.75f, 0.75f, 1.0f), 0.0f);
        characterRight.DOLocalMoveX(888.0f, 0.8f, false);
        characterRight.GetComponent<Image>().DOColor(new Color(0.75f, 0.75f, 0.75f, 1.0f), 0.0f);

        yield return new WaitForSeconds(0.8f);

        while (dialogueCnt < registeredDialogue.Count)
        {
            int dialogueTextCnt = 0;

            switch (registeredDialogue[dialogueCnt].fromDir)
            {
                case DialogueDir.Left:
                    characterLeft.DOLocalMoveX(-800.0f, 0.8f, false); 
                    characterRight.DOLocalMoveX(888.0f, 0.8f, false);
                    characterLeft.GetComponent<Image>().DOColor(new Color(1f, 1f, 1f, 1.0f), 0.0f);
                    characterRight.GetComponent<Image>().DOColor(new Color(0.75f, 0.75f, 0.75f, 1.0f), 0.0f);
                    break;
                case DialogueDir.Right:
                    characterLeft.DOLocalMoveX(-888.0f, 0.8f, false);
                    characterRight.DOLocalMoveX(800.0f, 0.8f, false);
                    characterLeft.GetComponent<Image>().DOColor(new Color(0.75f, 0.75f, 0.75f, 1.0f), 0.0f);
                    characterRight.GetComponent<Image>().DOColor(new Color(1f, 1f, 1f, 1.0f), 0.0f);
                    break;
                case DialogueDir.None:
                    characterLeft.DOLocalMoveX(-888.0f, 0.8f, false);
                    characterLeft.GetComponent<Image>().DOColor(new Color(0.75f, 0.75f, 0.75f, 1.0f), 0.0f);
                    characterRight.DOLocalMoveX(888.0f, 0.8f, false);
                    characterRight.GetComponent<Image>().DOColor(new Color(0.75f, 0.75f, 0.75f, 1.0f), 0.0f);
                    break;
            }

            while (dialogueTextCnt < registeredDialogue[dialogueCnt].text.Length)
            {
                dialogueTextCnt++;
                dialogueText.text = registeredDialogue[dialogueCnt].text.Substring(0, dialogueTextCnt);
                yield return new WaitForSeconds(0.1f);
            }

            while (!Input.GetKey(KeyCode.Space))
            {
                yield return new WaitForEndOfFrame();
            }

            dialogueCnt++;
        }
        
        StopDialogueBox();
        
    }
}
