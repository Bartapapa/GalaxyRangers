using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpeciality_1 : MonoBehaviour
{
    [SerializeField] private CharacterCombat combat;
    [SerializeField] private WeaponAttack specialAttack;
    [SerializeField] private Transform psychicBladeSpawnerR;
    [SerializeField] private Transform psychicBladeSpawnerL;
    [SerializeField] private Projectile psychicBladeHeroProjectile;

    [SerializeField] private bool _capacity_isUsed = false;
    [SerializeField] private float _capacityDurationTimer = 10.0f;
    private float _capacityCooldownTimer = 0.0f;
    public float _capacityLoadValue = 0.0f;



    // Update is called once per frame
    void Update()
    {
        CapacityCooldown();
    }


    private void CapacityCooldown()
    {
        if (_capacity_isUsed == true)
        {
            _capacityCooldownTimer += Time.deltaTime;
            if (_capacityCooldownTimer >= _capacityDurationTimer)
            {
                _capacity_isUsed = false;
                _capacityCooldownTimer = 0.0f;
            }
            _capacityLoadValue = 1 - (_capacityCooldownTimer / _capacityDurationTimer);
        }
        else
        {
            _capacityLoadValue = 0f;
        }
    }
    public void TryToLaunchAbility()
    {
        if (_capacity_isUsed == false) {
            _capacity_isUsed = true;
            _capacityCooldownTimer = 0.0f;
            _capacityLoadValue = 1.0f;
            LaunchAbility();
        }
    }

    // Si dessous la fonction pour lancer la competence
    private void LaunchAbility()
    {
        //Play animation.
        combat.SpecialAttack(specialAttack);
    }

    public void ShootProjectileCallback()
    {
        //R proj
        Projectile newRProjectile = Instantiate<Projectile>(psychicBladeHeroProjectile, psychicBladeSpawnerR.position, psychicBladeSpawnerR.rotation);
        if (WorldManager.Instance)
        {
            if (WorldManager.Instance.currentRogueRoom.resetParent != null)
            {
                newRProjectile.transform.parent = WorldManager.Instance.currentRogueRoom.resetParent;
            }
        }
        newRProjectile.InitializeProjectile(psychicBladeSpawnerR.forward);

        //L proj
        Projectile newLProjectile = Instantiate<Projectile>(psychicBladeHeroProjectile, psychicBladeSpawnerL.position, psychicBladeSpawnerL.rotation);
        if (WorldManager.Instance)
        {
            if (WorldManager.Instance.currentRogueRoom.resetParent != null)
            {
                newLProjectile.transform.parent = WorldManager.Instance.currentRogueRoom.resetParent;
            }
        }
        newLProjectile.InitializeProjectile(psychicBladeSpawnerL.forward);
    }
}
