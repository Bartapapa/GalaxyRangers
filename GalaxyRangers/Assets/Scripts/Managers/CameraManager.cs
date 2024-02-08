using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum CameraState
{
    None,
    Default,
    Exploration,
    Arena,
    Dialogue,
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

    [Header("EXPLORATION CAMERA")]
    [SerializeField] private float _desiredCameraDirectionChangeTime = 1.5f;
    [SerializeField] private float _explorationXOffset = 5f;
    [Range(-1, 1)] private int _desiredCameraDirection = 1;
    private float _desiredCameraDirectionChangeTimer = 0f;
    private Coroutine _changeCameraDirectionCoroutine;

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
        //_arenaFramingTransposer = ArenaCamera.Camera.GetCinemachineComponent<CinemachineFramingTransposer>();
        //_dialogueFramingTransposer = DialogueCamera.Camera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Start()
    {
        TransitionToState(_startingCameraState);
    }

    public void SetPlayerCharacterController(BaseCharacterController pcController)
    {
        _playerCharacterController = pcController;
        pcController.OnMove += OnCharacterMove;
        pcController.OnJump += OnCharacterJump;
        pcController.OnWallJump += OnCharacterWallJump;
        pcController.OnLand += OnCharacterLand;
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
                        ChangeDesiredCameraDirection(PlayerCharacterController.leftRight);
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
        switch (_cameraState)
        {
            case CameraState.None:
                break;
            case CameraState.Default:
                break;
            case CameraState.Exploration:
                _explorationFramingTransposer.m_DeadZoneHeight = .5f;
                break;
            case CameraState.Arena:
                break;
            case CameraState.Dialogue:
                break;
            default:
                break;
        }
    }

    private void OnCharacterWallJump(RaycastHit hit)
    {
        switch (_cameraState)
        {
            case CameraState.None:
                break;
            case CameraState.Default:
                break;
            case CameraState.Exploration:
                _explorationFramingTransposer.m_DeadZoneHeight = .5f;
                break;
            case CameraState.Arena:
                break;
            case CameraState.Dialogue:
                break;
            default:
                break;
        }
    }

    private void OnCharacterLand(float value, RaycastHit hit)
    {
        switch (_cameraState)
        {
            case CameraState.None:
                break;
            case CameraState.Default:
                break;
            case CameraState.Exploration:
                _explorationFramingTransposer.m_DeadZoneHeight = 0f;
                break;
            case CameraState.Arena:
                break;
            case CameraState.Dialogue:
                break;
            default:
                break;
        }
    }

    private void TransitionToState(CameraState toState)
    {
        if (_cameraState == toState) return;

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
                //ArenaCamera.Camera.enabled = false;
                break;
            case CameraState.Dialogue:
                //DialogueCamera.Camera.enabled = false;
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
                //ArenaCamera.Camera.enabled = false;
                //DialogueCamera.Camera.enabled = false;
                break;
            case CameraState.Exploration:
                ExplorationCamera.Camera.enabled = true;
                break;
            case CameraState.Arena:
                //ArenaCamera.Camera.enabled = true;
                break;
            case CameraState.Dialogue:
                //DialogueCamera.Camera.enabled = true;
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
