using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [Header("WHO DOES THIS HURT?")]
    [Space]
    [SerializeField] private List<GameFaction> _hurtFactions = new List<GameFaction>();

    [Header("PARAMETERS")]
    [Space]
    [SerializeField] private float _damage = 1f;
    [SerializeField] private float _knockbackForce = 5f;
    [SerializeField] private Vector3 _overrideKnockbackDirection = Vector3.zero;

    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        if (!_collider)
        {
            Debug.LogWarning(this.gameObject.name + " has no hurtbox collider!");
            return;
        }
        _collider.enabled = true;
        _collider.isTrigger = true;
    }
    private void OnTriggerStay(Collider other)
    {
        BaseCharacterController charController = other.GetComponent<BaseCharacterController>();
        if (charController)
        {
            if (!charController.hit && _hurtFactions.Contains(charController.faction))
            {
                if (_overrideKnockbackDirection != Vector3.zero)
                {
                    _overrideKnockbackDirection = _overrideKnockbackDirection.normalized;
                }

                charController.Hit(_damage, _collider, _knockbackForce, transform.rotation * _overrideKnockbackDirection);
            }
        }

        //Potentially also check Destructibles?
    }

    public void EnableHurtBox()
    {
        _collider.enabled = true;
    }

    public void DisableHurtBox()
    {
        _collider.enabled = false;
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
