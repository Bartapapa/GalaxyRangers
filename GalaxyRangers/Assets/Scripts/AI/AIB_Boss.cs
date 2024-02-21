using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIB_Boss : AIBrain_Base
{
    [Header("BOSS STATES")]
    public AIState illuApparateState;

    [Header("PROJECTILES")]
    public Projectile realPsychicBlade;
    public Projectile fakePsychicBlade;

    public Transform psychicBladeSpawner;

    protected override void Start()
    {
        base.Start();

        health.CharacterDied -= OnBossDeath;
        health.CharacterDied += OnBossDeath;
    }

    public override void OnAnimationCallback(int index)
    {
        if (index == 0)
        {
            //Change state to illusion appear.
            _currentState.ResetState();
            _currentState = illuApparateState;
            return;
        }
        if (index == 1)
        {
            //Shoot real projectile
            Projectile newProjectile = Instantiate<Projectile>(realPsychicBlade, psychicBladeSpawner.position, psychicBladeSpawner.rotation);
            if (WorldManager.Instance)
            {
                if (WorldManager.Instance.currentRogueRoom.resetParent != null)
                {
                    newProjectile.transform.parent = WorldManager.Instance.currentRogueRoom.resetParent;
                }
            }
            newProjectile.InitializeProjectile(psychicBladeSpawner.forward);
        }
        if (index == 2)
        {
            //Shoot fake projectile
            Projectile newProjectile = Instantiate<Projectile>(fakePsychicBlade, psychicBladeSpawner.position, psychicBladeSpawner.rotation);
            if (WorldManager.Instance)
            {
                if (WorldManager.Instance.currentRogueRoom.resetParent != null)
                {
                    newProjectile.transform.parent = WorldManager.Instance.currentRogueRoom.resetParent;
                }
            }
            newProjectile.InitializeProjectile(psychicBladeSpawner.forward);
        }
    }

    private void OnBossDeath(CharacterHealth health)
    {
        controller.rigid.freezeRotation = true;
        controller.rigid.constraints = RigidbodyConstraints.FreezePosition;
    }
}
