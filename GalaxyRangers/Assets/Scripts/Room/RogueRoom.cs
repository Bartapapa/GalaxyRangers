using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueRoom : MonoBehaviour
{
    [Header("ROOM")]
    public Room roomData;
    public Transform _defaultSpawnPoint;

    [Header("SCENARIOS")]
    [Space(10)]
    private float whee;

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

    public void SetPlayerAtSpawnPoint(TraversalLocation traversalLocation, BaseCharacterController player)
    {
        if (traversalLocation == TraversalLocation.None)
        {
            player.SetRigidbodyPosition(_defaultSpawnPoint.position);
            return;
        }
        switch (traversalLocation)
        {
            case TraversalLocation.LeftTop:
                player.SetRigidbodyPosition(_LT_spawnPoint.position);
                break;
            case TraversalLocation.LeftBottom:
                player.SetRigidbodyPosition(_LB_spawnPoint.position);
                break;
            case TraversalLocation.MiddleTop:
                player.SetRigidbodyPosition(_MT_spawnPoint.position);
                break;
            case TraversalLocation.MiddleBottom:
                player.SetRigidbodyPosition(_MB_spawnPoint.position);
                break;
            case TraversalLocation.RightTop:
                player.SetRigidbodyPosition(_RT_spawnPoint.position);
                break;
            case TraversalLocation.RightBottom:
                player.SetRigidbodyPosition(_RB_spawnPoint.position);
                break;
        }
    }

    public virtual void BuildRoom(Room room)
    {
        if (room == null)
            return;

        roomData = room;
        List<TraversalLocation> allTraversalLocations = GetAllTraversalLocations();

        if (room.entryPoint != null)
        {
            OpenExitPoint(room.entryPoint);
            allTraversalLocations.Remove(room.entryPoint.fromTraversalLocation);
        }

        foreach(TraversalPoint traversalPoint in room.exitPoints)
        {
            OpenExitPoint(traversalPoint);
            allTraversalLocations.Remove(traversalPoint.fromTraversalLocation);
        }

        foreach(TraversalLocation traversalLocation in allTraversalLocations)
        {
            CloseExitPoint(traversalLocation);
        }

        //Also build scenario fromm the room's scenario.
        //room.scenario;
    }

    private List<TraversalLocation> GetAllTraversalLocations()
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

}
