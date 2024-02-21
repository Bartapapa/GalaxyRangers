using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class SC_An_Menu : MonoBehaviour
{

    private enum buttonType
    {
        Start,
        NewGame,
        Settings,
        None
    }
    [SerializeField] private Animator startAnimator = null;
    [SerializeField] private Animator newGameAnimator = null;
    [SerializeField] private Animator settingsAnimator = null;
    [SerializeField] private Button newGameButton = null;
    [SerializeField] private Button optionsButton = null;
    [SerializeField] private Button controlButton = null;
    private bool _isClicked = false;
    private bool _isClickedSetting = false;

    private void Start()
    {
        newGameButton.interactable = false;
        optionsButton.interactable = false;
        controlButton.interactable = false;
    }

    public void ClickOnButStart()
    {
        if (_isClicked == true) {
            _isClicked = false;
            startAnimator.SetTrigger("ClickClose");
            newGameAnimator.SetTrigger("Disable");
            newGameButton.interactable = false;
        }
        else {
            _isClicked = true;
            startAnimator.SetTrigger("ClickOpen");
            newGameAnimator.SetTrigger("Enable");
            newGameButton.interactable = true;

            settingsAnimator.SetTrigger("ClickClose");
            _isClickedSetting = false;
            optionsButton.interactable = false;
            controlButton.interactable = false;
        }
    }

    public void ClickOnButSetting()
    {
        if (_isClickedSetting == true) {
            settingsAnimator.SetTrigger("ClickClose");
            _isClickedSetting = false;
            optionsButton.interactable = false;
            controlButton.interactable = false;

            startAnimator.SetTrigger("Down");
        }
        else {
            settingsAnimator.SetTrigger("ClickOpen");
            _isClickedSetting = true;
            optionsButton.interactable = true;
            controlButton.interactable = true;

            _isClicked = false;
            startAnimator.SetTrigger("Up");
            newGameAnimator.SetTrigger("Disable");
            newGameButton.interactable = false;
        }
    }





            // case buttonType.NewGame:
            //     if (_isClicked == false)
            //         newGameAnimator.SetTrigger("Click");
            //     break;
}
