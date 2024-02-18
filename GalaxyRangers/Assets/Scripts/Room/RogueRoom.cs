using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RogueRoomCameraSettings
{
    [Header("CAMERA STATE")]
    public CameraState cameraState = CameraState.Exploration;
    public float cameraDistance = 8f;

    [Header("CONFINER")]
    public PolygonCollider2D confiner;

    public RogueRoomCameraSettings()
    {

    }

    public RogueRoomCameraSettings(CameraState state, PolygonCollider2D confiner, float camDistance)
    {
        cameraState = state;
        this.confiner = confiner;
        cameraDistance = camDistance;
    }
}

public class RogueRoom : MonoBehaviour
{
    [Header("ROOM")]
    public Room roomData;
    public Transform _defaultSpawnPoint;

    [Header("ROOM CAMERA SETTINGS")]
    public RogueRoomCameraSettings cameraSettings = new RogueRoomCameraSettings();
    public RogueRoomCameraSettings completedCameraSettings = new RogueRoomCameraSettings();

    [Header("SCENARIOS")]
    [Space(10)]
    [SerializeField] [ReadOnlyInspector] private bool _ignoreScenario = false;
    [SerializeField] private Scenario _scenario1;
    [SerializeField] private Scenario _scenario2;
    [SerializeField] private Scenario _scenario3;
    protected Scenario _chosenScenario;
    protected bool _isCompleted = false;

    [Header("TELEPORTER")]
    [SerializeField] protected Interactible_RoomTeleporter _teleporter;
    [SerializeField] private float _teleporterActivationDistance = 5f;
    public Interactible_RoomTeleporter teleporter { get { return _teleporter; } }
    private bool _teleporterActivated = false;
    public bool teleporterActivated { get { return _teleporterActivated; } set { _teleporterActivated = value; } }

    [Header("EXIT POINTS")]
    [Space(10)]
    [Header("Left top")]
    [SerializeField] private GameObject _LT_closed;
    [SerializeField] private GameObject _LT_open;
    [SerializeField] private RoomTeleporter _LT_teleporter;
    [SerializeField] private Transform _LT_spawnPoint;
    [Space]
    [Header("Left bottom")]
    [SerializeField] private GameObject _LB_closed;
    [SerializeField] private GameObject _LB_open;
    [SerializeField] private RoomTeleporter _LB_teleporter;
    [SerializeField] private Transform _LB_spawnPoint;
    [Space]
    [Header("Middle top")]
    [SerializeField] private GameObject _MT_closed;
    [SerializeField] private GameObject _MT_open;
    [SerializeField] private RoomTeleporter _MT_teleporter;
    [SerializeField] private Transform _MT_spawnPoint;
    [Space]
    [Header("Middle bottom")]
    [SerializeField] private GameObject _MB_closed;
    [SerializeField] private GameObject _MB_open;
    [SerializeField] private RoomTeleporter _MB_teleporter;
    [SerializeField] private Transform _MB_spawnPoint;
    [Space]
    [Header("Right top")]
    [SerializeField] private GameObject _RT_closed;
    [SerializeField] private GameObject _RT_open;
    [SerializeField] private RoomTeleporter _RT_teleporter;
    [SerializeField] private Transform _RT_spawnPoint;
    [Space]
    [Header("Right bottom")]
    [SerializeField] private GameObject _RB_closed;
    [SerializeField] private GameObject _RB_open;
    [SerializeField] private RoomTeleporter _RB_teleporter;
    [SerializeField] private Transform _RB_spawnPoint;

    //Projectiles, VFX and whatnot are parented to this fucker.
    [Header("RESET PARENT")]
    [Space]
    [SerializeField] private Transform _resetParent;
    public Transform resetParent { get { return _resetParent; } }

    private void Update()
    {
        if (teleporter)
        {
            if (teleporter.canBeActivated && !teleporter.canBeInteractedWith)
            {
                Collider[] coll = Physics.OverlapSphere(_teleporter.transform.position, _teleporterActivationDistance);
                foreach (Collider collider in coll)
                {
                    InteractibleManager player = collider.GetComponent<InteractibleManager>();
                    if (player)
                    {
                        ToggleTeleporterActivation(true);
                    }
                }
            }
        }
    }

