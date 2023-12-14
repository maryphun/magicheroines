using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeiWeaponController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public KeiWeaponSprite leftWeapon;
    [SerializeField] public KeiWeaponSprite rightWeapon;

    private void Start()
    {
        leftWeapon.SetEnableMovement(true);
        rightWeapon.SetEnableMovement(true);
    }
}
