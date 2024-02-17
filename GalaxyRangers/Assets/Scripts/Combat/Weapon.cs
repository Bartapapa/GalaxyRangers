using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponStrike
{
    public WeaponAttack attack;
    public HurtBox hurtbox;
}

public class Weapon : MonoBehaviour
{
    [Header("OBJECT REFS & ANIMATION")]
    [Space]
    public WeaponObject weaponObjectPrefab;
    [ReadOnlyInspector] public WeaponObject currentWeaponObject;
    [SerializeField] private int _stanceType = 0;
    [Space]
    [SerializeField] private Transform _projectileSource;
    public Transform projectileSource { get { return _projectileSource; } }

    [Header("LIGHT COMBO")]
    [Space]
    [SerializeField] private List<WeaponStrike> _lightCombo = new List<WeaponStrike>();
    public List<WeaponStrike> lightCombo { get { return _lightCombo; } }

    [Header("HEAVY COMBO BREAK")]
    [Space]
    [SerializeField] private List<WeaponStrike> _heavyComboBreaks = new List<WeaponStrike>();
    public List<WeaponStrike> heavyComboBreaks { get { return _heavyComboBreaks; } }

    [Header("HEAVY ATTACK")]
    [Space]
    [SerializeField] private WeaponStrike _heavyAttack;
    public WeaponStrike heavyAttack { get { return _heavyAttack; } }
    [SerializeField] private float _maxChargedHeavyAttackDamageMultiplier = 2f;
    public float maxChargedHeavyAttackDamageMultiplier { get { return _maxChargedHeavyAttackDamageMultiplier; } }

    [Header("AIR ATTACK")]
    [Space]
    [SerializeField] private WeaponStrike _airAttack;
    public WeaponStrike airAttack { get { return _airAttack; } }

    public void ResetHurtBoxes()
    {
        foreach(WeaponStrike weaponStrike0 in _lightCombo)
        {
            if (weaponStrike0.hurtbox != null) weaponStrike0.hurtbox.DisableHurtBox();

        }
        foreach(WeaponStrike weaponStrike1 in _heavyComboBreaks)
        {
            if (weaponStrike1.hurtbox != null) weaponStrike1.hurtbox.DisableHurtBox();
        }
        if(_heavyAttack.attack != null)
        {
            if (_heavyAttack.hurtbox != null) _heavyAttack.hurtbox.DisableHurtBox();
        }
        if(_airAttack.attack != null)
        {
            if (_airAttack.hurtbox != null) _airAttack.hurtbox.DisableHurtBox();
        }
    }
}
