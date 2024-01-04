using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeiWeaponController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public KeiWeaponSprite leftWeapon;
    [SerializeField] public KeiWeaponSprite rightWeapon;

    [Header("Debug")]
    [SerializeField] private Vector2 leftWeaponLocalPosition;
    [SerializeField] private Vector2 rightWeaponLocalPosition;
    [SerializeField] private KeiControlledUnit controlledUnit;

    [HideInInspector] public Vector2 LeftWeaponLocalPosition { get { return leftWeaponLocalPosition; } }
    [HideInInspector] public Vector2 RightWeaponLocalPosition { get { return rightWeaponLocalPosition; } }
    [HideInInspector] public KeiControlledUnit ControlledUnit { get { return controlledUnit; } } // ˜ø™S

    private void Start()
    {
        leftWeapon.Init();
        rightWeapon.Init();

        leftWeaponLocalPosition = leftWeapon.Rect.localPosition;
        rightWeaponLocalPosition = rightWeapon.Rect.localPosition;
        
        leftWeapon.SetEnableMovement(true);
        rightWeapon.SetEnableMovement(true);

        ResetControlledUnit();
    }

    public void SetControlledUnit(KeiControlledUnit battler)
    {
        controlledUnit = battler;
    }

    public void ResetControlledUnit()
    {
        controlledUnit = null;
    }
}
