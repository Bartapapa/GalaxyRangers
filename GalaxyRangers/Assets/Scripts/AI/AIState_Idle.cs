using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Idle : AIState
{
    [SerializeField] private AIState chaseState;

    public override AIreturn Tick(AIBrain_Base brain)
    {
        PlayerInputs simulatedInputs = new PlayerInputs();

        if (brain.playerTransform != null)
        {
            return new AIreturn(chaseState, simulatedInputs);
        }

        Collider[] overlaps = Physics.OverlapSphere(brain.transform.position, brain.playerDetectionDistance, brain.playerMask);
        foreach(Collider coll in overlaps)
        {
            BaseCharacterController character = coll.GetComponent<BaseCharacterController>();
            if (character)
            {
                Debug.Log(brain.name + " has detected the player!");
                brain.playerDetected = true;
                brain.playerTransform = character.transform;
                brain.controller.SetTarget(brain.playerTransform);
                brain.GetInitialAttackCooldown();
                return new AIreturn(chaseState, simulatedInputs);
            }
        }
        //Default
        return new AIreturn(this, simulatedInputs);
    }
}