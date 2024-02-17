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
    [SerializeField] private string _chargingAnimationName = "";
    public string chargingAnimationName { get { return _chargingAnimationName; } }
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

    [Header("PROPULSION")]
    [Space]
    [SerializeField] private bool _doPropulsion = false;
    public bool doPropulsion { get { return _doPropulsion; } }
    [SerializeField] private float _propulsionStartTime;
    public float propulsionStartTime { get { return _propulsionStartTime; } }
    [SerializeField] private float _propulsionDuration;
    public float propulsionDuration { get { return _propulsionDuration; } }
    [SerializeField] private Vector2 _propulsionDirection = Vector2.zero;
    public Vector2 propulsionDirection { get { return _propulsionDirection.normalized; } }
    [SerializeField] private float _propulsionForce = 1f;
    public float propulsionForce { get { return _propulsionForce; } }
    [Space]

    [Header("COMBO")]
    [Space]
    [SerializeField] private bool _canCombo;
    public bool canCombo { get { return _canCombo; } }
    [SerializeField][Tooltip("The combo flag start time AFTER followThrough has been called.")] private float _comboStartTime;
    public float comboFlagStartTime { get { return _comboStartTime; } }
    [SerializeField] private float _comboFlagDuration;
    public float comboFlagDuration { get { return _comboFlagDuration; } }

    [Header("HYPERARMOR")]
    [Space]
    [SerializeField] private bool _createsHyperArmor;
    public bool createsHyperArmor { get { return _createsHyperArmor; } }
    [SerializeField] private float _hyperArmorStartTime;
    public float hyperArmorStartTime { get { return _hyperArmorStartTime; } }
    [SerializeField] private float _hyperArmorDuration;
    public float hyperArmorDuration { get { return _hyperArmorDuration; } }

    [Header("PROJECTILE")]
    [SerializeField] private Projectile _projectile;
    public Projectile projectile { get { return _projectile; } }
    [SerializeField] private float _projectileSpawnTime = 0f;
    public float projectileSpawnTime { get { return _projectileSpawnTime; } }

    [Header("AI COMBAT")]
    [Space]
    [SerializeField] private float _comboEndAttackCooldown = 1f;
    public float comboEndAttackCooldown { get { return _comboEndAttackCooldown; } }
    [SerializeField] private float _minAttackDistance = 1f;
    public float minAttackDistance { get { return _minAttackDistance; } }
    [SerializeField] private float _maxAttackDistance = 2f;
    public float maxAttackDistance { get { return _maxAttackDistance; } }
    [SerializeField] private int _attackPriority = 10;
    public int attackPriority { get { return _attackPriority; } }
    [SerializeField] private int _attackWeight = 10;
    public int attackWeight { get { return _attackWeight; } }
    [SerializeField] private int _attackType = 1;
    public int attackType { get { return _attackType; } }

}
