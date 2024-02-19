using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Aim : AIState
{
    [SerializeField] private AIState inCombatState;

    [Header("AIMLINE")]
    public AimLine aimLinePrefab;

    //Cached
    private AimLine _aimLine;
    public AimLine aimLine { get { return _aimLine?_aimLine: _aimLine = Instantiate<AimLine>(aimLinePrefab, transform); } }

    private float _aimingTimer = 0f;
    private float _aimingFlickerDuration = .3f;

    private Vector3 _aimingTarget = Vector3.zero;
    private Vector3 _aimingPoint = Vector3.zero;

    public override AIreturn Tick(AIBrain_Base brain)
    {
        PlayerInputs simulatedInputs = new PlayerInputs();

        _aimingTarget = brain.playerTransform.position + Vector3.up;

        if (_aimingTimer < brain.aimDuration)
        {
            _aimingTimer += Time.deltaTime;

            //brain.combat.currentWeapon.projectileSource.LookAt(_aimingTarget);

            RaycastHit hit;
            float distance = Vector3.Distance(brain.combat.currentWeapon.projectileSource.position, _aimingTarget);
            _aimingPoint = Physics.Raycast(brain.combat.currentWeapon.projectileSource.position, _aimingTarget - brain.combat.currentWeapon.projectileSource.position,
                out hit,
                distance) ?
                hit.point :
                brain.combat.currentWeapon.projectileSource.forward * distance;

            aimLine.UpdateLine(brain.combat.currentWeapon.projectileSource.position, _aimingPoint);

            Vector3 forwardDir = Vector3.right * brain.controller.facingRight;
            if (forwardDir == Vector3.zero) forwardDir = brain.transform.forward;
            Vector3 targetDir = brain.playerTransform.position - brain.transform.position;
            targetDir = targetDir.normalized;
            float upDown = Mathf.Sign(brain.playerTransform.position.y - brain.transform.position.y);
            float angle = Vector3.Angle(forwardDir, targetDir) * upDown;
            brain.SetBehaviorFloat("aimDir", angle / 90f);

            //Set brain's Animator aimDir to 1 (up), 0 (forward) or -1 (down) depending on player pos.
        }
        else
        {
            //Shoot
            brain.combat.RequestLightAttack();
            brain.attackCooldown = brain.combat.currentWeaponStrike.attack.comboEndAttackCooldown;
            ResetState();

            //Set brain's Animator isAiming to false.
            brain.SetBehaviorBool("isAiming", false);
            return new AIreturn(inCombatState, simulatedInputs);
        }



        //Aim for x seconds, from projectileSource to aimingTarget
        //aimingTarget = playerTransform + Vector3.up;
        //aimingPoint is at point of raycast from projectileSource + random Y offset.
        //Aim projectileSource to aimingPoint
        //Set origin point of lineRenderer to projectileSource, destination point to aimingPoint;

        //Default
        return new AIreturn(this, simulatedInputs);
    }

    public override void ResetState()
    {
        Debug.Log("State reset!");
        _aimingTimer = 0f;
        if (_aimLine != null) Destroy(_aimLine.gameObject);
    }
}
