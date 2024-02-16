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
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Animator _animator;
    public Animator animator { get { return _animator; } }

    [Header("BEHAVIOR")]
    [SerializeField] private float _shakeStrength = 1f;
    [SerializeField] private float _heavyLandingFallVelocity = -2f;

    //Cached
    private Vector2 velocity { get { return _characterController.rigid.velocity; } }
    private float airDirection = 0f;
    private float runSpeedFactor = 1f;
    private float groundTilt = 0f;
    private int uTurnDirection;
    private bool _hit;
    private bool _shake;

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
            4f * Time.deltaTime);
    }

    private void ApplyAnimatorParams()
    {
        animator.SetBool("isMoving", _characterController.isMoving);
        animator.SetBool("isGrounded", _characterController.isGrounded);
        animator.SetBool("isJumping", _characterController.isJumping);
        animator.SetBool("isDashing", _characterController.isDashing);

        animator.SetFloat("runSpeedFactor", runSpeedFactor);
        animator.SetFloat("airDirection", airDirection);
        animator.SetFloat("yVelocity", velocity.y);

        float speedLerp = Mathf.Lerp(animator.GetFloat("speedLerp"), _characterController.speedLerp, 10f * Time.deltaTime);
        animator.SetFloat("speedLerp", speedLerp);
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
    }

    #region PLAYANIMS

    private void PlayJumpAnim(int jumpCount)
    {
        animator.Play("Jump", 0, 0);

        //SFX
    }

    private void PlayWallJumpAnim(RaycastHit hit)
    {
        animator.Play("Jump", 0, 0);

        //SFX
    }

    private void PlayLandAnim(float landVelocity, RaycastHit groundHit)
    {
        if (landVelocity < _heavyLandingFallVelocity)
            animator.Play("Land", 0, 0);

        //SFX

        airDirection = 0f;
    }

    private void PlayUTurnAnim()
    {
        animator.Play("uTurn", 0, 0);
        uTurnDirection = -_characterController.leftRight;
    }

    private void PlayDashAnim()
    {
        animator.Play("Dash", 0, 0);

        //SFX
    }

    private void PlayHurtAnim()
    {
        animator.Play("Hurt", 0, 0);

        //SFX
    }

    private void StartHitLag(float duration, bool shake)
    {
        _hit = true;
        _shake = shake;
    }

    private void EndHitLag()
    {
        animator.speed = 1f;

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

        transform.localPosition = Vector3.Lerp(transform.localPosition, UnityEngine.Random.insideUnitSphere * _shakeStrength, 24f * Time.deltaTime);
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

        animator.Play("Idle", 0, 0);
    }
    #endregion
}
