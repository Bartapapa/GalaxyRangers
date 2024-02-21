using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCB_Lucy : AnimatorCallbacks
{
    public CharacterSpeciality_1 special1;

    public override void AnimationCallback(int callback)
    {
        if (callback == 0)
        {
            special1.ShootProjectileCallback();
        }
    }
}
