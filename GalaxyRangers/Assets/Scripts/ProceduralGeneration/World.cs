using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer
{
    public List<Room> roomsInLayer = new List<Room>();
}
public class World : Graph
{
    [Header("SEED")]
    [Space]
    [SerializeField][ReadOnlyInspector] private string _currentSeed;

    [Header("ROOMS")]
    [Space(10)]
    [SerializeField][ReadOnlyInspector] private int _numberOfRooms = 10;
    public int numberOfRooms { get { return _numberOfRooms; } }
    [Space]
    [SerializeField][ReadOnlyInspector] private int _maxNumberOfChildRooms = 2;
    public int maxNumberOfChildRooms { get { return _maxNumberOfChildRooms; } }
    private List<Room> _rooms = new List<Room>();
    public List<Room> rooms { get { return _rooms; } }
    [HideInInspector] public List<Layer> layers = new List<Layer>();

    //Cached
    [HideInInspector] public int currentInstantiatedRoom = 0;

    public World(int numberOfRooms, int maxNumberOfChildRooms, string seed = "")
    {
        _numberOfRooms = numberOfRooms;
        _maxNumberOfChildRooms = maxNumberOfChildRooms;
        if (seed == "")
        {
            _currentSeed = GenerateRandomSeed();
        }
        else
        {
            SetRandomSeed(seed);
        }

        GenerateWorld(_currentSeed);
    }

    #region SEED
    public string GenerateRandomSeed()
    {
        int seed = (int)System.DateTime.Now.Ticks;
        Random.InitState(seed);
        return seed.ToString();
    }

    public void SetRandomSeed(string seed = "")
    {
        _currentSeed = seed;
        int tempSeed = 0;
        tempSeed = seed.GetHashCode();
        Random.InitState(tempSeed);
    }
    #endregion

    #region GENERATION

    public void GenerateWorld(string seed = "")
    {
        if (seed == "")
        {
            GenerateRandomSeed();
        }
        else
        {
            SetRandomSeed(seed);
        }

        GenerateSpawnRoom();
        GenerateRooms();
        //world.SetBossRoom();
    }

    private void GenerateSpawnRoom()
    {
        Room spawnRoom = new Room(this);
        spawnRoom.roomType = RoomType.Spawn;
        AddRoom(spawnRoom);
        currentInstantiatedRoom++;

        Layer originLayer = new Layer();
        originLayer.roomsInLayer.Add(spawnRoom);
        layers.Add(originLayer);
    }

    private void GenerateRooms()
    {
        int currentLayer = 0;

        for (int i = currentInstantiatedRoom; i < numberOfRooms; i++)
        {
            List<Room> generatedRooms = new List<Room>();
            generatedRooms = GenerateRoomsForLayer(layers[currentLayer]);
            if (generatedRooms.Count == 0)
            {
                break;
            }
            else
            {
                currentLayer++;
                Layer newLayer = new Layer();
                layers.Add(newLayer);

                foreach(Room room in generatedRooms)
                {
                    newLayer.roomsInLayer.Add(room);
                }
            }
        }

        Debug.LogWarning("Generation has ended!");
    }

    private List<Room> GenerateRoomsForLayer(Layer layer)
    {
        List<Room> generatedRooms = new List<Room>();

        foreach(Room room in layer.roomsInLayer)
        {
            int randomNumberOfChildren = Random.Range(0, GetMaxNumberOfPotentialChildRooms());

            for (int i = 0; i < randomNumberOfChildren; i++)
            {
                Room childRoom = new Room(this, room);
                AddRoom(childRoom);
                room.AddChildRoom(childRoom);
                generatedRooms.Add(childRoom);
                currentInstantiatedRoom++;
            }
        }

        if (generatedRooms.Count == 0)
        {
            if (currentInstantiatedRoom < numberOfRooms)
            {
                Room childRoom = new Room(this, layer.roomsInLayer[0]);
                AddRoom(childRoom);
                layer.roomsInLayer[0].AddChildRoom(childRoom);
                generatedRooms.Add(childRoom);
                currentInstantiatedRoom++;
            }
        }

        return generatedRooms;
    }

    private int GetMaxNumberOfPotentialChildRooms()
    {
        int potentialChildRooms = 0;
        int remainingChildRooms = numberOfRooms - currentInstantiatedRoom;
        potentialChildRooms = remainingChildRooms;
        if(remainingChildRooms > maxNumberOfChildRooms)
        {
            potentialChildRooms = maxNumberOfChildRooms;
        }
        return potentialChildRooms;
    }

    private void AddRoom(Room room)
    {
        _rooms.Add(room);
        _nodes.Add(room);
        _nodeQueue.Enqueue(room);
        _nodeStack.Push(room);
    }
    #endregion
}
