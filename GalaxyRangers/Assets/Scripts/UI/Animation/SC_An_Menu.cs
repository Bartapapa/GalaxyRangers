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
    [SerializeField] private Button newGameButton = null;
    [SerializeField] private Button optionsButton = null;
    [SerializeField] private Button controlButton = null;
    private bool _isClicked = false;
    [SerializeField] private buttonType _buttonType = buttonType.None;

    /*private void Start()
    {
        if (newGameButton)
            newGameButton.interactable = false;
        if (optionsButton && controlButton) {
            optionsButton.interactable = false;
            controlButton.interactable = false;
        }
    }*/



    public void ClickOnBut()
    {
        switch (_buttonType)
        {
            case buttonType.Start:
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
                }
                break;
            case buttonType.NewGame:
                if (_isClicked == false)
                    newGameAnimator.SetTrigger("Click");
                break;
            case buttonType.Settings:
                if (_isClicked == true) {
                    startAnimator.SetTrigger("ClickClose");
                    newGameAnimator.SetTrigger("Down");
                    _isClicked = false;
                    optionsButton.interactable = false;
                    controlButton.interactable = false;
                }
                else {
                    startAnimator.SetTrigger("ClickOpen");
                    newGameAnimator.SetTrigger("Up");
                    _isClicked = true;
                    optionsButton.interactable = true;
                    controlButton.interactable = true;
                }
                break;
            case buttonType.None:
                break;
        }


        
    }


}
