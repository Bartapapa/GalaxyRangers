using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class Layer
{
    public List<Room> roomsInLayer = new List<Room>();

    public void AddToLayer(Room room)
    {
        roomsInLayer.Add(room);
        room.layer = this;
    }
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

    [SerializeField][ReadOnlyInspector] private int _mainBranchMinDistance = 5;
    public int mainBranchMinDistance { get { return _mainBranchMinDistance; } }
    [SerializeField][ReadOnlyInspector] private int _mainBranchMaxDistance = 7;
    public int mainBranchMaxDistance { get { return _mainBranchMaxDistance; } }
    [SerializeField][ReadOnlyInspector] private int _subBranchMaxDistance = 2;
    public int subBranchMaxDistance { get { return _subBranchMaxDistance; } }


    [SerializeField][ReadOnlyInspector] private int _maxNumberOfHealRooms = 1;
    public int maxNumberOfHealRooms { get { return _maxNumberOfHealRooms; } }
    [SerializeField][ReadOnlyInspector] private int _maxNumberOfShopRooms = 1;
    public int maxNumberOfShopRooms { get { return _maxNumberOfShopRooms; } }
    [SerializeField][ReadOnlyInspector] private int _maxNumberOfItemRooms = 1;
    public int maxNumberOfItemRooms { get { return _maxNumberOfItemRooms; } }
    [SerializeField][ReadOnlyInspector] private float _gasEventSpawnChance = .05f;
    public float gasEventSpawnChance { get { return _gasEventSpawnChance; } }
    [SerializeField][ReadOnlyInspector] private float _secondChanceEventSpawnChance = .05f;
    public float secondChanceEventSpawnChance { get { return _secondChanceEventSpawnChance; } }

    private List<Room> _rooms = new List<Room>();
    public List<Room> rooms { get { return _rooms; } }
    private List<Room> _arenas = new List<Room>();
    public List<Room> arenas { get { return _arenas; } }
    private List<Room> _explorations = new List<Room>();
    public List<Room> explorations { get { return _explorations; } }
    private Room _bossRoom;
    public Room bossRoom { get { return _bossRoom; } }
    [HideInInspector] public List<Layer> layers = new List<Layer>();

    private float _baseDifficulty = 0f;
    private float _difficultyVariance = 0f;

    //Cached
    [HideInInspector] public int currentInstantiatedRoom = 0;
    private int _highestDistance = 0;

    public World()
    {

    }
    public World(int numberOfRooms, int maxNumberOfChildRooms, int mainBranchMinDistance, int mainBranchMaxDistance, int subBranchMaxDistance,
                 int numberOfHealRooms, int numberOfShopRooms, int numberOfItemRooms, float gasEventSpawnChance, float secondChanceEventSpawnChance,
                 float baseDifficulty, float difficultyVariance,
                 string seed = "")
    {
        _numberOfRooms = numberOfRooms;
        _maxNumberOfChildRooms = maxNumberOfChildRooms;
        _mainBranchMinDistance = mainBranchMinDistance;
        _mainBranchMaxDistance = mainBranchMaxDistance;
        _subBranchMaxDistance = subBranchMaxDistance;

        _maxNumberOfHealRooms = numberOfHealRooms;
        _maxNumberOfShopRooms = numberOfShopRooms;
        _maxNumberOfItemRooms = numberOfItemRooms;

        _baseDifficulty = baseDifficulty;
        _difficultyVariance = difficultyVariance;

        _gasEventSpawnChance = gasEventSpawnChance;
        _secondChanceEventSpawnChance = secondChanceEventSpawnChance;

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
        UnityEngine.Random.InitState(seed);
        return seed.ToString();
    }

    public void SetRandomSeed(string seed = "")
    {
        _currentSeed = seed;
        int tempSeed = 0;
        tempSeed = seed.GetHashCode();
        UnityEngine.Random.InitState(tempSeed);
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
        //GenerateRooms();
        GenerateMainBranch();
        GenerateSubBranches();
        SetRoomDistance();
        SetBossRoom();
        SetHealRooms();
        SetShopRooms();
        SetItemRooms();
        SetArenaRooms();
        SetExploRooms();
        SetRoomScenarios();
        SetEventRooms();
    }

    private void GenerateSpawnRoom()
    {
        Room spawnRoom = new Room(this);
        spawnRoom.roomType = RoomType.Spawn;
        AddRoom(spawnRoom);
        //currentInstantiatedRoom++;

        Layer originLayer = new Layer();
        originLayer.AddToLayer(spawnRoom);
        layers.Add(originLayer);

        Room postSpawnRoom = new Room(this);
        AddRoom(postSpawnRoom);
        spawnRoom.AddChildRoom(postSpawnRoom);
        currentInstantiatedRoom++;

        Layer postSpawnLayer = new Layer();
        postSpawnLayer.AddToLayer(postSpawnRoom);
        layers.Add(postSpawnLayer);


        //LayerGenerated?.Invoke(originLayer);
        //RoomGenerated?.Invoke(spawnRoom, 0);
    }

    private void GenerateMainBranch()
    {
        int randomDistance = UnityEngine.Random.Range(mainBranchMinDistance, mainBranchMaxDistance + 1);
        Room parentRoom = rooms[1];
        for (int i = 0; i < randomDistance; i++)
        {
            Layer newLayer = new Layer();
            layers.Add(newLayer);

            Room mainBranchRoom = new Room(this);
            AddRoom(mainBranchRoom);
            parentRoom.AddChildRoom(mainBranchRoom);
            newLayer.AddToLayer(mainBranchRoom);

            currentInstantiatedRoom++;

            parentRoom = mainBranchRoom;
        }

        //parentRoom.roomType = RoomType.Boss;
    }

    private void GenerateSubBranches()
    {
        List<Room> mainBranchRooms = new List<Room>();

        foreach(Room room in rooms)
        {
            if (room.roomType != RoomType.Spawn)
            {
                mainBranchRooms.Add(room);
            }
        }

        for (int i = 0; i < mainBranchRooms.Count; i++)
        {
            if (mainBranchRooms[i].roomType == RoomType.None)
            {
                float randomNumber = UnityEngine.Random.Range(0f, 1f);
                int randomNumberOfChildren;

                if (mainBranchRooms[i] == rooms[1])
                {
                    //First room after spawn
                    randomNumberOfChildren = subBranchMaxDistance;
                }
                else
                {
                    int maxNumberOfChildren = GetMaxNumberOfPotentialChildRooms();

                    if (randomNumber < .2f)
                    {
                        randomNumberOfChildren = 0;
                    }
                    else if (randomNumber >= .2f && randomNumber < .8f)
                    {
                        randomNumberOfChildren = 1 <= maxNumberOfChildren ? 1 : 0;
                    }
                    else
                    {
                        randomNumberOfChildren = 2 <= maxNumberOfChildren ? 2 : 0;
                    }
                }

                if (randomNumberOfChildren >= 1)
                {
                    Room mainBranchParent = mainBranchRooms[i];
                    Room parentroom = mainBranchRooms[i];
                    mainBranchParent.teleporter = new Teleporter(true, null);

                    Layer parentLayer = mainBranchRooms[i].layer;
                    for (int j = 0; j < randomNumberOfChildren; j++)
                    {
                        Room childRoom = new Room(this, parentroom);
                        AddRoom(childRoom);

                        parentroom.AddChildRoom(childRoom);

                        currentInstantiatedRoom++;

                        parentroom = childRoom;

                        parentLayer.AddToLayer(childRoom);

                        if (j + 1 >= randomNumberOfChildren)
                        {
                            childRoom.teleporter = new Teleporter(true, mainBranchParent);
                        }
                    }
                }
            }
        }
    }

    private void GenerateRooms()
    {
        int currentLayer = 1;

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

                //LayerGenerated?.Invoke(newLayer);

                foreach(Room room in generatedRooms)
                {
                    newLayer.roomsInLayer.Add(room);
                }
            }
        }

        //Debug.LogWarning("Generation has ended!");
        //GenerationEnded?.Invoke();
    }

    private List<Room> GenerateRoomsForLayer(Layer layer)
    {
        List<Room> generatedRooms = new List<Room>();

        foreach(Room room in layer.roomsInLayer)
        {
            float randomNumber = UnityEngine.Random.Range(0f, 1f);
            int randomNumberOfChildren;

            if (layer == layers[1])
            {
                //Post spawn layer
                randomNumberOfChildren = 2;
            }
            else
            {
                int maxNumberOfChildren = GetMaxNumberOfPotentialChildRooms();
                if (randomNumber < .2f)
                {
                    randomNumberOfChildren = 0;
                }
                else if (randomNumber >= .2f && randomNumber < .4f)
                {
                    randomNumberOfChildren = 1 <= maxNumberOfChildren ? 1 : 0;
                }
                else
                {
                    randomNumberOfChildren = 2 <= maxNumberOfChildren ? 2 : 0;
                }
            }

            //int randomNumberOfChildren = UnityEngine.Random.Range(0, GetMaxNumberOfPotentialChildRooms()+1);

            for (int i = 0; i < randomNumberOfChildren; i++)
            {
                Room childRoom = new Room(this, room);
                AddRoom(childRoom);

                //RoomGenerated?.Invoke(childRoom, index);

                room.AddChildRoom(childRoom);
                generatedRooms.Add(childRoom);

                //RoomGivenChild?.Invoke(room, childRoom);
                currentInstantiatedRoom++;
            }
        }

        if (generatedRooms.Count == 0)
        {
            if (currentInstantiatedRoom < numberOfRooms)
            {
                Room childRoom = new Room(this, layer.roomsInLayer[0]);
                AddRoom(childRoom);

                //RoomGenerated?.Invoke(childRoom, index);

                layer.roomsInLayer[0].AddChildRoom(childRoom);
                generatedRooms.Add(childRoom);

                //RoomGivenChild?.Invoke(layer.roomsInLayer[0], childRoom);
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
        if(remainingChildRooms > subBranchMaxDistance)
        {
            potentialChildRooms = subBranchMaxDistance;
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

    private void SetRoomDistance()
    {
        foreach(Room room in rooms)
        {
            room.distance = BFS(rooms[0], room).Count-1;
        }
    }

    private void SetBossRoom()
    {
        int highestDistance = 0;
        List<Room> chosenRooms = new List<Room>();
        foreach(Room room in rooms)
        {
            if (room.distance >= highestDistance)
            {
                highestDistance = room.distance;
            }
        }
        _highestDistance = highestDistance;

        foreach(Room room in rooms)
        {
            if (room.distance >= highestDistance)
            {
                chosenRooms.Add(room);
            }
        }

        int randomInt = UnityEngine.Random.Range(0, chosenRooms.Count);
        chosenRooms[randomInt].roomType = RoomType.Boss;
        _bossRoom = chosenRooms[randomInt];

        if (_bossRoom.teleporter.isPresent)
        {
            if (_bossRoom.teleporter.toRoom != null)
            {
                _bossRoom.teleporter.toRoom.teleporter = new Teleporter(false, null);
                _bossRoom.teleporter = new Teleporter(false, null);
            }
        }
    }

    private void SetHealRooms()
    {
        List<Room> deadEnds = new List<Room>();
        foreach (Room room in rooms)
        {
            if (room.roomType == RoomType.None && room.childRooms.Count == 0)
            {
                deadEnds.Add(room);
            }
        }

        int randomInt = UnityEngine.Random.Range(0, deadEnds.Count);
        deadEnds[randomInt].roomType = RoomType.Heal;
    }

    private void SetItemRooms()
    {
        List<Room> deadEnds = new List<Room>();
        foreach (Room room in rooms)
        {
            if (room.roomType == RoomType.None && room.childRooms.Count == 0)
            {
                deadEnds.Add(room);
            }
        }

        for (int i = 0; i < maxNumberOfItemRooms; i++)
        {
            if (deadEnds.Count <= 0)
                break;

            int randomInt = UnityEngine.Random.Range(0, deadEnds.Count);
            deadEnds[randomInt].roomType = RoomType.Item;
            deadEnds.Remove(deadEnds[randomInt]);
        }
    }

    private void SetShopRooms()
    {
        int halfDistance = Mathf.RoundToInt(_highestDistance/2);
        List<Room> chosenRooms = new List<Room>();

        for (int i = halfDistance; i > 0; i--)
        {
            foreach (Room room in rooms)
            {
                if (room.distance >= i && room.roomType == RoomType.None && room.childRooms.Count >= 1)
                {
                    chosenRooms.Add(room);
                }
            }

            //if (chosenRooms.Count > 0)
            //{
            //    break;
            //}
        }

        if (chosenRooms.Count <= 0)
        {
            _bossRoom.parentRoom.roomType = RoomType.Shop;
        }
        else
        {
            int randomInt = UnityEngine.Random.Range(0, chosenRooms.Count);
            chosenRooms[randomInt].roomType = RoomType.Shop;
        }
    }

    private void SetExploRooms()
    {
        List<Room> chosenRooms = new List<Room>();
        foreach (Room room in arenas)
        {
            //chosenRooms.Add(room);
            if (room.childRooms.Count >= 2)
            {
                chosenRooms.Add(room);
            }
        }

        foreach(Room room in chosenRooms)
        {
            if(room.parentRoom.roomType == RoomType.Arena)
            {
                room.roomType = RoomType.Exploration;
                _explorations.Add(room);
                _arenas.Remove(room);
            }
        }
    }

    private void SetArenaRooms()
    {
        foreach (Room room in rooms)
        {
            if (room.roomType == RoomType.None)
            {
                room.roomType = RoomType.Arena;
                _arenas.Add(room);
            }
        }
    }

    private void SetRoomScenarios()
    {
        float easy = _highestDistance / 3;
        float medium = easy * 2;

        foreach(Room room in arenas)
        {
            room.difficultyLevel = room.distance + _baseDifficulty;
            float difficultyVariance = UnityEngine.Random.Range(-_difficultyVariance, _difficultyVariance);
            room.difficultyLevel += difficultyVariance;

            if (room.difficultyLevel <= easy)
            {
                room.scenario = DifficultyScenario.Easy;
            }
            else if (room.difficultyLevel > easy && room.difficultyLevel <= medium)
            {
                room.scenario = DifficultyScenario.Medium;
            }
            else
            {
                room.scenario = DifficultyScenario.Hard;
            }
        }

        foreach(Room room in explorations)
        {
            room.difficultyLevel = room.distance + _baseDifficulty;
            float difficultyVariance = UnityEngine.Random.Range(-_difficultyVariance, _difficultyVariance);
            room.difficultyLevel += difficultyVariance;

            if (room.difficultyLevel <= easy)
            {
                room.scenario = DifficultyScenario.Easy;
            }
            else if (room.difficultyLevel > easy && room.difficultyLevel <= medium)
            {
                room.scenario = DifficultyScenario.Medium;
            }
            else
            {
                room.scenario = DifficultyScenario.Hard;
            }
        }
    }

    private void SetEventRooms()
    {
        List<Room> possibleRooms = new List<Room>();
        foreach (Room room in arenas)
        {
            possibleRooms.Add(room);
        }
        foreach (Room room in explorations)
        {
            possibleRooms.Add(room);
        }

        List<Room> mediumHardRooms = new List<Room>();
        List<Room> easyMediumRooms = new List<Room>();
        foreach (Room room in possibleRooms)
        {
            if (room.scenario == DifficultyScenario.Easy)
            {
                easyMediumRooms.Add(room);
            }

            if (room.scenario == DifficultyScenario.Medium)
            {
                easyMediumRooms.Add(room);
                mediumHardRooms.Add(room);
            }

            if (room.scenario == DifficultyScenario.Hard)
            {
                mediumHardRooms.Add(room);
            }
        }

        //Second chance event
        float randomSecondChance = UnityEngine.Random.Range(0f, 1f);
        if (randomSecondChance <= _secondChanceEventSpawnChance)
        {
            int randomInt = UnityEngine.Random.Range(0, mediumHardRooms.Count);
            mediumHardRooms[randomInt].specialEventType = 1;
            if (easyMediumRooms.Contains(mediumHardRooms[randomInt]))
            {
                easyMediumRooms.Remove(mediumHardRooms[randomInt]);
            }
            mediumHardRooms.Remove(mediumHardRooms[randomInt]);
        }

        //Gas event
        float gasChance = UnityEngine.Random.Range(0f, 1f);
        if (gasChance <= _gasEventSpawnChance)
        {
            int randomInt = UnityEngine.Random.Range(0, mediumHardRooms.Count);
            easyMediumRooms[randomInt].specialEventType = 2;
            if (mediumHardRooms.Contains(easyMediumRooms[randomInt]))
            {
                mediumHardRooms.Remove(easyMediumRooms[randomInt]);
            }
            easyMediumRooms.Remove(easyMediumRooms[randomInt]);
        }
    }
    #endregion

    #region ALGOS

    public List<Room> BFS(Room start, Room end)
    {
        List<Room> result = new List<Room>();
        List<Room> visited = new List<Room>();
        Queue<Room> work = new Queue<Room>();

        start.roomHistory = new List<Room>();
        visited.Add(start);
        work.Enqueue(start);

        while (work.Count > 0)
        {
            Room current = work.Dequeue();
            if (current == end)
            {
                //Found room
                result = current.roomHistory;
                result.Add(current);
                return result;
            }
            else
            {
                //Didn't find room
                for (int i = 0; i < current.childRooms.Count; i++)
                {
                    Room currentRoom = current.childRooms[i];
                    if (!visited.Contains(currentRoom))
                    {
                        currentRoom.roomHistory = new List<Room>(current.roomHistory);
                        currentRoom.roomHistory.Add(current);
                        visited.Add(currentRoom);
                        work.Enqueue(currentRoom);
                    }
                }
            }
        }

        //Route not found, end loop
        return result;
    }

    public List<Room> DFS(Room start, Room end)
    {
        List<Room> result = new List<Room>();
        Stack<Room> work = new Stack<Room>();
        List<Room> visited = new List<Room>();

        work.Push(start);
        visited.Add(start);
        start.roomHistory = new List<Room>();

        while (work.Count > 0)
        {

            Room current = work.Pop();
            if (current == end)
            {
                result = current.roomHistory;
                result.Add(current);
                return result;
            }
            else
            {

                for (int i = 0; i < current.childRooms.Count; i++)
                {

                    Room currentChild = current.childRooms[i];
                    if (!visited.Contains(currentChild))
                    {

                        work.Push(currentChild);
                        visited.Add(currentChild);
                        currentChild.roomHistory = new List<Room>(current.roomHistory);
                        currentChild.roomHistory.Add(current);
                    }
                }
            }
        }

        return null;
    }

    #endregion
}
