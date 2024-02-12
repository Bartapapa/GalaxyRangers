using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public World world;
    public static WorldManager Instance;

    [Header("ROOM")]
    [Space(10)]
    [SerializeField] [ReadOnlyInspector] private RogueRoom _currentRogueRoom;

    [Header("PARAMETERS")]
    [Space(10)]
    public string seed = "";
    public int numberOfRooms = 10;
    public int maxNumberOfChildRooms = 2;
    public int mainBranchMinDistance = 5;
    public int mainBranchMaxDistance = 7;
    public int subBranchMaxDistance = 2;
    public int maxNumberOfHealRooms = 1;
    public int maxNumberOfShopRooms = 1;
    public int maxNumberOfItemRooms = 1;
    public float gasEventSpawnChance = .05f;
    public float secondChanceEventSpawnChance = .05f;
    public float baseDifficulty = 1f;
    public float difficultyVariance = .5f;

    [Header("OBJECT REFERENCES")]
    [Space(10)]
    public Transform CurrentRoomParent;
    public Transform WorldVisualization;
    [SerializeField] private RogueRoom _roomPrefab;
    [SerializeField] private DebugRoom _debugRoomPrefab;
    [SerializeField] private DebugEdge _debugEdgePrefab;
    private List<DebugRoom> _debugRooms = new List<DebugRoom>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("2 or more WorldManagers found. Destroying the later ones.");
            Destroy(this.gameObject);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (_debugRooms.Count > 0)
            {
                foreach(DebugRoom debugRoom in _debugRooms)
                {
                    Destroy(debugRoom.gameObject);
                }
                _debugRooms.Clear();
            }

            World newWorld = new World(numberOfRooms, maxNumberOfChildRooms, mainBranchMinDistance, mainBranchMaxDistance, subBranchMaxDistance,
                                       maxNumberOfHealRooms, maxNumberOfShopRooms, maxNumberOfItemRooms, gasEventSpawnChance, secondChanceEventSpawnChance,
                                       baseDifficulty, difficultyVariance,
                                       seed);
            world = newWorld;
            MoveToRoom(world.rooms[0]);
            VisualizeWorld();
        }
    }

    public void MoveToRoom(Room room, TraversalPoint usedTraversal = null)
    {
        if (_currentRogueRoom != null)
        {
            Destroy(_currentRogueRoom.gameObject);
        }

        RogueRoom newRoom = Instantiate<RogueRoom>(_roomPrefab, CurrentRoomParent);
        newRoom.BuildRoom(room);
        _currentRogueRoom = newRoom;

        if (usedTraversal == null)
        {
            _currentRogueRoom.SetPlayerAtSpawnPoint(TraversalLocation.None, Player.Instance.CharacterController);
        }
        else
        {
            _currentRogueRoom.SetPlayerAtSpawnPoint(usedTraversal.toTraversalLocation, Player.Instance.CharacterController);
        }

        for (int i = 0; i < _debugRooms.Count; i++)
        {
            _debugRooms[i].ExitRoom();
            DebugRoom inDebugRoom;
            foreach(Room worldRoom in world.rooms)
            {
                if (_debugRooms[i].room == room)
                {
                    _debugRooms[i].EnterRoom();
                }
            }
        }
    }

    private void VisualizeWorld()
    {
        if (world != null)
        {
            for (int i = 0; i < world.layers.Count; i++)
            {
                for (int j = 0; j < world.layers[i].roomsInLayer.Count; j++)
                {
                    Vector3 roomPos = new Vector3(WorldVisualization.position.x + (1 * j), WorldVisualization.position.y + (-1 * i), 0);
                    DebugRoom newRoom = Instantiate<DebugRoom>(_debugRoomPrefab, roomPos, Quaternion.identity);
                    newRoom.BuildRoom(world.layers[i].roomsInLayer[j]);
                    _debugRooms.Add(newRoom);
                }
            }

            for (int k = 0; k < _debugRooms.Count; k++)
            {
                for (int l = 0; l < _debugRooms[k].room.childRooms.Count; l++)
                {
                    DebugRoom childDebugRoom;
                    foreach (DebugRoom debugRoom in _debugRooms)
                    {
                        if (_debugRooms[k].room.childRooms[l] == debugRoom.room)
                        {
                            childDebugRoom = debugRoom;
                            _debugRooms[k].AddChild(childDebugRoom.transform);
                        }
                    }
                }
            }

            for (int m = 0; m < _debugRooms.Count; m++)
            {
                if (_debugRooms[m].room.teleporter != null)
                {
                    if (_debugRooms[m].room.teleporter.isPresent)
                    {
                        if (_debugRooms[m].room.teleporter.toRoom != null)
                        {
                            Transform teleportDestination = null;
                            foreach (DebugRoom debugRoom in _debugRooms)
                            {
                                if (debugRoom.room == _debugRooms[m].room.teleporter.toRoom)
                                {
                                    teleportDestination = debugRoom.transform;
                                    break;
                                }
                            }

                            if (teleportDestination != null)
                            {
                                _debugRooms[m].AddTeleportDestination(teleportDestination);
                            }
                        }
                    }
                }
            }
        }
    }
}
