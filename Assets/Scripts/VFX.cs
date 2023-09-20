using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class VFX : MonoBehaviour
{
    private void Start()
    {
        var animator = GetComponent<Animator>();
        if (ReferenceEquals(animator.runtimeAnimatorController, null))
        {
            Debug.LogError("Animator controller of " + gameObject.name + " doesn't exist!");
            return;
        }
        Object.Destroy(gameObject, animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
    }
}
