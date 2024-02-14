using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Chase : AIState
{
    [SerializeField] private AIState idleState;
    [SerializeField] private AIState inCombatState;
    public override AIreturn Tick(AIBrain_Base brain)
    {
        PlayerInputs simulatedInputs = new PlayerInputs();

        if (brain.playerTransform == null)
            return new AIreturn(idleState, simulatedInputs);

        if (brain.currentPlayerDistance <= brain.combatDistance && brain.controller.isGrounded && brain.combat.currentWeapon != null)
        {
            //Activate combat mode.
            return new AIreturn(inCombatState, simulatedInputs);
        }

        // >0 means to right, <0 means to left.
        int playerToRightLeft = (int)Mathf.Sign(brain.playerTransform.position.x - brain.transform.position.x);

        // >0 means above, <0 means below.
        int playerAboveBelow = (int)Mathf.Sign(brain.playerTransform.position.y - brain.transform.position.y);

        //Run after player.
        simulatedInputs.MoveX = playerToRightLeft;

        //While running, must check for walls, edges and such as they get closer to the player.
        if ((brain.controller.isFacingLeftWall && playerToRightLeft < 0) || (brain.controller.isFacingRightWall && playerToRightLeft > 0))
        {
            if (CanJumpOverWall(brain))
            {
                if (!brain.controller.isJumping)
                {
                    brain.RequestJump();
                }
            }

            simulatedInputs.JumpPressed = true;

            //if (playerAboveBelow > 0)
            //{

            //}
        }

        //Default
        return new AIreturn(this, simulatedInputs);
    }

    private bool CanJumpOverWall(AIBrain_Base brain)
    {
        //This doesn't work with varying jump values, sue me. Don't have time to calculate the actual height differential based on jumpforce, jumptime and jumpdecay.
        float maxHeight = 5f;
        Vector3 fromPoint;
        Vector3 dir = brain.controller.leftRight > 0 ? Vector3.right : Vector3.left;
        int maxIterations = 10;

        for (int i = 1; i <= maxIterations; i++)
        {
            fromPoint = brain.transform.position + Vector3.up * i;
            if (fromPoint.y > brain.transform.position.y + maxHeight)
            {
                //Max height attained : return
                return false;
            }

            if (Physics.Raycast(fromPoint, dir, 1f, brain.controller.currentGroundLayers))
            {
                //Surface found at this height, continue checking
            }
            else
            {
                //No surface found at this height : return
                return true;
            }
        }

        return false;
    }
}
