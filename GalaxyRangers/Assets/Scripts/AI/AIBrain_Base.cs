using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain_Base : MonoBehaviour
{
    [Header("OBJECT REFS")]
    [Space]
    public BaseCharacterController controller;
    public CharacterCombat combat;
    public CharacterHealth health;

    [Header("STATE MACHINE")]
    [Space]
    [SerializeField] private AIState _defaultState;
    [SerializeField] private AIState _hurtState;
    [SerializeField][ReadOnlyInspector] private AIState _currentState;
    public AIState currentState { get { return _currentState; } }

    [Header("SIMULATED INPUTS")]
    [Space]
    private PlayerInputs _currentInputs = new PlayerInputs();
    public PlayerInputs currentInputs { get { return _currentInputs; } }

    [Header("PLAYER DETECTION")]
    [Space]
    public LayerMask playerMask;
    [SerializeField] private float _playerDetectionDistance = 5f;
    public float playerDetectionDistance { get { return _playerDetectionDistance; } }
    [ReadOnlyInspector] public bool playerDetected = false;
    [ReadOnlyInspector] public Transform playerTransform;
    [SerializeField][ReadOnlyInspector] private float _currentPlayerDistance = float.MaxValue;
    public float currentPlayerDistance { get { return _currentPlayerDistance; } }

    [Header("COMBAT AI")]
    [SerializeField] private float _hurtDuration = .3f;
    public float hurtDuration { get { return _hurtDuration; } }
    [SerializeField] private float _maxInitialAttackCooldown = 1f;
    [SerializeField] private float _combatDistance = 3f;
    public float combatDistance { get { return _combatDistance; } }
    [SerializeField] private float _aimDuration = 3f;
    public float aimDuration { get { return _aimDuration; } }
    [SerializeField][ReadOnlyInspector] private float _attackCooldown = float.MinValue;
    public bool canPerformNewAttack { get { return combat.currentWeapon != null && (_attackCooldown <= 0 || (combat.isAttacking && combat.canDoCombo)); } }
    public float attackCooldown { get { return _attackCooldown; } set { _attackCooldown = value; } }

    private void Start()
    {
        if (_defaultState != null)
        {
            _currentState = _defaultState;
        }

        controller.OnHit -= OnHit;
        controller.OnHit += OnHit;
    }

    private void OnHit()
    {
        //Change to hurt state
        _currentState.ResetState();
        _currentState = _hurtState;
    }

    private void Update()
    {
        if (_currentState != null && !health.isDead)
        {
            AIreturn newReturn = _currentState.Tick(this);
            _currentState = newReturn.toState;
            SimulateInputs(newReturn.inputs);
        }

        HandlePlayerDistance();
        HandleCooldowns();
    }

    private void HandlePlayerDistance()
    {
        _currentPlayerDistance = playerTransform != null ? Vector3.Distance(this.transform.position, playerTransform.position) : float.MaxValue;
    }

    private void HandleCooldowns()
    {
        if (_attackCooldown > 0)
        {
            _attackCooldown -= Time.deltaTime;
        }
    }

    public void GetInitialAttackCooldown()
    {
        _attackCooldown = UnityEngine.Random.Range(.2f, _maxInitialAttackCooldown);
    }

    #region SIMULATED INPUTS
    private void SimulateInputs(PlayerInputs inputs)
    {
        controller.SetInputs(inputs);
        _currentInputs = inputs;
    }

    public void RequestJump()
    {
        controller.RequestJump();
    }

    public void RequestDash()
    {
        controller.RequestDash();
    }

    public void RequestLightAttack()
    {
        combat.RequestLightAttack();
    }

    public void RequestHeavyAttack()
    {
        combat.RequestHeavyAttack();
    }
    #endregion

    #region GIZMOS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _playerDetectionDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _combatDistance);
    }
    #endregion
}
