using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TraversalLocation
{
    None,
    LeftTop,
    LeftBottom,
    MiddleTop,
    MiddleBottom,
    RightTop,
    RightBottom,
}

public enum RoomType
{
    None,
    Spawn,
    Boss,
    Exploration,
    Arena,
    Shop,
    Heal,
    Item,
    Length,
}

public enum RoomEnvironment
{
    Exterior,
    Interior,
}

public enum DifficultyScenario
{
    None,
    Easy,
    Medium,
    Hard,
}

public class TraversalPoint
{
    public TraversalLocation fromTraversalLocation = TraversalLocation.None;
    public TraversalLocation toTraversalLocation = TraversalLocation.None;
    public int depthChange = 0;
    public Room toRoom;

    public TraversalPoint(TraversalLocation fromLoc, TraversalLocation toLoc, int depth, Room toR)
    {
        fromTraversalLocation = fromLoc;
        toTraversalLocation = toLoc;
        depthChange = depth;
        toRoom = toR;
    }
}

public class Teleporter
{
    public bool isPresent = false;
    public Room toRoom;

    public Teleporter(bool isPres, Room to)
    {
        isPresent = isPres;
        toRoom = to;
    }
}

public class Room : Node
{
    public World world;
    public RoomType roomType = RoomType.None;
    public RoomEnvironment roomEnvironment = RoomEnvironment.Exterior;

    public int distance = 0;
    public float difficultyLevel = 0f;
    public DifficultyScenario scenario = DifficultyScenario.None;
    public int specialEventType = 0;
    public Teleporter teleporter;

    public Layer layer;
    public Room parentRoom;
    public List<Room> childRooms = new List<Room>();
    public List<TraversalPoint> exitPoints = new List<TraversalPoint>();
    public TraversalPoint entryPoint;
    public List<Room> roomHistory = new List<Room>();

    public Room()
    {

    }

    public Room(World _world)
    {
        world = _world;
    }

    public Room(World _world, Room _parentRoom)
    {
        world = _world;
        parentRoom = _parentRoom;
    }

    public void AddChildRoom(Room room)
    {
        childRooms.Add(room);
        _childNodes.Add(room);
        room.SetParentRoom(this);

        //When adding a child room, take a random exit point among RightTop, RightBottom, and Bottom. This will be the exit leading to that room.
        //When doing so, set the entryPoint to be the corresponding LeftTop, LeftBottom or Top exit point.
        //Create a traversal point on the parent at the random exit, leading to the child room. Add it to exitPoints.
        //Create a traversal point on the child at the corresponding exit point, leading to the parent room. Set it as entryPoint, and add it to exitPoints.
        List<TraversalLocation> potentialExitTravelLocations = GetAllPotentialExitTraversalLocations();
        int randomInt = UnityEngine.Random.Range(0, potentialExitTravelLocations.Count);
        TraversalLocation chosenExitLocation = potentialExitTravelLocations[randomInt];

        TraversalLocation correspondingEntryLocation = TraversalLocation.None;
        switch (chosenExitLocation)
        {
            case TraversalLocation.MiddleBottom:
                correspondingEntryLocation = TraversalLocation.MiddleTop;
                break;
            case TraversalLocation.RightTop:
                correspondingEntryLocation = TraversalLocation.LeftTop;
                break;
            case TraversalLocation.RightBottom:
                correspondingEntryLocation = TraversalLocation.LeftBottom;
                break;
        }

        TraversalPoint exitPoint = new TraversalPoint(chosenExitLocation, correspondingEntryLocation, 0, room);
        exitPoints.Add(exitPoint);
        TraversalPoint entryPoint = new TraversalPoint(correspondingEntryLocation, chosenExitLocation, 0, this);
        room.entryPoint = entryPoint;
        room.exitPoints.Add(entryPoint);
    }

    private List<TraversalLocation> GetAllPotentialExitTraversalLocations()
    {
        List<TraversalLocation> allPotentialExitTraversalLocations = new List<TraversalLocation>();
        //Check all current exitPoints.
        TraversalLocation MB = TraversalLocation.MiddleBottom;
        allPotentialExitTraversalLocations.Add(MB);
        TraversalLocation RT = TraversalLocation.RightTop;
        allPotentialExitTraversalLocations.Add(RT);
        TraversalLocation RB = TraversalLocation.RightBottom;
        allPotentialExitTraversalLocations.Add(RB);

        foreach (TraversalPoint traversalPoint in exitPoints)
        {
            if (allPotentialExitTraversalLocations.Contains(traversalPoint.fromTraversalLocation))
            {
                allPotentialExitTraversalLocations.Remove(traversalPoint.fromTraversalLocation);
            }
        }

        return allPotentialExitTraversalLocations;
    }

    private void SetParentRoom(Room room)
    {
        parentRoom = room;
        _parentNode = room;
    }
}