    public void OpenExitPoint(TraversalPoint traversalPoint)
    {
        switch (traversalPoint.fromTraversalLocation)
        {
            case TraversalLocation.LeftTop:
                _LT_open.SetActive(true);
                _LT_closed.SetActive(false);
                _LT_teleporter.SetTraversalPoint(traversalPoint);
                break;
            case TraversalLocation.LeftBottom:
                _LB_open.SetActive(true);
                _LB_closed.SetActive(false);
                _LB_teleporter.SetTraversalPoint(traversalPoint);
                break;
            case TraversalLocation.MiddleTop:
                _MT_open.SetActive(true);
                _MT_closed.SetActive(false);
                _MT_teleporter.SetTraversalPoint(traversalPoint);
                break;
            case TraversalLocation.MiddleBottom:
                _MB_open.SetActive(true);
                _MB_closed.SetActive(false);
                _MB_teleporter.SetTraversalPoint(traversalPoint);
                break;
            case TraversalLocation.RightTop:
                _RT_open.SetActive(true);
                _RT_closed.SetActive(false);
                _RT_teleporter.SetTraversalPoint(traversalPoint);
                break;
            case TraversalLocation.RightBottom:
                _RB_open.SetActive(true);
                _RB_closed.SetActive(false);
                _RB_teleporter.SetTraversalPoint(traversalPoint);
                break;
        }
    }

    public void CloseExitPoint(TraversalLocation traversalLocation)
    {
        switch (traversalLocation)
        {
            case TraversalLocation.LeftTop:
                _LT_open.SetActive(false);
                _LT_closed.SetActive(true);
                break;
            case TraversalLocation.LeftBottom:
                _LB_open.SetActive(false);
                _LB_closed.SetActive(true);
                break;
            case TraversalLocation.MiddleTop:
                _MT_open.SetActive(false);
                _MT_closed.SetActive(true);
                break;
            case TraversalLocation.MiddleBottom:
                _MB_open.SetActive(false);
                _MB_closed.SetActive(true);
                break;
            case TraversalLocation.RightTop:
                _RT_open.SetActive(false);
                _RT_closed.SetActive(true);
                break;
            case TraversalLocation.RightBottom:
                _RB_open.SetActive(false);
                _RB_closed.SetActive(true);
                break;
        }
    }

    public Vector3 SetPlayerAtSpawnPoint(TraversalLocation traversalLocation, BaseCharacterController player)
    {
        if (traversalLocation == TraversalLocation.None)
        {
            player.ResetCharacter(_defaultSpawnPoint.position);
            //player.SetRigidbodyPosition(_defaultSpawnPoint.position);
            return _defaultSpawnPoint.position;
        }
        switch (traversalLocation)
        {
            case TraversalLocation.LeftTop:
                player.ResetCharacter(_LT_spawnPoint.position);
                return _LT_spawnPoint.position;
            case TraversalLocation.LeftBottom:
                player.ResetCharacter(_LB_spawnPoint.position);
                return _LB_spawnPoint.position;
            case TraversalLocation.MiddleTop:
                player.ResetCharacter(_MT_spawnPoint.position);
                return _MT_spawnPoint.position;
            case TraversalLocation.MiddleBottom:
                player.ResetCharacter(_MB_spawnPoint.position);
                return _MB_spawnPoint.position;
            case TraversalLocation.RightTop:
                player.ResetCharacter(_RT_spawnPoint.position);
                return _RT_spawnPoint.position;
            case TraversalLocation.RightBottom:
                player.ResetCharacter(_RB_spawnPoint.position);
                return _RB_spawnPoint.position;
        }

        return Vector3.zero;
    }

    public virtual void BuildRoom(Room room)
    {
        if (room == null)
            return;

        roomData = room;

        //Open doors entry and exit points, close the rest.
        BuildTraversalPoints(room);

        //Set scenario.
        SetScenario(room);

        //Create enemies and whatnot.
        RegenerateRoom();
    }

    protected void BuildTraversalPoints(Room room)
    {
        List<TraversalLocation> allTraversalLocations = GetAllTraversalLocations();

        if (room.entryPoint != null)
        {
            OpenExitPoint(room.entryPoint);
            allTraversalLocations.Remove(room.entryPoint.fromTraversalLocation);
        }

        foreach (TraversalPoint traversalPoint in room.exitPoints)
        {
            OpenExitPoint(traversalPoint);
            allTraversalLocations.Remove(traversalPoint.fromTraversalLocation);
        }

        foreach (TraversalLocation traversalLocation in allTraversalLocations)
        {
            CloseExitPoint(traversalLocation);
        }

        //Build teleporter, if present.
        BuildTeleporter();
    }

    public void BuildTeleporter()
    {
        if (_teleporter)
        {
            Debug.Log("Teleporter built!");

            _teleporter.gameObject.SetActive(roomData.teleporter.isPresent);

            _teleporter.teleporter = roomData.teleporter;

            if (roomData.teleporter.parentTeleporter)
            {
                _teleporter.canBeActivated = false;
            }

            if (_teleporterActivated)
            {
                _teleporter.canBeActivated = true;
                _teleporter.ActivateTeleporter();
            }
            else
            {
                _teleporter.DeactivateTeleporter();
            }
        }
    }

