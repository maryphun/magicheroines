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

[CreateAssetMenu(fileName = "NewHomeCharacter", menuName = "ì¬/ƒz[ƒ€‘äŒƒLƒƒƒ‰")]
public class HomeDialogue : ScriptableObject
{
    [SerializeField] public Sprite characterSprite;
    [SerializeField] public List<HomeSceneDialogue> dialogueList;
}