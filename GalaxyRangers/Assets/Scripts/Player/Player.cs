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
    public bool DashPressed;

    public PlayerInputs(float moveX, float moveY, bool jumpPressed, bool dashPressed)
    {
        MoveX = moveX;
        MoveY = moveY;
        JumpPressed = jumpPressed;
        DashPressed = dashPressed;
    }
}

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    //Singleton
    public static Player Instance;

    [Header("OBJECT REFERENCES")]
    [SerializeField] private BaseCharacterController _controller;
    public BaseCharacterController CharacterController { get { return _controller; } }

    [SerializeField] private CharacterCombat _combat;
    public CharacterCombat CharacterCombat { get { return _combat; } }

    [Header("INPUTS")]
    [Space(10)]
    private float _moveInput;
    private float _upDownInput;
    private bool _jumpButtonPressed;
    private bool _dashButtonPressed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("2 or more Players found. Removing the latest ones.");
            Destroy(this.gameObject);
        }
    }

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
    }

    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() >= .5f)
        {
            CharacterController.RequestDash();
            _dashButtonPressed = context.action.IsPressed();
        }
        else
        {
            _dashButtonPressed = false;
        }
    }

    public void OnLightAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _combat.RequestLightAttack();
        }
    }

    public void OnHeavyAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _combat.RequestHeavyAttack();
        }
    }
    #endregion

    private void HandleCharacterInputs()
    {
        PlayerInputs inputs = new PlayerInputs();
        inputs.MoveX = _moveInput;
        inputs.MoveY = _upDownInput;
        inputs.JumpPressed = _jumpButtonPressed;
        inputs.DashPressed = _dashButtonPressed;

        CharacterController.SetInputs(inputs);
    }
}
