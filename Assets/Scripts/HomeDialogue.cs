using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct HomeSceneDialogue
{
    public int startStage;
    public int endStage;
    public string dialogueID;
    public AudioClip clip;
}

[CreateAssetMenu(fileName = "NewHomeCharacter", menuName = "作成/ホーム台詞キャラ")]
public class HomeDialogue : ScriptableObject
{
    [SerializeField] public Sprite characterSprite;
    [SerializeField] public List<HomeSceneDialogue> dialogueList;
}