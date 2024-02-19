using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isInFade { get { return fadeCoroutine != null; } }
    private bool _isFading = false;
    public bool isFading { get { return _isFading; } }

    private Coroutine fadeCoroutine;

    private Action _onFadeInComplete;
    private Action _onFadeOutComplete;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("2 or more GameManagers found. Removing the latest ones.");
            Destroy(this.gameObject);
        }
    }

    #region FADING
    public void Fade(float overTime, Action onFadeInComplete = null, Action onFadeOutComplete = null)
    {
        if (isFading)
            return;

        _onFadeInComplete = onFadeInComplete;
        _onFadeOutComplete = onFadeOutComplete;

        FadeIn(overTime);
    }

    private void FadeIn(float overTime)
    {
        fadeCoroutine = StartCoroutine(CoFade(true, overTime));
    }

    private void FadeOut(float overTime)
    {
        fadeCoroutine = StartCoroutine(CoFade(false, overTime));
    }

    private IEnumerator CoFade(bool fadeIn, float overTime)
    {
        _isFading = true;

        if (fadeIn)
        {
            Debug.Log("Fading in");
        }
        else
        {
            Debug.Log("Fading out");
        }

        yield return new WaitForSecondsRealtime(overTime);

        if (!fadeIn)
        {
            fadeCoroutine = null;
        }
        _isFading = false;

        if (fadeIn)
        {
            FadeInCallback(overTime);
        }
        else
        {
            FadeOutCallback(overTime);
        }
    }

    private void FadeInCallback(float overtime)
    {
        FadeOut(overtime);
        if (_onFadeInComplete != null)
        {
            _onFadeInComplete();
            _onFadeInComplete = null;
        }
    }

    private void FadeOutCallback(float overtime)
    {
        if (_onFadeOutComplete != null)
        {
            _onFadeOutComplete();
            _onFadeOutComplete = null;
        }
    }

    #endregion
}
