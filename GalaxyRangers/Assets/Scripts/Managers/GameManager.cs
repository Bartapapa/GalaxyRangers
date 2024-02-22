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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            OnBossCharacterDeath(new CharacterHealth());
        }
    }

    #region FADING
    public void Fade(float overTime, bool screenFade = false, Action onFadeInComplete = null, Action onFadeOutComplete = null)
    {
        if (isFading)
            return;

        _onFadeInComplete = onFadeInComplete;
        _onFadeOutComplete = onFadeOutComplete;

        FadeIn(overTime, screenFade);
    }

    private void FadeIn(float overTime, bool screenFade = false)
    {
        fadeCoroutine = StartCoroutine(CoFade(true, overTime, screenFade));
    }

    private void FadeOut(float overTime, bool screenFade = false)
    {
        fadeCoroutine = StartCoroutine(CoFade(false, overTime, screenFade));
    }

    private IEnumerator CoFade(bool fadeIn, float overTime, bool screenFade = false)
    {
        _isFading = true;

        if (fadeIn)
        {
            Debug.Log("Fading in");
            UI_Manager.Instance.screenFader.ToggleVisibility(true);
            if (screenFade)
            {            
                UI_Manager.Instance.screenFader.FadeInScreen();
            }
            else
            {
                UI_Manager.Instance.screenFader.FadeInSide();
            }

        }
        else
        {
            Debug.Log("Fading out");
            UI_Manager.Instance.screenFader.ToggleVisibility(true);
            if (screenFade)
            {
                UI_Manager.Instance.screenFader.FadeOutScreen();
            }
            else
            {
                UI_Manager.Instance.screenFader.FadeOutSide();
            }
        }

        yield return new WaitForSecondsRealtime(overTime);

        if (!fadeIn)
        {
            fadeCoroutine = null;
            UI_Manager.Instance.screenFader.ToggleVisibility(false);
        }
        _isFading = false;

        if (fadeIn)
        {
            FadeInCallback(overTime, screenFade);
        }
        else
        {
            FadeOutCallback(overTime);
        }
    }

    private IEnumerator WaitForTime(float duration, Action onWaitEnd = null)
    {
        yield return new WaitForSecondsRealtime(duration);

        if (onWaitEnd != null)
        {
            onWaitEnd();
        }
    }

    private void FadeInCallback(float overtime, bool screenFade)
    {      
        if (_onFadeInComplete != null)
        {
            _onFadeInComplete();
            _onFadeInComplete = null;
        }

        StartCoroutine(WaitForTime(.2f,
            () => FadeOut(overtime, screenFade)));
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

    #region CHARDEATH

    public void OnPlayerCharacterDeath(CharacterHealth characterHealth)
    {
        CameraManager.Instance.CameraState = CameraState.Death;
        StartCoroutine(WaitForTime(3f,
            () => Fade(.5f, true,
                () => ReviveCharacter(characterHealth),
                null)));
    }

    private void ReviveCharacter(CharacterHealth character)
    {
        WorldManager.Instance.EndRun();

        character.Revive();
    }

    public void OnBossCharacterDeath(CharacterHealth bossHealth)
    {
        //Boss has died, roll credits lmfaooooooooooooooooooo
        CameraManager.Instance.RemoveFocusObjectFromCamera(Player.Instance.CharacterController.transform);
        StartCoroutine(WaitForTime(2f,
            () => EndFade()));
    }

    private void EndFade()
    {
        _isFading = true;
        UI_Manager.Instance.screenFader.FadeInScreen();
        StartCoroutine(WaitForTime(2f,
            () => RollCredits()));
    }

    private void RollCredits()
    {
        UI_Manager.Instance.screenFader.credits.SetActive(true);
        UI_Manager.Instance.screenFader.animator.Play("CreditsRoll");
    }

    #endregion
}
