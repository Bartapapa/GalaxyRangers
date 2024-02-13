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

    private Collider _collider;
    public Collider collider { get { return _collider ? _collider : _collider = GetComponent<Collider>(); } }

    public delegate void HurtBoxCallback(HurtBox hurtBox);
    public HurtBoxCallback OnHit;

    private void Awake()
    {
        collider.enabled = true;
        collider.isTrigger = true;
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
            if (_overrideKnockbackDirection != Vector3.zero)
            {
                _overrideKnockbackDirection = _overrideKnockbackDirection.normalized;
            }

            character.Hit(_damage, _collider, _knockbackForce, transform.rotation * _overrideKnockbackDirection);

            OnHit?.Invoke(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_overrideKnockbackDirection == Vector3.zero)
            return;

        Vector3 normalizedOverride = _overrideKnockbackDirection.normalized;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (transform.rotation * normalizedOverride));
    }
}
