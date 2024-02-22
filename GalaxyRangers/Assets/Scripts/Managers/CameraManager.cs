using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum CameraState
{
    None,
    Default,
    Exploration,
    Arena,
    Dialogue,
    Death,
}

public class CameraManager : MonoBehaviour
{
    //Singleton
    public static CameraManager Instance;

    [Header("OBJECT REFS")]
    [Space(10)]
    [SerializeField] private BaseCharacterController _playerCharacterController;
    public BaseCharacterController PlayerCharacterController { get { return _playerCharacterController; } }

    [Header("STATE")]
    [Space(10)]
    [SerializeField] private CameraState _startingCameraState = CameraState.Default;
    private CameraState _cameraState = CameraState.None;
    public CameraState CameraState { get { return _cameraState; } set { TransitionToState(value); } }

    [Header("VIRTUAL CAMERAS")]
    [Space(10)]
    [SerializeField] private PlayerCamera _defaultCamera;
    public PlayerCamera DefaultCamera { get { return _defaultCamera; } }
    [SerializeField] private PlayerCamera _explorationCamera;
    public PlayerCamera ExplorationCamera { get { return _explorationCamera; } set { _explorationCamera = value; } }
    private CinemachineFramingTransposer _explorationFramingTransposer;
    [SerializeField] private PlayerCamera _arenaCamera;
    public PlayerCamera ArenaCamera { get { return _arenaCamera; } set { _arenaCamera = value; } }
    private CinemachineFramingTransposer _arenaFramingTransposer;
    [SerializeField] private PlayerCamera _dialogueCamera;
    public PlayerCamera DialogueCamera { get { return _dialogueCamera; } set { _dialogueCamera = value; } }
    private CinemachineFramingTransposer _dialogueFramingTransposer;
    [SerializeField] private PlayerCamera _deathCamera;
    public PlayerCamera DeathCamera { get { return _deathCamera; } set { _dialogueCamera = value; } }
    private CinemachineFramingTransposer _deathFramingTransposer;

    [Header("EXPLORATION CAMERA")]
    [SerializeField] private float _desiredCameraDirectionChangeTime = 1.5f;
    [SerializeField] private float _explorationXOffset = 5f;
    [Range(-1, 1)] private int _desiredCameraDirection = 1;
    private float _desiredCameraDirectionChangeTimer = 0f;
    private Coroutine _changeCameraDirectionCoroutine;

