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

    public void FadeIn(float overTime)
    {
        fadeCoroutine = StartCoroutine(CoFade(true, overTime));
    }

    public void FadeOut(float overTime)
    {
        fadeCoroutine = StartCoroutine(CoFade(false, overTime));
    }

    private IEnumerator CoFade(bool fadeIn, float overTime)
    {
        _isFading = true;

        //Do stuff

        if (!fadeIn)
        {
            fadeCoroutine = null;
        }
        _isFading = false;
        yield return null;
    }
}
