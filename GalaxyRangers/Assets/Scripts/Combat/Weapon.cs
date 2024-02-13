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

    public void ResetHurtBoxes()
    {
        foreach(WeaponStrike weaponStrike0 in _lightCombo)
        {
            weaponStrike0.hurtbox.DisableHurtBox();
        }
        foreach(WeaponStrike weaponStrike1 in _heavyComboBreaks)
        {
            weaponStrike1.hurtbox.DisableHurtBox();
        }
        if(_heavyAttack.attack != null)
        {
            _heavyAttack.hurtbox.DisableHurtBox();
        }
    }
}