    //Cached
    private float _lastGroundedPlayerYPos = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("2 or more CameraManagers found. Removing the latest ones.");
            Destroy(this.gameObject);
            return;
        }

        _explorationFramingTransposer = ExplorationCamera.Camera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _arenaFramingTransposer = ArenaCamera.Camera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _deathFramingTransposer = DeathCamera.Camera.GetCinemachineComponent<CinemachineFramingTransposer>();
        //_dialogueFramingTransposer = DialogueCamera.Camera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Start()
    {
        TransitionToState(_startingCameraState);

        SetPlayerCharacterController(_playerCharacterController);
    }

    private void Update()
    {
        HandleCameraDeadZones();
        HandleCameraFocusOffset();
    }

    private void HandleCameraDeadZones()
    {
        if (CameraState != CameraState.Exploration)
            return;

        _explorationFramingTransposer.m_DeadZoneHeight = !_playerCharacterController.isGrounded ? .9f : 0f;
    }

    private void HandleCameraFocusOffset()
    {
        if (CameraState != CameraState.Exploration)
            return;

        _explorationCamera.FocusOffset = !_playerCharacterController.isGrounded && !_playerCharacterController.isJumping && (_playerCharacterController.transform.position.y < _lastGroundedPlayerYPos)?
            new Vector3(_explorationCamera.FocusOffset.x, -1, _explorationCamera.FocusOffset.z) : new Vector3(_explorationCamera.FocusOffset.x, 3, _explorationCamera.FocusOffset.z);
    }

    public void SetPlayerCharacterController(BaseCharacterController pcController)
    {
        _playerCharacterController = pcController;
        pcController.OnMove += OnCharacterMove;
        pcController.OnJump += OnCharacterJump;
        pcController.OnWallJump += OnCharacterWallJump;
        //pcController.OnLand += OnCharacterLand;
    }

    private void OnCharacterMove(float fixedDeltaTime)
    {
        switch (_cameraState)
        {
            case CameraState.None:
                break;
            case CameraState.Default:
                break;
            case CameraState.Exploration:
                if (PlayerCharacterController.leftRight != _desiredCameraDirection)
                {
                    //The player is moving in the opposite direction as the desired direction.
                    _desiredCameraDirectionChangeTimer += fixedDeltaTime;
                    if (_desiredCameraDirectionChangeTimer >= _desiredCameraDirectionChangeTime)
                    {
                        ChangeDesiredCameraDirection(PlayerCharacterController.facingRight);
                    }
                }
                else
                {
                    _desiredCameraDirectionChangeTimer = 0f;
                }
                break;
            case CameraState.Arena:
                break;
            case CameraState.Dialogue:
                break;
            default:
                break;
        }
    }

    private void OnCharacterJump(int intValue)
    {
        _lastGroundedPlayerYPos = _playerCharacterController.transform.position.y;
    }

    private void OnCharacterWallJump(RaycastHit hit)
    {
        _lastGroundedPlayerYPos = _playerCharacterController.transform.position.y;
    }

    private void OnCharacterLand(float value, RaycastHit hit)
    {
        _lastGroundedPlayerYPos = _playerCharacterController.transform.position.y;
    }

    private void TransitionToState(CameraState toState)
    {
        CameraState oldState = _cameraState;
        switch (oldState)
        {
            case CameraState.None:
                break;
            case CameraState.Default:
                break;
            case CameraState.Exploration:
                ExplorationCamera.Camera.enabled = false;
                break;
            case CameraState.Arena:
                ArenaCamera.Camera.enabled = false;
                break;
            case CameraState.Dialogue:
                //DialogueCamera.Camera.enabled = false;
                break;
            case CameraState.Death:
                DeathCamera.Camera.enabled = false;
                break;
            default:
                break;
        }

        _cameraState = toState;

        switch (toState)
        {
            case CameraState.None:
                break;
            case CameraState.Default:
                ExplorationCamera.Camera.enabled = false;
                ArenaCamera.Camera.enabled = false;
                //DialogueCamera.Camera.enabled = false;
                break;
            case CameraState.Exploration:
                ExplorationCamera.Camera.enabled = true;
                break;
            case CameraState.Arena:
                ArenaCamera.Camera.enabled = true;
                break;
            case CameraState.Dialogue:
                //DialogueCamera.Camera.enabled = true;
                break;
            case CameraState.Death:
                DeathCamera.Camera.enabled = false;
                break;
            default:
                break;
        }
    }

    public void SetRogueRoomCameraSettings(RogueRoomCameraSettings cameraSettings)
    {
        //StartCoroutine(SetNewCamSettingsWaitFrame(cameraSettings));
        PolygonCollider2D roomCameraCollider = cameraSettings.confiner;
        TransitionToState(cameraSettings.cameraState);

        CinemachineConfiner2D defaultconfiner = DefaultCamera.GetComponent<CinemachineConfiner2D>();
        defaultconfiner.m_BoundingShape2D = roomCameraCollider;

        CinemachineConfiner2D exploconfiner = ExplorationCamera.GetComponent<CinemachineConfiner2D>();
        exploconfiner.m_BoundingShape2D = roomCameraCollider;

        CinemachineConfiner2D arenaconfiner = ArenaCamera.GetComponent<CinemachineConfiner2D>();
        arenaconfiner.m_BoundingShape2D = roomCameraCollider;

        //_dialogueFramingTransposer.m_CameraDistance = cameraSettings.cameraDistance;
        //CinemachineConfiner2D dialogueconfiner = DialogueCamera.GetComponent<CinemachineConfiner2D>();
        //dialogueconfiner.m_BoundingShape2D = roomCameraCollider;

        CinemachineConfiner2D deathconfiner = DeathCamera.GetComponent<CinemachineConfiner2D>();
        deathconfiner.m_BoundingShape2D = roomCameraCollider;

        switch (_cameraState)
        {
            case CameraState.None:
                break;
            case CameraState.Default:
                break;
            case CameraState.Exploration:
                _explorationFramingTransposer.m_CameraDistance = cameraSettings.cameraDistance;
                break;
            case CameraState.Arena:
                _arenaFramingTransposer.m_CameraDistance = cameraSettings.cameraDistance;
                break;
            case CameraState.Dialogue:
                _dialogueFramingTransposer.m_CameraDistance = cameraSettings.cameraDistance;
                break;
            case CameraState.Death:
                _deathFramingTransposer.m_CameraDistance = cameraSettings.cameraDistance;
                break;
            default:
                break;
        }
    }

    private IEnumerator SetNewCamSettingsWaitFrame(RogueRoomCameraSettings cameraSettings)
    {
        PolygonCollider2D roomCameraCollider = cameraSettings.confiner;
        switch (_cameraState)
        {
            case CameraState.None:
                break;
            case CameraState.Default:
                CinemachineConfiner2D defaultconfiner = DefaultCamera.GetComponent<CinemachineConfiner2D>();
                defaultconfiner.m_BoundingShape2D = null;
                break;
            case CameraState.Exploration:
                _explorationFramingTransposer.m_CameraDistance = 8f;
                CinemachineConfiner2D exploconfiner = ExplorationCamera.GetComponent<CinemachineConfiner2D>();
                exploconfiner.m_BoundingShape2D = null;
                break;
            case CameraState.Arena:
                _arenaFramingTransposer.m_CameraDistance = 8f;
                CinemachineConfiner2D arenaconfiner = ArenaCamera.GetComponent<CinemachineConfiner2D>();
                arenaconfiner.m_BoundingShape2D = null;
                break;
            case CameraState.Dialogue:
                _dialogueFramingTransposer.m_CameraDistance = 8f;
                CinemachineConfiner2D dialogueconfiner = DialogueCamera.GetComponent<CinemachineConfiner2D>();
                dialogueconfiner.m_BoundingShape2D = null;
                break;
            case CameraState.Death:
                _deathFramingTransposer.m_CameraDistance = 8f;
                CinemachineConfiner2D deathconfiner = DialogueCamera.GetComponent<CinemachineConfiner2D>();
                deathconfiner.m_BoundingShape2D = null;
                break;
            default:
                break;
        }
        yield return null;
        TransitionToState(cameraSettings.cameraState);
        switch (_cameraState)
        {           
            case CameraState.None:
                break;
            case CameraState.Default:
                CinemachineConfiner2D defaultconfiner = DefaultCamera.GetComponent<CinemachineConfiner2D>();
                defaultconfiner.m_BoundingShape2D = roomCameraCollider;
                break;
            case CameraState.Exploration:
                _explorationFramingTransposer.m_CameraDistance = cameraSettings.cameraDistance;
                CinemachineConfiner2D exploconfiner = ExplorationCamera.GetComponent<CinemachineConfiner2D>();
                exploconfiner.m_BoundingShape2D = roomCameraCollider;
                break;
            case CameraState.Arena:
                _arenaFramingTransposer.m_CameraDistance = cameraSettings.cameraDistance;
                CinemachineConfiner2D arenaconfiner = ArenaCamera.GetComponent<CinemachineConfiner2D>();
                arenaconfiner.m_BoundingShape2D = roomCameraCollider;
                break;
            case CameraState.Dialogue:
                _dialogueFramingTransposer.m_CameraDistance = cameraSettings.cameraDistance;
                CinemachineConfiner2D dialogueconfiner = DialogueCamera.GetComponent<CinemachineConfiner2D>();
                dialogueconfiner.m_BoundingShape2D = roomCameraCollider;
                break;
            case CameraState.Death:
                _deathFramingTransposer.m_CameraDistance = cameraSettings.cameraDistance;
                CinemachineConfiner2D deathconfiner = DialogueCamera.GetComponent<CinemachineConfiner2D>();
                deathconfiner.m_BoundingShape2D = roomCameraCollider;
                break;
            default:
                break;
        }
    }

    public void ForceCameraToPosition(Vector3 position)
    {
        switch (_cameraState)
        {
            case CameraState.None:
                break;
            case CameraState.Default:
                DefaultCamera.ForceFocusToPosition(position);
                break;
            case CameraState.Exploration:
                ExplorationCamera.ForceFocusToPosition(position);
                break;
            case CameraState.Arena:
                ArenaCamera.ForceFocusToPosition(position);
                break;
            case CameraState.Dialogue:
                DialogueCamera.ForceFocusToPosition(position);
                break;
            default:
                break;
        }
    }

    public void AddFocusObjectToCamera(Transform focusObject, int weight = 1, bool ignoreZ = true)
    {
        switch (_cameraState)
        {
            case CameraState.None:
                break;
            case CameraState.Default:
                DefaultCamera.AddFocusObject(focusObject, weight, ignoreZ);
                break;
            case CameraState.Exploration:
                ExplorationCamera.AddFocusObject(focusObject, weight, ignoreZ);
                break;
            case CameraState.Arena:
                ArenaCamera.AddFocusObject(focusObject, weight, ignoreZ);
                break;
            case CameraState.Dialogue:
                DialogueCamera.AddFocusObject(focusObject, weight, ignoreZ);
                break;
            case CameraState.Death:
                DeathCamera.AddFocusObject(focusObject, weight, ignoreZ);
                break;
            default:
                break;
        }
    }

    public void RemoveFocusObjectFromCamera(Transform focusObject)
    {
        switch (_cameraState)
        {
            case CameraState.None:
                break;
            case CameraState.Default:
                DefaultCamera.RemoveFocusObject(focusObject);
                break;
            case CameraState.Exploration:
                ExplorationCamera.RemoveFocusObject(focusObject);
                break;
            case CameraState.Arena:
                ArenaCamera.RemoveFocusObject(focusObject);
                break;
            case CameraState.Dialogue:
                DialogueCamera.RemoveFocusObject(focusObject);
                break;
            case CameraState.Death:
                DeathCamera.RemoveFocusObject(focusObject);
                break;
            default:
                break;
        }
    }

    #region EXPLORATION
    public void ChangeDesiredCameraDirection(int desiredDirection)
    {
        if (desiredDirection == 0) return;
        desiredDirection = (int)Mathf.Sign(desiredDirection);
        _desiredCameraDirection = desiredDirection;

        float from = _explorationFramingTransposer.m_TrackedObjectOffset.x;
        float to = _explorationXOffset * desiredDirection;

        if (_changeCameraDirectionCoroutine != null)
        {
            StopCoroutine(_changeCameraDirectionCoroutine);
        }
        _changeCameraDirectionCoroutine = StartCoroutine(CoChangeDesiredCameraDirection(from, to));
    }

    private IEnumerator CoChangeDesiredCameraDirection(float from, float to)
    {
        float timer = 0f;
        float time = .5f;
        while (timer <= time)
        {
            float lerp = Mathf.Lerp(from, to, timer / time);
            ExplorationCamera.FocusOffset = new Vector3(lerp, ExplorationCamera.FocusOffset.y, ExplorationCamera.FocusOffset.z);

            timer += Time.deltaTime;
            yield return null;
        }
        ExplorationCamera.FocusOffset = new Vector3(to, ExplorationCamera.FocusOffset.y, ExplorationCamera.FocusOffset.z);
        _changeCameraDirectionCoroutine = null;
    }
    #endregion
}
