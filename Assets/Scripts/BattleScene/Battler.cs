using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battler : MonoBehaviour
{
    [SerializeField] public bool isEnemy;

    [SerializeField] public int max_hp;
    [SerializeField] public int max_mp;
    [SerializeField] public int current_hp;
    [SerializeField] public int current_mp;
    [SerializeField] public string name;

    public BattlerAnimation animations;
}
