using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public World world;

    public int numberOfRooms = 10;
    public int maxNumberOfWhildRooms = 2;
    public string seed = "";

    //Debug
    [SerializeField] private DebugRoom _roomPrefab;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (world != null)
            {
                world.RoomGenerated -= OnRoomGenerated;
                world.RoomGivenChild -= OnRoomGivenChild;
            }

            World newWorld = new World(numberOfRooms, maxNumberOfWhildRooms, seed);
            world = newWorld;

            world.RoomGenerated += OnRoomGenerated;
            world.RoomGivenChild += OnRoomGivenChild;
        }
    }

    private void OnRoomGenerated(Room room, int layer, Room parentroom)
    {
        
    }

    private void OnRoomGivenChild(Room parentRoom, Room childRoom)
    {
        
    }
}
