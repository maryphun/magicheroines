using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Enemy", menuName = "作成/敵キャラ生成")]
public class EnemyDefine : ScriptableObject
{
    [Header("基本資料")]
    [SerializeField] public string enemyName = "スライム";
    [SerializeField] public int maxHP = 100;
    [SerializeField] public int maxMP = 100;
    [SerializeField] public int attackDamage = 10;

    [Header("アイコン")]
    [SerializeField] public Sprite icon;

    [Header("スプライト")]
    [SerializeField] public BattlerAnimation animations;
}
