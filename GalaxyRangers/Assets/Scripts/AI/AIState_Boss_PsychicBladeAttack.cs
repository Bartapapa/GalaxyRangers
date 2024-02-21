using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Boss_PsychicBladeAttack : AIState
{
    public AIState idleState;
    public AIState_Boss_AttackDisplacement attackDisplacement;

    private bool _initialized = false;

    public List<BossIllusion> illusions = new List<BossIllusion>();
    public List<Transform> illusionWaypoints = new List<Transform>();

    private float _timeBeforeAttack = 2f;
    private float _timeAfterAttack = 2f;
    private float _currentTimer = float.MaxValue;
    private int _numberOfAttacks = 3;
    private int _currentNumberOfAttacks = 0;
    private bool _attackedOnce = false;
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

        if (_currentTimer > 0)
        {
            _currentTimer -= Time.deltaTime;
        }
        else
        {
            if (_currentNumberOfAttacks < _numberOfAttacks)
            {
                if (!_attackedOnce)
                {
                    //Attack first time
                    _currentNumberOfAttacks++;
                    _currentTimer = _timeBeforeAttack;
                    _attackedOnce = true;
                    brain.behavior.PlayAnim("Illusion");
                    foreach (BossIllusion illusion in illusions)
                    {
                        illusion.controller.characterBehavior.PlayAnim("Illusion");
                    }
                    //Add event to anim so that projectile spawns at same point.
                    return new AIreturn(this, simulatedInputs);
                }
                else
                {
                    //Move
                    _attackedOnce = false;
                    foreach (BossIllusion illusion in illusions)
                    {
                        illusion.bossAI.currentState.ResetState();
                    }
                    DistributeDestinationWayPoints(brain);
                    foreach (BossIllusion illusion in illusions)
                    {
                        illusion.bossAI.currentState = illusion.illusionDisplacementState;
                    }
                    return new AIreturn(attackDisplacement, simulatedInputs);
                }
            }
            else
            {
                //Revert to idle state. Disapparate illusions.
                foreach (BossIllusion illusion in illusions)
                {
                    illusion.controller.characterBehavior.PlayAnim("Illusion");
                    illusion.bossAI.currentState.ResetState();
                    illusion.bossAI.currentState = illusion.bossAI.defaultState;
                    illusion.Disapparate();
                }
                ResetState();
                return new AIreturn(idleState, simulatedInputs);
            }
        }

        //Default
        return new AIreturn(this, simulatedInputs);
    }

    public override void ResetState()
    {
        _initialized = false;

        _currentTimer = float.MaxValue;

        _attackedOnce = false;

        _currentNumberOfAttacks = 0;
    }

    private void InitializeState(AIBrain_Base brain)
    {
        _initialized = true;

        _currentTimer = _timeBeforeAttack;
    }

    private void DistributeDestinationWayPoints(AIBrain_Base brain)
    {
        List<Transform> potentialWaypoints = new List<Transform>();
        foreach (Transform wp in illusionWaypoints)
        {
            potentialWaypoints.Add(wp);
        }
        int randomInt0 = UnityEngine.Random.Range(0, potentialWaypoints.Count);
        attackDisplacement.toWayPoint = potentialWaypoints[randomInt0];
        potentialWaypoints.Remove(potentialWaypoints[randomInt0]);

        foreach (BossIllusion illusion in illusions)
        {
            int randomInt1 = UnityEngine.Random.Range(0, potentialWaypoints.Count);
            illusion.illusionDisplacementState.toWayPoint = potentialWaypoints[randomInt1];
            potentialWaypoints.Remove(potentialWaypoints[randomInt1]);
        }
    }
}
