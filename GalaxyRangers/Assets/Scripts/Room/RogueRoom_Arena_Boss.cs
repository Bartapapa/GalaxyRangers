using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueRoom_Arena_Boss : RogueRoom_Arena
{
    [Header("BOSS")]
    [SerializeField] private AIBrain_Base _boss;

    protected override void StartArena()
    {
        base.StartArena();
        //Here is where the boss arena starts - show UI, etc, etc.
        UI_Manager.Instance.EnterInBossRoom(_boss.health);
        _boss.health.CharacterDied += OnBossDied;

        CameraManager.Instance.AddFocusObjectToCamera(_boss.transform, 5);
    }

    private void OnBossDied(CharacterHealth characterHealth)
    {
        //Boss has died, roll credits lmfaooooooooooooooooooo
        CameraManager.Instance.RemoveFocusObjectFromCamera(Player.Instance.CharacterController.transform);
    }
}
