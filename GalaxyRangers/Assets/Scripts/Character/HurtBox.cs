using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
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
        PlayerCharacterController charController = other.GetComponent<PlayerCharacterController>();
        if (charController)
        {
            Debug.Log("!");
            if (!charController.hit)
            {
                charController.Hit(_damage, _collider, _knockbackForce);
            }
        }
    }
}
