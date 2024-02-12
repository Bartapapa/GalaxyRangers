using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AIreturn
{
    public AIState toState;
    public PlayerInputs inputs;

    public AIreturn(AIState state, PlayerInputs newInput)
    {
        toState = state;
        inputs = newInput;
    }
}

public abstract class AIState : MonoBehaviour
{
    public virtual AIreturn Tick(AIBrain_Base brain)
    {
        return new AIreturn(this, new PlayerInputs());
    }
}
