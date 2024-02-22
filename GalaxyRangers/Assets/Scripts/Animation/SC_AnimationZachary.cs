using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_AnimationZachary : MonoBehaviour
{
    [SerializeField] Animator animator = null;

    void Start()
    {
    }

    private void OnEnable()
    {
        animator.SetFloat("alert",0.1f);
    }


}
