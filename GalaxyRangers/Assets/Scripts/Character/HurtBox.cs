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
                charController.Hit(_damage, _collider, _knockbackForce);
            }
        }

        //Potentially also check Destructibles?
    }
}
