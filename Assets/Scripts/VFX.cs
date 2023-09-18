using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class VFX : MonoBehaviour
{
    private void Start()
    {
        Object.Destroy(gameObject, GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length);
    }
}
