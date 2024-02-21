using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BCC_Boss : BaseCharacterController
{
    protected override void Start()
    {
        base.Start();

        hasHyperArmor = true;
        _isJumping = true;
    }

    protected override void FixedUpdate()
    {
        FixedCacheData();

        Gravity();

        GroundDetection();
        WallDetection();
        CeilingDetection();

        CharacterOrientation();
    }
}
