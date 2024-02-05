using Cinemachine;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private CameraState _startingCameraState = CameraState.Default;
    private CameraState _cameraState = CameraState.None;
    public CameraState CameraState { get { return _cameraState; } set { TransitionToState(value); } }

    [Header("VIRTUAL CAMERAS")]
    [SerializeField] private PlayerCamera _defaultCamera;
    public PlayerCamera DefaultCamera { get { return _defaultCamera; } }
    [SerializeField] private PlayerCamera _explorationCamera;
    public PlayerCamera ExplorationCamera { get { return _explorationCamera; } set { _explorationCamera = value; } }
    [SerializeField] private PlayerCamera _arenaCamera;
    public PlayerCamera ArenaCamera { get { return _arenaCamera; } set { _arenaCamera = value; } }
    [SerializeField] private PlayerCamera _dialogueCamera;
    public PlayerCamera DialogueCamera { get { return _dialogueCamera; } set { _dialogueCamera = value; } }

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
        }
    }

    private void Start()
    {
        TransitionToState(_startingCameraState);
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
                ArenaCamera.Camera.enabled = false;
                break;
            case CameraState.Dialogue:
                DialogueCamera.Camera.enabled = false;
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
                DialogueCamera.Camera.enabled = false;
                break;
            case CameraState.Exploration:
                ExplorationCamera.Camera.enabled = true;
                break;
            case CameraState.Arena:
                ArenaCamera.Camera.enabled = true;
                break;
            case CameraState.Dialogue:
                DialogueCamera.Camera.enabled = true;
                break;
            default:
                break;
        }
    }
}
