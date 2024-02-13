using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("OBJECT REFENCES")]
    [SerializeField] private HurtBox _hurtBox;
    [SerializeField] private Transform _root;

    [Header("PARAMETERS")]
    [Space]
    [SerializeField] private float _gravity = 0;
    [Space]
    [SerializeField] private float _projectileSpeed = 10f;
    [SerializeField] private AnimationCurve _speedDecay = new AnimationCurve();
    [SerializeField] private float _decayOverTime = -1f;
    [SerializeField][ReadOnlyInspector] private Vector3 _currentDirection;
    [SerializeField][ReadOnlyInspector] private Vector3 _previousPosition;
    [Space]
    [SerializeField] private int _pierceNumber = 0;
    [SerializeField] private bool _pierceThroughWalls = false;
    [Space]
    [SerializeField] private float _lifeSpan = -1f;

    //Cached
    private Rigidbody _rigid;
    private int _currentPierceNumber = 0;
    private float _currentTimer = 0f;
    private float _currentSpeed = 0f;

    private void Start()
    {
        _currentSpeed = _projectileSpeed;
        _previousPosition = transform.position;

        _rigid = GetComponent<Rigidbody>();
    }

    public void InitializeProjectile(Vector3 direction)
    {
        direction = new Vector3(direction.x, direction.y, 0f).normalized;
        _currentDirection = direction;
        _root.LookAt(transform.position + _currentDirection, Vector3.up);
    }

    private void OnEnable()
    {
        if (_hurtBox != null)
        {

            EnableHurtBox();

            _hurtBox.OnHit -= OnHurtBoxHit;
            _hurtBox.OnHit += OnHurtBoxHit;
        }
    }

    private void OnDisable()
    {
        if (_hurtBox != null)
        {
            DisableHurtBox();

            _hurtBox.OnHit -= OnHurtBoxHit;
        }
    }

    private void Update()
    {
        _currentTimer += Time.deltaTime;
        HandleLifeSpan();
        HandleSpeedDecay();
    }

    private void FixedUpdate()
    {
        _rigid.velocity = _currentDirection * _currentSpeed;

        Vector3 newPosition = transform.position;

        //Check if collided with Characters in between current position and previous position
        RaycastHit[] hits = Physics.RaycastAll(_previousPosition, newPosition - _previousPosition, Vector3.Distance(_previousPosition, newPosition));
        foreach(RaycastHit hit in hits)
        {
            BaseCharacterController charController = hit.collider.GetComponent<BaseCharacterController>();
            if (charController)
            {
                if (!charController.hit && _hurtBox.hurtFactions.Contains(charController.faction))
                {
                    _hurtBox.TriggerHit(charController);
                }
            }
        }

        //Place new previous position.
        _previousPosition = transform.position;

        HandleCurrentDirection();
    }

    #region CALLBACKS
    private void OnHurtBoxHit(HurtBox hurtBox)
    {
        //hurt box hit
        CheckPierce();
    }
    #endregion

    private void CheckPierce()
    {
        _currentPierceNumber++;
        if (_currentPierceNumber > _pierceNumber && _pierceNumber >= 0)
        {
            DestroyProjectile();
        }
        else
        {
            //Proceed normally. Projectiles with _pierceNumber < 0 pierce infinitely.
        }
    }

    private void EnableHurtBox()
    {
        if (_hurtBox != null)
        {
            _hurtBox.EnableHurtBox();
        }
    }

    private void DisableHurtBox()
    {
        if (_hurtBox != null)
        {
            _hurtBox.DisableHurtBox();
        }
    }

    private void HandleSpeedDecay()
    {
        if (_decayOverTime > 0f)
        {
            if (_currentTimer <= _decayOverTime)
            {
                _currentSpeed = _speedDecay.Evaluate(_currentTimer / _decayOverTime);
            }
            else
            {
                _currentSpeed = _speedDecay.Evaluate(1f);
            }           
        }
    }

    private void HandleLifeSpan()
    {
        if (_lifeSpan > 0f)
        {
            if (_currentTimer >= _lifeSpan)
            {
                DestroyProjectile();
            }
        }
    }

    private void HandleCurrentDirection()
    {
        float newY = (_currentDirection.y + _gravity)*Time.fixedDeltaTime;
        _currentDirection = new Vector3(_currentDirection.x, newY, 0f).normalized;
        _root.LookAt(transform.position + _currentDirection, Vector3.up);
    }

    private void DestroyProjectile()
    {
        //Create SFX at position.
        Destroy(this.gameObject);
    }
}
