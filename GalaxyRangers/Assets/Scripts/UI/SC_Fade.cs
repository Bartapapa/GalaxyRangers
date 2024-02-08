using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Fade : MonoBehaviour
{
    [SerializeField]
    private GameObject _panelOutRef = null;
    private Animator _animatorPanelOut = null;

    private void Start()
    {
    }

    public void launchAnimationPanel()
    {
        if (_panelOutRef)
        {
            _panelOutRef.SetActive(true);

            _animatorPanelOut = _panelOutRef.GetComponent<Animator>();
            StartCoroutine(FadeOutCoroutine());
        }
    }

    public IEnumerator FadeOutCoroutine()
    {
        // Lance l'animation
        _animatorPanelOut.SetTrigger("TriggerFadeOut");
        yield return new WaitForSeconds(2f);
        _panelOutRef.SetActive(false);
    }
}
