using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GalaxyRangers/Combat/WeaponAttack")]
public class WeaponAttack : ScriptableObject
{
    [Header("ATTACK PARAMETERS")]
    [Space]
    [SerializeField] private bool _canBeCharged;
    public bool canBeCharged { get { return _canBeCharged; } }
    [SerializeField] private float _minChargeTime = .1f;
    public float minChargeTime { get { return _minChargeTime; } }
    [SerializeField] private float _maxChargeTime = 1f;
    public float maxChargeTime { get { return _maxChargeTime; } }

    [Header("ANIMATION")]
    [Space]
    [SerializeField] private string _windUpAnimationName = "";
    public string windUpAnimationName { get { return _windUpAnimationName; } }
    [SerializeField] private string _attackAnimationName = "";
    public string attackAnimationName { get { return _attackAnimationName; } }
    [SerializeField] private string _followThroughAnimationName = "";
    public string followThroughAnimationName { get { return _followThroughAnimationName; } }
    [SerializeField] private string _chargedAttackAnimationName = "";
    public string chargedAttackAnimationName { get { return _chargedAttackAnimationName; } }
    [Space]

    [Header("ANIMATION TIMING")]
    [Space]
    [SerializeField] private float _windUpAnimTime;
    public float windUpAnimTime { get { return _windUpAnimTime; } }
    [SerializeField] private float _attackAnimTime;
    public float attackAnimTime { get { return _attackAnimTime; } }
    [SerializeField] private float _followThroughAnimTime;
    public float followThroughAnimTime { get { return _followThroughAnimTime; } }
    [SerializeField] private float _chargedAttackAnimTime;
    public float chargedAttackAnimTime { get { return _chargedAttackAnimTime; } }
    [Space]

    [Header("ATTACK TIMING")]
    [Space]
    [SerializeField] private float _attackStartTime;
    public float attackStartTime { get { return _attackStartTime; } }
    [SerializeField] private float _attackDuration;
    public float attackDuration { get { return _attackDuration; } }
    [Space]

    [Header("COMBO")]
    [Space]
    [SerializeField] private bool _canCombo;
    public bool canCombo { get { return _canCombo; } }
    [SerializeField] private float _comboStartTime;
    public float comboFlagStartTime { get { return _comboStartTime; } }
    [SerializeField] private float _comboFlagDuration;
    public float comboFlagDuration { get { return _comboFlagDuration; } }

    [Header("HYPERARMOR")]
    [Space]
    [SerializeField] private bool _createsHyeprArmor;
    public bool createsHyperArmor { get { return _createsHyeprArmor; } }
    [SerializeField] private float _hyperArmorStartTime;
    public float hyperArmorStartTime { get { return _hyperArmorStartTime; } }
    [SerializeField] private float _hyperArmorDuration;
    public float hyperArmorDuration { get { return _hyperArmorDuration; } }

}
