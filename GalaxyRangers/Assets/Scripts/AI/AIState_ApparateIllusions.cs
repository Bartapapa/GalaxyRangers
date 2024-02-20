using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_ApparateIllusions : AIState
{
    private bool _initialized = false;

    public List<BossIllusion> illusions = new List<BossIllusion>();

    public override AIreturn Tick(AIBrain_Base brain)
    {
        PlayerInputs simulatedInputs = new PlayerInputs();

        if (!_initialized)
        {
            InitializeIllusionState(brain);
        }

        //Default
        return new AIreturn(this, simulatedInputs);
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
