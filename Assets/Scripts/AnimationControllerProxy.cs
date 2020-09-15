using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllerProxy : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
    }

    public void Play(string stateName, out float duration)
    {
        animator.Play(stateName);

        int layerIndex = animator.GetLayerIndex("Base Layer");
        var animatatorStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

        duration = animatatorStateInfo.length;
    }

    public void SetEnabled(bool enable)
    {
        animator.enabled = enable;
    }
}
