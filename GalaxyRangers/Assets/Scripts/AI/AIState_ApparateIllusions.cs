using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_ApparateIllusions : AIState
{
    public AIState_Boss_AttackDisplacement attackDisplacement;

    private bool _initialized = false;

    public List<BossIllusion> illusions = new List<BossIllusion>();

    public List<Transform> illusionWaypoints = new List<Transform>();

    public override AIreturn Tick(AIBrain_Base brain)
    {
        PlayerInputs simulatedInputs = new PlayerInputs();

        if (!_initialized)
        {
            InitializeIllusionState(brain);
            //Set waypoints randomly for this guy, and each illusion.
            DistributeDestinationWayPoints(brain);
            //Set state to displacement state.
            foreach(BossIllusion illusion in illusions)
            {
                if (illusion.bossAI.currentState != null)
                {
                    illusion.bossAI.currentState.ResetState();
                }

                illusion.bossAI.currentState = illusion.illusionDisplacementState;
            }
            ResetState();
            return new AIreturn(attackDisplacement, simulatedInputs);
        }

        //Default
        return new AIreturn(this, simulatedInputs);
    }

    private void DistributeDestinationWayPoints(AIBrain_Base brain)
    {
        List<Transform> potentialWaypoints = new List<Transform>();
        foreach(Transform wp in illusionWaypoints)
        {
            potentialWaypoints.Add(wp);
        }
        int randomInt0 = UnityEngine.Random.Range(0, potentialWaypoints.Count);
        attackDisplacement.toWayPoint = potentialWaypoints[randomInt0];
        potentialWaypoints.Remove(potentialWaypoints[randomInt0]);

        foreach(BossIllusion illusion in illusions)
        {
            int randomInt1 = UnityEngine.Random.Range(0, potentialWaypoints.Count);
            illusion.illusionDisplacementState.toWayPoint = potentialWaypoints[randomInt1];
            potentialWaypoints.Remove(potentialWaypoints[randomInt1]);
        }
    }

    private void InitializeIllusionState(AIBrain_Base brain)
    {
        _initialized = true;

        foreach(BossIllusion illusion in illusions)
        {
            illusion.controller.SetRigidbodyPosition(brain.transform.position);
            illusion.Apparate();
        }
    }

    public override void ResetState()
    {
        _initialized = false;
    }
}