    protected void SetScenario(Room room)
    {
        switch (room.roomType)
        {
            case RoomType.None:
                _ignoreScenario = true;
                break;
            case RoomType.Spawn:
                _ignoreScenario = true;
                break;
            case RoomType.Boss:
                _ignoreScenario = true;
                break;
            case RoomType.Exploration:
                _ignoreScenario = false;
                break;
            case RoomType.Arena:
                _ignoreScenario = false;
                break;
            case RoomType.Shop:
                _ignoreScenario = true;
                break;
            case RoomType.Heal:
                _ignoreScenario = true;
                break;
            case RoomType.Item:
                _ignoreScenario = true;
                break;
            default:
                _ignoreScenario = true;
                break;
        }
        if (!_ignoreScenario)
        {
            switch (room.scenario)
            {
                case DifficultyScenario.None:
                    _scenario1.gameObject.SetActive(true);
                    _scenario2.gameObject.SetActive(false);
                    _scenario3.gameObject.SetActive(false);

                    _chosenScenario = _scenario1;
                    break;
                case DifficultyScenario.Easy:
                    _scenario1.gameObject.SetActive(true);
                    _scenario2.gameObject.SetActive(false);
                    _scenario3.gameObject.SetActive(false);

                    _chosenScenario = _scenario1;
                    break;
                case DifficultyScenario.Medium:
                    _scenario1.gameObject.SetActive(false);
                    _scenario2.gameObject.SetActive(true);
                    _scenario3.gameObject.SetActive(false);

                    _chosenScenario = _scenario2;
                    break;
                case DifficultyScenario.Hard:
                    _scenario1.gameObject.SetActive(false);
                    _scenario2.gameObject.SetActive(false);
                    _scenario3.gameObject.SetActive(true);

                    _chosenScenario = _scenario3;
                    break;
                default:
                    _scenario1.gameObject.SetActive(true);
                    _scenario2.gameObject.SetActive(false);
                    _scenario3.gameObject.SetActive(false);

                    _chosenScenario = _scenario1;
                    break;
            }
        }
    }

    protected List<TraversalLocation> GetAllTraversalLocations()
    {
        List<TraversalLocation> allTraversalLocations = new List<TraversalLocation>();
        TraversalLocation LT = TraversalLocation.LeftTop;
        allTraversalLocations.Add(LT);
        TraversalLocation LB = TraversalLocation.LeftBottom;
        allTraversalLocations.Add(LB);
        TraversalLocation MT = TraversalLocation.MiddleTop;
        allTraversalLocations.Add(MT);
        TraversalLocation MB = TraversalLocation.MiddleBottom;
        allTraversalLocations.Add(MB);
        TraversalLocation RT = TraversalLocation.RightTop;
        allTraversalLocations.Add(RT);
        TraversalLocation RB = TraversalLocation.RightBottom;
        allTraversalLocations.Add(RB);
        return allTraversalLocations;
    }

    public virtual void ResetRoom()
    {
        for (int i = 0; i < _resetParent.childCount; i++)
        {
            Destroy(_resetParent.GetChild(i).gameObject);
        }
    }

    public virtual void RegenerateRoom()
    {
        //Take the room's scenario, and regenerate all non-killed enemies.
        //This is necessary, though, since we don't want enemies spawning at the same place as when the player left.
        if (!_ignoreScenario)
        {
            if (!_isCompleted)
            {
                foreach (EnemySpawner enemySpawner in _chosenScenario.enemySpawners)
                {
                    enemySpawner.GenerateEnemy();
                }
            }
        }

        BuildTeleporter();

        //If the room is an item room, regenerate the same item.
        //Actually, no need since we just don't destroy the previous room. once generated, they'll still be here, with all of their variables. Just don't spawn them on the reset transform.
        //If the room is a shop room, regenerate the same items.
        //Actually, no need since we just don't destroy the previous room. once generated, they'll still be here, with all of their variables. Just don't spawn them on the reset transform.
        //If the room is a heal room, regenerate the same healing pad.
        //Actually, no need since we just don't destroy the previous room. once generated, they'll still be here, with all of their variables. Just don't spawn them on the reset transform.
    }

    public virtual void UseCameraSettings()
    {
        if (!_isCompleted)
        {
            CameraManager.Instance.SetRogueRoomCameraSettings(cameraSettings);
        }
        else
        {
            CameraManager.Instance.SetRogueRoomCameraSettings(completedCameraSettings);
        }
        
    }

    public void ToggleTeleporterActivation(bool activate, bool forceActivation = false)
    {
        if (!roomData.teleporter.isPresent)
            return;

        _teleporterActivated = activate;
        if (activate)
        {
            if (forceActivation) _teleporter.canBeActivated = true;
            _teleporter.ActivateTeleporter();
        }
        else
        {
            _teleporter.DeactivateTeleporter();
        }
    }

}
