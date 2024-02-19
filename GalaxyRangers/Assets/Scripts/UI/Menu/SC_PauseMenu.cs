using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SC_PauseMenu : MonoBehaviour
{
    private bool _isPause = false;
    // [SerializeField]
    // private Button buttonSpeed;
    [SerializeField] GameObject _pauseMenuGO = null;
    private bool _isDead = false;
    private float _speedOfLevel = 1f;
    private string sceneName;
    // [SerializeField] private TextMeshProUGUI _textSpeedOfLevel;

    public void Pause()
    {
        /*if (_isDead == false)
        {
            // buttonPause.gameObject.SetActive(false);
            // buttonSpeed.gameObject.SetActive(false);
            Time.timeScale = 0;
        }*/
        if (_isPause)
        {
            _isPause = false;
            _pauseMenuGO.SetActive(false);   
            //Time.timeScale = 0;
        }
        else
        {
            _isPause = true;
            _pauseMenuGO.SetActive(true);
            //Time.timeScale = 1;
        }
    }
    public void Resume()
    {
        if (_isDead == false)
        {
            // _textSpeedOfLevel.SetText("x1");
            // buttonSpeed.gameObject.SetActive(true);
            _isPause = false;
            Time.timeScale = 1;
        }
    }

    public void ChangeSpeedOfLevel()
    {

        if (_speedOfLevel < 8)
            _speedOfLevel *= 2;
        else
            _speedOfLevel = 0.5f;
        Time.timeScale = _speedOfLevel;
        // _textSpeedOfLevel.SetText("x" + _speedOfLevel);
    }

    private void Update()
    {
        /*if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) && _isDead == false)
        {

        }*/
    }
}
