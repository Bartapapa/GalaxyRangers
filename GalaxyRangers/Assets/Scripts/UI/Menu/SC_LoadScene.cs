using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_LoadScene : MonoBehaviour
{
    [SerializeField]
    private GameObject _panelInRef = null;
    private Animator _animatorPanel = null;
    [SerializeField]
    private SC_Fade _fadeScript = null;


    public void LoadScene(string sceneName)
    {
        // Debug.Log("Change to level "+ sceneName);
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        if (_fadeScript != null && _panelInRef != null) {
            _panelInRef.SetActive(true);
            _animatorPanel = _panelInRef.GetComponent<Animator>();
            _animatorPanel.SetTrigger("TriggerFadeIn");
        }
        else
        {
            Debug.LogWarning("Pas Drag and drop de panel ou de fade dans le bouton start");
        }
        StartCoroutine(loadNextScene(sceneName));
    }

    public IEnumerator loadNextScene(string sceneName)
    {
        // Lance l'animation
        yield return new WaitForSeconds(1f);
        Debug.Log("Change to level " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    public void CurLoadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
