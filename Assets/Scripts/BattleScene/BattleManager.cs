using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Battle : MonoBehaviour
{
    [SerializeField]
    List<Battler> characters = new List<Battler>();
    List<Battler> enemies = new List<Battler>();

    [Header("References")]
    GameObject battlerPrefab;

    private void Awake()
    {
        battlerPrefab = Resources.Load<GameObject>("Prefabs/Battler");

        AlphaFadeManager.Instance.FadeIn(5.0f);
    }

    // Start is called before the first frame update
    void InitializeBattleScene(List<PlayerCharacterDefine> _actors, List<EnemyDefine> _enemies)
    {
        // ÉLÉÉÉâê∂ê¨
        for (int i = 0; i < _actors.Count; i++)
        {
            Battler battler = new Battler();
            battler.isEnemy = false;
            battler.max_hp = _actors[i].detail.max_hp;
            battler.max_mp = _actors[i].detail.max_mp;
            battler.current_mp = _actors[i].detail.max_hp;
            battler.current_mp = _actors[i].detail.max_mp;
        }
    }
}
