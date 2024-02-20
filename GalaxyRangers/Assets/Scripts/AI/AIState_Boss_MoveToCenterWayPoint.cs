using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Boss_MoveToCenterWayPoint : AIState
{
    [SerializeField] private AIState bladeAttackState;

    [Header("WAYPOINTS")]
    [Space]
    public Transform centerWayPoint;
    [SerializeField][ReadOnlyInspector] private Transform _currentWayPoint;

    private AIBrain_Base _currentBrain;
    private bool _initialized = false;

    public bool isAtCurrentWayPoint { get { return _currentWayPoint ? Vector3.Distance(_currentBrain.controller.characterCenter, _currentWayPoint.position) <= 3f : false; } }

    public override AIreturn Tick(AIBrain_Base brain)
    {
        PlayerInputs simulatedInputs = new PlayerInputs();
        if (!_initialized)
        {
            InitializeState(brain);
        }

        MoveToCurrentWayPoint(brain);

        if (isAtCurrentWayPoint)
        {
            ResetState();
            return new AIreturn(bladeAttackState, simulatedInputs);
        }

        //Default
        return new AIreturn(this, simulatedInputs);
    }

    public override void ResetState()
    {
        _initialized = false;

        _currentWayPoint = null;
    }

    private void InitializeState(AIBrain_Base brain)
    {
        _currentBrain = brain;

        _currentWayPoint = centerWayPoint;

        _initialized = true;
    }

    private void MoveToCurrentWayPoint(AIBrain_Base brain)
    {
        if (_currentWayPoint != null)
        {
            //Movement logic
            Vector2 direction = (Vector2)_currentWayPoint.position - (Vector2)brain.controller.characterCenter;
            direction = direction.normalized;

            //Lerp from actual velocity to desired velocity
            Vector2 lerpdVel = Vector2.Lerp(brain.controller.rigidbodyVelocity, direction * brain.controller.maxSpeed, brain.controller.acceleration * Time.deltaTime);
            brain.controller.SetRigidbodyVelocity(lerpdVel);
        }
    }
}
