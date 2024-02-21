using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class AIState_Boss_AttackDisplacement : AIState
{
    //For the illusions, link a 'fake' projectile attack state.
    public AIState psychicBladeAttackState;

    public List<AIState_Boss_AttackDisplacement> BossDisplacementStates = new List<AIState_Boss_AttackDisplacement>();

    public Transform toWayPoint;
    private bool _initialized = false;
    private AIBrain_Base _currentBrain;
    public bool isAtCurrentWayPoint { get { return toWayPoint ? Vector3.Distance(_currentBrain.controller.characterCenter, toWayPoint.position) <= 1f : false; } }
    public bool allCharsAtCurrentWayPoint { get { return AllBrainsAtCurrentWayPoint(); } }

    public override AIreturn Tick(AIBrain_Base brain)
    {
        PlayerInputs simulatedInputs = new PlayerInputs();

        if (!_initialized)
        {
            InitializeState(brain);
        }

        //Move to waypoint
        MoveToCurrentWayPoint(brain);

        if (isAtCurrentWayPoint)
        {
            Vector2 lerpdVel = Vector2.Lerp(brain.controller.rigidbodyVelocity, Vector2.zero, brain.controller.acceleration * 5f * Time.deltaTime);
            brain.controller.SetRigidbodyVelocity(lerpdVel);
            if (allCharsAtCurrentWayPoint && BossDisplacementStates.Count > 0)
            {
                ResetState();
                //Proceed to attack.
                return new AIreturn(psychicBladeAttackState, simulatedInputs);
            }
        }

        //Default
        return new AIreturn(this, simulatedInputs);
    }

    public override void ResetState()
    {
        _initialized = false;

        toWayPoint = null;
    }

    private void InitializeState(AIBrain_Base brain)
    {
        _currentBrain = brain;

        _initialized = true;
    }

    private void MoveToCurrentWayPoint(AIBrain_Base brain)
    {
        if (toWayPoint != null)
        {
            //Movement logic
            Vector2 direction = (Vector2)toWayPoint.position - (Vector2)brain.controller.characterCenter;
            direction = direction.normalized;

            //Lerp from actual velocity to desired velocity
            Vector2 lerpdVel = Vector2.Lerp(brain.controller.rigidbodyVelocity, direction * brain.controller.maxSpeed, brain.controller.acceleration * Time.deltaTime);
            brain.controller.SetRigidbodyVelocity(lerpdVel);
        }
    }

    private bool AllBrainsAtCurrentWayPoint()
    {
        bool value = true;
        foreach(AIState_Boss_AttackDisplacement brainState in BossDisplacementStates)
        {
            if (!brainState.isAtCurrentWayPoint)
            {
                value = false;
            }
        }

        return value;
    }
}
