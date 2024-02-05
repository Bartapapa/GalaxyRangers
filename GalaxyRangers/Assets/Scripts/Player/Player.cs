using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public struct PlayerInputs
{
    public float MoveX;
    public float MoveY;
    public bool JumpPressed;

    public PlayerInputs(float moveX, float moveY, bool jumpPressed)
    {
        MoveX = moveX;
        MoveY = moveY;
        JumpPressed = jumpPressed;
    }
}

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    [SerializeField] private PlayerCharacterController _controller;
    public PlayerCharacterController CharacterController { get { return _controller; } }

    [Header("INPUTS")]
    [Space(10)]
    private float _moveInput;
    private float _upDownInput;
    private bool _jumpButtonPressed;

    private void Update()
    {
        HandleCharacterInputs();
    }

    #region INPUT CALLS
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<float>();
    }

    public void OnUpDownInput(InputAction.CallbackContext context)
    {
        _upDownInput = context.ReadValue<float>();
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            CharacterController.RequestJump();
        }

        _jumpButtonPressed = context.action.IsPressed();
        
        //_jumpButtonPressed = context.action.IsPressed();
        //_jumpButtonUp = context.canceled;
    }
    #endregion

    private void HandleCharacterInputs()
    {
        PlayerInputs inputs = new PlayerInputs();
        inputs.MoveX = _moveInput;
        inputs.MoveY = _upDownInput;
        inputs.JumpPressed = _jumpButtonPressed;

        CharacterController.SetInputs(inputs);
    }
}
