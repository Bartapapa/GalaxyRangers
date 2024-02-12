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

    public bool canAttack { get { return _currentWeapon != null && !_controller.isDisabled && (!isAttacking || canDoCombo); } }
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
    private bool _lightAttackBuffer;
    private bool _heavyAttackBuffer;
    private WeaponStrike _currentWeaponStrike;
    public WeaponStrike currentWeaponStrike { get { return _currentWeaponStrike; } }

    //Coroutines
    private Coroutine lightAttackBufferCoroutine;
    private Coroutine heavyAttackBufferCoroutine;
    private Coroutine windUpCoroutine;
    private Coroutine attackCoroutine;
    private Coroutine followThroughCoroutine;

    private void Start()
    {
        if (_currentWeapon != null)
        {
            EquipWeapon(_currentWeapon);
        }
    }

    private void Update()
    {
        HandleAttackTimer();
        HandleCurrentStrikeHurtbox();

        if (_lightAttackBuffer && canAttack)
        {
            LightAttack();
        }
        else if (_heavyAttackBuffer && canAttack)
        {
            HeavyAttack();
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
            _currentWeaponStrike.hurtbox.EnableHurtBox();
        }
        else
        {
            _currentWeaponStrike.hurtbox.DisableHurtBox();
        }
    }

    public void RequestLightAttack()
    {
        if (_currentWeapon == null)
            return;
        LightAttack();
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
        HeavyAttack();
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


        GameObject weaponMesh = Instantiate<GameObject>(_currentWeapon.weaponMesh, _weaponHoldSocket);

        CancelFullAttack();

        //Spawn weapon's mesh on _weaponHoldSocket;
    }

    public void UnEquipWeapon()
    {
        CancelFullAttack();
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
            Debug.Log("Combo'd light attack!");
            _comboCounter++;
            Debug.Log(_comboCounter);
            CancelFollowThrough();
            _currentWeaponStrike = _currentWeapon.lightCombo[_comboCounter];
            DoWindUp();

        }
        else
        {
            _lightAttackBuffer = false;
            Debug.Log("Started light attack!");
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

    private void DoWindUp()
    {
        windUpCoroutine = StartCoroutine(CoWindUp(_currentWeaponStrike.attack.windUpAnimTime));
        _attackTimer = _currentWeaponStrike.attack.windUpAnimTime;
    }

    private void DoAttack()
    {
        attackCoroutine = StartCoroutine(CoAttack(_currentWeaponStrike.attack.attackAnimTime));
        _attackTimer = _currentWeaponStrike.attack.attackAnimTime;
    }

    private void DoFollowThrough()
    {
        followThroughCoroutine = StartCoroutine(CoFollowThrough(_currentWeaponStrike.attack.followThroughAnimTime));
        _attackTimer = _currentWeaponStrike.attack.followThroughAnimTime;
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
        _comboCounter = -1;
    }
}
