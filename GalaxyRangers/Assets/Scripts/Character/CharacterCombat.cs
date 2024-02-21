using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    [Header("OBJECT REFS")]
    [Space]
    [SerializeField] private BaseCharacterController _controller;
    [SerializeField] private Transform _weaponRoot;

    [Header("WEAPON")]
    [SerializeField] private Weapon _currentWeapon;
    public Weapon currentWeapon { get { return _currentWeapon; } }
    [SerializeField] private Transform _weaponHoldSocket;

    public bool canAttack { get { return _currentWeapon != null && !_controller.isDisabled && !_controller.isDashing && (!isAttacking || canDoCombo); } }
    public bool isAttacking { get { return windUpCoroutine!=null || attackCoroutine!=null || followThroughCoroutine!=null ? true : false; } }
    public bool canDoCombo { get { return followThroughCoroutine != null &&
                _currentWeaponStrike.attack.canCombo &&
                _comboCounter < _currentWeapon.lightCombo.Count - 1 &&
                (_attackTimer <= _currentWeaponStrike.attack.followThroughAnimTime - _currentWeaponStrike.attack.comboFlagStartTime) &&
                (_attackTimer >= _currentWeaponStrike.attack.followThroughAnimTime - _currentWeaponStrike.attack.comboFlagStartTime - _currentWeaponStrike.attack.comboFlagDuration);
        } }

    //Cache
    private float _attackTimer = float.MinValue;
    private int _comboCounter = -1;
    private int _projectileSpawnCounter = 0;
    private bool _lightAttackBuffer;
    private bool _heavyAttackBuffer;
    private WeaponStrike _currentWeaponStrike;
    public WeaponStrike currentWeaponStrike { get { return _currentWeaponStrike; } }
    private bool _cancelAttackOnLand = false;

    //Events
    public delegate void DefaultCallback();
    public delegate void AttackCallback(WeaponAttack attack);
    public event AttackCallback OnAttack;
    public event AttackCallback OnWindUp;
    public event AttackCallback OnFollowThrough;
    public event DefaultCallback OnCancelFullAttack;
    public event DefaultCallback OnAttackCancelledOnLand;

    //Coroutines
    private Coroutine lightAttackBufferCoroutine;
    private Coroutine heavyAttackBufferCoroutine;
    private Coroutine windUpCoroutine;
    private Coroutine attackCoroutine;
    private Coroutine followThroughCoroutine;

    private void Start()
    {
        InitializeEvents();

        if (_currentWeapon != null)
        {
            EquipWeapon(_currentWeapon);
        }
    }

    private void InitializeEvents()
    {
        _controller.OnLand += OnCharacterLand;
        _controller.OnHit += OnCharacterHit;
        _controller.OnDash += OnCharacterDash;
        _controller.OnJump += OnCharacterJump;
        _controller.OnResetCharacter += OnCharacterReset;
    }

    private void OnCharacterLand(float value, RaycastHit hit)
    {
        //Cancel attack if _cancelAttackOnLand
        if (_cancelAttackOnLand)
        {
            Debug.Log("Attack canceled on land.");

            CancelFullAttack();

            OnAttackCancelledOnLand?.Invoke();
        }
    }

    private void OnCharacterHit()
    {
        //Cancel attack
        CancelFullAttack();
    }

    private void OnCharacterDash()
    {
        //Cancel attack
        CancelFullAttack();
    }

    private void OnCharacterJump(int intValue)
    {
        //Cancel attack
        CancelFullAttack();
    }

    private void OnCharacterReset()
    {
        //Cancel attack
        CancelFullAttack();
    }

    private void Update()
    {
        HandleAttackTimer();
        HandleCurrentStrikeHurtbox();
        HandleProjectileSpawn();
        HandleCurrentAttackPropulsion();

        if (_lightAttackBuffer && canAttack)
        {
            if (_controller.isGrounded)
            {
                LightAttack();
            }
            else
            {
                AirAttack();
            }         
        }
        else if (_heavyAttackBuffer && canAttack)
        {
            if (_controller.isGrounded)
            {
                HeavyAttack();
            }
            else
            {
                AirAttack();
            }
        }
    }

    private void HandleAttackTimer()
    {
        if (_attackTimer > 0) _attackTimer -= Time.deltaTime;
    }

    private void HandleCurrentStrikeHurtbox()
    {
        if (attackCoroutine == null)
            return;

        if (_attackTimer <= _currentWeaponStrike.attack.attackAnimTime - _currentWeaponStrike.attack.attackStartTime &&
           _attackTimer >= _currentWeaponStrike.attack.attackAnimTime - _currentWeaponStrike.attack.attackStartTime - _currentWeaponStrike.attack.attackDuration)
        {
            if (_currentWeaponStrike.hurtbox != null)
            {
                _currentWeaponStrike.hurtbox.EnableHurtBox();
            }

        }
        else
        {
            if (_currentWeaponStrike.hurtbox != null)
            {
                _currentWeaponStrike.hurtbox.DisableHurtBox();
            }

        }
    }

    private void HandleProjectileSpawn()
    {
        if (attackCoroutine == null || _currentWeaponStrike.attack.projectile == null)
            return;
        if (_attackTimer <= _currentWeaponStrike.attack.attackAnimTime - _currentWeaponStrike.attack.projectileSpawnTime && _projectileSpawnCounter == 0)
        {
            //Spawn projectile
            _projectileSpawnCounter++;
            Projectile newProjectile = Instantiate<Projectile>(_currentWeaponStrike.attack.projectile, _currentWeapon.projectileSource.position, _currentWeapon.projectileSource.rotation);
            if (WorldManager.Instance)
            {
                if (WorldManager.Instance.currentRogueRoom.resetParent != null)
                {
                    newProjectile.transform.parent = WorldManager.Instance.currentRogueRoom.resetParent;
                }
            }
            newProjectile.InitializeProjectile(_currentWeapon.projectileSource.forward);
        }
    }

    private void HandleCurrentAttackPropulsion()
    {
        if (attackCoroutine == null || !_currentWeaponStrike.attack.doPropulsion)
            return;

        if (_attackTimer <= _currentWeaponStrike.attack.attackAnimTime - _currentWeaponStrike.attack.propulsionStartTime &&
            _attackTimer >= _currentWeaponStrike.attack.attackAnimTime - _currentWeaponStrike.attack.propulsionStartTime - _currentWeaponStrike.attack.propulsionDuration)
        {
            //Move character along propulsion vector.
            Vector2 directedPropulsion = new Vector2(_currentWeaponStrike.attack.propulsionDirection.x * _controller.facingRight * _currentWeaponStrike.attack.propulsionForce,
                                                    _currentWeaponStrike.attack.propulsionDirection.y * _currentWeaponStrike.attack.propulsionForce);
            _controller.CharacterImpulse(directedPropulsion);
        }
    }

    public void RequestLightAttack()
    {
        if (_currentWeapon == null)
            return;
        if (_controller.isGrounded)
        {
            LightAttack();
        }
        else
        {
            AirAttack();
        }     
    }

    private IEnumerator CoLightAttackBuffer()
    {
        _lightAttackBuffer = true;
        yield return new WaitForSecondsRealtime(.2f);
        _lightAttackBuffer = false;
    }

    public void RequestHeavyAttack()
    {
        if (_currentWeapon == null)
            return;
        if (_controller.isGrounded)
        {
            HeavyAttack();
        }
        else
        {
            AirAttack();
        }
    }
    private IEnumerator CoHeavyAttackBuffer()
    {
        _heavyAttackBuffer = true;
        yield return new WaitForSecondsRealtime(.2f);
        _heavyAttackBuffer = false;
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (_currentWeapon != null)
        {
            if (_currentWeapon != weapon)
            {
                UnEquipWeapon();
            }           
        }
        //Instantiate weapon, normally.
        _currentWeapon = weapon; //instantiated weapon
        InitializeCurrentWeapon();

        //Instantiate weapon mesh in weaponHoldSocket
        WeaponObject weaponObject = Instantiate<WeaponObject>(_currentWeapon.weaponObjectPrefab, _weaponHoldSocket);
        _currentWeapon.currentWeaponObject = weaponObject;

        CancelFullAttack();
    }

    private void InitializeCurrentWeapon()
    {
        //Initialize attacker for each hurtbox.
        foreach(WeaponStrike strike in _currentWeapon.lightCombo)
        {
            if (strike.hurtbox)
            {
                strike.hurtbox.SetAttacker(_controller);
            }
        }
        foreach (WeaponStrike strike in _currentWeapon.heavyComboBreaks)
        {
            if (strike.hurtbox)
            {
                strike.hurtbox.SetAttacker(_controller);
            }
        }
        if (_currentWeapon.heavyAttack.hurtbox)
        {
            _currentWeapon.heavyAttack.hurtbox.SetAttacker(_controller);
        }
        if (_currentWeapon.airAttack.hurtbox)
        {
            _currentWeapon.airAttack.hurtbox.SetAttacker(_controller);
        }
    }

    public void UnEquipWeapon()
    {
        CancelFullAttack();
        //Destroy _currentWeaponObject, then destroy the child of _weaponParent.
    }

    public void LightAttack()
    {
        if (!canAttack)
        {
            if (heavyAttackBufferCoroutine != null)
            {
                StopCoroutine(heavyAttackBufferCoroutine);
            }
            _heavyAttackBuffer = false;

            lightAttackBufferCoroutine = StartCoroutine(CoLightAttackBuffer());

            return;
        }

        if (_comboCounter >= 0)
        {
             //We're in a combo.
            _lightAttackBuffer = false;
            //Debug.Log("Combo'd light attack!");
            _comboCounter++;
            //Debug.Log(_comboCounter);
            CancelFollowThrough();
            _currentWeaponStrike = _currentWeapon.lightCombo[_comboCounter];
            DoWindUp();

        }
        else
        {
            _lightAttackBuffer = false;
            //Debug.Log("Started light attack!");
            _comboCounter++;
            _currentWeaponStrike = _currentWeapon.lightCombo[_comboCounter];
            DoWindUp();
        }

        //if _comboCounter > 0, means we're in a combo already.
        //if yes, do _comboCounter's index of current weapon's lightattacks.
        //else,
        //do the first attack out of the list of the current weapon's lightattacks.
        //then,
        //_comboCounter++;
    }

    public void HeavyAttack()
    {
        if (!canAttack)
        {
            if (lightAttackBufferCoroutine != null)
            {
                StopCoroutine(lightAttackBufferCoroutine);
            }
            _lightAttackBuffer = false;

            heavyAttackBufferCoroutine = StartCoroutine(CoHeavyAttackBuffer());

            return;
        }

        if (_comboCounter >= 0)
        {
            //We're in a combo - perform a heavy combo break.
            _heavyAttackBuffer = false;
            Debug.Log("Heavy combo break!");
            CancelFollowThrough();
            _projectileSpawnCounter = 0;
            _currentWeaponStrike = _currentWeapon.heavyComboBreaks[_comboCounter];
            DoWindUp();

        }
        else
        {
            _heavyAttackBuffer = false;
            Debug.Log("Started heavy attack!");
            _comboCounter++;
            _currentWeaponStrike = _currentWeapon.heavyAttack;
            DoWindUp();
        }
    }

    private void AirAttack()
    {
        if (_currentWeapon.airAttack.attack == null)
            return;

        if (!canAttack)
        {
            if (heavyAttackBufferCoroutine != null)
            {
                StopCoroutine(heavyAttackBufferCoroutine);
            }
            _heavyAttackBuffer = false;

            lightAttackBufferCoroutine = StartCoroutine(CoLightAttackBuffer());

            return;
        }

        _cancelAttackOnLand = true;

        _lightAttackBuffer = false;
        Debug.Log("Started aerial attack!");
        _currentWeaponStrike = _currentWeapon.airAttack;
        DoWindUp();
    }

    public void SpecialAttack(WeaponAttack specialAttack)
    {
        if (!canAttack)
            return;

        if (heavyAttackBufferCoroutine != null)
        {
            StopCoroutine(heavyAttackBufferCoroutine);
        }
        if (lightAttackBufferCoroutine != null)
        {
            StopCoroutine(lightAttackBufferCoroutine);
        }
        _lightAttackBuffer = false;
        _heavyAttackBuffer = false;

        WeaponStrike newstrike = new WeaponStrike();
        newstrike.attack = specialAttack;

        _currentWeaponStrike = newstrike;
        DoWindUp();
    }

    private void DoWindUp()
    {
        windUpCoroutine = StartCoroutine(CoWindUp(_currentWeaponStrike.attack.windUpAnimTime));
        _attackTimer = _currentWeaponStrike.attack.windUpAnimTime;

        _controller.CharacterOrientation(true);

        OnWindUp?.Invoke(_currentWeaponStrike.attack);
    }

    private void DoAttack()
    {
        attackCoroutine = StartCoroutine(CoAttack(_currentWeaponStrike.attack.attackAnimTime));
        _attackTimer = _currentWeaponStrike.attack.attackAnimTime;

        OnAttack?.Invoke(_currentWeaponStrike.attack);
    }

    private void DoFollowThrough()
    {
        followThroughCoroutine = StartCoroutine(CoFollowThrough(_currentWeaponStrike.attack.followThroughAnimTime));
        _attackTimer = _currentWeaponStrike.attack.followThroughAnimTime;

        OnFollowThrough?.Invoke(_currentWeaponStrike.attack);
    }

    private IEnumerator CoWindUp(float duration)
    {
        yield return new WaitForSeconds(duration);
        windUpCoroutine = null;
        OnWindupEndCallback();
    }
    private IEnumerator CoFollowThrough(float duration)
    {
        yield return new WaitForSeconds(duration);
        followThroughCoroutine = null;
        OnFollowThroughEndCallback();
    }
    private IEnumerator CoAttack(float duration)
    {
        yield return new WaitForSeconds(duration);
        attackCoroutine = null;
        OnAttackEndCallback();
    }

    private void OnWindupEndCallback()
    {
        DoAttack();
    }

    private void OnFollowThroughEndCallback()
    {
        //End attack;
        _currentWeapon.ResetHurtBoxes();
        _currentWeaponStrike = null;
        _comboCounter = -1;
        _projectileSpawnCounter = 0;
        _cancelAttackOnLand = false;
        Debug.Log("ended attack!");
    }

    private void OnAttackEndCallback()
    {
        DoFollowThrough();
    }

    public void CancelAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        attackCoroutine = null;
        _currentWeapon.ResetHurtBoxes();
    }
    public void CancelWindup()
    {
        if (windUpCoroutine != null)
        {
            StopCoroutine(windUpCoroutine);
        }

        windUpCoroutine = null;
        _currentWeapon.ResetHurtBoxes();
    }
    public void CancelFollowThrough()
    {
        if (followThroughCoroutine != null)
        {
            StopCoroutine(followThroughCoroutine);
        }

        followThroughCoroutine = null;
        _currentWeapon.ResetHurtBoxes();
    }

    public void CancelFullAttack()
    {
        CancelAttack();
        CancelWindup();
        CancelFollowThrough();

        _currentWeapon.ResetHurtBoxes();
        _currentWeaponStrike = null;
        _comboCounter = -1;
        _projectileSpawnCounter = 0;
        _cancelAttackOnLand = false;

        OnCancelFullAttack?.Invoke();
    }
}
