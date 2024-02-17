using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [Header("WHO DOES THIS HURT?")]
    [Space]
    [SerializeField] private List<GameFaction> _hurtFactions = new List<GameFaction>();
    public List<GameFaction> hurtFactions { get { return _hurtFactions; } }

    [Header("PARAMETERS")]
    [Space]
    [SerializeField] private float _damage = 1f;
    public float damage { get { return _damage; } }
    [SerializeField] private float _knockbackForce = 5f;
    public float knockbackForce { get { return _knockbackForce; } }
    [SerializeField] private Vector3 _overrideKnockbackDirection = Vector3.zero;
    public Vector3 overrideKnockbackDirection { get { return _overrideKnockbackDirection; } }
    [SerializeField] private float _disableInputDuration = .3f;
    [SerializeField] private float _hitLagDuration = .2f;
    [SerializeField] private bool _pierceInvulnerability = false;
    [SerializeField] private float _invulnerabilityDuration = .5f;

    [Header("ATTACKER")]
    [Space]
    [SerializeField][ReadOnlyInspector] private BaseCharacterController _attacker;
    public BaseCharacterController attacker { get { return _attacker; } }
    [SerializeField] private bool _usePogoForce = false;
    public bool usePogoForce { get { return _usePogoForce; } }
    [SerializeField] private bool _onlyPogoInAir = true;
    public bool onlyPogoInAir { get { return _onlyPogoInAir; } }
    [SerializeField] private float _pogoForce = 1f;
    public float pogoForce { get { return _pogoForce; } }
    [SerializeField] private Vector3 _pogoDirection = Vector3.zero;
    public Vector3 pogoDirection { get { return _pogoDirection; } }

    private Collider _collider;
    public Collider collider { get { return _collider ? _collider : _collider = GetComponent<Collider>(); } }

    public delegate void HurtBoxCallback(HurtBox hurtBox);
    public HurtBoxCallback OnHit;
    public HurtBoxCallback OnPogo;

    private void Awake()
    {
        collider.enabled = true;
        collider.isTrigger = true;
    }

    public void SetAttacker(BaseCharacterController character)
    {
        _attacker = character;
    }

    private void OnTriggerStay(Collider other)
    {
        BaseCharacterController charController = other.GetComponent<BaseCharacterController>();
        if (charController)
        {
            TriggerHit(charController);
        }

        //Potentially also check Destructibles?
    }

    public void EnableHurtBox()
    {
        collider.enabled = true;
    }

    public void DisableHurtBox()
    {
        collider.enabled = false;
    }

    public void TriggerHit(BaseCharacterController character)
    {
        if (!character.hit && _hurtFactions.Contains(character.faction) && !character.characterHealth.isInvulnerable && !character.characterHealth.isDead)
        {
            Vector2 attackerVel = Vector2.zero;
            if (_attacker != null)
            {
                attackerVel = _attacker.rigid.velocity;
            }

            if (_attacker != null)
            {
                _attacker.HitLag(_hitLagDuration);
            }

            if (_usePogoForce && _pogoDirection != Vector3.zero && _attacker != null)
            {
                if (_onlyPogoInAir && _attacker.isGrounded)
                {

                }
                else
                {
                    _pogoDirection = _pogoDirection.normalized;
                    Vector3 pogoDir = ((transform.rotation * _pogoDirection) * _pogoForce) + new Vector3(attackerVel.x, 0f, 0f);
                    _attacker.CacheVelocity(pogoDir);
                    //_attacker.CharacterImpulse(pogoDir);

                    OnPogo?.Invoke(this);
                }
            }

            if (_overrideKnockbackDirection != Vector3.zero)
            {
                _overrideKnockbackDirection = _overrideKnockbackDirection.normalized;
            }

            character.Hit(_damage, _collider, _knockbackForce, transform.rotation * _overrideKnockbackDirection, _disableInputDuration, _hitLagDuration, _pierceInvulnerability, _invulnerabilityDuration);

            OnHit?.Invoke(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_overrideKnockbackDirection != Vector3.zero)
        {
            Vector3 normalizedOverride = _overrideKnockbackDirection.normalized;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + (transform.rotation * normalizedOverride));
        }

        if (_usePogoForce && _pogoDirection != Vector3.zero)
        {
            Vector3 normalizedPogo = _pogoDirection.normalized;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + (transform.rotation * normalizedPogo));
        }
    }
}
