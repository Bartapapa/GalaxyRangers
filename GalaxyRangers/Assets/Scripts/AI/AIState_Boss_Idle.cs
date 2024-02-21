using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Boss_Idle : AIState
{
    [SerializeField] private AIState moveToWayPointState;

    [Header("PARAMETERS")]
    public float minIdleWaitTime = 1f;
    public float maxIdleWaitTime = 5f;

    //Cached
    private float _idleWaitTimer = float.MinValue;
    private bool _initialized = false;

    public override AIreturn Tick(AIBrain_Base brain)
    {
        PlayerInputs simulatedInputs = new PlayerInputs();

        if (!_initialized)
        {
            InitializeIdle();
        }

        if (_idleWaitTimer > 0f)
        {
            _idleWaitTimer -= Time.deltaTime;

            Vector2 lerpdVel = Vector2.Lerp(brain.controller.rigidbodyVelocity, Vector2.zero, brain.controller.acceleration * Time.deltaTime * 5f);
            brain.controller.SetRigidbodyVelocity(lerpdVel);

            return new AIreturn(this, simulatedInputs);
        }
        else
        {
            ResetState();
            return new AIreturn(moveToWayPointState, simulatedInputs);
        }

        //Default
        return new AIreturn(this, simulatedInputs);
    }

    public override void ResetState()
    {
        _initialized = false;
    }

    private void InitializeIdle()
    {
        _initialized = true;

        _idleWaitTimer = UnityEngine.Random.Range(minIdleWaitTime, maxIdleWaitTime);
    }
}
