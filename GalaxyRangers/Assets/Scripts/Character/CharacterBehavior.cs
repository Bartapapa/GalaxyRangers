using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehavior : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    [Space]
    [SerializeField] private BaseCharacterController _characterController;
    [SerializeField] private CharacterHealth _characterHealth;
    [SerializeField] private CharacterCombat _characterCombat;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Transform _characterMesh;
    [SerializeField] private Animator _animator;
    public Animator animator { get { return _animator; } }

    [Header("BEHAVIOR")]
    [SerializeField] private float _shakeStrength = 1f;
    [SerializeField] private float _heavyLandingFallVelocity = -2f;
    [SerializeField] private float _hitFlashDuration = .1f;
    [SerializeField] private float _hitFlashIntensity = 50f;

    //Cached
    private Vector2 velocity { get { return _characterController.rigid.velocity; } }
    private float airDirection = 0f;
    private float runSpeedFactor = 1f;
    private float groundTilt = 0f;
    private int uTurnDirection;
    private bool _hit;
    private bool _shake;

    private Coroutine hitFlashCoroutine;


    private void Start()
    {
        InitEvents();
    }

    private void Update()
    {
        GetValuesFromController();
        ApplyAnimatorParams();

        HitLag();
        HitLagShake();
    }

    private void GetValuesFromController()
    {
        airDirection = Mathf.Lerp(airDirection,
            _characterController.isGrounded ? 0f : (_characterController.rigidbodyVelocity.x / _characterController.maxSpeed * _characterController.leftRight),
            10f * Time.deltaTime);
    }

    private void ApplyAnimatorParams()
    {
        if (animator == null)
            return;

        animator.SetBool("isMoving", _characterController.isMoving);
        animator.SetBool("isGrounded", _characterController.isGrounded);
        animator.SetBool("isJumping", _characterController.isJumping);
        animator.SetBool("isDashing", _characterController.isDashing);

        animator.SetFloat("runSpeedFactor", runSpeedFactor);
        animator.SetFloat("airDirection", airDirection);
        animator.SetFloat("yVelocity", velocity.y);

        float speedLerp = Mathf.Lerp(animator.GetFloat("speedLerp"), _characterController.speedLerp, 10f * Time.deltaTime);
        animator.SetFloat("speedLerp", speedLerp);

        animator.SetLayerWeight(1, _characterCombat.isAttacking ? 1 : 0);
    }

    private void InitEvents()
    {
        _characterController.OnJump += PlayJumpAnim;
        _characterController.OnWallJump += PlayWallJumpAnim;
        _characterController.OnLand += PlayLandAnim;
        _characterController.OnUTurn += PlayUTurnAnim;
        _characterController.OnDash += PlayDashAnim;
        _characterController.OnResetCharacter += ResetAnimator;
        _characterController.OnHit += PlayHurtAnim;
        _characterController.OnHitLag += StartHitLag;
        _characterController.OnEndHitlag += EndHitLag;

        _characterHealth.CharacterDied += PlayDeathAnim;
        _characterHealth.CharacterRevived += OnCharacterRevived;

        _characterCombat.OnWindUp += PlayWindUpAnim;
        _characterCombat.OnAttack += PlayAttackAnim;
        _characterCombat.OnFollowThrough += PlayFollowThroughAnim;
        _characterCombat.OnAttackCancelledOnLand += AttackCancelledOnLanding;
    }

    #region PLAYANIMS

    private void PlayJumpAnim(int jumpCount)
    {
        if (_characterHealth.isDead)
            return;

        animator.Play("Jump", 0, 0);

        //SFX
    }

    private void PlayWallJumpAnim(RaycastHit hit)
    {
        if (_characterHealth.isDead)
            return;

        animator.Play("Jump", 0, 0);

        //SFX
    }

    private void PlayLandAnim(float landVelocity, RaycastHit groundHit)
    {
        if (_characterHealth.isDead)
            return;

        if (landVelocity > _heavyLandingFallVelocity)
            animator.Play("Land", 0, 0);

        //SFX

        airDirection = 0f;
    }

    private void PlayUTurnAnim()
    {
        if (_characterHealth.isDead)
            return;

        animator.Play("uTurn", 0, 0);
        uTurnDirection = -_characterController.leftRight;
    }

    private void PlayDashAnim()
    {
        if (_characterHealth.isDead)
            return;

        animator.Play("Dash", 0, 0);

        //SFX
    }

    private void PlayHurtAnim()
    {
        if (_characterHealth.isDead)
            return;

        animator.Play("Hurt", 0, 0);

        //SFX
    }

    private void PlayWindUpAnim(WeaponAttack attack)
    {
        if (_characterHealth.isDead || attack.windUpAnimationName == "" || attack.windUpAnimTime <= 0)
            return;

        string windupAnimationName = attack.windUpAnimationName;
        float animSpeed = 1f / attack.windUpAnimTime;

        animator.SetFloat("animSpeed", animSpeed);
        animator.Play(windupAnimationName, 1, 0);
    }

    private void PlayAttackAnim(WeaponAttack attack)
    {
        if (_characterHealth.isDead || attack.attackAnimationName == "" || attack.attackAnimTime <= 0)
            return;

        string attackAnimationName = attack.attackAnimationName;
        float animSpeed = 1f / attack.attackAnimTime;

        animator.SetFloat("animSpeed", animSpeed);
        animator.Play(attackAnimationName, 1, 0);
    }

    private void PlayFollowThroughAnim(WeaponAttack attack)
    {
        if (_characterHealth.isDead || attack.followThroughAnimationName == "" || attack.followThroughAnimTime <= 0)
            return;

        string followthroughAnimationName = attack.followThroughAnimationName;
        float animSpeed = 1f / attack.followThroughAnimTime;

        animator.SetFloat("animSpeed", animSpeed);
        animator.Play(followthroughAnimationName, 1, 0);
    }

    private void StartHitLag(float duration, bool shake)
    {
        _hit = true;
        _shake = shake;

        if (_shake)
        {
            HitFlash();
        }
    }

    private void EndHitLag()
    {
        animator.speed = 1f;
        _characterMesh.localPosition = Vector3.zero;

        _hit = false;
        _shake = false;
    }

    private void HitLag()
    {
        if (!_hit)
            return;

        animator.speed = 0f;
    }

    private void HitLagShake()
    {
        if (!_shake)
            return;

        _characterMesh.localPosition = Vector3.Lerp(_characterMesh.localPosition, UnityEngine.Random.insideUnitSphere * _shakeStrength, 24f * Time.deltaTime);
    }

    private void PlayDeathAnim(CharacterHealth characterHealth)
    {
        animator.Play("Death", 0, 0);

        //SFX
    }

    private void OnCharacterRevived(CharacterHealth characterHealth)
    {
        ResetAnimator();

        //SFX
    }

    private void ResetAnimator()
    {
        animator.SetBool("isMoving", false);
        animator.SetBool("isGrounded", false);
        animator.SetBool("isJumping", false);

        animator.SetFloat("runSpeedFactor", 0);
        animator.SetFloat("speedLerp", 0);
        animator.SetFloat("airDirection",0);
        animator.SetFloat("yVelocity", 0);

        animator.Play("Locomotion", 0, 0);
    }

    private void AttackCancelledOnLanding()
    {
        PlayLandAnim(_characterController.rigid.velocity.y, _characterController.groundHit);
    }
    #endregion

    #region HITFLASH
    private void HitFlash()
    {
        if (hitFlashCoroutine != null)
        {
            StopCoroutine(hitFlashCoroutine);
        }
        hitFlashCoroutine = StartCoroutine(CoHitFlash(_hitFlashDuration));
    }

    private IEnumerator CoHitFlash(float overtime)
    {
        float timer = 0f;
        Renderer[] renderers = _characterMesh.GetComponentsInChildren<Renderer>();
        while (timer < overtime)
        {
            foreach(Renderer rend in renderers)
            {
                foreach(Material mat in rend.materials)
                {
                    float flashLerp = Mathf.Lerp(_hitFlashIntensity, 1f, timer / overtime);
                    mat.color = Color.white * flashLerp;
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                mat.color = Color.white;
            }
        }
    }
    #endregion
}
