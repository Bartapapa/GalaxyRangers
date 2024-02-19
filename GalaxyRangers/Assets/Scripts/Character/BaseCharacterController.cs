using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public enum GameFaction
{
    None,
    Player,
    Enemy,
    Hostile,
    Destructible,
}

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public partial class BaseCharacterController : MonoBehaviour
{

    private Transform _cachedTransform;
    public Transform cachedTransform
    {
        get
        {
            if (!_cachedTransform)
                _cachedTransform = transform;

            return _cachedTransform;
        }
    }

    private CapsuleCollider _capsuleCollider;
    public CapsuleCollider capsuleCollider
    {
        get
        {
            if (!_capsuleCollider)
                _capsuleCollider = GetComponent<CapsuleCollider>();

            return _capsuleCollider;
        }
    }

    private Rigidbody _rigid;
    public Rigidbody rigid
    {
        get
        {
            if (!_rigid)
                _rigid = GetComponent<Rigidbody>();

            return _rigid;
        }
    }

    [Header("INPUTS")]
    [Space(10)]
    private bool _inputsLocked;
    public bool LockInputs
    {
        set
        {
            _inputsLocked = value;
        }
    }
    private float _moveInput;
    private float _upDownInput;
    private bool _jumpRequested;
    private bool _jumpPressed;

    [Header("COMPONENTS")]
    [Space(10)]
    [SerializeField] private CharacterBehavior _characterBehavior;
    public CharacterBehavior characterBehavior { get { return _characterBehavior; } }
    [Space]
    [SerializeField] private CharacterHealth _characterHealth;
    public CharacterHealth characterHealth { get { return _characterHealth; } }
    [Space]
    [SerializeField] private CharacterCombat _characterCombat;
    public CharacterCombat characterCombat { get { return _characterCombat; } }

    [Header("BASE CHARACTER STATS")]
    [Space]
    [SerializeField] private CharacterStats _characterStats;

    [Header("MOTION")]
    [Space(10)]
    [SerializeField] private float _rightMeshOrientation = .1f;
    [SerializeField] private float _leftMeshOrientation = 179.9f;
    [SerializeField] private bool _isMoving;
    public bool isMoving { get { return _isMoving; } }
    [Space]
    [SerializeField] private float _minSpeed = 1f;
    public float minSpeed { get { return _minSpeed; } }
    [SerializeField][ReadOnlyInspector] private CharacterStat _maxSpeed;
    public float maxSpeed { get { return _maxSpeed.MaxValue; } }
    [SerializeField] private float currentSpeed;
    [Space]
    [SerializeField][ReadOnlyInspector] private CharacterStat _speedLerpRate;
    public float acceleration { get { return _speedLerpRate.MaxValue; } }
    [Space]
    [SerializeField][Range(0f, 1f)] private float _runSpeedThresold = 0.5f;
    public float runSpeedThresold { get { return _runSpeedThresold; } }
    [Space]
    [SerializeField][Range(0f, 1f)] private float _speedLerp;
    public float speedLerp { get { return _speedLerp; } }
    [Space]
    [SerializeField][Range(-1, 1)] private int _leftRight = 1;
    public int leftRight { get { return _leftRight; } }
    [Space]
    [SerializeField] private float uTurnDelay = 0.2f;
    [SerializeField] private bool _uTurn = false;
    public bool uTurn { get { return _uTurn; } }

    [Header("PHYSICS")]
    [Space(10)]
    [SerializeField] private Vector2 _rigidbodyVelocity;
    public Vector2 rigidbodyVelocity { get { return _rigidbodyVelocity; } }
    [Space]
    [SerializeField] [ReadOnlyInspector] private LayerMask _currentGroundLayers;
    [SerializeField] private LayerMask defaultEnvironmentLayers;
    [SerializeField] private LayerMask onlyGroundLayers;
    public LayerMask currentGroundLayers { get { return _currentGroundLayers; } }
    [Space]
    [SerializeField] private float _defaultColliderHeight = 1.8f;
    public float defaultColliderHeight { get { return _defaultColliderHeight; } }
    [SerializeField] private float _defaultColliderRadius = 0.5f;
    public float defaultColliderRadius { get { return _defaultColliderRadius; } }
    [Header("Air behaviour")]
    [Space]
    [SerializeField] private CharacterStat gravity;
    [SerializeField] private CharacterStat minFallSpeed;
    [Space]
    [SerializeField] private CharacterStat fastFallGravityFactor;
    [SerializeField] private CharacterStat minFastFallSpeed;
    [SerializeField] private bool isFastFalling = false;
    [Space]
    [SerializeField] private float airTime;
    [Header("Ground behaviour")]
    [Space]
    [SerializeField] private bool _isGrounded = true;
    public bool isGrounded { get { return _isGrounded; } }
    public bool isOnOneWayPlatform { get { return _isGrounded && _groundHit.collider.gameObject.layer == 7 ? true : false; } }
    [Space]
    [SerializeField] private float groundDetectionDistance = 0.2f;
    [SerializeField] private float feetDetectionOffset = 0.1f;
    [Space]
    [SerializeField][Range(0, 90)] private float _groundAngle = 0;
    public float groundAngle { get { return _groundAngle; } }
    [SerializeField][Range(0, 90)] private int _maximumGroundAngle = 45;
    public int maximumGroundAngle { get { return _maximumGroundAngle; } }
    [Header("Ceiling behaviour")]
    [Space]
    [SerializeField] private bool _isTouchingCeiling = true;
    public bool isTouchingCeiling { get { return _isTouchingCeiling; } }
    [Space]
    [SerializeField] private float ceilingDetectionDistance = 0.2f;
    [SerializeField] private float headDetectionOffset = 0.1f;
    [Header("Jump behaviour")]
    [Space]
    [SerializeField] private bool _isJumping = false;
    public bool isJumping { get { return _isJumping; } }
    [SerializeField] private bool _wallJump = false;
    public bool wallJump { get { return _wallJump; } }
    [Space]
    [SerializeField] private CharacterStat maxJumpCount;
    [SerializeField] private int jumpCount;
    [SerializeField] private CharacterStat maxWallJumpCount;
    [SerializeField] private int wallJumpCount = 0;
    [SerializeField] private bool jumpBuffer = false;
    [Space]
    [SerializeField] private CharacterStat jumpStrength;
    [SerializeField] private CharacterStat maxJumpDuration;
    [SerializeField] private AnimationCurve jumpDosageCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField][Range(1f, 4f)] private float shortJumpSpeedFactor = 2f;
    [Header("Wall Detection")]
    [Space]
    [SerializeField] private bool _isFacingLeftWall = false;
    public bool isFacingLeftWall { get { return _isFacingLeftWall; } }
    [SerializeField] private bool _isFacingRightWall = false;
    public bool isFacingRightWall { get { return _isFacingRightWall; } }
    [Space]
    [SerializeField] private float wallDetectionDistance = 0.2f;
    [SerializeField] private AnimationCurve wallJumpMotionFactorCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [Space]
    [SerializeField][Range(0, 180)] private float wallAngle = 0;
    [SerializeField][Range(0, 180)] private int minimumWallAngle = 60;
    [SerializeField][Range(0, 180)] private int maximumWallAngle = 100;
    [Header("Edge Detection")]
    [Space]
    [SerializeField] private bool isFacingEdge = false;
    [Space]
    [SerializeField] private float minimumEdgeDetectionDistance = 0.5f;
    [SerializeField] private float maximumEdgeDetectionDistance = 1.5f;
    [Space]
    [SerializeField] private bool isFrozen = false;

    [Header("Dash behavior")]
    [Space]
    [SerializeField] private bool _isDashing = false;
    public bool isDashing { get { return _isDashing; } }
    [Space]
    [SerializeField] private CharacterStat _dashSpeed;
    public float dashSpeed { get { return _dashSpeed.MaxValue; } }
    [SerializeField] private CharacterStat _dashDuration;
    [SerializeField] private AnimationCurve dashSpeedCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float dashDuration { get { return _dashDuration.MaxValue; } }
    [SerializeField] private CharacterStat _dashCooldown;
    public float dashCooldown { get { return _dashCooldown.MaxValue; } }
    [Space]
    [SerializeField] private CharacterStat _maxAirDashes;
    public int maxAirDashes { get { return _maxAirDashes.MaxValueInt; } }
    [SerializeField] private int _currentAirDashes = 0;
    public int currentAirDashes { get { return _currentAirDashes; } }
    private float _dashCooldownTimer = 0f;
    private bool dashBuffer = false;

    [Header("COMBAT")]
    [Space(10)]
    [SerializeField] private GameFaction _faction;
    public GameFaction faction { get { return _faction; } }
    [Space]
    [SerializeField] private bool _hit = false;
    public bool hit { get { return _hit; } }
    public bool isDead { get { return _characterHealth.isDead; } }
    public bool isDisabled { get { return _inputsLocked || isDead || _hit ? true : false; } }
    [SerializeField] private float resurrectDelay = 1f;
    public bool hasHyperArmor = false;
    private bool _shake = false;
    public bool shake { get { return _shake; } }
    [Space]
    [SerializeField][ReadOnlyInspector] private Vector3 _cachedVelocity = Vector3.zero;
    [SerializeField][ReadOnlyInspector] Transform _aimingTarget = null;

    [Header("LOCKS")]
    [Space(10)]
    [SerializeField] private bool enableGroundDetection = true;
    [SerializeField] private bool enableCeilingDetection = true;

    [Header("CHEATS")]
    [Space(10)]
    [SerializeField] private bool enableResetCharacter = false;
    [SerializeField] private bool enableInfiniteJumps = false;

    [Header("EDITOR & DEBUG")]
    [Space(10)]
    [SerializeField] private bool drawGizmos = true;

    // Events
    public delegate void DefaultCallback();
    public delegate void IntCallback(int intValue);
    public delegate void FloatCallback(float floatValue);
    public delegate void RaycastHitCallback(RaycastHit hit);
    public delegate void LandingCallback(float value, RaycastHit hit);
    public delegate void CharacterControllerCallback(BaseCharacterController playerCharacterController);
    public delegate void HitLagCallback(float duration, bool shake);
    public FloatCallback OnMove;
    public IntCallback OnSetPlayer;
    public IntCallback OnJump;
    public RaycastHitCallback OnWallJump;
    public LandingCallback OnLand;
    public DefaultCallback OnUTurn;
    public DefaultCallback OnResetCharacter;
    public DefaultCallback OnHit;
    public HitLagCallback OnHitLag;
    public DefaultCallback OnEndHitlag;
    public DefaultCallback OnFreeze;
    public DefaultCallback OnUnfreeze;
    public DefaultCallback OnDash;
    public CharacterControllerCallback OnDeath;
    public CharacterControllerCallback OnWaitForResurrect;
    public CharacterControllerCallback OnResurrect;

    // Cache
    public Vector3 characterCenter { get { return cachedTransform.position + capsuleCollider.center; } }
    private RaycastHit _groundHit;
    public RaycastHit groundHit { get { return _groundHit; } }
    private RaycastHit _ceilingHit;
    public RaycastHit ceilingHit { get { return _ceilingHit; } }
    //private RaycastHit groundEdgeHit;
    //private RaycastHit leftWallHit;
    //private RaycastHit rightWallHit;
    private RaycastHit wallHit;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private RaycastHit lastWallJumpHit;
    private bool wallHitUpperThanLastWallJump { get { return wallJump && (wallHit.normal.x * lastWallJumpHit.normal.x > 0f && wallHit.point.y > lastWallJumpHit.point.y); } }
    private float wallJumpAirTime;
    public bool isFacingAWall { get { return (isFacingLeftWall || isFacingRightWall); } }
    [Range(0, 180)] private float leftWallAngle;
    [Range(0, 180)] private float rightWallAngle;
    private Vector2 _movingVector
    {
        get
        {
            float sign = Mathf.Sign(_moveInput);

            return (isGrounded && !isFacingEdge) ? (Vector2)(Quaternion.Euler(0, 0, groundAngle * Mathf.Sign(-groundHit.normal.x)) * Vector2.right * sign) : Vector2.right * sign;
        }
    }
    public Vector2 movingVector { get { return _movingVector; } }
    private float _targetSpeed { get { return isMoving ? Mathf.Lerp(minSpeed, maxSpeed, speedLerp) : 0f; } }
    public float targetSpeed { get { return _targetSpeed; } }
    private Vector2 startPos;
    private int _facingRight = 0;
    public int facingRight { get { return _facingRight; } }

    // Coroutines
    private Coroutine jumpCoroutine;
    private Coroutine jumpBufferCoroutine;
    private Coroutine uTurnCoroutine;
    private Coroutine hitCoroutine;
    private Coroutine resurrectCoroutine;
    private Coroutine dashCoroutine;
    private Coroutine dashBufferCoroutine;
    private Coroutine disableInputCoroutine;
    private Coroutine oneWayPlatformCoroutine;

    #region UNITY_BASED

    private void Awake()
    {
#if UNITY_EDITOR
        OnJump += CheatJumpCallback;
#endif     
    }

    private void Start()
    {
        //This is just for debug purposes - should be cleaned up later, when confronting SceneLoading.
        //CameraManager.Instance.SetPlayerCharacterController(this);

        InitializeCharacterStats();

        InitCurrentGroundLayers();
    }

    private void InitializeCharacterStats()
    {
        if (_characterStats == null)
        {
            Debug.LogWarning(this.name + " has no base character stats!");
            return;
        }

        _maxSpeed = new CharacterStat(_characterStats.baseMaxSpeed);
        _speedLerpRate = new CharacterStat(_characterStats.baseMaxSpeed);

        gravity = new CharacterStat(_characterStats.gravity);
        minFallSpeed = new CharacterStat(_characterStats.minFallSpeed);
        fastFallGravityFactor = new CharacterStat(_characterStats.fastFallGravityFactor);
        minFastFallSpeed = new CharacterStat(_characterStats.minFastFallSpeed);

        maxJumpCount = new CharacterStat(_characterStats.baseMaxJumpCount);
        maxWallJumpCount = new CharacterStat(_characterStats.baseMaxWallJumpCount);
        jumpStrength = new CharacterStat(_characterStats.baseJumpStrength);
        maxJumpDuration = new CharacterStat(_characterStats.baseMaxJumpDuration);
        jumpDosageCurve = _characterStats.jumpDosageCurve;
        shortJumpSpeedFactor = _characterStats.shortJumpSpeedFactor;

        _dashSpeed = new CharacterStat(_characterStats.baseDashSpeed);
        _dashDuration = new CharacterStat(_characterStats.baseDashDuration);
        _dashCooldown = new CharacterStat(_characterStats.baseDashCooldown);
        _maxAirDashes = new CharacterStat(_characterStats.baseMaxAirDashes);

        _faction = _characterStats.faction;
        _characterHealth.Health = new CharacterStat(_characterStats.baseHealth);
    }

    private void Update()
    {
        HandleCooldowns();
    }

    private void HandleCooldowns()
    {
        //Dashing
        if (_dashCooldownTimer > 0 && !_isDashing)
        {
            _dashCooldownTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        FixedCacheData();

        Gravity();

        GroundDetection();
        WallDetection();
        CeilingDetection();
        AirBehaviour();

        Motion();
        CharacterOrientation();
    }

    #endregion

    #region INIT_AND_MISC

    /// <summary>
    /// Cache values during FixedUpdate
    /// </summary>
    private void FixedCacheData()
    {
        _rigidbodyVelocity = rigid.velocity;

        GetWallHit();
    }

    private void InitCurrentGroundLayers()
    {
        _currentGroundLayers = defaultEnvironmentLayers;
    }

    #endregion

    #region INPUTS
    public void SetInputs(PlayerInputs inputs)
    {
        if (_inputsLocked || characterHealth.isDead)
        {
            _moveInput = 0f;
            _upDownInput = 0f;
            _jumpPressed = false;
            return;
        }

        _moveInput = inputs.MoveX;
        _upDownInput = inputs.MoveY;
        _jumpPressed = inputs.JumpPressed;
    }

    public void RequestJump()
    {
        SetJumpBuffer();
    }

    public void RequestDash()
    {
        SetDashBuffer();
    }

    public void DisableInputs(float duration)
    {
        if (disableInputCoroutine != null)
        {
            StopCoroutine(disableInputCoroutine);
        }
        disableInputCoroutine = StartCoroutine(CoDisableInputs(duration));
    }

    private IEnumerator CoDisableInputs(float duration)
    {
        _inputsLocked = true;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            yield return null;
        }
        _inputsLocked = false;
    }
    #endregion

    #region BEHAVIOUR

    private void Gravity()
    {
        // Character physics is frozen
        if (isFrozen)
            return;

        if (isGrounded && !isFacingEdge)
            return;

        if (isJumping)
            return;

        if (isDashing)
            return;

        //if (!isFastFalling && !isGrounded && !isJumping && rigid.velocity.y < 0f && _upDownInput < -0.5f)
        //    isFastFalling = true;

        isFastFalling = !isGrounded && !isJumping && rigid.velocity.y < 0f && _upDownInput < -0.5f ? true : false;

        float targetGravity = isFastFalling ? gravity.CurrentValue * fastFallGravityFactor.CurrentValue : gravity.CurrentValue;
        rigid.AddForce(Vector3.up * targetGravity, ForceMode.Acceleration);

        float targetFallSpeed = isFastFalling ? minFastFallSpeed.CurrentValue : minFallSpeed.CurrentValue;
        if (rigid.velocity.y < targetFallSpeed)
            SetRigidbodyVelocity(new Vector3(rigid.velocity.x, targetFallSpeed, 0));
    }

    private void Motion()
    {
        _isMoving = Mathf.Abs(_moveInput) != 0f;

        if (isMoving)
        {
            if (isGrounded && !uTurn && !isDashing)
                _leftRight = (int)Mathf.Sign(_moveInput);
            if (OnMove != null)
            {
                OnMove(Time.fixedDeltaTime);
            }
        }

        // Character physics is frozen
        if (isFrozen)
            return;

        // Stop motion if player moves towards a wall
        if ((wallAngle > maximumGroundAngle) && (_moveInput * wallHit.normal.x < 0f))
        {
            _isMoving = false;

            if (isGrounded)
                SetRigidbodyVelocity(Vector3.zero);

            return;
        }

        // Is doing a U Turn
        if (uTurn)
            return;

        _speedLerp = Mathf.InverseLerp(0f, runSpeedThresold, Mathf.Abs(_moveInput));

        // Ground tilt speed
        Vector2 targetVector = _movingVector * targetSpeed;

        //Prevent movement when attacking on ground.
        if (_characterCombat.isAttacking && isGrounded)
        {
            targetVector = Vector2.zero;
        }

        float xVelocityLerp = Mathf.Lerp(rigidbodyVelocity.x, targetVector.x, _speedLerpRate.CurrentValue * Time.fixedDeltaTime);
        Vector2 velocityLerp = new Vector2(xVelocityLerp, targetVector.y);

        if (!isGrounded) // Air movements
        {
            if (wallJump)
            {
                velocityLerp = Vector2.Lerp(rigidbodyVelocity, velocityLerp, wallJumpMotionFactorCurve.Evaluate(airTime - wallJumpAirTime));
            }          

            velocityLerp.y = rigid.velocity.y;
        }

        currentSpeed = velocityLerp.magnitude;
        SetRigidbodyVelocity(velocityLerp);

        // U Turn
        if (!uTurn && isGrounded && isMoving && // Default conditions
            (leftRight * rigid.velocity.x < 0f) // Stick on opposite side
                                                //&& Mathf.Abs(rigidbodyVelocity.x) > 2f) // Character was moving fast
            && currentSpeed > 2f
            && !isDashing
            && !_characterCombat.isAttacking)
        {
            UTurn();
        }
    }

    public void CharacterOrientation(bool forceOrientation = false)
    {
        // Character physics is frozen
        if (isFrozen)
            return;

        if (isDead)
            return;

        if (!forceOrientation)
        {
            if (uTurn)
                return;

            if (_characterCombat.isAttacking)
                return;
        }

        float toEulerRot;
        if (_aimingTarget != null)
        {
            int leftRight = (int)Mathf.Sign(_aimingTarget.position.x - cachedTransform.position.x);
            if (leftRight < 0)
            {
                toEulerRot = _leftMeshOrientation;
                _facingRight = -1;
            }
            else
            {
                toEulerRot = _rightMeshOrientation;
                _facingRight = 1;
            }
        }
        else
        {
            if (_leftRight < 0)
            {
                toEulerRot = _leftMeshOrientation;
                _facingRight = -1;
            }
            else
            {
                toEulerRot = _rightMeshOrientation;
                _facingRight = 1;
            }
        }
        characterBehavior.transform.rotation = Quaternion.Slerp(characterBehavior.transform.rotation, Quaternion.Euler(0, toEulerRot, 0), forceOrientation ? 1 : 10f * Time.fixedDeltaTime);
    }

    private void GroundDetection()
    {
        // Ground detection explicitely disabled
        if (!enableGroundDetection)
            return;

        // Do not detect ground while jumping
        if (isJumping)
            return;

        // When grounded, the cast is a sphere, else it's a ray
        bool raycast = isGrounded ?
            Physics.SphereCast(
                cachedTransform.position + new Vector3(0, capsuleCollider.radius + feetDetectionOffset, 0),
                capsuleCollider.radius, Vector3.down, out _groundHit,
                groundDetectionDistance, _currentGroundLayers) :
            Physics.Raycast(
                characterCenter,
                Vector3.down, out _groundHit,
                capsuleCollider.radius + groundDetectionDistance, _currentGroundLayers);

        if (raycast) // Ground detected
        {
            if (rigid.velocity.y > 0 && !isGrounded) return;
              GroundAttach();
        }
        else // No ground detected
        {
            GroundDetach();
        }

        // Edge detection
        isFacingEdge = EdgeCast();
    }

    private void GroundAttach(bool forceGroundDetection = false)
    {
        // Catch landing position
        if (forceGroundDetection)
        {
            Physics.SphereCast(
                cachedTransform.position + new Vector3(0, capsuleCollider.radius + feetDetectionOffset, 0),
                capsuleCollider.radius, Vector3.down, out _groundHit,
                groundDetectionDistance, _currentGroundLayers);
        }

        _groundAngle = GetFaceAngleFromNormal(Vector3.up, groundHit.normal);
        if (groundAngle > maximumGroundAngle)
        {
            GroundDetach();
            return;
        }

        // Landing
        if (!isGrounded)
        {
            // Character is moving backwards
            if (leftRight * rigidbodyVelocity.x < 0)
            {
                if (isMoving)
                    _leftRight *= -1;

                if (rigidbodyVelocity.y > -2f)
                    CharacterOrientation(true);
            }

            isFastFalling = false;
            _wallJump = false;
            _currentGroundLayers = defaultEnvironmentLayers;

            SetColliderMode(0);

            if (OnLand != null)
            {
                OnLand(rigid.velocity.y, groundHit);
            }

            SetRigidbodyVelocity(new Vector3(rigidbodyVelocity.x, 0, 0));
        }

        if (!isFacingEdge)
            SetRigidbodyPosition(GetSphereHitPosition(groundHit));

        _isGrounded = true;
        jumpCount = 0;
        wallJumpCount = 0;
        _currentAirDashes = 0;

        if (jumpBuffer)
            Jump(0);
    }

    private void GroundDetach()
    {
        // Ground just not found (air start)
        if (isGrounded)
        {
            SetColliderMode(1);
        }

        if (uTurn)
            CancelUTurn();

        _groundAngle = 0;

        // Cancel first jump
        if (jumpCount == 0)
            jumpCount++;

        _isGrounded = false;
    }

    private void CeilingDetection()
    {
        if (!enableCeilingDetection) return;

        // When grounded, the cast is a sphere, else it's a ray
        bool raycast = isGrounded ?
            Physics.SphereCast(
                cachedTransform.position + new Vector3(0, capsuleCollider.height - headDetectionOffset, 0),
                capsuleCollider.radius, Vector3.down, out _ceilingHit,
                ceilingDetectionDistance, _currentGroundLayers) :
            Physics.Raycast(
                characterCenter,
                Vector3.up, out _groundHit,
                capsuleCollider.radius + ceilingDetectionDistance, _currentGroundLayers);

        _isTouchingCeiling = raycast;
    }

    private bool EdgeCast()
    {
        if (!isGrounded)
            return false;

        return !Physics.Raycast(
                characterCenter + new Vector3((isMoving ? Mathf.Sign(_moveInput) : leftRight) * capsuleCollider.radius, 0, 0),
                Vector3.down, capsuleCollider.height / 2f + Mathf.Lerp(minimumEdgeDetectionDistance, maximumEdgeDetectionDistance, groundAngle / maximumGroundAngle), _currentGroundLayers);
    }

    private Vector3 GetSphereHitPosition(RaycastHit hit)
    {
        return hit.point + (hit.normal - Vector3.up) * capsuleCollider.radius;
    }

    private Vector3 GetRayHitPosition(RaycastHit hit)
    {
        return hit.point + (hit.normal - Vector3.up) * capsuleCollider.radius;
    }

    private void WallDetection()
    {
        _isFacingLeftWall = SideWallDetection(-1);
        _isFacingRightWall = SideWallDetection(1);

        if (jumpBuffer && CheckWallJump() && isFacingAWall)
            Jump(1);
    }

    private bool SideWallDetection(int direction)
    {
        float xDetectionOffset = 0.1f;
        Vector3 defaultPos = cachedTransform.position + new Vector3(-direction * xDetectionOffset, capsuleCollider.center.y, 0);
        RaycastHit hit;

        // When grounded, the cast is a capsule, else it's a sphere
        bool hitState = false;

        if (isGrounded)
        {
            float capsuleYOffsetFromCenter = (capsuleCollider.height - (capsuleCollider.radius * 2f)) / 2f;

            hitState = Physics.CapsuleCast(
                defaultPos + new Vector3(0, -capsuleYOffsetFromCenter + feetDetectionOffset, 0), // Bottom sphere
                defaultPos + new Vector3(0, capsuleYOffsetFromCenter, 0), // Top sphere
                capsuleCollider.radius,
                new Vector3(direction, 0, 0),
                out hit,
                wallDetectionDistance,
                onlyGroundLayers);
        }
        else
        {
            hitState = Physics.SphereCast(
                characterCenter + new Vector3(-direction * xDetectionOffset, 0, 0),
                capsuleCollider.radius,
                new Vector3(direction, 0, 0),
                out hit,
                wallDetectionDistance,
                onlyGroundLayers);
        }

        if (hitState)
        {
            float angle = GetFaceAngleFromNormal(Vector3.up, hit.normal);
            if (direction == -1)
            {
                leftWallHit = hit;
                leftWallAngle = angle;
            }
            if (direction == 1)
            {
                rightWallHit = hit;
                rightWallAngle = angle;
            }

            wallAngle = angle;

            // The raycasted "wall" is actually a walkable surface
            if (!isJumping && wallAngle < maximumGroundAngle)
            {
                GroundAttach(true);
                return false;
            }

            if (angle < minimumWallAngle || angle > maximumWallAngle)
                return false;
        }

        return hitState;
    }

    private void GetWallHit()
    {
        if (isFacingLeftWall)
        {
            wallHit = leftWallHit;
            wallAngle = leftWallAngle;
        }
        else if (isFacingRightWall)
        {
            wallHit = rightWallHit;
            wallAngle = rightWallAngle;
        }
        else
        {
            wallAngle = 0;
        }
    }

    private void AirBehaviour()
    {
        if (isGrounded)
        {
            airTime = 0f;
            return;
        }

        airTime += Time.fixedDeltaTime;
    }

    public void CacheVelocity(Vector3 velocity, bool overrideCachedVelociy = false)
    {
        if (overrideCachedVelociy)
        {
            _cachedVelocity = velocity;
        }
        else
        {
            _cachedVelocity += velocity;
        }
    }

    public void CharacterImpulse(Vector2 vel)
    {
        SetRigidbodyVelocity(vel);
    }

    private void SetRigidbodyVelocity(Vector2 vel, string message = "")
    {
        rigid.velocity = vel;
        _rigidbodyVelocity = vel;

        if (!string.IsNullOrEmpty(message))
            Debug.Log(message);
    }

    public void SetRigidbodyPosition(Vector2 pos, bool forcePos = false)
    {
        if (forcePos)
            cachedTransform.position = pos;

        rigid.MovePosition(pos);
    }

    private float GetFaceAngleFromNormal(Vector3 direction, Vector3 normal)
    {
        normal.z = 0;
        float dot = Vector3.Dot(direction, normal.normalized);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
        return angle;
    }

    private void SetJumpBuffer()
    {
        if (isDead)
            return;

        bool canWallJump = CheckWallJump();
        bool canJump = jumpCount < maxJumpCount.CurrentValueInt;

        if (_upDownInput < -0.5f && isOnOneWayPlatform)
        {
            if (oneWayPlatformCoroutine != null)
            {
                StopCoroutine(oneWayPlatformCoroutine);
            }
            oneWayPlatformCoroutine = StartCoroutine(CoOneWayPlatformBuffer());
        }
        else
        {
            if (canWallJump)
            {
                Jump(1);
            }
            else if (canJump)
            {
                Jump(0);
            }
            else // Set a short buffer if the character cannot jump
            {
                if (jumpBufferCoroutine != null)
                    StopCoroutine(jumpBufferCoroutine);
                jumpBufferCoroutine = StartCoroutine(CoSetJumpBuffer());
            }
        }
    }

    private IEnumerator CoOneWayPlatformBuffer()
    {
        _currentGroundLayers = onlyGroundLayers;
        yield return new WaitForSecondsRealtime(0.35f);
        _currentGroundLayers = defaultEnvironmentLayers;
    }

    private IEnumerator CoSetJumpBuffer()
    {
        jumpBuffer = true;
        yield return new WaitForSecondsRealtime(0.2f);
        jumpBuffer = false;
        oneWayPlatformCoroutine = null;
    }

    private void Jump(int jumpMode)
    {
        // Character physics is frozen
        if (isFrozen)
            return;

        if (_hit)
            return;

        if (jumpCoroutine != null)
            StopCoroutine(jumpCoroutine);
        jumpCoroutine = StartCoroutine(JumpDosage());

        CancelUTurn();

        _isJumping = true;
        jumpBuffer = false;
        _wallJump = false;
        isFastFalling = false;
        isFacingEdge = false;

        SetColliderMode(1);

        if (jumpMode == 0)
            DoAJump();
        else
            DoAWallJump();
    }

    private void DoAJump()
    {
        if (OnJump != null)
        {
            OnJump(jumpCount);
        }

        _isGrounded = false;
        _groundAngle = 0;

        SetRigidbodyVelocity(new Vector3(rigid.velocity.x, jumpStrength.CurrentValue, 0f));

        jumpCount++;
    }

    private void DoAWallJump()
    {
        GetWallHit();

        int direction = isFacingLeftWall ? 1 : -1;

        if (isDashing)
        {
            CancelDash();
            SetRigidbodyVelocity(new Vector3(maxSpeed*10f * direction, jumpStrength.CurrentValue*10f, 0f));
            Debug.Log("!");
        }
        else
        {
            SetRigidbodyVelocity(new Vector3(maxSpeed * direction, jumpStrength.CurrentValue, 0f));
        }

        SetRigidbodyVelocity(new Vector3(maxSpeed * direction, jumpStrength.CurrentValue, 0f));
        SetRigidbodyPosition(GetSphereHitPosition(wallHit));
        _leftRight = direction;
        CharacterOrientation(true);
        wallJumpAirTime = airTime;
        _wallJump = true;

        lastWallJumpHit = wallHit;

        wallJumpCount++;
        _currentAirDashes--;
        if (_currentAirDashes < 0)
        {
            _currentAirDashes = 0;
        }

        if (OnWallJump != null)
        {
            OnWallJump(wallHit);
        }
    }

    private bool CheckWallJump()
    {
        if (isGrounded)
            return false;

        // Cannot make a wall jump on the same direction, upper than the previous one
        if (wallHitUpperThanLastWallJump)
            return false;

        if (wallJumpCount >= maxWallJumpCount.CurrentValueInt)
            return false;

        return isFacingAWall;
    }

    private IEnumerator JumpDosage()
    {
        float t = 0;
        float d = maxJumpDuration.CurrentValue;

        while (t < d)
        {
            if (_isTouchingCeiling)
            {
                CancelJumpDosage();
            }

            float holdFactor = _jumpPressed ? 1f : shortJumpSpeedFactor;
            t += Time.deltaTime * holdFactor;
            float jumpDosageStrength = jumpStrength.CurrentValue*jumpDosageCurve.Evaluate(t/d);

            //rigid.AddForce(Vector3.up * jumpDosageStrength, ForceMode.Acceleration);
            SetRigidbodyVelocity(new Vector3(rigid.velocity.x, jumpDosageStrength, 0));

            yield return null;
        }

        _isJumping = false;
        
    }

    private void CancelJumpDosage()
    {
        if (!isJumping)
            return;

        if (jumpCoroutine != null)
            StopCoroutine(jumpCoroutine);

        _isJumping = false;
    }

    private void UTurn()
    {
        if (uTurnCoroutine != null)
            StopCoroutine(uTurnCoroutine);
        uTurnCoroutine = StartCoroutine(CoUTurn());

        if (OnUTurn != null)
        {
            OnUTurn();
        }
    }

    private IEnumerator CoUTurn()
    {
        float t = 0f;
        float startXVelocity = rigidbodyVelocity.x;

        _uTurn = true;

        _leftRight = (int)Mathf.Sign(_moveInput);
        CharacterOrientation(true);

        while (t < uTurnDelay)
        {
            t += Time.deltaTime;
            float lerp = 1f - (t / uTurnDelay);

            SetRigidbodyVelocity(new Vector3(lerp * startXVelocity, rigidbodyVelocity.y, 0));

            yield return null;
        }

        _uTurn = false;
    }

    private void CancelUTurn()
    {
        if (!uTurn)
            return;

        if (uTurnCoroutine != null)
            StopCoroutine(uTurnCoroutine);

        _uTurn = false;
    }

    private void SetDashBuffer()
    {
        bool canDash = true;
        if (_dashCooldownTimer > 0 || currentAirDashes >= maxAirDashes)
        {
            canDash = false;
        }

        if (canDash)
        {
            Dash();
        }
        else
        {
            if (dashBufferCoroutine != null)
                StopCoroutine(dashBufferCoroutine);
            dashBufferCoroutine = StartCoroutine(CoSetDashBuffer());
        }
    }

    private IEnumerator CoSetDashBuffer()
    {
        dashBuffer = true;
        yield return new WaitForSecondsRealtime(0.2f);
        dashBuffer = false;
    }

    private void Dash()
    {
        if (isFrozen)
            return;

        if (_hit)
            return;

        if (dashCoroutine != null)
            StopCoroutine(dashCoroutine);
        dashCoroutine = StartCoroutine(CoDash());

        characterHealth.Invulnerability(.2f);

        CancelJumpDosage();

        _isDashing = true;
        dashBuffer = false;
        _wallJump = false;
        isFastFalling = false;
        _dashCooldownTimer = _dashCooldown.CurrentValue;

        //SetColliderMode(1);

        DoADash();
    }

    private void DoADash()
    {
        if (OnDash != null)
        {
            OnDash();
        }

        _isMoving = Mathf.Abs(_moveInput) != 0f;

        if (isMoving)
        {
            _leftRight = (int)Mathf.Sign(_moveInput);
            if (OnMove != null)
            {
                OnMove(999f);
            }
        }

        if (!isGrounded)
        {
            _currentAirDashes++;
        }

        SetRigidbodyVelocity(new Vector3(_leftRight * _dashSpeed.CurrentValue, 0f, 0f));
        CharacterOrientation(true);
    }

    private IEnumerator CoDash()
    {
        float t = 0;
        float d = _dashDuration.CurrentValue;

        while (t < d)
        {
            t += Time.deltaTime;
            float curvedDashSpeed = _dashSpeed.CurrentValue * dashSpeedCurve.Evaluate(t/d);

            SetRigidbodyVelocity(new Vector3(_leftRight * curvedDashSpeed, 0, 0));

            yield return null;
        }

        _isDashing = false;
        //SetColliderMode(0);
    }

    private void CancelDash()
    {
        if (!isDashing)
            return;

        if (dashCoroutine != null)
            StopCoroutine(dashCoroutine);

        _isDashing = false;
        //SetColliderMode(0);
    }

    private void SetColliderMode(int mode)
    {
        SetColliderParameters(capsuleCollider.radius, mode == 0 ? _defaultColliderHeight : _defaultColliderRadius * 2f, defaultColliderHeight / 2f);
    }

    public void SetColliderParameters(float radius, float height, float yPos)
    {
#if UNITY_EDITOR
        if (!_capsuleCollider)
            _capsuleCollider = capsuleCollider;
#endif

        _capsuleCollider.radius = radius;
        _capsuleCollider.height = height;
        _capsuleCollider.center = new Vector3(0, yPos, 0);
    }

    private void FreezeCharacter()
    {
        isFrozen = true;

        CancelJumpDosage();
        CancelUTurn();

        _jumpPressed = false;
        _isMoving = false;
        isFastFalling = false;
        _wallJump = false;

        SetRigidbodyVelocity(Vector3.zero);

        if (OnFreeze != null)
        {
            OnFreeze();
        }

    }

    private void UnfreezeCharacter()
    {
        isFrozen = false;

        if (OnUnfreeze != null)
        {
            OnUnfreeze();
        }

        if (_cachedVelocity != Vector3.zero)
        {
            SetRigidbodyVelocity(_cachedVelocity);
            _cachedVelocity = Vector3.zero;
        }
    }

    public void SetStartPosition(Vector2 newStartPos)
    {
        startPos = newStartPos;
    }

    #endregion

    #region COMBAT

    public void Hit(float damage, Collider hitCollider, float knockbackForce, Vector3 overrideKnockbackDirection, float disableInputDuration = .3f, float hitLagDuration = 0.1f, bool pierceInvulnerability = false, float invulnerabilityDuration = .5f)
    {
        if (characterHealth.isInvulnerable && !pierceInvulnerability)
            return;

        characterHealth.Hurt(damage);

        HitLag(hitLagDuration, true);

        if (!hasHyperArmor)
        {
            if (invulnerabilityDuration > 0)
            {
                characterHealth.Invulnerability(invulnerabilityDuration);
            }
            if (disableInputDuration > 0)
            {
                DisableInputs(disableInputDuration);
            }
            if (knockbackForce > 0)
            {
                Vector3 hitPoint = hitCollider.ClosestPoint(characterCenter);
                Vector3 knockbackDirection = Vector3.zero;
                if (overrideKnockbackDirection != Vector3.zero)
                {
                    knockbackDirection = new Vector3(knockbackDirection.x, knockbackDirection.y, 0f);
                    knockbackDirection = overrideKnockbackDirection;
                }
                else
                {
                    knockbackDirection = characterCenter - hitPoint;
                    knockbackDirection = new Vector3(knockbackDirection.x, knockbackDirection.y, 0f);
                    knockbackDirection = knockbackDirection.normalized;
                }

                Vector3 knockback = knockbackDirection * knockbackForce;
                _cachedVelocity += knockback;

                CancelDash();
                CancelUTurn();
                CancelJumpDosage();
            }
        }

        if (!_characterHealth.isDead)
        {
            OnHit?.Invoke();
        }

    }

    public void HitLag(float duration, bool shake = false)
    {
        if (duration <= 0)
        {
            duration = 0.01f;
        }

        if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);
        hitCoroutine = StartCoroutine(CoHit(duration, shake));

        CancelDash();
        CancelUTurn();
        CancelJumpDosage();

        OnHitLag?.Invoke(duration, shake);
    }

    private IEnumerator CoHit(float hitLagDuration, bool shake)
    {
        _hit = true;
        FreezeCharacter();
        _shake = shake;

        float t = 0f;
        while (t < hitLagDuration)
        {
            t += Time.deltaTime;

            yield return null;
        }

        _shake = false;
        _hit = false;
        UnfreezeCharacter();

        if (OnEndHitlag != null)
        {
            OnEndHitlag();
        }
    }

    private void CancelHit()
    {
        if (!_hit)
            return;

        if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);

        _hit = false;

        if (OnEndHitlag != null)
        {
            OnEndHitlag();
        }

    }

    public void Death()
    {
        FreezeCharacter();

        capsuleCollider.enabled = false;

        //resurrectCoroutine = StartCoroutine(WaitForResurrect());

        if (OnDeath != null)
        {
            OnDeath(this);
        }

    }

    public void Revive(Vector3 pos)
    {
        ResetCharacter(pos);
    }

    //private IEnumerator WaitForResurrect()
    //{
    //    yield return new WaitForSeconds(resurrectDelay);

    //    OnWaitForResurrect(this);

    //    if (!gameManager.isSoloMode)
    //    {
    //        // Wait until resurrect position is within camera bounds
    //        yield return new WaitUntil(() => cameraBehaviour.IsResurrectPositionInsideCameraBounds(this));
    //    }

    //    Resurrect();
    //}

    //public void CancelWaitForResurrect()
    //{
    //    if (!isDead)
    //        return;

    //    if (resurrectCoroutine != null)
    //        StopCoroutine(resurrectCoroutine);

    //    Resurrect();
    //}

    //public void SetResurrectCheckpoint(Checkpoint checkpoint)
    //{
    //    _resurrectCheckpoint = checkpoint;
    //}

    //public void Resurrect()
    //{
    //    ResetCharacter(targetResurrectPos + new Vector2(0, resurrectYOffset));

    //    _isDead = false;
    //    capsuleCollider.enabled = true;

    //    OnResurrect(this);
    //}

    //public void Resurrect(Vector2 forcePos)
    //{
    //    ResetCharacter(forcePos + new Vector2(0, resurrectYOffset));

    //    _isDead = false;

    //    OnResurrect(this);
    //}

    public void ResetCharacter(Vector3 pos)
    {
        UnfreezeCharacter();

        CancelJumpDosage();
        CancelUTurn();
        //CancelWaitForResurrect();
        CancelHit();

        SetColliderMode(0);

        _isGrounded = true;
        _jumpPressed = false;
        _isMoving = false;
        isFastFalling = false;
        _wallJump = false;

        wallJumpCount = 0;
        jumpCount = 0;
        _currentAirDashes = 0;
        airTime = 0;

        SetRigidbodyVelocity(Vector3.zero);
        transform.position = pos;
        SetRigidbodyPosition(pos);

        if (OnResetCharacter != null)
        {
            OnResetCharacter();
        }
    }

    #endregion

    #region DEBUG_AND_EDITOR

    public void UpdateColliderParameters()
    {
        _defaultColliderRadius = capsuleCollider.radius;
        _defaultColliderHeight = capsuleCollider.height;
    }

    public void SetAllCheats(bool enabled)
    {
        enableResetCharacter = enabled;
        enableInfiniteJumps = enabled;
    }

    private void CheatJumpCallback(int count)
    {
        if (!enableInfiniteJumps)
            return;

        if (count == maxJumpCount.CurrentValueInt - 1)
            jumpCount--;
    }

    #endregion

    #region AI

    public void SetTarget(Transform target)
    {
        _aimingTarget = target;
    }

    #endregion
}
