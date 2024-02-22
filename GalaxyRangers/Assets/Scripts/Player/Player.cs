using System;
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

    [SerializeField] private CharacterHealth _health;
    public CharacterHealth CharacterHealth { get { return _health; } }

    [SerializeField] private CharacterQuest _quest;
    public CharacterQuest CharacterQuest { get { return _quest; } }


    [SerializeField] private CharacterSpeciality_1 _speciality_1;
    public CharacterSpeciality_1 _specialityRef_1 { get { return _speciality_1; } }

    [SerializeField] private InteractibleManager _interactibleManager;
    public InteractibleManager interactibleManager { get { return _interactibleManager; } }

    [Header("INPUTS")]
    [Space(10)]
    private float _moveInput;
    private float _upDownInput;
    private bool _jumpButtonPressed;
    private bool _dashButtonPressed;

    public SC_Currency_Relation _currencyScript = null;


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

    private void OnEnable()
    {
        if (_health)
        {
            _health.CharacterDied -= OnCharacterDeath;
            _health.CharacterDied += OnCharacterDeath;
        }
    }

    private void OnDisable()
    {
        if (_health)
        {
            _health.CharacterDied -= OnCharacterDeath;
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

    public void OnQuestDisplayInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            UI_Manager.Instance._scriptDisplayRef.DisplayQuestPanel();
        }
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
    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _interactibleManager.InteractWithCurrentInteractible();
        }      
    }

    public void OnSpecialAbility1Input(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _specialityRef_1.TryToLaunchAbility();
        }
        //Special ability.
    }
    public void OnSpecialAbility2Input(InputAction.CallbackContext context)
    {
        if (context.started)
        {

        }
        //Special ability.
    }
    public void OnSpecialAbility3Input(InputAction.CallbackContext context)
    {
        if (context.started)
        {

        }
        //Special ability.
    }

    public void OnPauseInput(InputAction.CallbackContext context)
    {
        //Pause game.
        UI_Manager.Instance._scriptPauseMenu.Pause();
        // Debug.LogWarning("Marche");
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

    #region EVENTS
    private void OnCharacterDeath(CharacterHealth characterHealth)
    {
        //Tell gamemanager to do everything related to dying.
        if (GameManager.Instance)
        {
            GameManager.Instance.OnPlayerCharacterDeath(characterHealth);
        }
    }

    #endregion
}
