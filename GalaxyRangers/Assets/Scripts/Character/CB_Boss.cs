using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CB_Boss : CharacterBehavior
{
    protected override void GetValuesFromController()
    {
        
    }
    protected override void ApplyAnimatorParams()
    {
        Vector2 adjustedVel = velocity / _characterController.maxSpeed;
        animator.SetFloat("yVelocity", adjustedVel.y);
        animator.SetFloat("xVelocity", adjustedVel.x);
    }
}
