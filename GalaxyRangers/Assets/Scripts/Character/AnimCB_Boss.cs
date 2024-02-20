using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCB_Boss : AnimatorCallbacks
{
    public AIBrain_Base boss;

    public override void AnimationCallback(int callback)
    {
        boss.OnAnimationCallback(callback);
    }
}
