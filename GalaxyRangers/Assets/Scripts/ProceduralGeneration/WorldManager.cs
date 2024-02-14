using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public World world;
    public static WorldManager Instance;

    [Header("ROOM DATABASE")]
    [Space]
    [SerializeField] private RoomFolder _exploRooms;
    [SerializeField] private RoomFolder _arenaRooms;
    [SerializeField] private RoomFolder _healRooms;
    [SerializeField] private RoomFolder _itemRooms;
    [SerializeField] private RoomFolder _shopRooms;
    [SerializeField] private RoomFolder _spawnRooms;
    [SerializeField] private RogueRoom _bossRoom;
    [SerializeField] private RogueRoom _HUBRoom;

    [Header("ENEMY DATABASE")]
    [SerializeField] private EnemyFolder _enemyFolder;
    public EnemyFolder enemyFolder { get { return _enemyFolder; } }

    [Header("ROOM")]
    [Space(10)]
    [SerializeField] [ReadOnlyInspector] private RogueRoom _currentRogueRoom;
    public RogueRoom currentRogueRoom { get { return _currentRogueRoom; } }

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
    public Transform GeneratedRoomParent;
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

            for (int i = 0; i < GeneratedRoomParent.childCount; i++)
            {
                Destroy(GeneratedRoomParent.GetChild(i).gameObject);
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
            _currentRogueRoom.ResetRoom();

            _currentRogueRoom.gameObject.SetActive(false);
            _currentRogueRoom = null;
        }

        //Generate and build room if first time in room.
        if (room.GeneratedRoom == null)
        {
            Debug.Log("Room not generated, generating now.");
            RogueRoom newRoom = Instantiate<RogueRoom>(GetRandomRoomOfType(room.roomType), GeneratedRoomParent);
            newRoom.BuildRoom(room);
            _currentRogueRoom = newRoom;
            room.GeneratedRoom = newRoom;
        }
        else
        {
            Debug.Log("Found room, using that one.");
            RogueRoom newRoom = room.GeneratedRoom;
            newRoom.gameObject.SetActive(true);
            _currentRogueRoom = newRoom;
            _currentRogueRoom.RegenerateRoom();
            //Perhaps have a function for 'regenerating' rooms, so that when enemies are dead they stay dead, when item is spawned it stays that way, etc.
            //The RogueRoom itself should make BuildRoom a virtual method so that other sub classes (Item Room, Arena Room, etc, etc) can all build themselves in their own way, like spawning
            //enemies and items and whatnot. For now this will suffice.
        }

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
            //DebugRoom inDebugRoom;
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

    private RogueRoom GetRandomRoomOfType(RoomType type)
    {
        int randomInt;
        switch (type)
        {
            case RoomType.None:
                return null;
            case RoomType.Spawn:
                randomInt = UnityEngine.Random.Range(0, _spawnRooms.Rooms.Count);
                return _spawnRooms.Rooms[randomInt];
            case RoomType.Boss:
                return _bossRoom;
            case RoomType.Exploration:
                randomInt = UnityEngine.Random.Range(0, _exploRooms.Rooms.Count);
                return _exploRooms.Rooms[randomInt];
            case RoomType.Arena:
                randomInt = UnityEngine.Random.Range(0, _arenaRooms.Rooms.Count);
                return _arenaRooms.Rooms[randomInt];
            case RoomType.Shop:
                randomInt = UnityEngine.Random.Range(0, _shopRooms.Rooms.Count);
                return _shopRooms.Rooms[randomInt];
            case RoomType.Heal:
                randomInt = UnityEngine.Random.Range(0, _healRooms.Rooms.Count);
                return _healRooms.Rooms[randomInt];
            case RoomType.Item:
                randomInt = UnityEngine.Random.Range(0, _itemRooms.Rooms.Count);
                return _itemRooms.Rooms[randomInt];
            case RoomType.Length:
                return null;
            default:
                return null;
        }
    }
}
