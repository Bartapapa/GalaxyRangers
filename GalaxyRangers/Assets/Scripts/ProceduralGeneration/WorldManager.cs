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
    public int numberOfRooms = 10;
    public int maxNumberOfChildRooms = 2;
    public string seed = "";

    [Header("OBJECT REFERENCES")]
    [Space(10)]
    public Transform CurrentRoomParent;
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

            World newWorld = new World(numberOfRooms, maxNumberOfChildRooms, seed);
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
                    Vector3 roomPos = new Vector3(0 + (1 * j), 0 + (-1 * i), 0);
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
        }
    }

    private void OnDrawGizmos()
    {
        //if (world != null)
        //{
        //    for (int i = 0; i < world.layers.Count; i++)
        //    {
        //        for (int j = 0; j < world.layers[i].roomsInLayer.Count; j++)
        //        {
        //            Vector3 roomPos = new Vector3(0 + (1 * j), 0 + (-1 * i), 0);

        //            switch (world.layers[i].roomsInLayer[j].roomType)
        //            {
        //                case RoomType.None:
        //                    Gizmos.color = Color.black;
        //                    break;
        //                case RoomType.Spawn:
        //                    Gizmos.color = Color.white;
        //                    break;
        //                case RoomType.Exploration:
        //                    Gizmos.color = Color.blue;
        //                    break;
        //                case RoomType.Arena:
        //                    Gizmos.color = Color.yellow;
        //                    break;
        //                case RoomType.Boss:
        //                    Gizmos.color = Color.red;
        //                    break;
        //                case RoomType.Length:
        //                    Gizmos.color = Color.clear;
        //                    break;
        //            }

        //            Gizmos.DrawCube(roomPos, new Vector3(.7f, .7f, .7f));                  
        //        }
        //    }
        //}
    }
}
