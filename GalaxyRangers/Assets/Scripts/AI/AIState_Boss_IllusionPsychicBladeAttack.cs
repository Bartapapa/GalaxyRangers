using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class AIState_Boss_IllusionPsychicBladeAttack : AIState
{
    private bool _initialized = false;

    public override AIreturn Tick(AIBrain_Base brain)
    {
        PlayerInputs simulatedInputs = new PlayerInputs();

        if (!_initialized)
        {
            InitializeState(brain);
        }

        //Center self
        Vector2 lerpdVel = Vector2.Lerp(brain.controller.rigidbodyVelocity, Vector2.zero, brain.controller.acceleration * 5f * Time.deltaTime);
        brain.controller.SetRigidbodyVelocity(lerpdVel);


        //Default
        return new AIreturn(this, simulatedInputs);
    }

    public override void ResetState()
    {
        _initialized = false;
    }

    private void InitializeState(AIBrain_Base brain)
    {
        _initialized = true;
    }
}
