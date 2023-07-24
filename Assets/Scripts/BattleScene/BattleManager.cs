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
    Image alpha;

    private void Awake()
    {
        battlerPrefab = Resources.Load<GameObject>("Prefabs/Battler");
        alpha = GameObject.Find("FadeAlpha").GetComponent<Image>();

        alpha.enabled = true;
        alpha.color = new Color(0, 0, 0, 1);
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
