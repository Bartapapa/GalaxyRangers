using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Boss_BladeAttack : AIState
{
    private bool _initialized = false;

    public List<BossIllusion> illusions = new List<BossIllusion>();

    public override AIreturn Tick(AIBrain_Base brain)
    {
        PlayerInputs simulatedInputs = new PlayerInputs();
        if (!_initialized)
        {
            InitializeAttackState(brain);
        }

        //Center character
        Vector2 lerpdVel = Vector2.Lerp(brain.controller.rigidbodyVelocity, Vector2.zero, brain.controller.acceleration * Time.deltaTime * 5f);
        brain.controller.SetRigidbodyVelocity(lerpdVel);

        //Default
        return new AIreturn(this, simulatedInputs);
    }

    private void InitializeAttackState(AIBrain_Base brain)
    {
        _initialized = true;

        //Play animation
        brain.behavior.PlayAnim("IllusionEvent");
        foreach (BossIllusion illusion in illusions)
        {
            illusion.controller.characterBehavior.PlayAnim("Illusion");
        }
    }

    public override void ResetState()
    {
        _initialized = false;
    }
}
