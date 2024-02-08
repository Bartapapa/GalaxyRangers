using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    None,
    Light,
    Heavy,
}
public class CharacterCombat : MonoBehaviour
{
    [Header("WEAPON")]
    [Space]
    [SerializeField][HideInInspector] private Weapon _currentWeapon;
    [SerializeField] private Transform _weaponHoldSocket;

    private bool _isAttacking;
    public bool isAttacking { get { return _isAttacking; } }

    private Coroutine attackCoroutine;

    public void EquipWeapon(Weapon weapon)
    {
        if (_currentWeapon != null)
        {
            UnEquipWeapon();
        }

        //Spawn weapon's mesh on _weaponHoldSocket;
    }

    public void UnEquipWeapon()
    {

    }

    public void Attack(AttackType attackType)
    {
        //attackCoroutine =
    }

    public void CancelAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        _isAttacking = false;
    }
}
