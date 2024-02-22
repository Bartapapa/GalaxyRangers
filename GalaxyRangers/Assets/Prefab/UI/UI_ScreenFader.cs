using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ScreenFader : MonoBehaviour
{
    public Animator animator;
    public GameObject screen;
    public GameObject credits;

    public void FadeInSide()
    {
        animator.Play("FadeInSide", 0, 0);
    }

    public void FadeOutSide()
    {
        animator.Play("FadeOutSide", 0, 0);
    }

    public void FadeInScreen()
    {
        animator.Play("FadeInScreen", 0, 0);
    }

    public void FadeOutScreen()
    {
        animator.Play("FadeOutScreen", 0, 0);
    }

    public void ToggleVisibility(bool see)
    {
        screen.SetActive(see);
    }
}
