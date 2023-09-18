using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Enemy", menuName = "作成/敵キャラ生成")]
public class EnemyDefine : ScriptableObject
{
    [Header("基本資料")]
    [SerializeField] public string enemyName = "スライム";
    [SerializeField] public int maxHP = 100;
    [SerializeField] public int maxMP = 100;
    [SerializeField] public int attack = 10;
    [SerializeField] public int defense = 10;
    [SerializeField] public int speed = 10;
    [SerializeField] public int level = 1;
    [SerializeField] public Color character_color = new Color(1,1,1,1);

    [Header("アイコン")]
    [SerializeField] public Sprite icon;

    [Header("レファレンス")]
    [SerializeField] public GameObject battler;
}
