using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GalaxyRangers/LD/RoomFolder")]
public class RoomFolder : ScriptableObject
{
    public List<RogueRoom> Rooms = new List<RogueRoom>();
}
