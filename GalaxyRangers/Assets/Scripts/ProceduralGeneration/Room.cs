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
    Exploration,
    Arena,
    Boss,
    Length,
}

public enum RoomEnvironment
{
    Exterior,
    Interior,
}

public class TraversalPoint
{
    public TraversalLocation traversalLocation = TraversalLocation.None;
    public int depthChange = 0;
    public Room toRoom;
}

public class Room : Node
{
    public World world;
    public RoomType roomType = RoomType.None;
    public RoomEnvironment roomEnvironment = RoomEnvironment.Exterior;
    public Room parentRoom;
    public List<Room> childRooms = new List<Room>();
    public List<TraversalPoint> exitPoints = new List<TraversalPoint>();
    public TraversalPoint entryPoint;

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

    public void SetParentRoom(Room room)
    {
        parentRoom = room;
        _parentNode = room;
    }

    public void AddChildRoom(Room room)
    {
        childRooms.Add(room);
        _childNodes.Add(room);
    }
}
