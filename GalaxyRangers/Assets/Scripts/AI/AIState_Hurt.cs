using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Hurt : AIState
{
    [SerializeField] private AIState idleState;

    private float _hurtTimer = 0f;

    public override AIreturn Tick(AIBrain_Base brain)
    {
        PlayerInputs simulatedInputs = new PlayerInputs();
        if (_hurtTimer < brain.hurtDuration)
        {
            _hurtTimer += Time.deltaTime;
            return new AIreturn(this, new PlayerInputs());
        }
        else
        {
            ResetState();
            return new AIreturn(idleState, new PlayerInputs());
        }

    }

    public override void ResetState()
    {
        _hurtTimer = 0f;
    }
}
