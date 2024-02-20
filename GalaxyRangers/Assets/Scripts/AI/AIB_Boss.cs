using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIB_Boss : AIBrain_Base
{
    [Header("BOSS STATES")]
    public AIState illuApparateState;

    public override void OnAnimationCallback(int index)
    {
        if (index == 0)
        {
            //Change state to illusion appear.
            _currentState.ResetState();
            _currentState = illuApparateState;
        }
    }
}
